using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Start is called before the first frame update
    void Start()
    {
        MobXExist = false;
    }

    // Update is called once per frame
    void Update()
    {
        ARCamPos = arCamera.transform.position;

        if(MobX == null) // MobX terminated
        {
            MobXExist = false;
        }
        if(MobXExist) 
        {
            runTime += Time.deltaTime;
            if(runTime < duration)
            {
                MobX.transform.position = Vector3.Lerp(getTmpPos() , ARCamPos, runTime / duration);
            }
            else
            {
                Destroy(MobX);
            }
        }
    }

    // spawn MobX
    public void SpawnMobX()
    {
        runTime = 0.0f;
        duration = 10.0f;

        MobX = Instantiate(MobXfab, getTmpPos() , Quaternion.Euler(new Vector3(0,180,0)));
        SpawnController.GetComponent<SpawnManager>().FieldMobCnt++;
        MobXExist = true;
    }

    // temporary position getter
    Vector3 getTmpPos()
    {
        return new Vector3(0,2,10);
    }
}
