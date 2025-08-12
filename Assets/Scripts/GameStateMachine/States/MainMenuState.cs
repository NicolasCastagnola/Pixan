using System;
using System.Threading.Tasks;
using Audio;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace GameStateMachine.States
{
    internal class MainMenuState : AsyncState
    {
        private readonly Action _loadLevelCallback;
        private readonly Action<int> _loadCustomLevel;
        public MainMenuState(Action loadLevelCallback, Action<int> loadCustomLevel) : base(GameLevelsManager.Instance.Menu)
        {
            _loadLevelCallback = loadLevelCallback;
            _loadCustomLevel = loadCustomLevel;
        }

        protected override Task Enter()
        {
            Canvas_MainMenu.Instance.Initialize(LoadLevel, LoadCustomLevelByIndex, ExitGame, ResetProgress);
            return Task.CompletedTask;
        }
        protected override Task Exit()
        {
            Canvas_MainMenu.Instance.Terminate();
            return base.Exit();
        }
        private void LoadLevel() => _loadLevelCallback?.Invoke();
        private void LoadCustomLevelByIndex(int value) => _loadCustomLevel?.Invoke(value);
        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
        private void ResetProgress()
        {
            PlayerPrefs.DeleteAll();//podria generar inestabilidades,testear
        }
    }
}