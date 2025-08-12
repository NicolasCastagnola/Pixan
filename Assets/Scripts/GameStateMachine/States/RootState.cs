using System.Threading.Tasks;
using UnityEngine;

namespace GameStateMachine.States
{
    public class RootState : AsyncState
    {
        private static bool InTransition = false;
        private static int CurrentLevel = 0;
        
        public static void Initialize()
        {
            var rootState = new RootState();

            rootState.InitializeAsRoot();
        }
        protected override Task Enter() => GoToMainMenuAsync();
        private async Task TransitionTo(AsyncState state)
        {
            if(InTransition)
            {
                if (!GameManager.disableLogs) Debug.LogWarning("Transition in execution. Transition request will be ignored");
                return;
            }

            InTransition = true;
            
            await ScreenTransitionManager.Show();

            var stateSwitchTask = SwitchStateAsync(state);

            while (!stateSwitchTask.IsCompleted)
            {
                ScreenTransitionManager.SetProgress(state.SceneLoadProgress);
                await Task.Yield();
            }

            GameManager.ShowLog($"Completed Transition to -> {state}! Hiding Transition Screen...");

            ScreenTransitionManager.SetProgress(1);
            
            await AsyncUtils.WaitRealtimeAsync(.5f);
        
            await ScreenTransitionManager.Hide();
            
            InTransition=false;
        }
        private static int GetSavedLevel() => PlayerPrefs.GetInt("SavedLevel", 1);

        private async void LoadNextLevel()
        {
            CurrentLevel += 1;
            PlayerPrefs.SetInt("SavedLevel", CurrentLevel);


            if (CurrentLevel >= GameLevelsManager.Instance.Levels.Count)
            {
                CurrentLevel = 1;
            }

            Debug.Log($"Loading level ({CurrentLevel})...");
            
            await TransitionTo(new LevelState(CurrentLevel, Restart, GoToMainMenu,LoadNextLevel, WinLevel));
        }
        
        private async void LoadLevel()
        {
            CurrentLevel = GetSavedLevel();
            await TransitionTo(new LevelState(GetSavedLevel(), Restart, GoToMainMenu,LoadNextLevel, WinLevel));
        }
        private async void Restart()
        {
            CurrentLevel = GetSavedLevel();
            await TransitionTo(new LevelState(GetSavedLevel(), Restart, GoToMainMenu,LoadNextLevel, WinLevel));
        }
        private async void LoadLevelByIndex(int value)
        {
            CurrentLevel = value;
            await TransitionTo(new LevelState(value, Restart, GoToMainMenu,LoadNextLevel, WinLevel));
        }
        private async void WinLevel() => await TransitionTo(new WinState(GoToMainMenu));
        private Task GoToMainMenuAsync() => TransitionTo(new MainMenuState(LoadLevel, LoadLevelByIndex));
        private void GoToMainMenu() => GoToMainMenuAsync();

    }
}