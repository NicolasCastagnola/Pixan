using System;
using System.Threading.Tasks;
namespace GameStateMachine.States
{
    internal class LoseState : AsyncState
    {
        public LoseState(Action onRestart, Action onBackToMenu) => AwaitForFeedback(onRestart);

        private static async void AwaitForFeedback(Action onRestart)
        {
            await Task.Delay(TimeSpan.FromSeconds(5f));
            
            onRestart?.Invoke();
        }
    }
}