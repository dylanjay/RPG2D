using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public static PlayerControl instance { get { return _instance; } }
    private static PlayerControl _instance;

    public float moveSpeed = 0f;

    Animator anim;
    GameObject inventoryPanel;
    Inventory inv;
    Transform slotPanel;
    Tooltip tooltip;
    public bool inventoryCheck = false;

    // Use this for initialization
    void Start()
    {
        _instance = this;
        inv = Inventory.instance;
        anim = GetComponent<Animator>();
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = GameObject.Find("Slot Panel").transform;
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();
        inventoryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !inventoryCheck)
        {
            inventoryPanel.SetActive(true);
            inventoryCheck = true;
        }

        else if(Input.GetButtonDown("Inventory") && inventoryCheck)
        {
            inventoryCheck = false;

            if (tooltip.tooltip.activeSelf)
            {
                tooltip.Deactivate();
            }

            if (slotPanel.GetChild(slotPanel.childCount - 1).GetComponent<ItemData>())
            {
                slotPanel.GetChild(slotPanel.childCount - 1).GetComponent<ItemData>().Reset();
            }

            inventoryPanel.SetActive(false);
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

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.GetComponent<ItemComponent>() && Input.GetButtonDown("Use"))
        {
            inv.AddItem(other.GetComponent<ItemComponent>().itemID);
            other.gameObject.SetActive(false);
        }
    }
}
