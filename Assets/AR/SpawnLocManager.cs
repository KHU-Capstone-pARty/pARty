using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnLocManager : MonoBehaviour
{
    public ARCameraManager CameraManager
    {
        get => _cameraManager;
        set => _cameraManager = value;
    }

    [SerializeField]
    [Tooltip("The ARCameraManager which will produce camera frame events.")]
    private ARCameraManager _cameraManager;


    public AROcclusionManager OcclusionManager
    {
        get => _occlusionManager;
        set => _occlusionManager = value;
    }

    [SerializeField]
    [Tooltip("The AROcclusionManager which will produce depth textures.")]
    private AROcclusionManager _occlusionManager;

    [SerializeField]
    private ARRaycastManager raycastManager;

    [SerializeField]
    [Tooltip("The UI RawImage used to display the image on screen.")]
    private RawImage _rawImage;

    [SerializeField]
    private GameObject spawnObj;

    [SerializeField]
    private Text info;

    //private Texture2D texture;
    private float tw;
    private float th;
    private float sw ;
    private float sh;

    private float currTime = 0;

    private void Start()
    {
        sw = Screen.width; sh = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        currTime += Time.deltaTime;

        if (currTime > 10)
        {
            SpawnMonster();
            currTime = 0;
        }*/
    }

    private List<Vector2> GetSpawnLoc()
    {
        List<Vector2> locations = new List<Vector2>();
        CloudAnchorMgr.Singleton.DebugLog("Get Spawn Loc");
        
        if (OcclusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            CloudAnchorMgr.Singleton.DebugLog("DepthCpuImage success");
            
            using (image)
            {
                // Use the texture.
                Texture2D texture = UpdateRawImage(_rawImage, image);
                tw = texture.width; th = texture.height;
                sw = Screen.width; sh = Screen.height;
                //info.text = tw + " " + th + ", " + sw + " " + sh;

                for (int i = 3; i < tw; i += 3)
                {
                    for (int j = 3; j < th; j += 3)
                    {
                        if (texture.GetPixel(i - 3, j).r - texture.GetPixel(i, j).r > 0.1f
                            || texture.GetPixel(i, j - 3).r - texture.GetPixel(i, j).r > 0.1f)
                        {
                            locations.Add(new Vector2(i, j));
                            //texture.SetPixel(i, j, new Color(255, 255, 255));
                            // info.text = ("(" + i * (sw / tw) + ", " + j * (sh / th) + "," + ")");
                        }
                    }
                }
            }
        }
        else{
            

        }
        // info.text = locations.Count.ToString();
        return locations;
    }

    // private void SpawnMonster()
    public Vector3 getSpawnPose()
    {
        List<Vector2> locations = GetSpawnLoc();

        Vector3 E_FAIL = Vector3.zero;
        if (locations.Count != 0)
        {
            System.Random rand = new System.Random();
            int idx = rand.Next(locations.Count);
            Vector2 textureLoc = locations[idx];
            Vector2 spawnLoc = new Vector2(sw * (1 - textureLoc.y / th), textureLoc.x * (sh / tw));
            
            //info.text = (idx + ": " + spawnLoc.x + " " + spawnLoc.y);
            
            //return new Vector3(spawnLoc.x, spawnLoc.y, 16);
            List<ARRaycastHit> hitsList = new List<ARRaycastHit>();
            CloudAnchorMgr.Singleton.DebugLog("Try Raycast");
            if (raycastManager.Raycast(spawnLoc, hitsList, TrackableType.PlaneWithinPolygon))
            {
                CloudAnchorMgr.Singleton.DebugLog("Raycast success");
                var h = hitsList[0].pose;
                
                return h.position;
                /*
                Instantiate(spawnObj, h.position, h.rotation);
                info.text = ("(" + h.position.x + ", " + h.position.y + "," + h.position.z + ")");
                */
            }
            else
            {
                // info.text = (idx + ": " + spawnLoc.x + " " + spawnLoc.y);
                
                return E_FAIL;
            }
        }
        else {         
            
            return E_FAIL;
        }
    }

    private static Texture2D UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage)
    {
        // Get the texture associated with the UI.RawImage that we wish to display on screen.
        var texture = rawImage.texture as Texture2D;

        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            rawImage.texture = texture;
        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), XRCpuImage.Transformation.MirrorY);

        //Debug.Log("Texture format: " + cpuImage.format.AsTextureFormat()); -> RFloat

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Perform the conversion.
        cpuImage.Convert(conversionParams, rawTextureData);

        // Get the aspect ratio for the current texture.
        var textureAspectRatio = (float)texture.width / texture.height;

        // Determine the raw image rectSize preserving the texture aspect ratio, matching the screen orientation,
        // and keeping a minimum dimension size.
        const float minDimension = 480.0f;
        var maxDimension = Mathf.Round(minDimension * textureAspectRatio);
        var rectSize = new Vector2(maxDimension, minDimension);
        //var rectSize = new Vector2(minDimension, maxDimension);   //Portrait
        rawImage.rectTransform.sizeDelta = rectSize;

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();

        return texture;
    }
}
