using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameStateMachine.States
{
    public class PausedState : AsyncState
    {
        private readonly Action _onResume;
        private readonly Action _onBackToMenu;
        private readonly Action _onRestart;
        public PausedState(Action onResume, Action onBackToMenu, Action onRestart)
        {
            _onResume = onResume;
            _onBackToMenu = onBackToMenu;
            _onRestart = onRestart;
        }
        protected override Task Enter()
        {
            foreach(var enemy in EnemyManager.Instance.GetEnemies())
            {
                if (enemy.StateMachine.States.Contains(global::States.Paused))
                {
                    enemy.StateMachine.ChangeState(global::States.Paused);
                }
                else
                {
                    Debug.LogWarning(enemy.name + "DOES NOT HAVE A PAUSED STATE");
                }
            }
            Player.Instance.StateMachine.ChangeState(global::States.Paused);
            Cursor.lockState = CursorLockMode.Confined;
            Canvas_Pause.Instance.Initialize(_onResume, _onBackToMenu, _onRestart);
            return base.Enter();
        }
        protected override Task Exit()
        {
            foreach (var enemy in EnemyManager.Instance.GetEnemies())
            {
                if (enemy.StateMachine.States.Contains(global::States.EnemyIdle))
                {
                    enemy.StateMachine.ChangeState(global::States.EnemyIdle);
                }
                else
                {
                    Debug.LogWarning(enemy.name + "DOES NOT HAVE AN IDLE STATE");
                }
            }
            Player.Instance.StateMachine.ChangeState(global::States.PlayerIdle);
            Canvas_Pause.Instance.Terminate();
            return base.Exit();
        }
    }
}