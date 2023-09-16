using HarmonyLib;
using StackingRelics.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(SettingsManager))]
    class SettingsManagerPatch
    {
        [HarmonyPatch("ApplySettings")]
        [HarmonyPostfix]
        public static void ApplySettingsPostfix(SettingsManager __instance)
        {
            PTSSettingsManager.Instance.ApplySettings();
        }

        [HarmonyPatch("RevertSettings")]
        [HarmonyPrefix]
        public static void RevertSettingsPrefix(SettingsManager __instance)
        {
            PTSSettingsManager.Instance.RevertSettings();
        }
    }
}
