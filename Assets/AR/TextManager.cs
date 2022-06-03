using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text TextMsg;
    public Text TextHP;

    public GameObject SpawnController;
    int CurrMobCnt;
    int ObjectiveMobCnt;

    int MaxHP;
    int CurrHP;

    // Start is called before the first frame update
    void Start()
    {
        TextMsg.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        CurrMobCnt = SpawnController.GetComponent<SpawnManager>().CurrMobCnt;
        ObjectiveMobCnt = SpawnController.GetComponent<SpawnManager>().ObjectiveMobCnt;
        TextMsg.text = CurrMobCnt + " / " + ObjectiveMobCnt;
        
        MaxHP = SpawnController.GetComponent<SpawnManager>().MaxHP;
        CurrHP = SpawnController.GetComponent<SpawnManager>().CurrHP;
        TextHP.text = "Your HP : " + CurrHP + " / " + MaxHP;
    }
}
