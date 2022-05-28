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

    // Start is called before the first frame update
    void Start()
    {
        MobYExist = false;
    }

    // Update is called once per frame
    void Update()
    {
        ARCamPos = arCamera.transform.position;

        if(MobY == null) // MobY terminated
        {
            MobYExist = false;
        }
        if(MobYExist) 
        {
            runTime += Time.deltaTime;
            if(runTime < duration)
            {
                MobY.transform.position = Vector3.Lerp(getTmpPos() , ARCamPos, runTime / duration);
            }
            else
            {
                Destroy(MobY);
            }
        }
    }

    // spawn MobY
    public void SpawnMobY()
    {
        runTime = 0.0f;
        duration = 10.0f;

        MobY = Instantiate(MobYfab, getTmpPos() , Quaternion.Euler(new Vector3(0,180,0)));
        SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
        MobYExist = true;
    }

    // temporary position getter
    Vector3 getTmpPos()
    {
        return new Vector3(0,2,10);
    }
}
