using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlayerPrefabRef : MonoBehaviour
{
    public ARAnchorManager anchorMgr;
    public Camera arCam;
    public ARRaycastManager raycastManager;
    public GameObject anchorPrefab;
    public Text text_log;
    public Text text_State;
    public List<GameObject> ARSyncPrefab = new List<GameObject>();
    public Button hostAnchorButton;
    public Button resolveAnchorButton;
    public Toggle placeTestObjToggle;
    public Toggle placeMonster;
}
