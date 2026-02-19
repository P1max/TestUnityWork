using AxGrid.FSM;

namespace LootBoxOpening.States
{
    [State("LootBoxScrollingState")]
    public class LootBoxScrollingState : FSMState
    {
        private bool _isStopRequested;

        [Enter]
        private void Enter()
        {
            _isStopRequested = false;

            Model.Set("BtnLootBoxStartButtonEnable", false);
            Model.EventManager.Invoke("StartVisualScroll");
        }

        [One(3f)]
        private void AllowStop()
        {
            Model.Set("BtnLootBoxStopButtonEnable", true);
            Model.EventManager.AddAction("OnStopLootBoxScroll", OnStopCmd);
        }

        private void OnStopCmd()
        {
            _isStopRequested = true;
        }

        [Loop(0f)]
        private void Tick(float delta)
        {
            if (!_isStopRequested) return;

            _isStopRequested = false;
            Parent.Change("LootBoxStoppingState");
        }

        [Exit]
        private void Exit()
        {
            Model.EventManager.RemoveAction("OnStopLootBoxScroll", OnStopCmd);
        }
    }
}