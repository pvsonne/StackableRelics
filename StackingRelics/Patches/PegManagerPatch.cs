using Battle;
using Cruciball;
using HarmonyLib;
using PeglinUtils;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(PegManager), "GetPegCount")]
    public class PegManagerGetPegCountPatch : Singleton<PegManagerGetPegCountPatch>
    {
        public static Boolean Prefix(ref int __result, RelicManager ____relicManager, CruciballManager ____cruciballManager, Peg.PegType type)
        {
            switch (type)
            {
                case Peg.PegType.CRIT:
                    {
                        int num2 = ((____cruciballManager != null && ____cruciballManager.FewerCritPegs()) ? 1 : 2);
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_CRIT1))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_CRIT1]; i++)
                            {
                                num2++;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_CRIT2))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_CRIT2]; i++)
                            {
                                num2 += 2;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.CRIT_PIT))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CRIT_PIT]; i++)
                            {
                                num2 += 2;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_CRIT3))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_CRIT3]; i++)
                            {
                                num2 += 3;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.REDUCE_CRIT))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REDUCE_CRIT]; i++)
                            {
                                num2--;
                            }
                        }
                        __result = num2;
                        break;
                    }
                case Peg.PegType.RESET:
                    {
                        int num = ((____cruciballManager != null && ____cruciballManager.FewerRefreshPegs()) ? 1 : 2);
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_REFRESH1))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_REFRESH1]; i++)
                            {
                                num++;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_REFRESH2))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_REFRESH2]; i++)
                            {
                                num += 2;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_REFRESH3))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_REFRESH3]; i++)
                            {
                                num += 3;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.REDUCE_REFRESH))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REDUCE_REFRESH]; i++)
                            {
                                num--;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ALL_ORBS_MORBID))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_ORBS_MORBID]; i++)
                            {
                                num--;
                            }
                        }
                        if (____relicManager.RelicEffectActive(RelicEffect.ONLY_REFRESH_X_PEGS))
                        {
                            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ONLY_REFRESH_X_PEGS]; i++)
                            {
                                num += 10;
                            }
                        }
                        __result = num;
                        break;
                    }
                default:
                    __result = 1;
                    break;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PegManager), "ResetPegs")]
    public class PegManagerResetPegsPatch : Singleton<PegManagerResetPegsPatch>
    {
        public static Boolean Prefix(PegManager __instance, List<Peg> ____nonSpecialPegs, Peg ____lastHitResetPeg, List<Bomb> ____bombs, List<Peg> ____allPegs, RelicManager ____relicManager, bool crit)
        {
            int num = 0;
            if (____relicManager.RelicEffectActive(RelicEffect.ONLY_REFRESH_X_PEGS))
            {
                ____allPegs.Shuffle();
                ____bombs.Shuffle();
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.BOMBS_RESPAWN))
            {
                foreach (Bomb bomb in ____bombs)
                {
                    if (bomb.CanResetBomb())
                    {
                        if (num >= 4 && ____relicManager.AttemptUseRelic(RelicEffect.ONLY_REFRESH_X_PEGS))
                        {
                            break;
                        }
                        bomb.Reset();
                        num++;
                    }
                }
            }
            foreach (Peg allPeg in ____allPegs)
            {
                if (num >= 4 && ____relicManager.AttemptUseRelic(RelicEffect.ONLY_REFRESH_X_PEGS))
                {
                    break;
                }
                if (!(allPeg == ____lastHitResetPeg))
                {
                    bool _result = false;
                    PegManagerResetPegPatch.Prefix(ref _result, ____relicManager, allPeg, crit);
                    if (_result)
                    {
                        num++;
                    }
                    if (allPeg.pegType == Peg.PegType.REGULAR && !____nonSpecialPegs.Contains(allPeg) && !allPeg.IsDisabled())
                    {
                        ____nonSpecialPegs.Add(allPeg);
                    }
                }
            }
            ____lastHitResetPeg = null;
            if (num > 0)
            {
                if (____relicManager.AttemptUseRelic(RelicEffect.REFRESH_DAMAGES_PEG_COUNT))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REFRESH_DAMAGES_PEG_COUNT]; i++)
                    {
                        TargetingManager.OnDamageTargetedEnemy?.Invoke(num);
                    }
                }
                __instance.OnPegsRefreshed?.Invoke(num);
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.CREATE_GOLD_ON_REFRESH))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CREATE_GOLD_ON_REFRESH] - 1; i++)
                {
                    __instance.ApplyGoldToPegs(2);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PegManager), "ResetPeg")]
    public class PegManagerResetPegPatch : Singleton<PegManagerResetPegPatch>
    {
        public static Boolean Prefix(ref bool __result, RelicManager ____relicManager, Peg peg, bool crit)
        {
            peg.ResetHitCount();
            if (peg.IsDisabled() && peg.pegType != Peg.PegType.DESTROYED)
            {
                if (!____relicManager.RelicEffectActive(RelicEffect.PEG_TO_BOMB) || !peg.SupportsPegType(Peg.PegType.BOMB))
                {
                    if (!peg.gameObject.activeInHierarchy)
                    {
                        peg.gameObject.SetActive(value: true);
                    }
                    peg.Reset(crit);
                    __result = true;
                    return false;
                }
                float chance = ____relicManager.PEG_TO_BOMB_CHANCE;
                bool turnIntoBomb = false;
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.PEG_TO_BOMB]; i++)
                {
                    if (UnityEngine.Random.value < chance)
                    {
                        turnIntoBomb = true;
                        break;
                    }
                }
                if (!turnIntoBomb)
                {
                    peg.Reset(crit);
                    __result = true;
                    return false;
                }
                ____relicManager.AttemptUseRelic(RelicEffect.PEG_TO_BOMB);
                peg.ConvertPegToType(Peg.PegType.BOMB);
            }
            else if (peg.IsDelayedDeath() && peg.pegType != Peg.PegType.CRIT && peg.pegType != Peg.PegType.RESET)
            {
                bool result = !Peg.PegTypeIndestructible(peg.pegType) && peg.IsWaitingForDeath();
                peg.Reset(crit);
                __result = result;
                return false;
            }
            return false;
        }
    }
}
