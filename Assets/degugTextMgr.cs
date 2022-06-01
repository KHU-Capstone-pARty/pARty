using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class degugTextMgr : MonoBehaviour
{
    public Text TextDebug;

    // Update is called once per frame
    void Update()
    {
        if(TextDebug.text.Length > 200)
            TextDebug.text = TextDebug.text.Substring(150);
    }
}
