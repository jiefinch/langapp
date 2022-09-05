using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    private bool facingRight;
    private Vector3 target;

    public Animator animator;
    

    void Start()
    {
        target = transform.position;
        facingRight = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) // no click ui
            {
                target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = transform.position.z;

                float d = transform.position.x - target.x; // direction of movement
                if (facingRight && d > 0)
                {
                    facingRight = false;
                    flip();
                }
                if (!facingRight && d < 0)
                {
                    facingRight = true;
                    flip();
                }
            }
         
            
            
        }
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f) { animator.SetBool("isWalking", false); }
        else { animator.SetBool("isWalking", true); }

    }

    private void flip() 
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        //transform.Rotate(new Vector3(0, 180, 0));
    }

    void OnCollisionEnter2D(Collision2D col) { target = transform.position; } // stop moving once hit obj

}
