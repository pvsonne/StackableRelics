using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Peg;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(RegularPeg), "ShouldPopPegOnHit")]
    public class RegularPegShouldPopPegOnHitPatch : Singleton<RegularPegShouldPopPegOnHitPatch>
    {
        public static void Prefix(ref int ___BouncesToPop)
        {
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.UNPOPPABLE_PEGS))
            {
                ___BouncesToPop = RelicManage.relicQuantity[RelicEffect.UNPOPPABLE_PEGS] * 3;
            }
        }
    }
    [HarmonyPatch(typeof(RegularPeg), "DoPegCollision")]
    public class RegularPegDoPegCollisionPatch : Singleton<RegularPegDoPegCollisionPatch> 
    { 
        public static void Prefix(ref int ___BouncesToPop)
        {
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.UNPOPPABLE_PEGS))
            {
                ___BouncesToPop = RelicManage.relicQuantity[RelicEffect.UNPOPPABLE_PEGS] * 3;
            }
        }
    }
    [HarmonyPatch(typeof(RegularPeg), "CheckAndApplySlime")]
    public class RegularPegCheckAndApplySlimePatch : Singleton<RegularPegCheckAndApplySlimePatch>
    {
        public static void Postfix(RegularPeg __instance, RelicManager ___relicManager, bool ____slimedThisHit)
        {
            if (____slimedThisHit && ___relicManager.AttemptUseRelic(RelicEffect.SLIME_BUFFS_PEGS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.SLIME_BUFFS_PEGS] - 1; i++)
                {
                    __instance.AddBuff(2);
                }
            }
        }
    }
}
