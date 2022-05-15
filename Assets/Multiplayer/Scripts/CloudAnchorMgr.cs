using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public enum AnchorHostingPhase
{
    nothingToHost, readyToHost, hostInProgress, success, fail
}

public enum AnchorResolvingPhase
{
    nothingToResolve, readyToResolve, resolveInProgress, success, fail
}

public class CloudAnchorMgr : MonoBehaviour
{
    [SerializeField]
    private ARAnchorManager anchorMgr;
    [SerializeField]
    private Camera arCam;
    private ARCloudAnchor cloudAnchor; // 현재 작업중인 클라우드 앵커 참조
    public ARAnchor anchorToHost; // 외부에서 지정해준 호스팅할 앵커
    public AnchorHostingPhase hostPhase;
    public AnchorResolvingPhase resolvePhase;
    public Text text_log;
    public Text text_State;
    private string idToResolve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text_State.text = $"Host: {hostPhase.ToString()}, Resolve: {resolvePhase.ToString()}, Cloud Anchor State: {cloudAnchor?.cloudAnchorState.ToString()}";

        if (anchorToHost == null)
        {
            hostPhase = AnchorHostingPhase.nothingToHost;
        }
        else if (cloudAnchor != null && hostPhase == AnchorHostingPhase.hostInProgress)
        {
            CheckHostProgress();
        }

        if (cloudAnchor == null)
        {
            resolvePhase = AnchorResolvingPhase.nothingToResolve;
        }
        else if (cloudAnchor != null && resolvePhase == AnchorResolvingPhase.resolveInProgress)
        {
            CheckResolveProgress();
        }
    }

    public void HostAnchor()
    {
        text_log.text = "Host Anchor ...\n" + text_log.text;
        Pose camPose = new Pose(arCam.transform.position, arCam.transform.rotation);
        var quality = anchorMgr.EstimateFeatureMapQualityForHosting(camPose);
        text_log.text = $"Feature map quality: {quality.ToString()}\n" + text_log.text;
        cloudAnchor = anchorMgr.HostCloudAnchor(anchorToHost, 1);
        hostPhase = AnchorHostingPhase.hostInProgress;
        if (cloudAnchor == null)
        {
            //fail
            text_log.text = "Host failed\n" + text_log.text;
            hostPhase = AnchorHostingPhase.fail;
        }
        else
        {
            //success
            text_log.text = "Cloud anchor has been created\n" + text_log.text;
        }
    }

    void CheckHostProgress()
    {
        var state = cloudAnchor.cloudAnchorState;
        if (state == CloudAnchorState.Success)
        {
            hostPhase = AnchorHostingPhase.success;
            idToResolve = cloudAnchor.cloudAnchorId;
            text_log.text = $"Successfully Hosted. Anchor ID: {idToResolve}\n" + text_log.text;
            resolvePhase = AnchorResolvingPhase.readyToResolve;
        }
        else if (state != CloudAnchorState.TaskInProgress)
        {
            hostPhase = AnchorHostingPhase.fail;
        }
        else
        {
            hostPhase = AnchorHostingPhase.hostInProgress;
        }

    }

    public void CreateTestAnchor()
    {
        text_log.text = "Test anchor created\n" + text_log.text;
        anchorToHost = anchorMgr.AddAnchor(new Pose(Vector3.zero, Quaternion.identity));
        if (anchorToHost != null)
        {
            hostPhase = AnchorHostingPhase.readyToHost;
        }
    }

    public void ResolveAnchor()
    {
        text_log.text = "Resolve Anchor ...\n" + text_log.text;
        cloudAnchor = null;
        cloudAnchor = anchorMgr.ResolveCloudAnchorId(idToResolve);
        resolvePhase = AnchorResolvingPhase.resolveInProgress;
        if (cloudAnchor == null)
        {
            text_log.text = "Resolve failed\n" + text_log.text;
            resolvePhase = AnchorResolvingPhase.fail;
        }
        else
        {
            text_log.text = "Cloud anchor has been created\n" + text_log.text;
        }
    }

    void CheckResolveProgress()
    {
        var state = cloudAnchor.cloudAnchorState;
        if (state == CloudAnchorState.Success)
        {
            resolvePhase = AnchorResolvingPhase.success;
            var pos = cloudAnchor.pose.position;
            text_log.text = $"Successfully Resolved. Cloud anchor position: {pos}\n" + text_log.text;
        }
        else if (state != CloudAnchorState.TaskInProgress)
        {
            resolvePhase = AnchorResolvingPhase.fail;
        }
        else
        {
            resolvePhase = AnchorResolvingPhase.resolveInProgress;
        }

    }

}
