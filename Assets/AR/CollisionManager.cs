using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public GameObject SpawnController;

    // Start is called before the first frame update
    void Start()
    {
        SpawnController = GameObject.Find("SpawnController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnCollisionEnter(Collision collision) 
    {
        GameObject parent = transform.parent.gameObject;
        if (collision.collider.gameObject.CompareTag("Ball"))
        {
            //Collider coll = parent.GetComponent<Collider>();
            //coll.attachedRigidbody.useGravity = true;
            Destroy(parent);
            SpawnController.GetComponent<SpawnManager>().CurrMobCnt++;
            SpawnController.GetComponent<SpawnManager>().FieldMobCnt--;
        }
    } 
}
