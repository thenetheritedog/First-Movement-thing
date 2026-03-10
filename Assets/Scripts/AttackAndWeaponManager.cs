using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttackAndWeaponManager : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    public Collider weaponCollider;
    public Transform lockOnTransform;
    public bool isLockedOn;
    public CharacterManager currentTarget;
    public bool isHealing;
    [SerializeField] private float damage;
    [SerializeField] private float poiseDamage;
    [SerializeField] private bool isParrying;
    [SerializeField] private ParticleSystem imNotGoingToSugercoatIt;
    private ParticleSystem parryInstance;
    public List<AttackCombo> combos = new();
    private List<string> currentInputs = new();
    private void Awake()
    {
        AttackCombo Light = new AttackCombo
        {
            comboName = "Light Combo",
            inputSequence = new List<string>()
            {
                "LightAttack"
            }
        };
        AttackCombo Heavy = new AttackCombo
        {
            comboName = "Heavy Combo",
            inputSequence = new List<string>()
            {
                "HeavyAttack"
            }
        };
        AttackCombo LightLight = new AttackCombo
        {
            comboName = "Light Light Combo",
            inputSequence = new List<string>()
            {
                "LightAttack",
                "LightAttack"
            }
        };
        AttackCombo LightHeavy = new AttackCombo
        {
            comboName = "Light Heavy Combo",
            inputSequence = new List<string>()
            {
                "LightAttack",
                "HeavyAttack"
            }
        };
        combos.Add(LightLight);
        combos.Add(LightHeavy);
        combos.Add(Light);
        combos.Add(Heavy);
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();

    }
    public void HandleAttack()
    {
        if (playerManager.currentStamina < playerManager.attackStaminaConsumption) return;
        //if (!playerManager.isInteracting && !playerManager.isUsingRootMotion)
        //animatorManager.PlayTargetAnimation("LightAttack", true, true);
        if (playerManager.inputManager.actionTestInput)
        {
            currentInputs.Add(playerManager.inputManager.nextActionWanted);
            CheckForCombo();
            playerManager.inputManager.nextActionWanted = string.Empty;
        }
    }
    public void TakeDamage(float enemyDamage, float enemyPoiseDamage, bool fury, bool grab, CharacterManager enemy)
    {
        if (isParrying && !grab)
        {
            if (fury) enemyPoiseDamage *= 2;
            enemy.TakeDamage(0, enemyPoiseDamage);
            parryInstance = Instantiate(imNotGoingToSugercoatIt, transform.position, Quaternion.LookRotation(transform.forward));
            playerManager.deflectSfx.Play();
        }
        else if (playerManager.isBlocking && playerManager.currentStamina > enemyPoiseDamage / 2 && !grab && !fury)
        {
            playerManager.currentStamina -= enemyPoiseDamage / 2;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            playerManager.blockSfx.Play();
        }
        else
        {
            // the is blocking is stated to false incase the player is hit without enough stamina
            playerManager.isBlocking = false;
            playerManager.currentHealth -= enemyDamage;
            playerManager.currentStamina -= enemyPoiseDamage;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            animatorManager.PlayTargetAnimation("Hit", true);
            playerManager.hitSfx.Play();
        }
        Vector3 playerVelocity = playerManager.playerLocomotion.playerRigidbody.linearVelocity;
        playerVelocity.x = 0;
        playerVelocity.z = 0;
        playerManager.playerLocomotion.playerRigidbody.linearVelocity = playerVelocity;


        playerManager.healthBar.UpdateHealthBar(playerManager.maxHealth, playerManager.currentHealth);
    }
    public void HandleBlock()
    {
        if ((!playerManager.isInteracting) && playerManager.playerLocomotion.isGrounded)
        {
            // FIX LATER
        }
    }
    public void EnableWeaponCollider(string properties)
    {
        string[] values = properties.Split(';');
        damage = float.Parse(values[0]);
        poiseDamage = float.Parse(values[1]);
        weaponCollider.enabled = true;
        playerManager.currentStamina -= playerManager.attackStaminaConsumption;
        playerManager.recoverySpentTime = 0;
        playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
        animatorManager.animator.SetBool(("isInteracting"), false);
    }

    public void SwingSound()
    {
        playerManager.attackSfx.Play();
    }

    public void EnableDodgeCollider()
    {
        playerManager.hitboxCollider.enabled = true;
    }
    public void DisableDodgeCollider()
    {
        playerManager.hitboxCollider.enabled = false;
    }
    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
        playerManager.cameraManager.SetLockCameraHeight();
    }

    public void EnableParry()
    {
        isParrying = true;
    }
    public void DisableParry()
    {
        isParrying = false;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            CharacterManager enemy = other.GetComponent<CharacterManager>();
            enemy.TakeDamage(damage, poiseDamage);
        }
    }
    void CheckForCombo()
    {
        foreach (var combo in combos)
        {
            if (IsExactMatch(combo.inputSequence))
            {
                Debug.Log(combo.comboName);
                return;
            }
        }
        currentInputs.Clear();
        currentInputs.Add(playerManager.inputManager.nextActionWanted);
        foreach (var combo in combos)
        {
            if (IsExactMatch(combo.inputSequence))
            {
                Debug.Log(combo.comboName);
                return;
            }
        }
    }
    bool IsExactMatch(List<string> sequence)
    {
        if (sequence.Count != currentInputs.Count)
            return false;
        for (int i = 0; i < sequence.Count; i++)
        {
            if (sequence[i] != (currentInputs[i])) 
                return false;
        }
        return true;
    }
}
#region Combo Data Class
[Serializable]
public class AttackCombo
{
    public string comboName;
    public List<string> inputSequence;
}
#endregion
