using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }

    public static PlayerControl instance { get; private set; }

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
    
    Inventory inv;
    
    public Player player;

    //The last direction the player was facing. Useful for projectiles/abilities.
    public Vector2 lastDirection = Vector2.down;
    //Same as lastDirection, but lastInput also allows itself to be 0,0.
    private Vector2 lastInput = Vector2.zero;

    bool moving = false;

    float comboTimer = 0.0f;
    public float maxComboTime = 3.0f;

    Transform weapon;

    List<GameObject> pickableItems = new List<GameObject>();

    private bool _lockMovement;
    public bool lockMovement
    {
        get { return _lockMovement; }
        set { _lockMovement = value; }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {   
        inv = Inventory.instance;
        anim = GetComponent<Animator>();
        SetDirection(Vector2.down);

        weapon = transform.FindChild("Weapon");
    }

    void Update()
    {
        if(Input.GetButtonDown("Use"))
        {
            PickUpItems();
        }

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
        GameObject itemInstance = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
        itemInstance.GetComponent<ItemComponent>().Reset(itemData.item.id);
        itemInstance.GetComponent<ItemComponent>().SetStack(itemData.stackAmount);
    }
    
    void PickUpItems()
    {
        while (pickableItems.Count != 0)
        {
            ItemComponent item = pickableItems[0].GetComponent<ItemComponent>();
            for (int i = 0; i < item.stackAmount; i++)
            {
                inv.AddItem(item.itemID);
            }
            Destroy(pickableItems[0]);
            pickableItems.RemoveAt(0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<ItemComponent>())
        {
            pickableItems.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<ItemComponent>() && pickableItems.Contains(other.gameObject))
        {
            pickableItems.Remove(other.gameObject);
        }
    }
}
