using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiguelFinalAnimatorController : EntityAnimatorController
{

    public Transform leftAreaPivot, rightAreaPivot;
    [SerializeField] MiguelAreaAttack miguelAreaPrefab;
    [SerializeField] MiguelFinalBoss migue;

    public void LeftAttack()
    => Instantiate(miguelAreaPrefab, leftAreaPivot.position, Quaternion.Euler(new Vector3Int(-90,0,0)));
    public void RightAttack()
    => Instantiate(miguelAreaPrefab, rightAreaPivot.position, Quaternion.Euler(new Vector3Int(-90, 0, 0)));

    public void EnableBoss()
    {
        _animator.SetBool("Enable",true);
    }
    public void OnDead()
    {
        _animator.SetBool("Death",true);
    }
}
