using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    idle = 0, walk = 1, attack = 2
}

public class MonsterCtrl : MonoBehaviour
{
    private ARSyncObject syncObject;
    private Transform attckTargetTransform;
    private float moveSpeed = 5f;
    private Animator animator;
    private MonsterState state;

    private void Start()
    {
        syncObject = GetComponent<ARSyncObject>();
        attckTargetTransform = CloudAnchorMgr.Singleton.cloudAnchor.transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        FollowTargetOnXZPlane();
    }

    private void FollowTargetOnXZPlane()
    {
        Vector3 dir = attckTargetTransform.position - transform.position;
        dir = new Vector3(dir.x,0,dir.z);
        float distance = dir.magnitude;

        SetState(MonsterState.walk);
        if (distance < 0.5f)
        {
            SetState(MonsterState.attack);
            return;
        }

        dir = dir.normalized * 0.001f;

        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        var targetPosition = transform.position + dir * moveSpeed;
        transform.LookAt(targetPosition);

        Pose worldPose = new Pose(targetPosition, transform.rotation);
        var relativePose = CloudAnchorMgr.Singleton.GetRelativePose(worldPose);

        syncObject.RelativeMove(relativePose.position);
        syncObject.RelativeRotate(relativePose.rotation);
    }

    private void SetState(MonsterState s)
    {
        animator.SetInteger("state",((int)s));
    }
}
