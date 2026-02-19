using AxGrid.FSM;

namespace LootBoxOpening.States
{
    [State("LootBoxStoppingState")]
    public class LootBoxStoppingState : FSMState
    {
        private bool _isAnimationFinished;

        [Enter]
        private void Enter()
        {
            _isAnimationFinished = false;

            Model.Set("BtnLootBoxStopButtonEnable", false);
            Model.EventManager.Invoke("StopVisualScroll");
            Model.EventManager.AddAction("OnAnimationFinished", OnFinished);
        }

        private void OnFinished()
        {
            _isAnimationFinished = true;
        }

        [Loop(0f)]
        private void Tick(float delta)
        {
            if (!_isAnimationFinished) return;

            _isAnimationFinished = false;

            Parent.Change("LootBoxIdleState");
        }

        [Exit]
        private void Exit()
        {
            Model.EventManager.RemoveAction("OnAnimationFinished", OnFinished);
        }
    }
}