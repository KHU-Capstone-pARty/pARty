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

    public bool MobXExist; // If MobX exists, set true
    public bool MobYExist; 

    float runTime;
    int MobXCycle; // cycle time for respawn
    int MobYCycle; 
    float MobXSpawnTime; // initial spawn time
    float MobYSpawnTime;

    public Text TextDebug;

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

        TextDebug.text += "SPM started\n";
    }

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        // Debug.Log(runTime);
        CheckMobExist();

        if(CurrMobCnt >= ObjectiveMobCnt)
        {
            //Game End

        }
        else
        {
            Vector3 spawnPose = SpawnController.GetComponent<SpawnLocManager>().getSpawnPose();
            spawnPose.z = 10.0f; // 플레이어와의 일정 거리를 위해 z축 고정
            if(spawnPose != new Vector3(0,2,10))
            {
                TextDebug.text += "spawnPose : " + spawnPose + "\n";
                if(MobXExist == false)
                {
                    if(runTime > MobXSpawnTime)
                    {
                        SpawnController.GetComponent<MobXManager>().SpawnMobX(spawnPose); // spawn MobX
                        MobXSpawnTime = runTime + MobXCycle;
                    }
                }
                if(MobYExist == false)
                {
                    if(runTime > MobYSpawnTime)
                    {
                        SpawnController.GetComponent<MobYManager>().SpawnMobY(spawnPose); // spawn MobX
                        MobYSpawnTime = runTime + MobYCycle;
                    }
                }
            }
        }
    }

    void CheckMobExist()
    {
        MobXExist = SpawnController.GetComponent<MobXManager>().MobXExist;
        MobYExist = SpawnController.GetComponent<MobYManager>().MobYExist;
    }
}
