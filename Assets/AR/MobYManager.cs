using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class MobYManager : MonoBehaviour
{
    public GameObject MobYfab;
    GameObject MobY;
    Vector3 ARCamPos;
    public GameObject arCamera;
    float runTime;
    float duration;
    public GameObject SpawnController;
    public bool MobYExist;

    Vector3 spawnPos;
    Vector3 NexusPosition;

    public Text TextDebug2;

    // Start is called before the first frame update
    void Start()
    {
        MobYExist = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ARCamPos = arCamera.transform.position;

        if(MobY == null) // MobY terminated
        {
            MobYExist = false;
        }
        if(MobYExist) 
        {
            runTime += Time.deltaTime;
            if(runTime < duration)
            {
                MobY.transform.position = Vector3.Lerp(spawnPos , NexusPosition, runTime / duration);
            }
            else
            {
                SpawnController.GetComponent<SpawnManager>().CurrHP--;
                TextDebug2.text = "Nexus got attacked by MobY";
                Destroy(MobY);
            }
        }
    }

    // spawn MobY
    public void SpawnMobY(Vector3 _spawnPos)
    {
        runTime = 0.0f;
        duration = 10.0f;
        spawnPos = _spawnPos;
        NexusPosition = SpawnController.GetComponent<CreateNexus>().NexusPosition;

        MobY = Instantiate(MobYfab, spawnPos , Quaternion.Euler(new Vector3(0,180,0)));
        SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
        MobYExist = true;
    }
}
