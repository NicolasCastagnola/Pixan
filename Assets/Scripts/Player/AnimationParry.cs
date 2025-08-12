using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationParry : MonoBehaviour
{
    [SerializeField] SphereCollider col;
    [SerializeField] BoxCollider weapon;

    bool isLooping = false;

    public void EnableCol()
    {
        if (isLooping) return;

        isLooping = true;
        col.enabled = true;
    }

    public void DisableCol()
    {
        col.enabled = false;
    }

    public void EnableColWeapon()
    {
        weapon.enabled = true;

        GameManager.instance.WaitForXSeconds(0.5f, () => DisableColWeapon());
    }

    public void DisableColWeapon()
    {
        weapon.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1)) isLooping = false;
    }
}
