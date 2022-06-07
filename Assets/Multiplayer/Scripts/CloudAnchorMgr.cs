using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using Unity.Netcode;
using UnityEngine.EventSystems; 

public enum AnchorHostingPhase
{
    nothingToHost, readyToHost, hostInProgress, success, fail
}

public enum AnchorResolvingPhase
{
    nothingToResolve, readyToResolve, resolveInProgress, success, fail
}

public enum ARSyncObjectID
{
    testObj = 0, testMonster = 1,
}

public class CloudAnchorMgr : NetworkBehaviour
{
    public static CloudAnchorMgr Singleton;
    [HideInInspector]
    public ARCloudAnchor cloudAnchor; // 현재 작업중인 클라우드 앵커 참조
    [HideInInspector]
    public ARAnchor anchorToHost; // 외부에서 지정해준 호스팅할 앵커
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [HideInInspector]
    public AnchorHostingPhase hostPhase;
    [HideInInspector]
    public AnchorResolvingPhase resolvePhase;
    private string idToResolve;
    private bool isStartEstimate = false;
    private GameObject cloudAnchorObj;
    private bool isPlacingTestObj = false;
    private bool isPlacingMonster = false;
    private PlayerPrefabRef playerRef;
    private bool debugTextActive = true;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Singleton != this)
                Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindWithTag("Ref").GetComponent<PlayerPrefabRef>();
        playerRef.hostAnchorButton.onClick.AddListener(HostAnchor);
        playerRef.resolveAnchorButton.onClick.AddListener(ResolveAnchor);
        playerRef.placeTestObjToggle.onValueChanged.AddListener((b)=>{isPlacingTestObj = !isPlacingTestObj;});
        playerRef.placeMonster.onValueChanged.AddListener((b)=>{isPlacingMonster = !isPlacingMonster;});
    }

    // Update is called once per frame
    void Update()
    {
        InputProcess();
        HostResolveProcess();
    }

    private void InputProcess()
    {
        if(Input.touchCount < 1) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;
        
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        if (anchorToHost != null && !isPlacingTestObj && !isPlacingMonster)
        {
            playerRef.text_log.text = "Anchor already exists\n" + playerRef.text_log.text;
            return;
        }
        else if (isPlacingMonster)
        {
            if (playerRef.raycastManager.Raycast(touch.position,hits,TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                var relPose = GetRelativePose(hitPose);
                SpawnARSyncObject(((int)ARSyncObjectID.testMonster), relPose.position, relPose.rotation);
            }

            return;
        }
        else if (isPlacingTestObj)
        {
            if (playerRef.raycastManager.Raycast(touch.position,hits,TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                var relPose = GetRelativePose(hitPose);
                SpawnARSyncObject(((int)ARSyncObjectID.testObj), relPose.position, relPose.rotation);
            }

            return;
        }


        if (!NetworkManager.IsServer)
        {
            playerRef.text_log.text = $"You cannot create cloud anchor.\n" + playerRef.text_log.text;
            return;
        }    

        if (playerRef.raycastManager.Raycast(touch.position,hits,TrackableType.PlaneWithinPolygon))
        {
            anchorToHost = playerRef.anchorMgr.AddAnchor(hits[0].pose);
            cloudAnchorObj = Instantiate(playerRef.anchorPrefab,anchorToHost.transform);
            if (anchorToHost != null)
            {
                hostPhase = AnchorHostingPhase.readyToHost;
            }
            playerRef.text_log.text = $"Anchor created at {anchorToHost.transform.position}\n" + playerRef.text_log.text;
            isStartEstimate = true;
        }
    }

    private void HostResolveProcess()
    {
        FeatureMapQuality quality = FeatureMapQuality.Insufficient;
        if (isStartEstimate)
            quality = playerRef.anchorMgr.EstimateFeatureMapQualityForHosting(GetCamPose());
        Vector3 anchorPos = Vector3.zero;
        if (cloudAnchor != null)
            anchorPos = cloudAnchor.transform.position;
        playerRef.text_State.text = $"Anchor: {anchorPos}, Map Quality: {quality.ToString()}, Host: {hostPhase.ToString()}, Resolve: {resolvePhase.ToString()}, Cloud Anchor State: {cloudAnchor?.cloudAnchorState.ToString()}";
        
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

    [ClientRpc]
    public void SendAnchorIDClientRPC(string id)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            playerRef.text_log.text = $"Ignore received Anchor ID: {idToResolve}\n" + playerRef.text_log.text;
            return;
        }
        
        idToResolve = id;
        playerRef.text_log.text = $"Receive Anchor ID: {idToResolve}\n" + playerRef.text_log.text;
    }

    private Pose GetCamPose()
    {
        return new Pose(playerRef.arCam.transform.position, playerRef.arCam.transform.rotation);
    }

    public void HostAnchor()
    {
        playerRef.text_log.text = "Host Anchor ...\n" + playerRef.text_log.text;
        var quality = playerRef.anchorMgr.EstimateFeatureMapQualityForHosting(GetCamPose());
        playerRef.text_log.text = $"Feature map quality: {quality.ToString()}\n" + playerRef.text_log.text;
        cloudAnchor = playerRef.anchorMgr.HostCloudAnchor(anchorToHost, 1);
        hostPhase = AnchorHostingPhase.hostInProgress;
        if (cloudAnchor == null)
        {
            //fail
            playerRef.text_log.text = "Host failed\n" + playerRef.text_log.text;
            hostPhase = AnchorHostingPhase.fail;
        }
        else
        {
            //success
            playerRef.text_log.text = "Cloud anchor has been created\n" + playerRef.text_log.text;
        }
    }

    void CheckHostProgress()
    {
        var state = cloudAnchor.cloudAnchorState;
        if (state == CloudAnchorState.Success)
        {
            hostPhase = AnchorHostingPhase.success;
            idToResolve = cloudAnchor.cloudAnchorId;
            playerRef.text_log.text = $"Successfully Hosted. Anchor ID: {idToResolve}\n" + playerRef.text_log.text;
            resolvePhase = AnchorResolvingPhase.readyToResolve;
            SendAnchorIDClientRPC(idToResolve);
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
        playerRef.text_log.text = "Test anchor created\n" + playerRef.text_log.text;
        anchorToHost = playerRef.anchorMgr.AddAnchor(new Pose(Vector3.zero, Quaternion.identity));
        if (anchorToHost != null)
        {
            hostPhase = AnchorHostingPhase.readyToHost;
        }
    }

    public void ResolveAnchor()
    {
        playerRef.text_log.text = "Resolve Anchor ...\n" + playerRef.text_log.text;
        cloudAnchor = null;
        cloudAnchor = playerRef.anchorMgr.ResolveCloudAnchorId(idToResolve);
        resolvePhase = AnchorResolvingPhase.resolveInProgress;
        if (cloudAnchor == null)
        {
            playerRef.text_log.text = "Resolve failed\n" + playerRef.text_log.text;
            resolvePhase = AnchorResolvingPhase.fail;
        }
        else
        {
            playerRef.text_log.text = "Cloud anchor has been created\n" + playerRef.text_log.text;
        }
    }

    void CheckResolveProgress()
    {
        var state = cloudAnchor.cloudAnchorState;
        if (state == CloudAnchorState.Success)
        {
            resolvePhase = AnchorResolvingPhase.success;
            var pos = cloudAnchor.pose.position;
            if (cloudAnchorObj != null) Destroy(cloudAnchorObj);
            cloudAnchorObj = Instantiate(playerRef.anchorPrefab,cloudAnchor.transform);
            playerRef.text_log.text = $"Successfully Resolved. Cloud anchor position: {pos}\n" + playerRef.text_log.text;
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

    public Pose GetRelativePose(Pose worldVec)
    {
        if (cloudAnchor == null)
        {
            Debug.LogError("clouad anchor is null");
            return Pose.identity;
        }
        return cloudAnchor.transform.InverseTransformPose(worldVec);
    }

    public Pose GetWorldPose(Pose relVec)
    {
        if (cloudAnchor == null)
        {
            Debug.LogError("clouad anchor is null");
            return Pose.identity;
        }
        return cloudAnchor.transform.TransformPose(relVec);
    }

    public void TogglePlacingTestObj(bool b)
    {
        isPlacingTestObj = !isPlacingTestObj;
    }

    [ClientRpc]
    private void SpawnObjClientRpc(int objNum, Vector3 relPos, Quaternion relRot, ulong ownerId)
    {
        if (!NetworkManager.IsServer) return;
        Pose relPose = new Pose(relPos, relRot);
        Pose worldPose = GetWorldPose(relPose);
        var instance = Instantiate(playerRef.ARSyncPrefab[objNum], worldPose.position,worldPose.rotation,cloudAnchor.transform);
        playerRef.text_log.text = $"Test obj created. Owner: {ownerId}, Relative: {relPose.ToString()}, World: {worldPose.ToString()}\n" + playerRef.text_log.text;
        NetworkObject netObj = instance.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            DebugLog($"NetObj is null");
            return;
        }
        netObj.SpawnWithOwnership(ownerId);
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnObjServerRpc(int objNum, Vector3 relPos, Quaternion relRot, ulong ownerId)
    {
        DebugLog("Receive SpawnObjServerRPC");
        Pose relPose = new Pose(relPos, relRot);
        Pose worldPose = GetWorldPose(relPose);
        var instance = Instantiate(playerRef.ARSyncPrefab[objNum], worldPose.position,worldPose.rotation,cloudAnchor.transform);
        playerRef.text_log.text = $"Test obj created. Owner: {ownerId}, Relative: {relPose.ToString()}, World: {worldPose.ToString()}\n" + playerRef.text_log.text;
        NetworkObject netObj = instance.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            DebugLog($"NetObj is null");
            return;
        }
        netObj.SpawnWithOwnership(ownerId);
    }

    public void DebugLog(string msg)
    {
        playerRef.text_log.text = $"{msg}\n" + playerRef.text_log.text;
    }

    public void SpawnARSyncObject(int objNum, Vector3 relPos, Quaternion relRot)
    {
        if (NetworkManager.IsServer)
        {
            DebugLog("Spawn AR Sync Object: call client rpc");
            SpawnObjClientRpc(objNum, relPos, relRot, NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            DebugLog("Spawn AR Sync Object: call server rpc");
            SpawnObjServerRpc(objNum, relPos, relRot, NetworkManager.Singleton.LocalClientId);
        }
    }

    public void ToggleDebugText()
    {
        debugTextActive = !debugTextActive;
        playerRef.text_log.gameObject.SetActive(debugTextActive);
        playerRef.text_State.gameObject.SetActive(debugTextActive);
    }
}
