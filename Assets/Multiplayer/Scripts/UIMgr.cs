using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    public static UIMgr Singleton;
    public GameObject selectHostClientPanel;
    public GameObject anchorPanel;
    public GameObject debugPanel;
    public GameObject heartGroup;
    public List<GameObject> userIcons = new List<GameObject>();
    public GameObject startGameButton;
    public PlaneToggle planeToggle;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Singleton != this)
                Destroy(this.gameObject);
        }
    }

    public void DebugToggle()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }
}
