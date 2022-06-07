using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text TextMobCnt;

    public GameObject SpawnController;
    int CurrMobCnt;
    int ObjectiveMobCnt;

    int MaxHP;
    int CurrHP;

    public Sprite emptyHeartImg;
    public Image[] Heart;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CurrMobCnt = SpawnController.GetComponent<SpawnManager>().CurrMobCnt;
        ObjectiveMobCnt = SpawnController.GetComponent<SpawnManager>().ObjectiveMobCnt;
        TextMobCnt.text = "<b>" + CurrMobCnt + " / " + ObjectiveMobCnt + "</b>";
        
        MaxHP = SpawnController.GetComponent<SpawnManager>().MaxHP;
        CurrHP = SpawnController.GetComponent<SpawnManager>().CurrHP;
        UpdateHP();
    }

    void UpdateHP()
    {
        if(CurrHP < MaxHP)
            Heart[4-CurrHP].sprite = emptyHeartImg;
    }
}
