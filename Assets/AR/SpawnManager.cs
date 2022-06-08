using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnManager : MonoBehaviour
{
    [HideInInspector] 
    public int FieldMobCnt; // Number of mobs in field
    [HideInInspector] 
    public int CurrMobCnt; // Number of mobs terminated
    [HideInInspector] 
    public int ObjectiveMobCnt; // Number of mobs to terminate

    public GameObject SpawnController;
    public GameObject ARCam;

    public GameObject MobX;
    GameObject MobXfab;
    public GameObject MobY;
    GameObject MobYfab;
    
    [HideInInspector] 
    public bool MobXExist; // If MobX exists, set true
    [HideInInspector] 
    public bool MobYExist; 

    float runTime;
    int MobXCycle; // cycle time for respawn
    int MobYCycle; 
    float MobXSpawnTime; // initial spawn time
    float MobYSpawnTime;

    Vector3 NexusPosition;
    bool NexusExists;

    [HideInInspector] 
    public int MaxHP;
    
    [HideInInspector] 
    public int CurrHP;

    public Text TextDebug;
    public Text TextMsg;

    // Start is called before the first frame update
    void Start()
    {
        CurrMobCnt = 0;
        ObjectiveMobCnt = 16;

        MobXCycle = 11;
        MobXExist = false;
        MobXSpawnTime = 5.0f;

        MobYCycle = 17;
        MobYExist = false;
        MobYSpawnTime = 8.0f;

        MaxHP = 5;
        CurrHP = 5;

        TextDebug.text += "SPM started\n";
    }

    // Update is called once per frame
    void Update()
    {
        CheckNexusExist();
        if(NexusExists)
        {    
            runTime += Time.deltaTime;
            CheckMobExist();

            if(CheckGameStatus()) // If game is running
            {
                Vector3 spawnPose = SpawnController.GetComponent<SpawnLocManager>().getSpawnPose();
                
                spawnPose.z = 16.0f; // 플레이어와의 일정 거리를 위해 z축 고정
                spawnPose = ARCam.transform.rotation * (spawnPose - ARCam.transform.position);
                    
                if(spawnPose != new Vector3(0,2,16)) // SpawnLocManager에서 location 받지 못한 경우 디폴트 위치값 (E_FAIL)
                {
                    if(MobXExist == false)
                    {
                        if(runTime > MobXSpawnTime)
                        {
                            // 넥서스와 몬스터가 같은 y값을 가짐
                            SpawnMobX(new Vector3(spawnPose.x, gameObject.GetComponent<CreateNexus>().NexusPosition.y, spawnPose.z));
                            SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
                            MobXSpawnTime = runTime + MobXCycle;
                        }
                    }
                }
                if (MobYExist == false)
                {
                    if (runTime > MobYSpawnTime)
                    {
                        Vector3 eulerAngle = new Vector3(ARCam.transform.eulerAngles.x, ARCam.transform.eulerAngles.y+180, ARCam.transform.eulerAngles.z);    // 플레이어가 바라보는 방향의 반대
                        spawnPose = Quaternion.Euler(eulerAngle) * (new Vector3(Random.Range(2,4), Random.Range(-2,0), 16.0f) - ARCam.transform.position);
                        SpawnMobY(spawnPose);
                        SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
                        MobYSpawnTime = runTime + MobYCycle;
                    }
                }
            }
        }
    }

    bool CheckGameStatus() // If game ended, return false.
    {
        if(CurrMobCnt >= ObjectiveMobCnt)
        {
            TextMsg.text = "Defeated all enemies.\n You Win!!";
            return false;
        }
        else if(CurrHP <= 0)
        {
            TextMsg.text = "Nexus has been destroyed.\n You Lose...";
            return false;
        }
        else
            return true;
    }

    void CheckMobExist()
    {
        if(MobXfab == null)
            MobXExist = false;
        else
            MobXExist = true;
        if(MobYfab == null)
            MobYExist = false;
        else
            MobYExist = true;
    }

    void CheckNexusExist()
    {
        NexusExists = SpawnController.GetComponent<CreateNexus>().NexusExists;
    }

    void GetNexusPosition()
    {
        if(NexusExists)
        {
            NexusPosition = SpawnController.GetComponent<CreateNexus>().NexusPosition;
            TextDebug.text += "Get Nexus Pos : " + NexusPosition + "\n";
        }
    }

    void SpawnMobX(Vector3 _spawnPos)
    {
        MobXfab = Instantiate(MobX, _spawnPos, Quaternion.LookRotation(_spawnPos) * Quaternion.Euler(new Vector3(0,180,0)));
        MobXfab.GetComponent<MobXManager>().spawnPos = _spawnPos;
        MobXfab.GetComponent<MobXManager>().NexusPosition = SpawnController.GetComponent<CreateNexus>().NexusPosition;
        MobXfab.GetComponent<MobXManager>().TextMsg = TextMsg;
        TextMsg.text = "Monster X has spawned.";
    }

    void SpawnMobY(Vector3 _spawnPos)
    {
        MobYfab = Instantiate(MobY, _spawnPos, Quaternion.LookRotation(_spawnPos) * Quaternion.Euler(new Vector3(0,180,0)));
        MobYfab.GetComponent<MobYManager>().spawnPos = _spawnPos;
        MobYfab.GetComponent<MobYManager>().NexusPosition = SpawnController.GetComponent<CreateNexus>().NexusPosition;
        MobYfab.GetComponent<MobYManager>().TextMsg = TextMsg;
        TextMsg.text = "Monster Y has spawned.";
    }
}

