using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(LongPeg), "Update")]
    public class LongPegUpdatePatch : Singleton<LongPegUpdatePatch>
    {
        public static void Prefix(ref int ___BouncesToPop)
        {
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.UNPOPPABLE_PEGS))
            {
                ___BouncesToPop = RelicManage.relicQuantity[RelicEffect.UNPOPPABLE_PEGS] * 3;
            }
        }
    }
    [HarmonyPatch(typeof(LongPeg), "ShouldPopPegOnHit")]
    public class LongPegShouldPopPegOnHitPatch : Singleton<LongPegShouldPopPegOnHitPatch>
    {
        public static void Prefix(ref int ___BouncesToPop)
        {
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.UNPOPPABLE_PEGS))
            {
                ___BouncesToPop = RelicManage.relicQuantity[RelicEffect.UNPOPPABLE_PEGS] * 3;
            }
        }
    }
    [HarmonyPatch(typeof(LongPeg), "DoPegCollision")]
    public class LongPegDoPegCollisionPatch : Singleton<LongPegDoPegCollisionPatch>
    {
        public static void Prefix(ref int ___BouncesToPop)
        {
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.UNPOPPABLE_PEGS))
            {
                ___BouncesToPop = RelicManage.relicQuantity[RelicEffect.UNPOPPABLE_PEGS] * 3;
            }
        }
    }
    [HarmonyPatch(typeof(LongPeg), "CheckAndApplySlime")]
    public class LongPegCheckAndApplySlimePatch : Singleton<LongPegCheckAndApplySlimePatch>
    {
        public static void Postfix(LongPeg __instance, RelicManager ___relicManager, bool ____slimedThisHit)
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
