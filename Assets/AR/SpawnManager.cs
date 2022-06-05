using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnManager : MonoBehaviour
{
    public int FieldMobCnt; // Number of mobs in field
    public int CurrMobCnt; // Number of mobs terminated
    public int ObjectiveMobCnt; // Number of mobs to terminate

    public GameObject SpawnController;
    public GameObject ARCam;

    public bool MobXExist; // If MobX exists, set true
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
    public Text TextDebug2;
    public Text TextMsg;

    // Start is called before the first frame update
    void Start()
    {
        CurrMobCnt = 0;
        ObjectiveMobCnt = 16;

        MobXCycle = 10;
        MobXExist = false;
        MobXSpawnTime = 0.0f;

        MobYCycle = 16;
        MobYExist = false;
        MobYSpawnTime = 4.0f;

        MaxHP = 5;
        CurrHP = 5;

        TextDebug.text += "SPM started\n";
    }

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        // Debug.Log(runTime);
        CheckNexusExist();
        if(NexusExists)
        {
            CheckMobExist();

            if(CheckGameStatus()) // If game is running
            {
                Vector3 spawnPose = SpawnController.GetComponent<SpawnLocManager>().getSpawnPose();
                TextDebug.text += "SpawnPose : " + spawnPose + ", arcam:" +  ARCam.transform.position + "\n";
                spawnPose.z = 16.0f; // 플레이어와의 일정 거리를 위해 z축 고정
                spawnPose = ARCam.transform.rotation * (spawnPose - ARCam.transform.position);

                // if(spawnPose.y > arCam.transform.position.y + 0.5f)  -- TODO
                    
                if(spawnPose != new Vector3(0,2,16)) // SpawnLocManager에서 location 받지 못한 경우 디폴트 위치값 (E_FAIL)
                {
                    // ARSessionOrigin.MakeContentAppearAt(ARSessionOrigin.transform, ARCam.transform.position, ARCam.transform.rotation);
                    if(MobXExist == false)
                    {
                        if(runTime > MobXSpawnTime)
                        {
                            TextDebug2.text = "MobX spawned at : " + spawnPose;
                            SpawnController.GetComponent<MobXManager>().SpawnMobX(spawnPose); // spawn MobX
                            MobXSpawnTime = runTime + MobXCycle;
                        }
                    }
                    if(MobYExist == false)
                    {
                        if(runTime > MobYSpawnTime)
                        {
                            TextDebug2.text = "MobY spawned at : " + spawnPose;
                            SpawnController.GetComponent<MobYManager>().SpawnMobY(spawnPose); // spawn MobY
                            MobYSpawnTime = runTime + MobYCycle;
                        }
                    }
                }
            }
        }
    }

    bool CheckGameStatus() // If game ended, return false.
    {
        if(CurrMobCnt >= ObjectiveMobCnt)
        {
            TextMsg.text = "Defeated all enemies.\n Game finished!\n You Win.";
            return false;
        }
        else if(CurrHP <= 0)
        {
            TextMsg.text = "Nexus has been destroyed.\n Game finished!\n You Lose.";
            return false;
        }
        else
            return true;
    }

    void CheckMobExist()
    {
        MobXExist = SpawnController.GetComponent<MobXManager>().MobXExist;
        MobYExist = SpawnController.GetComponent<MobYManager>().MobYExist;
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
}
