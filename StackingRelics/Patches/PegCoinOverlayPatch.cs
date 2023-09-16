using Battle.PegBehaviour;
using Currency;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(PegCoinOverlay),"TriggerCoinCollected")]
    public class PegCoinOverlayTriggerCoinCollectedPatch : Singleton<PegCoinOverlayTriggerCoinCollectedPatch> 
    {
        public static Boolean Prefix(PegCoinOverlay __instance, Peg ____peg)
        {
            if (!____peg.relicManager.RelicEffectActive(RelicEffect.CONVERT_COIN_TO_DAMAGE))
            {
                int amount = ((BattleController.CurrentBattleState != BattleController.BattleState.NAVIGATION || !____peg.relicManager.AttemptUseRelic(RelicEffect.INCREASED_NAV_GOLD)) ? 1 : 4);
                if (BattleController.CurrentBattleState == BattleController.BattleState.NAVIGATION)
                {
                    if (____peg.relicManager.AttemptUseRelic(RelicEffect.INCREASED_NAV_GOLD))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASED_NAV_GOLD] - 1; i++)
                        {
                            amount += 4;
                        }
                    }
                }
                CurrencyManager.Instance.AddGold(amount);
            }
            if (BattleController.CurrentBattleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION && ____peg.relicManager.AttemptUseRelic(RelicEffect.GOLD_ADDS_TO_DAMAGE))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.GOLD_ADDS_TO_DAMAGE]; i++)
                {
                    BattleController.OnAdditionalBasicPegRequested?.Invoke(____peg.GetCenterOfPeg() + Vector3.up * 0.5f);
                    Peg.OnPegAudioRequest?.Invoke(Peg.PegType.REGULAR);
                }
            }
            PegCoinOverlay.OnCoinCollected?.Invoke(____peg.GetCenterOfPeg());
            return false;
        }
    }
}
