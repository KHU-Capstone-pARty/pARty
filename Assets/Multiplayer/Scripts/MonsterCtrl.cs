using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum MonsterState
{
    idle = 0, walk = 1, attack = 2
}

public class MonsterCtrl : NetworkBehaviour
{
    private ARSyncObject syncObject;
    private Transform attackTargetTransform;
    private float moveSpeed = 5f;
    private Animator animator;
    private MonsterState state;
    private int maxHP = 3;
    public NetworkVariable<int> HP = new NetworkVariable<int>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private bool isAttacked = false;

    private void Start()
    {
        syncObject = GetComponent<ARSyncObject>();
        attackTargetTransform = CloudAnchorMgr.Singleton.cloudAnchor.transform;
        animator = GetComponent<Animator>();
        CloudAnchorMgr.Singleton.DebugLog($"(상대위치) y값: {syncObject.Position.Value.y}");
        GameStatusMgr.Singleton.MonsterSpawnAlert();
    }

    private void Update()
    {
        FollowTargetOnXZPlane();
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

    private void FollowTargetOnXZPlane()
    {
        Vector3 dir = attackTargetTransform.position - transform.position;
        float distance = dir.magnitude;

        SetState(MonsterState.walk);
        if (distance < 0.5f)
        {
            SetState(MonsterState.attack);
            if (isAttacked) return;
            GameStatusMgr.Singleton.NexusDamaged();
            isAttacked = true;
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

    public void GetDamage(int d)
    {
        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        HP.Value -= d;
    }

    private void OnHPChanged(int pre, int cur)
    {
        if (cur <= 0)
        {
            GameStatusMgr.Singleton.MonsterDieAlert();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.collider.gameObject.CompareTag("Ball"))
        {
            GetDamage(maxHP);
        }
    } 
}
