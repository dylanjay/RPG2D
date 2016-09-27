using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }

    public static PlayerControl instance { get { return _instance; } }
    private static PlayerControl _instance;

    //These two variables are dependent on one another to work. Please keep them in order.
    //Usage: anim.SetBool(AnimParamIDs[(int)AnimParams.Moving], true);
    public enum AnimParams{ Direction, Moving, SpeedX, SpeedY, Swing }
    public static int[] AnimParamIDs = new int[]
    {
        Animator.StringToHash("Direction"),
        Animator.StringToHash("Moving"),
        Animator.StringToHash("SpeedX"),
        Animator.StringToHash("SpeedY"),
        Animator.StringToHash("Swing")
    };

    public float moveSpeed = 0f;

    public Animator anim;
    GameObject inventoryPanel;
    GameObject equipmentPanel;
    Inventory inv;
    Transform slotPanel;
    Tooltip tooltip;
    public bool inventoryCheck = false;
    
    public Player player;

    public bool swing = false;

    //The last direction the player was facing. Useful for projectiles/abilities.
    public Vector2 lastDirection = Vector2.down;
    //Same as lastDirection, but lastInput also allows itself to be 0,0.
    private Vector2 lastInput = Vector2.zero;
    bool moving = false;

    public float comboTimer = 3.0f;

    Transform weapon;

    private bool _lockMovement;
    public bool lockMovement
    {
        get { return _lockMovement; }
        set { _lockMovement = value; }
    }

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
        weapon = transform.FindChild("Weapon");
    }

    void Update()
    {
        if(comboTimer <= 0)
        {
            Player.instance.ResetCombo();
            comboTimer = 3.0f;
        }

        if(Player.instance.combo > 0)
        {
            comboTimer -= Time.deltaTime;
        }

        if(!swing)
        {
            anim.SetBool(AnimParamIDs[(int)AnimParams.Swing], false);
            //swing = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //anim.SetBool(AnimParamIDs[(int)AnimParams.Swing], true);
            anim.SetTrigger(AnimParamIDs[(int)AnimParams.Swing]);
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

        if (!_lockMovement)
        {
            UpdatePlayerMovementInput();
        }
    }

    void UpdatePlayerMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 curDirection = new Vector2(moveX, moveY);

        //Store whether or not the input changed between frames, then write to last input.
        bool inputChanged = curDirection != lastInput;
        lastInput = curDirection;
        if (inputChanged)
        {
            curDirection.Normalize();

            if (moveX != 0 || moveY != 0)
            {
                anim.SetBool(AnimParamIDs[(int)AnimParams.Moving], true);
                moving = true;
                lastDirection.Set(moveX, moveY);
                lastDirection.Normalize();
            }
            else
            {
                anim.SetBool(AnimParamIDs[(int)AnimParams.Moving], false);
                moving = false;
            }

            if (moveX != 0)
            {
                anim.SetInteger(AnimParamIDs[(int)AnimParams.Direction], (int)(moveX * -1) + 2);
                weapon.localPosition = new Vector2(moveX / Mathf.Abs(moveX) / 4, 0);
                if(moveX < 0)
                {
                    weapon.eulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, 180);
                }
                else
                {
                    weapon.eulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, 0);
                }
            }
            else if (moveY != 0)
            {
                anim.SetInteger(AnimParamIDs[(int)AnimParams.Direction], (int)moveY * -1 + 1);
                weapon.localPosition = new Vector2(0, moveY / Mathf.Abs(moveY) / 4);
                if (moveY < 0)
                {
                    weapon.eulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, 270);
                }
                else
                {
                    weapon.eulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, 90);
                }
            }
            
            anim.SetFloat(AnimParamIDs[(int)AnimParams.SpeedX], moveSpeed * lastDirection.x);
            anim.SetFloat(AnimParamIDs[(int)AnimParams.SpeedY], moveSpeed * lastDirection.y);
        }

    }

    void FixedUpdate()
    {
        if (moving)
        {
            transform.Translate(lastDirection * (moveSpeed * Time.deltaTime));
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
