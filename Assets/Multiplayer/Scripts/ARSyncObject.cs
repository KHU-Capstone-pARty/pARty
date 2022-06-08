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

    public NetworkVariable<Pose> Pose = new NetworkVariable<Pose>(
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
        //TestMoveFunction();
    }

    private void OnEnable()
    {
        // Position.OnValueChanged += OnPositionChanged;
        // Rotation.OnValueChanged += OnRotationChanged;
        Pose.OnValueChanged += OnPoseChanged;
    }

    private void OnDisable() 
    {
        // Position.OnValueChanged -= OnPositionChanged;
        // Rotation.OnValueChanged -= OnRotationChanged;
        Pose.OnValueChanged -= OnPoseChanged;
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

    private void OnPositionChanged(Vector3 pre, Vector3 cur)
    {
        var pose = new Pose(cur, Rotation.Value);
        pose = CloudAnchorMgr.Singleton.GetWorldPose(pose);
        transform.position = pose.position;
        //CloudAnchorMgr.Singleton.DebugLog($"Position Changed {pre} -> {cur}");

    }

    private void OnRotationChanged(Quaternion pre, Quaternion cur)
    {
        var pose = new Pose(Position.Value, cur);
        pose = CloudAnchorMgr.Singleton.GetWorldPose(pose);
        transform.rotation = pose.rotation;
    }
    
    private void OnPoseChanged(Pose pre, Pose cur)
    {
        var worldPose = CloudAnchorMgr.Singleton.GetWorldPose(cur);
        transform.position = worldPose.position;
        transform.rotation =worldPose.rotation;
    }

    public void RelativeMove(Vector3 relativePos)
    {
        if (!IsServer) return;

        Position.Value = relativePos;
    }

    public void RelativeRotate(Quaternion relativeRot)
    {
        if (!IsServer) return;

        Rotation.Value = relativeRot;
    }

    public void RelativePose(Pose pose)
    {
        if (!IsServer) return;

        Pose.Value = pose;
    }
}
