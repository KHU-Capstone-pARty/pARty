using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class MobYManager : MonoBehaviour
{
    public GameObject MobY;
    
    float runTime;
    float duration;

    [HideInInspector]
    public GameObject SpawnController;

    [HideInInspector]
    public Vector3 spawnPos;
    [HideInInspector]
    public Vector3 NexusPosition;

    [HideInInspector]
    public Text TextMsg;

    void Awake()
    {
        SpawnController = GameObject.Find("SpawnController");
        TextMsg = GameObject.Find("Text_Msg").GetComponent<Text>();

        runTime = 0.0f;
        duration = 9.0f;
    }

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        if(runTime < duration)
        {
            MobY.transform.position = Vector3.Lerp(spawnPos , NexusPosition, runTime / duration);
        }
        else
        {
            SpawnController.GetComponent<SpawnManager>().CurrHP--;
            TextMsg.text = "Nexus got attacked!";
            Destroy(MobY);
        }
    }
}
