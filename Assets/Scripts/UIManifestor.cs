using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManifestor : MonoBehaviour
{

    public GameObject toManifest;
    private GameObject manifested;
    public bool asNew;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (asNew) { manifested = Instantiate(toManifest, GameObject.Find("Canvas").transform); } // create UI in canvas
        else { toManifest.SetActive(true); }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //print("No longer in contact with " + other.transform.name);
        if (asNew) { Destroy(manifested.gameObject); } // create UI in canvas
        else { toManifest.SetActive(false); }
        
    }


}
