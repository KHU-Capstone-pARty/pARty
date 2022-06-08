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
    private Transform attackTargetTransform;
    private float moveSpeed = 5f;
    private Animator animator;
    private MonsterState state;

    private void Start()
    {
        syncObject = GetComponent<ARSyncObject>();
        attackTargetTransform = CloudAnchorMgr.Singleton.cloudAnchor.transform;
        animator = GetComponent<Animator>();
        CloudAnchorMgr.Singleton.DebugLog($"Target Pos: {attackTargetTransform.position}");
    }

    private void Update()
    {
        FollowTargetOnXZPlane();
    }

    private void FollowTargetOnXZPlane()
    {
        Vector3 dir = attackTargetTransform.position - transform.position;
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
        // 클라우드 앵커의 윗방향으로 transform 윗방향 설정
        Quaternion rot = Quaternion.LookRotation(dir.normalized,CloudAnchorMgr.Singleton.cloudAnchorObj.transform.up);

        Pose worldPose = new Pose(targetPosition, rot);
        var relativePose = CloudAnchorMgr.Singleton.GetRelativePose(worldPose);

        // syncObject.RelativeMove(relativePose.position);
        // syncObject.RelativeRotate(relativePose.rotation);
        syncObject.RelativePose(relativePose);
    }

    private void SetState(MonsterState s)
    {
        animator.SetInteger("state",((int)s));
    }
}
