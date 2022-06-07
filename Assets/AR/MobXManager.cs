using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MobXManager : MonoBehaviour
{
    public GameObject MobX;

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
        duration = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        if(runTime < duration)
        {
            MobX.transform.position = Vector3.Lerp(spawnPos, NexusPosition, runTime / duration);
        }
        else // when Monster attacked nexus
        {
            SpawnController.GetComponent<SpawnManager>().CurrHP--;
            TextMsg.text = "Nexus got attacked!";
            Destroy(MobX);
        }
    }
}
