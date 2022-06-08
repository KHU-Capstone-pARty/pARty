using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class GameStatusMgr : NetworkBehaviour
{
    public static GameStatusMgr Singleton;
    public NetworkVariable<int> MonsterKillCount = new NetworkVariable<int>( // Number of mobs terminated
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> FieldMonsterCount = new NetworkVariable<int>( // Number of mobs in field
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> MonsterGoal = new NetworkVariable<int>( // Number of mobs to terminate
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private int maxHP = 5;

    public NetworkVariable<int> currHP = new NetworkVariable<int>( // Current HP of Nexus
       default,
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server);

    public NetworkVariable<int> Gold = new NetworkVariable<int>( // Current HP of Nexus
       default,
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server);

    private PlayerPrefabRef playerRef;


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
        InitGameStatus();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStatus();
    }

    private void OnEnable()
    {
        currHP.OnValueChanged += OnHPChanged;
    }

    private void OnDisable() 
    {
        currHP.OnValueChanged -= OnHPChanged;
    }

    public void InitGameStatus()
    {
        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        CloudAnchorMgr.Singleton.DebugLog("Gamestatus manager spawned");
        UIMgr.Singleton.selectHostClientPanel.SetActive(false);
        UIMgr.Singleton.anchorPanel.SetActive(true);

        MonsterKillCount.Value = 0;
        FieldMonsterCount.Value = 0;
        MonsterGoal.Value = 16;

        currHP.Value = maxHP;
    }

    public void NexusDamaged()
    {
        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        currHP.Value--;
    }

    public void MonsterDieAlert()
    {
        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        MonsterKillCount.Value++;
        FieldMonsterCount.Value--;
    }

    public void MonsterSpawnAlert()
    {
        if (!CloudAnchorMgr.Singleton.NetworkManager.IsServer) return;

        FieldMonsterCount.Value++;
    }

    public void CheckGameStatus()
    {
        if (currHP.Value <= 0)
        {
            CloudAnchorMgr.Singleton.DebugLog("플레이어 패배");
        }
        else if (MonsterKillCount.Value >= MonsterGoal.Value)
        {
            CloudAnchorMgr.Singleton.DebugLog("플레이어 승리");
            // Player Win
        }
    }

    public void OnHPChanged(int pre, int cur)
    {
        for (int i =1; i <= maxHP; i++)
        {
            playerRef.hearts[i-1].gameObject.SetActive(i<=cur);
        }
    }
}