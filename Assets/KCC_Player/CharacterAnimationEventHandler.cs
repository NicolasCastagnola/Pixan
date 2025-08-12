using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class CharacterAnimationEventHandler : MonoBehaviour
{
    public Vector3 normalCollider, chargeCollider;

    [SerializeField] private ParryComponent ParryComponent;


    CharacterControllerKCC characterController;
    Player player;
    public GameObject gameObjectWeapon;
    BoxCollider weaponCollider;
    public GameObject gameObjectParry;
    Collider parryCollider;
    public Animator weaponPlayer;
    public ParticleSystem particleSystem;
    public GameObject ChargeBulletPrefab;

    private void Start()
    {
        characterController = GetComponentInParent<CharacterControllerKCC>();
        player = GetComponentInParent<Player>();
        weaponCollider = gameObjectWeapon.GetComponent<BoxCollider>();
        parryCollider = gameObjectParry.GetComponent<Collider>();
    }

    public void AnimationFinished()
    {
        characterController.AnimationFinished();
    }

    public void ActionFinished()
    {
        characterController.ActionFinished();
    }

    public void EnableWeaponColission()
    =>EnableWeaponColl(normalCollider);

    public void EnableBigWeaponColission()
    {
        var obj = Instantiate(ChargeBulletPrefab, transform.position+Vector3.up*2, Quaternion.identity);
        //obj.transform.localScale = Vector3.zero;
        obj.transform.forward = characterController.transform.forward;
        var objPlayerWeapon = obj.GetComponentInChildren<PlayerWeapon>();
        objPlayerWeapon.owner = player;
        objPlayerWeapon.damageEntitiesOnce = true;

    }

    void EnableWeaponColl(Vector3 size)
    {
        weaponCollider.size=size;
        weaponCollider.enabled = true;
    }

    public void DisableWeaponColission()
    {
        weaponCollider.enabled = false;
    }

    public void AttackWeaponAnim()
    {
        weaponPlayer.SetBool("IsAttacking", true);
    }
    public void IdleWeaponAnim()
    {
        weaponPlayer.SetBool("IsAttacking", false);
    }
    public void EnableBlockingColission()
    {
        parryCollider.enabled = true;
        
        DisableParry();
    }
    public void DisableBlockingColission()
    {
        DisableParry();
        
        parryCollider.enabled = false;
    }

    public void EnableParry() => ParryComponent.SetParryState(true);
    public void DisableParry() => ParryComponent.SetParryState(false);

    public void CanAttack()
    {
        characterController.CanAttack();
    }
    public void CantAttack()
    {
        characterController.CantAttack();
    }

    public void EnableParticleSystem()
    {
        particleSystem.Play();
    }

    public void DisableParticleSystem()
    {
        particleSystem.Stop();
    }

    public void EnablePlayerDmg()
    {
        player.EnableDamage();
    }
    public void DisablePlayerDmg()
    {
        player.DisableDamage();
    }

    public bool IsMoving()
    {
        if (player.Inputs.MoveAxisRight == 0 && player.Inputs.MoveAxisForward == 0)
        {
            return false;
        }
        return true;
    }
}