using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float moveSpeed = 0f;

    Animator anim;
    GameObject inv;
    bool inventoryCheck = false;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        inv = GameObject.Find("Inventory Panel");
        inv.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Inventory") && !inventoryCheck)
        {
            inv.SetActive(true);
            inventoryCheck = true;
        }

        else if(Input.GetButtonDown("Inventory") && inventoryCheck)
        {
            inv.SetActive(false);
            inventoryCheck = false;
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if(moveX != 0 || moveY != 0)
        {
            anim.SetBool("Moving", true);
        }

        else
        {
            anim.SetBool("Moving", false);
        }

        if(moveX < 0)
        {
            anim.SetInteger("Direction", 1);
        }

        else if(moveX > 0)
        {
            anim.SetInteger("Direction", 3);
        }

        else if(moveX == 0 && moveY > 0)
        {
            anim.SetInteger("Direction", 2);
        }

        else if(moveX == 0 && moveY < 0)
        {
            anim.SetInteger("Direction", 0);
        }

        anim.SetFloat("SpeedX", moveX);
        anim.SetFloat("SpeedY", moveY);

        GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed * moveX, moveSpeed * moveY);
    }
}
