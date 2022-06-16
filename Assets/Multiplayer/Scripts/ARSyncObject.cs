using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class ARSyncObject : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkObject netObj;
    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdatePose();
    }

    private void OnEnable()
    {
        //Position.OnValueChanged += OnPositionChanged;
        //Rotation.OnValueChanged += OnRotationChanged;
    }

    private void OnDisable() 
    {
        //Position.OnValueChanged -= OnPositionChanged;
        //Rotation.OnValueChanged -= OnRotationChanged;
    }

    public void Init()
    {
        netObj = GetComponent<NetworkObject>();
        CloudAnchorMgr.Singleton.DebugLog($"ARSyncObject Init. Owner: {IsOwner}, NetObj: {netObj != null}");
        var pose = new Pose(transform.position, transform.rotation);
        pose = CloudAnchorMgr.Singleton.GetRelativePose(pose);

        if (!IsServer) return;

        Position.Value = pose.position;
        Rotation.Value = pose.rotation;
    }

    private void UpdatePose()
    {
        Pose curPose = new Pose(Position.Value,Rotation.Value);
        var worldPose = CloudAnchorMgr.Singleton.GetWorldPose(curPose);
        transform.position = worldPose.position;
        transform.rotation = worldPose.rotation;
    }

    private void OnPositionChanged(Vector3 pre, Vector3 cur)
    {
        Pose curPose = new Pose(cur, Rotation.Value);
        var worldPose = CloudAnchorMgr.Singleton.GetWorldPose(curPose);
        transform.position = worldPose.position;
    }

    private void OnRotationChanged(Quaternion pre, Quaternion cur)
    {
        Pose curPose = new Pose(Position.Value, cur);
        var worldPose = CloudAnchorMgr.Singleton.GetWorldPose(curPose);
        transform.rotation = worldPose.rotation;
    }

    public void RelativePose(Pose pose)
    {
        if (!IsServer) return;

        Position.Value = pose.position;
        Rotation.Value = pose.rotation;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RelativePoseServerRpc(Vector3 pos, Quaternion rot)
    {
        RelativePose(new Pose(pos,rot));
    }
}
