using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithSkill : MonoBehaviour
{
    string Name { get => gameObject.name + (transform.position.x + transform.position.y + transform.position.z); }
    [SerializeField] PlayerSkills skillThatDestroysObject;

    private void Start()
    {
        if(PlayerPrefs.GetInt(Name,0)==1)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<PlayerWeapon>())
        {
            CheckSkills();
        }
    }

    void CheckSkills()
    {
        if(SaveCheckManager.Instance)
            if (SaveCheckManager.Instance.CheckIfHaveSkills(skillThatDestroysObject))
            {
                PlayerPrefs.SetInt(Name, 1);
                Destroy(gameObject);
            }
    }
}
