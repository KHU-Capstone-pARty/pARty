using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// https://www.andreasjakl.com/visualize-ar-depth-maps-in-unity-part-5/
public class DepthImageVisualizer : MonoBehaviour
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

    public RawImage RawImage
    {
        get => _rawImage;
        set => _rawImage = value;
    }

    [SerializeField]
    [Tooltip("The UI RawImage used to display the image on screen.")]
    private RawImage _rawImage;

    [SerializeField]
    private Text info;

    // Update is called once per frame
    void Update()
    {
        if (OcclusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            using (image)
            {
                
                // Use the texture.
                Texture2D texture = UpdateRawImage(_rawImage, image);
                float tw = texture.width; float th = texture.height;
                float sw = Screen.width; float sh = Screen.height;
                for (int i = 10; i < tw; i += 10)
                {
                    for (int j = 10; j < th; j += 10)
                    {
                        if (texture.GetPixel(i-10, j).r- texture.GetPixel(i, j).r>1
                            || texture.GetPixel(i, j - 10).r - texture.GetPixel(i, j).r > 1)
                        {
                            info.text = ("(" + i * (sw / tw) + ", " + j * (sh / th) + "," + ")");
                        }
                    }
                }
            }
        }
    }

    // Source: https://github.com/Unity-Technologies/arfoundation-samples/blob/6296272a416925b56ce85470e0c7bef5c913ec0c/Assets/Scripts/CpuImageSample.cs
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

        /*
        // Get the aspect ratio for the current texture.
        var textureAspectRatio = (float)texture.width / texture.height;

        // Determine the raw image rectSize preserving the texture aspect ratio, matching the screen orientation,
        // and keeping a minimum dimension size.
        const float minDimension = 480.0f;
        var maxDimension = Mathf.Round(minDimension * textureAspectRatio);
        var rectSize = new Vector2(maxDimension, minDimension);
        //var rectSize = new Vector2(minDimension, maxDimension);   //Portrait
        rawImage.rectTransform.sizeDelta = rectSize;
        */

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();

        return texture;
    }
}
