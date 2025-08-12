using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameStateMachine.States
{
    public class GameplayState : AsyncState
    {
        private readonly int _currentLevel;
        private readonly Action _onLoseCallback;
        private readonly Action _onWinCallback;
        private readonly Action _onNextLevelCallback;
        private readonly Action _onRestart;
        private readonly Action _onBackToMenu;

        public GameplayState(int currentLevel, Action onRestart, Action onLoseCallback, Action onNextLevelCallback, Action onWinCallback, Action onBackToMenu)
        {
            _currentLevel = currentLevel;
            _onLoseCallback = onLoseCallback;
            _onNextLevelCallback = onNextLevelCallback;
            _onWinCallback = onWinCallback;
            _onBackToMenu = onBackToMenu;
            _onRestart = onRestart;
        }
        protected override Task Enter()
        {
            Application.targetFrameRate = 30;
            
            return SwitchStateAsync(new PlayingState(GoToPause, _onRestart, NextLevel, Win, Lose));
        }
        private void NextLevel() => _onNextLevelCallback?.Invoke();
        private void GoToPause() => SwitchState(new PausedState(GoToPlaying, _onBackToMenu, _onRestart));
        private void GoToPlaying() => SwitchState(new PlayingState(GoToPause, _onRestart, NextLevel, Win, Lose));
        private void Win() => _onWinCallback?.Invoke();
        private void Lose() => _onLoseCallback?.Invoke();
    }
}