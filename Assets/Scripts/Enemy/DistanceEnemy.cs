// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
//
// public class DistanceEnemy : Enemy
// {
//     [SerializeField] GameObject projectileGameObject;
//
//     [SerializeField] Transform projectilePivot;
//
//     GameObject currentProjectileGameObject;
//
//     public override void EnemySetStates()
//     {
//         OnStart += () => {
//             StateMachine = new StateMachine();
//             StateMachine.AddState(States.EnemyIdle, new DistanceEnemyIdleState(this, viewRange));
//             StateMachine.AddState(States.EnemyAttack, new EnemyAttackState(this, attackTime)/*Cambiar anim por flechas*/);
//             StateMachine.AddState(States.EnemyStun, new EnemyStunState(this));
//             StateMachine.AddState(States.EnemyHurt, new EnemyHurtState(this));
//             StateMachine.AddState(States.EnemyDeath, new EnemyDeathState(this));
//             StateMachine.ChangeState(States.EnemyIdle);
//
//             if(EnemyManager.Instance!=null)
//                 EnemyManager.Instance.Addenemy(this);
//         };
//     }
//     public override void TakeDamage(float amount)
//     {
//         var current = StateMachine.GetCurrentState();
//
//         if (current != States.EnemyDeath || current != States.EnemyHurt &&_currentHealth>0)
//             StateMachine.ChangeState(States.EnemyHurt);
//
//         base.TakeDamage(amount);
//     }
//     public void PrepareProjectile()
//     {
//         currentProjectileGameObject = Instantiate(projectileGameObject, projectilePivot.position, Quaternion.identity);
//
//         // Set the target for the bullet
//         currentProjectileGameObject.gameObject.SetActive(false);
//     }
//     public void ShootProjectile()
//     {
//         currentProjectileGameObject.gameObject.SetActive(true);
//         currentProjectileGameObject.GetComponent<BulletController>().SetTarget(Player.Instance.gameObject.transform
//             , true/*follow the player*/, 2/*offset*/);
//     }
// }
