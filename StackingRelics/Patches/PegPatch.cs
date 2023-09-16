using Battle.StatusEffects;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(Peg), "ApplyShielding")]
    public class PegApplyShieldingPatch : Singleton<PegApplyShieldingPatch>
    {
        public static void Postfix(RelicManager ___relicManager, bool claimed = false, bool startupShield = false)
        {
            if (!startupShield && ___relicManager.AttemptUseRelic(RelicEffect.ADD_BALLWARK_WHEN_SHIELD_PEG_CREATED))
            {
                int ballsWork = 4 * RelicManage.relicQuantity[RelicEffect.ADD_BALLWARK_WHEN_SHIELD_PEG_CREATED];
                StatusEffect[] statusEffects = new StatusEffect[1]
                {
                new StatusEffect(StatusEffectType.Ballwark, ballsWork)
                };
                PlayerStatusEffectController.OnStatusEffectsAdded?.Invoke(statusEffects);
            }
        }
    }
}
