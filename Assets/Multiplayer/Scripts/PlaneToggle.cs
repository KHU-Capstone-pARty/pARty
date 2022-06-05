using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneToggle : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private bool status = true;

    private void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void Update() 
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(status);
        }
    }

    public void Toggle()
    {
        status = !status;
    }

    public void ToggleDebugText()
    {
        CloudAnchorMgr.Singleton.ToggleDebugText();
    }
}
