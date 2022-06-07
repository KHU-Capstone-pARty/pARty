using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class CreateNexus : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    public bool NexusExists;
    public GameObject NexusPrefab;

    public Vector3 NexusPosition;

    public Text TextDebug;
    public Text TextMsg;

    void Start()
    {
        TextMsg.text = "Please set Nexus by touching plane.\n<size=36>(If setting not working, wait for second and try again.)</size>";
        TextDebug.text += "CreateNexus started\n";
        NexusExists = false;
    }

    void Update()
    {
        if(!NexusExists)
            InputProcess();
    }

    private void InputProcess()
    {
        if(Input.touchCount < 1) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;
        
        else if (!NexusExists)
        {
            TextDebug.text += "CreateNexus : Touch Detected\n";
            if (raycastManager.Raycast(touch.position,hits,TrackableType.All))
            {
                var hitPose = hits[0].pose;
                // var relPose = GetRelativePose(hitPose);
                // SpawnARSyncObject(0, relPose.position, relPose.rotation);
                Instantiate(NexusPrefab, hitPose.position, Quaternion.Euler(new Vector3(0,180,0)));
                NexusExists = true;
                NexusPosition = hitPose.position;
                TextDebug.text += "CreateNexus Done\n";
                TextMsg.text = "Nexus created at "+ NexusPosition + ".\n Monsters will be spawned soon.";
            }
            return;
        }
    }
}
