using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerManager player;
    AnimatorManager animatorManager;
    CameraManager cameraManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool b_Input;
    public bool jump_Input;
    public bool x_Input;
    public bool attack_Input;
    public bool block_Input;
    public bool lockOn_Input;
    public bool menu_Input;
    public bool interact_Input;
    public bool save_Input;
    public bool actionTestInput;
    private Coroutine lockOnCoroutine;
    public string nextActionWanted;
    public float actionTiming;
    public float actionTimingMax;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        player = GetComponent<PlayerManager>();
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.B.performed += i => b_Input = true;
            playerControls.PlayerActions.B.canceled += i => b_Input = false;
            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
            playerControls.PlayerActions.X.performed += i => x_Input = true;
            playerControls.PlayerActions.Attack.performed += i => attack_Input = true;
            playerControls.PlayerActions.Block.performed += i => block_Input = true;
            playerControls.PlayerActions.Block.canceled += i => block_Input = false;
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerActions.Menu.performed += i => menu_Input = true;
            playerControls.PlayerActions.Interact.performed += i => interact_Input = true;
            playerControls.PlayerActions.SaveTest.performed += i => save_Input = true;
            playerControls.PlayerActions.TestActionQueue.performed += i => actionTestInput = true;
        }

        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    public void HandleAllInputs()
    {
        HandleSaveInput();
        HandleMenuInput();
        HandleInteractInput();
        if (player.inMenu) 
        {
            cameraInputX = 0;
            cameraInputY = 0;
            return;
        }
        HandleLockOnInput();
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleDodgeInput();
        HandleAttackInput();
        HandleBlockInput();

        actionTiming -= Time.deltaTime;
        Debug.Log(nextActionWanted);
        if (actionTiming < 0)
            nextActionWanted = string.Empty;

    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;


        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;



        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        if (player.playerAttackAndWeaponManager.isLockedOn == true && player.playerLocomotion.isSprinting == false)
        {
            animatorManager.UpdateAnimatorValues(horizontalInput, verticalInput, false);
        }
        else
        {
            animatorManager.UpdateAnimatorValues(0, moveAmount, player.playerLocomotion.isSprinting);
        }

        

    }
    private void HandleSprintingInput()
    {
        if (b_Input && moveAmount > 0.5f && player.currentStamina > player.sprintStaminaConsumption)
        {
            player.playerLocomotion.isSprinting = true;
        }
        else 
        { 
            player.playerLocomotion.isSprinting = false;
        }
    }
    private void HandleJumpingInput()
    {
        if (jump_Input)
        {
            nextActionWanted = "Jump";
            actionTiming = actionTimingMax;
            jump_Input = false;
        }
        if (nextActionWanted == "Jump")
        {    
            player.playerLocomotion.HandleJumping();
        }
    }
    private void HandleDodgeInput()
    {
        if (x_Input)
        {
            x_Input = false;
            player.playerLocomotion.HandleDodge();
        }
    }

    private void HandleAttackInput()
    {
        if (attack_Input)
        {
            nextActionWanted = "LightAttack";
            actionTiming = actionTimingMax;
            attack_Input = false;
        }
        if (nextActionWanted == "LightAttack")
        {
            player.playerAttackAndWeaponManager.HandleAttack();
        }
    }
    private void HandleBlockInput()
    {
        if (block_Input)
        {
            nextActionWanted = "HeavyAttack";
            actionTiming = actionTimingMax;
            block_Input = false;
        }
        if (nextActionWanted == "HeavyAttack")
        {
            player.playerAttackAndWeaponManager.HandleAttack();
        }
    }
    private void HandleLockOnInput()
    {
        if (lockOn_Input && player.playerAttackAndWeaponManager.isLockedOn)
        {
            lockOn_Input = false;
            cameraManager.ClearLockOnTargets();
            player.playerAttackAndWeaponManager.isLockedOn = false;
            player.playerAttackAndWeaponManager.currentTarget = null;
            return;
        }
        if (lockOn_Input && !player.playerAttackAndWeaponManager.isLockedOn)
        {
            lockOn_Input = false;
            cameraManager.HandleLocatingLockOnTargets();

            if (cameraManager.nearestLockOnTarget != null)
            {
                player.playerAttackAndWeaponManager.SetTarget(cameraManager.nearestLockOnTarget);
                player.playerAttackAndWeaponManager.isLockedOn = true;
            }

        }



        if (player.playerAttackAndWeaponManager.isLockedOn)
        {
            if (player.playerAttackAndWeaponManager.currentTarget == null)
                return;
            if (cameraManager.unavalibleLockOnTarget)
            {
                player.playerAttackAndWeaponManager.isLockedOn = false;
            }
            if (lockOnCoroutine != null)
                StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = StartCoroutine(cameraManager.WaitThenFindNewTarget());
        }
        
    }
    private void HandleMenuInput() 
    {
        if (menu_Input) 
        {
            player.OpenMenu();
            menu_Input = false;
        }
        if (lockOn_Input)
        {
            player.inventoryManager.AddItemToInventory("ItemTest", 1);
            lockOn_Input = false;
            Debug.Log("huh");
        }
    }
    private void HandleInteractInput()
    {
        if (interact_Input)
        {
            player.inventoryManager.AddItemToInventory("WeaponTest", 5, new string[] { "no", "yes" });
            interact_Input = false;
        }
    }
    private void HandleSaveInput() 
    {
        if (save_Input)
        {
            DataPersistenceManager.instance.SaveGame();
            
            save_Input = false;
        }
    }
}

// wakatime do you work?