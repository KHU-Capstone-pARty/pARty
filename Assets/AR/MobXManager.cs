using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MobXManager : MonoBehaviour
{
    public GameObject MobXfab;
    GameObject MobX;
    Vector3 ARCamPos;
    public GameObject arCamera;
    float runTime;
    float duration;
    public GameObject SpawnController;
    public bool MobXExist;

    Vector3 spawnPos;
    Vector3 NexusPosition;

    public Text TextDebug2;

    // Start is called before the first frame update
    void Start()
    {
        MobXExist = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ARCamPos = arCamera.transform.position;
        
        if(MobX == null) // MobX terminated
        {
            MobXExist = false;
        }
        if(MobXExist) 
        {
            runTime += Time.deltaTime;
            if(runTime < duration)
            {
                MobX.transform.position = Vector3.Lerp(spawnPos , NexusPosition, runTime / duration);
            }
            else
            {
                SpawnController.GetComponent<SpawnManager>().CurrHP--;
                TextDebug2.text = "Nexus got attacked by MobX";
                Destroy(MobX);
            }
        }
    }

    // spawn MobX
    public void SpawnMobX(Vector3 _spawnPos)
    {
        runTime = 0.0f;
        duration = 10.0f;
        spawnPos = _spawnPos;
        NexusPosition = SpawnController.GetComponent<CreateNexus>().NexusPosition;

        MobX = Instantiate(MobXfab, spawnPos, Quaternion.LookRotation(spawnPos));
        SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
        MobXExist = true;
    }
}
