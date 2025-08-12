using System;
using System.Threading.Tasks;
namespace GameStateMachine.States
{
    internal class WinState : AsyncState
    {
        private readonly Action _loadNext;
        public WinState(Action loadNext) : base(GameLevelsManager.Instance.WinLevel)
        {
            _loadNext = loadNext;
        }
        protected override Task Enter()
        {
            FinishScreenManager.Instance.Initialize(LoadNext);
            
            return base.Enter();
        }
        private void LoadNext() => _loadNext?.Invoke();
    }
}