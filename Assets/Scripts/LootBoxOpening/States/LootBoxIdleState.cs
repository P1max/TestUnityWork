using AxGrid.FSM;

namespace LootBoxOpening.States
{
    [State("LootBoxIdleState")]
    public class LootBoxIdleState : FSMState
    {
        private bool _isStartRequested;

        [Enter]
        private void Enter()
        {
            _isStartRequested = false;

            Model.Set("BtnLootBoxStartButtonEnable", true);
            Model.Set("BtnLootBoxStopButtonEnable", false);

            Model.EventManager.AddAction("OnStartLootBoxScroll", OnStartCmd);
        }

        private void OnStartCmd()
        {
            _isStartRequested = true;
        }

        [Loop(0f)]
        private void Tick(float delta)
        {
            if (!_isStartRequested) return;

            _isStartRequested = false;
            Parent.Change("LootBoxScrollingState");
        }

        [Exit]
        private void Exit()
        {
            Model.EventManager.RemoveAction("OnStartLootBoxScroll", OnStartCmd);
        }
    }
}