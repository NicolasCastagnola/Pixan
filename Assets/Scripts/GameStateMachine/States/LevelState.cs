using System;
using System.Threading.Tasks;

namespace GameStateMachine.States
{
    public class LevelState : AsyncState
    {
        private static int _currentLevel;
        private readonly Action _onRestart;
        private readonly Action _onBackToMenu;
        private readonly Action _onNextLevel;
        private readonly Action _onWin;

        public LevelState(int currentLevel, Action restartCurrentLevelCallback, Action goToMainMenuCallback,Action nextLevelCallback, Action onWin) : base(GameLevelsManager.Instance.Levels[currentLevel], GameLevelsManager.Instance.CanvasPlaying)
        {
            _currentLevel = currentLevel;
            _onRestart = restartCurrentLevelCallback;
            _onNextLevel = nextLevelCallback;
            _onBackToMenu = goToMainMenuCallback;
            _onWin = onWin;
        }
        protected override Task Enter() => SwitchStateAsync(new GameplayState(_currentLevel, TryRestart, Lose, NextLevel, Win, _onBackToMenu));
        private void NextLevel() => _onNextLevel?.Invoke();
        private void Win() => _onWin?.Invoke();
        private void Lose() => SwitchState(new LoseState(_onRestart, _onBackToMenu));
        private void TryRestart()
        {
            if(ScreenTransitionManager.InTransition) return;
            
            _onRestart?.Invoke();
        }
    }
}