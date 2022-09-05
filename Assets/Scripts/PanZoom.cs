using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;

    public float wait;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            timer = wait;
        }
        if (Input.GetMouseButton(0)) {

            if (timer > 0) { timer -= Time.deltaTime; }
            else 
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
            }
            
        }
    }
}
