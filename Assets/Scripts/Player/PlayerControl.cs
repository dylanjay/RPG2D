using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }
    public enum Menu { Empty = 0, Main = 1, Inventory = 2, Skills = 3 }

    public static PlayerControl instance { get { return _instance; } }
    private static PlayerControl _instance;

    //Basically an enum, but C# enums do not support values from functions, even if they are static.
    //When Unity 5.5 drops with .NET 4.6 support, we might want to look into this:
    //http://unity3de.blogspot.com/2013/12/create-enum-template-for-your.html
    public static class AnimParams
    {
        public static readonly int Direction = Animator.StringToHash("Direction");
        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int SpeedX = Animator.StringToHash("SpeedX");
        public static readonly int SpeedY = Animator.StringToHash("SpeedY");
        public static readonly int Swing = Animator.StringToHash("Swing");
    }

    public float moveSpeed = 0f;

    public Animator anim;
    GameObject inventoryPanel;
    GameObject equipmentPanel;
    GameObject skillsPanel;
    GameObject mainMenuPanel;
    Inventory inv;
    Transform slotPanel;
    Tooltip tooltip;
    
    public Player player;

    //The last direction the player was facing. Useful for projectiles/abilities.
    public Vector2 lastDirection = Vector2.down;
    //Same as lastDirection, but lastInput also allows itself to be 0,0.
    private Vector2 lastInput = Vector2.zero;

    Menu activeMenu = Menu.Empty;

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
        _instance = this;
    }

    void Start()
    {   
        inv = Inventory.instance;
        anim = GetComponent<Animator>();

        //CR: For encapsulation reasons, it would make more sense if we moved UI and Menu Management
        //inside of a script attached to the Canvas. This way the PlayerControl doesn't have to require a UI
        //in order to function properly. It will also get rid of all of these ugly GameObject.Find calls.

        inventoryPanel = GameObject.Find("Inventory Panel");
        equipmentPanel = GameObject.Find("Equipment Panel");
        skillsPanel = GameObject.Find("Skill Tree Panel");
        mainMenuPanel = GameObject.Find("Main Menu Panel");
        slotPanel = GameObject.Find("Slot Panel").transform;
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        skillsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        weapon = transform.FindChild("Weapon");
    }

    void Update()
    {
        MenuManager();

        ComboManager();

        if (_lockMovement)
        {
            anim.SetBool(AnimParams.Moving, false);
            lastInput = Vector2.zero;
            lastDirection = Vector2.zero;
        }
        else
        {
            UpdatePlayerMovementInput();
        }
        
    }

    void MenuCloser()
    {
        switch (activeMenu)
        {
            case Menu.Main:
                mainMenuPanel.SetActive(false);
                break;

            case Menu.Inventory:
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
                break;

            case Menu.Skills:
                skillsPanel.SetActive(false);
                break;
        }
        activeMenu = Menu.Empty;
    }

    void MenuOpener(Menu menuQueued)
    {
        activeMenu = menuQueued;

        switch (activeMenu)
        {
            case Menu.Main:
                mainMenuPanel.SetActive(true);
                break;

            case Menu.Inventory:
                inventoryPanel.SetActive(true);
                equipmentPanel.SetActive(true);
                break;

            case Menu.Skills:
                skillsPanel.SetActive(true);
                break;
        }
    }

    void MenuManager()
    {
        Menu menuQueued = Menu.Empty;
        
        if(Input.GetButtonDown("Main Menu"))
        {
            menuQueued = Menu.Main;
        }

        else if(Input.GetButtonDown("Inventory"))
        {
            menuQueued = Menu.Inventory;
        }

        else if (Input.GetButtonDown("Skills"))
        {
            menuQueued = Menu.Skills;
        }

        else if(Input.GetButtonDown("Escape"))
        {
            MenuCloser();
        }

        //CR: The branching here should be cleaned up.
        //If the first if statement's condition is false, then the second two already have
        //their first condition true no matter what.
        //Also, the logic can be reduced:
        //Menu currentActiveMenu = activeMenu;
        //if(currentActiveMenu != Menu.Empty){MenuCloser();}
        //if(currentActiveMenu != menuQueued){MenuOpener();}

        if (activeMenu == Menu.Empty)
        {
            MenuOpener(menuQueued);
        }

        else if(activeMenu != Menu.Empty && menuQueued == activeMenu)
        {
            MenuCloser();
        }

        else if(activeMenu != Menu.Empty && menuQueued != Menu.Empty)
        {
            MenuCloser();
            MenuOpener(menuQueued);
        }
    }
    
    void ComboManager()
    {
        if (comboTimer <= 0)
        {
            Player.instance.ResetCombo();
            //CR: Should use a field instead of a magic number, like private float maxComboTime = 3.0f
            //This way it's possible to have the value increase as the player levels up,
            //or make it tweakable in the inspector.
            comboTimer = 3.0f;
        }

        if (Player.instance.combo > 0)
        {
            comboTimer -= Time.deltaTime;
        }
    }

    public void SetDirection(Vector2 dir)
    {
        anim.SetFloat(AnimParams.SpeedX, moveSpeed * dir.x);
        anim.SetFloat(AnimParams.SpeedY, moveSpeed * dir.y);
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
                anim.SetBool(AnimParams.Moving, true);
                moving = true;
                lastDirection.Set(moveX, moveY);
                lastDirection.Normalize();
            }
            else
            {
                anim.SetBool(AnimParams.Moving, false);
                moving = false;
            }

            //TODO: Eventually make this with less branches for easier maintainability.
            if (moveX != 0)
            {
                //Wait, why 2?
                anim.SetInteger(AnimParams.Direction, (int)(moveX * -1) + 2);
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
                anim.SetInteger(AnimParams.Direction, (int)moveY * -1 + 1);
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
            
            anim.SetFloat(AnimParams.SpeedX, moveSpeed * lastDirection.x);
            anim.SetFloat(AnimParams.SpeedY, moveSpeed * lastDirection.y);
        }

    }

    void FixedUpdate()
    {
        if (moving && !_lockMovement)
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

    //CR: We should implement a list/queue of possible items to collect instead.
    //This will prevent what is essentially a FixedUpate function that runs whenever
    //the player stands over an object.
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
