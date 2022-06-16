using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AvatarCtrl : MonoBehaviour
{
    private ARSyncObject syncObj;
    private NetworkObject netObj;
    private MeshRenderer meshRenderer;

    void Start()
    {
        syncObj = GetComponent<ARSyncObject>();
        netObj = GetComponent<NetworkObject>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (netObj.IsOwner)
        {
            meshRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (!netObj.IsOwner) return;
        
        Pose worldCameraPose = new Pose(Camera.main.transform.position, Camera.main.transform.rotation);
        Pose relativeCameraPose = CloudAnchorMgr.Singleton.GetRelativePose(worldCameraPose);

        if (CloudAnchorMgr.Singleton.NetworkManager.IsServer)
        {
            syncObj.RelativePose(relativeCameraPose);
        }
        else
        {
            syncObj.RelativePoseServerRpc(relativeCameraPose.position, relativeCameraPose.rotation);
        }
    }
}
