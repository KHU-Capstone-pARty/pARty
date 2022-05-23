using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceOnTouchPlane : MonoBehaviour
{
    //[SerializeField]
    //private Text info;
    [SerializeField]
    private GameObject spawnObj;
    [SerializeField]
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hitsList = new List<ARRaycastHit>();

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount>0)
        {
            var t = Input.GetTouch(0);
            if (t.phase==TouchPhase.Began)
            {
                //info.text = ("(" + t.position.x + ", " + t.position.y + "," + ")");
                if (raycastManager.Raycast(t.position, hitsList, TrackableType.PlaneWithinPolygon))
                {
                    var h = hitsList[0].pose;
                    Instantiate(spawnObj, h.position, h.rotation);
                    //info.text = ("(" + h.position.x + ", " + h.position.y + "," + h.position.z + ")");
                }
            }
        }
    }
}
