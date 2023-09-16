using PeglinUI.PostBattle;
using PeglinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace StackingRelics
{
    public static partial class Utils
    {
        public static void PressButton(GameObject button)
        {
            (button.GetComponent(typeof(Button)) as Button).SendMessage("Press");
        }

        public static void EnsureGameUnpaused()
        {
            if (PauseMenu.Paused)
            {
                (GameObject.Find("PauseCanvas").GetComponent(typeof(PauseMenu)) as PauseMenu).Resume();
            }
        }

        public static void EnsureInventoryClosed()
        {
            if (InventoryViewController.isOpen)
            {
                (GameObject.Find("InventoryView").GetComponent(typeof(InventoryViewController)) as InventoryViewController).HideInventory();
            }
        }

        public static void EnsurePostBattleUpgradesMainWindowOpen()
        {
            (GameObject.Find("BattleUpgradesCanvas").GetComponent(typeof(BattleUpgradeCanvas)) as BattleUpgradeCanvas).BackFromInventory();
        }
    }
}
