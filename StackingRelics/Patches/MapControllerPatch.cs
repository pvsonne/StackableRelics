using HarmonyLib;
using Map;
using StackingRelics.Patches.RelicPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(MapController),"SaveData")]
    internal class MapControllerPatch
    {
        public static void Prefix()
        {
            RelicManage.saveData();
        }
    }
}
