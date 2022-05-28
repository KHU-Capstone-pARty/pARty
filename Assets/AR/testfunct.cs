using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class testfunct : MonoBehaviour
{
    public Button btn;

    void Start() 
    {
        btn.onClick.AddListener(
            ()=>{
                changeScene();
            }
        );
    }

    public void changeScene()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if(SceneManager.GetActiveScene().name != "MainScene")
            SceneManager.LoadScene("MainScene");
        else
            SceneManager.LoadScene("MultiTestScene");
    }
}
