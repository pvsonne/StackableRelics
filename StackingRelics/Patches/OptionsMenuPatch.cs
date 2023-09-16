using HarmonyLib;
using PeglinUI.SettingsMenu;
using StackingRelics.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(OptionsMenu))]
    class OptionsMenuPatch
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        public static void OnEnable()
        {
            PTSSettingsManager.Instance.ShowSettings();
            PTSSettingsManager.Instance.AddApplyActions();
        }

        [HarmonyPatch("OnDisable")]
        [HarmonyPrefix]
        public static void OnDisable()
        {
            PTSSettingsManager.Instance.RemoveApplyActions();
        }
    }
}
