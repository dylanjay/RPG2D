using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public static PlayerControl instance { get { return _instance; } }
    private static PlayerControl _instance;

    public float moveSpeed = 0f;

    Animator anim;
    GameObject inventoryPanel;
    GameObject equipmentPanel;
    Inventory inv;
    Transform slotPanel;
    Tooltip tooltip;
    public bool inventoryCheck = false;

    Vector2 lerpPos;
    Vector2 curPos;
    public float lerpSpeed;
    bool startLerp = false;
    float lerpLength;
    float startTime;

    // Use this for initialization
    void Awake()
    {

    }

    void Start()
    {
        _instance = this;
        inv = Inventory.instance;
        anim = GetComponent<Animator>();
        inventoryPanel = GameObject.Find("Inventory Panel");
        equipmentPanel = GameObject.Find("Equipment Panel");
        slotPanel = GameObject.Find("Slot Panel").transform;
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();

        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            inv.AddItem(0);
        }

        if (Input.GetButtonDown("Inventory") && !inventoryCheck)
        {
            inventoryPanel.SetActive(true);
            equipmentPanel.SetActive(true);
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
            equipmentPanel.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if(startLerp)
        {
            float distCovered = (Time.time - startTime) * lerpSpeed;
            float fracLerp = distCovered / lerpLength;
            transform.position = Vector2.Lerp(curPos, lerpPos, fracLerp);
        }   
        
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

    void MoveTo(Vector2 position)
    {
        lerpPos = position;
        curPos = transform.position;
        lerpLength = Vector2.Distance(lerpPos, curPos);
        startTime = Time.time;
        while(new Vector2(transform.position.x, transform.position.y) != position)
        {
            startLerp = true;
        }
    }

    public void DropItem(Vector2 position, ItemData itemData)
    {
        //MoveTo(position);
        /*if (slotPanel.GetChild(slotPanel.childCount - 1).GetComponent<ItemData>())
        {
            Destroy(slotPanel.GetChild(slotPanel.childCount - 1).gameObject);
        }*/
        GameObject itemInstance = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
        itemInstance.GetComponent<ItemComponent>().reset(itemData.item.id);
        itemInstance.GetComponent<ItemComponent>().setStack(itemData.stackAmount);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.GetComponent<ItemComponent>() && Input.GetButtonDown("Use"))
        {
            for (int i = 0; i < other.GetComponent<ItemComponent>().stackAmount; i++)
            {
                inv.AddItem(other.GetComponent<ItemComponent>().itemID);
            }
            Destroy(other.gameObject);
        }
    }
}
