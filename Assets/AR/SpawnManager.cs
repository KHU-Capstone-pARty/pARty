using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int FieldMobCnt; // Number of mobs in field
    public int CurrMobCnt; // Number of mobs terminated
    public int ObjectiveMobCnt; // Number of mobs to terminate
    public GameObject SpawnController;

    public bool MobXExist; // If MobX exists, set true
    public bool MobYExist; // If MobX exists, set true

    float runTime;
    int MobXCycle; // cycle time for respawn
    int MobYCycle; // cycle time for respawn
    float MobXSpawnTime; // 
    float MobYSpawnTime;

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
            if(MobXExist == false)
            {
                if(runTime > MobXSpawnTime)
                {
                    SpawnController.GetComponent<MobXManager>().SpawnMobX(); // spawn MobX
                    MobXSpawnTime = runTime + MobXCycle;
                }
            }
            if(MobYExist == false)
            {
                if(runTime > MobYSpawnTime)
                {
                    SpawnController.GetComponent<MobYManager>().SpawnMobY(); // spawn MobX
                    MobYSpawnTime = runTime + MobYCycle;
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
