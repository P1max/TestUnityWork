using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using LootBoxOpening.States;
using UnityEngine;

namespace LootBoxOpening
{
    public class LootBoxPanel : MonoBehaviourExtBind
    {
        [OnAwake]
        private void Init()
        {
            Settings.Fsm = new FSM();

            Settings.Fsm.Add(new LootBoxIdleState());
            Settings.Fsm.Add(new LootBoxScrollingState());
            Settings.Fsm.Add(new LootBoxStoppingState());

            Settings.Fsm.Start("LootBoxIdleState");

            Model.EventManager.AddAction("OnLootBoxStartButtonClick",
                () => Model.EventManager.Invoke("OnStartLootBoxScroll"));

            Model.EventManager.AddAction("OnLootBoxStopButtonClick",
                () => Model.EventManager.Invoke("OnStopLootBoxScroll"));
        }

        [OnUpdate]
        private void OnUpdate()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }
    }
}