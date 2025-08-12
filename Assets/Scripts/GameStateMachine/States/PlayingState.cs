using System;
using System.Threading.Tasks;
using UnityEngine;
namespace GameStateMachine.States
{
    public class PlayingState : AsyncState
    {
        private readonly Action _onPause;
        private readonly Action _onRestart;
        private readonly Action _winLevel;
        private readonly Action _nextLevel;
        private readonly Action _lose;
        public PlayingState(Action onPause, Action onRestart, Action onNextLevel, Action onWin, Action OnLose)
        {
            _onPause = onPause;
            _onRestart = onRestart;
            _winLevel = onWin;
            _nextLevel = onNextLevel;
            _lose = OnLose;
        }
        protected override Task Enter()
        {
            LevelController.Instance.Initialize(Win, Restart, Lose, NextLevel);
            
            Canvas_Playing.Instance.Initialize(Pause);
            
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            
            return Task.CompletedTask;
        }
        protected override Task Exit()
        {
            Canvas_Playing.Instance.Terminate();
            
            return Task.CompletedTask;
        }
        private void Pause() => _onPause?.Invoke();
        private void Win() => _winLevel?.Invoke();
        private void Lose() => _lose?.Invoke();
        private void NextLevel() => _nextLevel?.Invoke();
        private void Restart() => _onRestart?.Invoke();
    }
}