using Battle;
using Battle.StatusEffects;
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
    [HarmonyPatch(typeof(PlayerStatusEffectController), "Start")]
    public class PlayerStatusEffectControllerStartPatch : Singleton<PlayerStatusEffectControllerStartPatch> 
    {
        public static Boolean Prefix(PlayerStatusEffectController __instance, RelicManager ____relicManager)
        {
            if (____relicManager != null && ____relicManager.AttemptUseRelic(RelicEffect.CONFUSION_RELIC))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CONFUSION_RELIC]; i++)
                {
                    __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Confusion, 4));
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerStatusEffectController), "CheckRefreshEffects")]
    public class PlayerStatusEffectControllerCheckRefreshEffectsPatch : Singleton<PlayerStatusEffectControllerCheckRefreshEffectsPatch>
    {
        public static Boolean Prefix(PlayerStatusEffectController __instance, RelicManager ____relicManager)
        {
            if (____relicManager.RelicEffectActive(RelicEffect.REFRESH_BUFF))
            {
                Debug.Log("Registered having book");
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REFRESH_BUFF]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.REFRESH_BUFF) ? RelicManage.relicRemainingCountdowns[RelicEffect.REFRESH_BUFF][i] : RelicManager.relicCountdownValues[RelicEffect.REFRESH_BUFF]);
                    num--;
                    Debug.Log("Lowered num");
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.REFRESH_BUFF][i] <= 1)
                    {
                        Debug.Log("Entered if statement");
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.REFRESH_BUFF][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.REFRESH_BUFF][i] = RelicManager.relicCountdownValues[RelicEffect.REFRESH_BUFF];
                        if (UnityEngine.Random.value < 0.5f)
                        {
                            __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Strength));
                        }
                        else
                        {
                            __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Finesse));
                        }
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.REFRESH_BUFF][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.REFRESH_BUFF][i], num);
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.STRENGTH_ON_REFRESH))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.STRENGTH_ON_REFRESH]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.STRENGTH_ON_REFRESH) ? RelicManage.relicRemainingCountdowns[RelicEffect.STRENGTH_ON_REFRESH][i] : RelicManager.relicCountdownValues[RelicEffect.STRENGTH_ON_REFRESH]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.STRENGTH_ON_REFRESH][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.STRENGTH_ON_REFRESH][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.STRENGTH_ON_REFRESH][i] = RelicManager.relicCountdownValues[RelicEffect.STRENGTH_ON_REFRESH];
                        __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Strength));
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.STRENGTH_ON_REFRESH][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.STRENGTH_ON_REFRESH][i], num);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerStatusEffectController), "ApplyStrengthBonus")]
    public class PlayerStatusEffectControllerApplyStrengthBonusPatch : Singleton<PlayerStatusEffectControllerApplyStrengthBonusPatch>
    {
        public static Boolean Prefix(PlayerStatusEffectController __instance, RelicManager ____relicManager)
        {
            if (____relicManager.AttemptUseRelic(RelicEffect.START_WITH_STR))
            {
                int strengthAmount = RelicManage.relicQuantity[RelicEffect.START_WITH_STR];
                __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Strength, strengthAmount));
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerStatusEffectController), "ApplyStatusEffect")]
    public class PlayerStatusEffectControllerApplyStatusEffectPatch : Singleton<PlayerStatusEffectControllerApplyStatusEffectPatch>
    {
        public static Boolean Prefix(PlayerStatusEffectController __instance, StatusEffectIconManager ____statusEffectUI, List<StatusEffect> ____statusEffects, StatusEffectType[] ____positiveStatusEffects, PlayerHealthController ____playerHealthController, PegManager ____pegManager, RelicManager ____relicManager, StatusEffect statusEffect)
        {
            if (statusEffect.EffectType == StatusEffectType.None)
            {
                return false;
            }
            switch (statusEffect.EffectType)
            {
                case StatusEffectType.Slimed:
                    ____pegManager.ApplyEnemySlimeToPegs(Peg.SlimeType.Slow, statusEffect.Intensity);
                    return false;
                case StatusEffectType.Webbed:
                    ____pegManager.ApplyEnemySlimeToPegs(Peg.SlimeType.Web, statusEffect.Intensity);
                    return false;
                case StatusEffectType.Rigged:
                    {
                        int num = statusEffect.Intensity - ____pegManager.ConvertRegularBombsToRigged(statusEffect.Intensity);
                        if (num > 0)
                        {
                            ____pegManager.ConvertPegsToBombs(Mathf.CeilToInt((float)num * 0.5f), rigged: true);
                        }
                        return false;
                    }
                case StatusEffectType.ShieldPegs:
                    ____pegManager.ApplyShieldToRegularPegs(statusEffect.Intensity);
                    return false;
                case StatusEffectType.FlamePegs:
                    ____pegManager.ApplyEnemySlimeToPegs(Peg.SlimeType.FlameOverlay, statusEffect.Intensity);
                    return false;
            }
            int num2 = statusEffect.Intensity;
            if (statusEffect.EffectType == StatusEffectType.Ballwark)
            {
                if (statusEffect.Intensity >= 1 && ____relicManager.AttemptUseRelic(RelicEffect.ARMOUR_PLUS_ONE))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ARMOUR_PLUS_ONE]; i++)
                    {
                        num2++;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.BALLWARK_TO_MUSCIRCLE))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BALLWARK_TO_MUSCIRCLE]; i++)
                    {
                        RelicManage.currentRelicAdd = i;
                        int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.BALLWARK_TO_MUSCIRCLE) ? RelicManage.relicRemainingCountdowns[RelicEffect.BALLWARK_TO_MUSCIRCLE][i] : RelicManager.relicCountdownValues[RelicEffect.BALLWARK_TO_MUSCIRCLE]);
                        num -= num2;
                        if (num < 1)
                        {
                            RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.BALLWARK_TO_MUSCIRCLE][i]);
                            RelicManage.relicRemainingCountdowns[RelicEffect.BALLWARK_TO_MUSCIRCLE][i] = RelicManager.relicCountdownValues[RelicEffect.BALLWARK_TO_MUSCIRCLE] + num;
                            
                            int num3 = 1;
                            if (____relicManager.AttemptUseRelic(RelicEffect.STRENGTH_PLUS_ONE))
                            {
                                for (int k = 0; k < RelicManage.relicQuantity[RelicEffect.STRENGTH_PLUS_ONE]; i++)
                                {
                                    num3++;
                                }
                            }
                            if (____relicManager.AttemptUseRelic(RelicEffect.INCREASE_POSITIVE_STATUS))
                            {
                                for (int k = 0; k < RelicManage.relicQuantity[RelicEffect.INCREASE_POSITIVE_STATUS]; i++)
                                {
                                    num3++;
                                }
                            }
                            MyFindAndUpdateStatusEffect(__instance, ____statusEffectUI, ____statusEffects, ____relicManager, StatusEffectType.Strength, num3);
                            
                            continue;
                        }
                        RelicManage.relicRemainingCountdowns[RelicEffect.BALLWARK_TO_MUSCIRCLE][i] = num;
                        RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.BALLWARK_TO_MUSCIRCLE][i], num);
                    }
                }
                
                ____playerHealthController.AddArmour(num2);
            }
            if (statusEffect.EffectType == StatusEffectType.Strength)
            {
                if (statusEffect.Intensity >= 1 && ____relicManager.AttemptUseRelic(RelicEffect.STRENGTH_PLUS_ONE))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.STRENGTH_PLUS_ONE]; i++)
                    {
                        num2++;
                    }
                }
                if (statusEffect.Intensity >= 1 && ____relicManager.RelicEffectActive(RelicEffect.BALLWARK_TO_MUSCIRCLE))
                {
                    int num4 = 2 * RelicManage.relicQuantity[RelicEffect.BALLWARK_TO_MUSCIRCLE];
                    if (____relicManager.AttemptUseRelic(RelicEffect.ARMOUR_PLUS_ONE))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ARMOUR_PLUS_ONE]; i++)
                        {
                            num4++;
                        }
                    }
                    if (____relicManager.AttemptUseRelic(RelicEffect.INCREASE_POSITIVE_STATUS))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASE_POSITIVE_STATUS]; i++)
                        {
                            num4++;
                        }
                    }
                    ____playerHealthController.AddArmour(num4);
                    MyFindAndUpdateStatusEffect(__instance, ____statusEffectUI, ____statusEffects, ____relicManager, StatusEffectType.Ballwark, num4);
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.INCREASE_POSITIVE_STATUS) && ____positiveStatusEffects.Contains(statusEffect.EffectType) && statusEffect.Intensity >= 1)
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASE_POSITIVE_STATUS]; i++)
                {
                    num2++;
                }
            }
            foreach (StatusEffect statusEffect2 in ____statusEffects)
            {
                if (statusEffect2.EffectType == statusEffect.EffectType)
                {
                    statusEffect2.Intensity = Mathf.Min(statusEffect2.Intensity + num2, 999);
                    if (____statusEffectUI != null)
                    {
                        ____statusEffectUI.UpdateStatusEffects(____statusEffects.ToArray());
                    }
                    ____relicManager.UpdateStatusDependentRelics(__instance.HasNegativeStatusEffect());
                    return false;
                }
            }
            ____statusEffects.Add(new StatusEffect(statusEffect.EffectType, num2));
            ____statusEffectUI.UpdateStatusEffects(____statusEffects.ToArray());
            ____relicManager.UpdateStatusDependentRelics(__instance.HasNegativeStatusEffect());
            return false;
        }
        public static void MyFindAndUpdateStatusEffect(PlayerStatusEffectController __instance, StatusEffectIconManager ____statusEffectUI, List<StatusEffect> ____statusEffects, RelicManager _relicManager, StatusEffectType effectType, int intensity)
        {
            foreach (StatusEffect statusEffect in ____statusEffects)
            {
                if (statusEffect.EffectType == effectType)
                {
                    statusEffect.Intensity = Mathf.Min(statusEffect.Intensity + intensity, 999);
                    if (____statusEffectUI != null)
                    {
                        ____statusEffectUI.UpdateStatusEffects(____statusEffects.ToArray());
                    }
                    _relicManager.UpdateStatusDependentRelics(__instance.HasNegativeStatusEffect());
                    return;
                }
            }
            ____statusEffects.Add(new StatusEffect(effectType, intensity));
        }
    }
    [HarmonyPatch(typeof(PlayerStatusEffectController), "CheckShieldBreakEffects")]
    public class PlayerStatusEffectControllerCheckShieldBreakEffectsPatch : Singleton<PlayerStatusEffectControllerCheckShieldBreakEffectsPatch>
    {
        public static Boolean Prefix(PlayerStatusEffectController __instance, RelicManager ____relicManager)
        {
            if (____relicManager.AttemptUseRelic(RelicEffect.ADD_BALLWARK_WHEN_SHIELD_PEG_BROKEN))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADD_BALLWARK_WHEN_SHIELD_PEG_BROKEN]; i++)
                {
                    __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Ballwark, 4));
                }
            }
            return false;
        }
    }
}
