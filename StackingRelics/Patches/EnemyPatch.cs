
using Battle;
using Battle.Enemies;
using Battle.StatusEffects;
using HarmonyLib;
using Peglin.Achievements;
using Peglin.ClassSystem;
using Relics;
using Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(Enemy),"CheckForKnockOnEffects")]
    public class EnemyCheckForKnockOnEffectsPatch : Singleton<EnemyCheckForKnockOnEffectsPatch>
    {
        public static Boolean Prefix(Enemy __instance, RelicManager ____relicManager, StatusEffect statusEffect)
        {
            if (____relicManager.AttemptUseRelic(RelicEffect.BLIND_BRAMBLE_COMBO))
            {
                if (statusEffect.EffectType == StatusEffectType.Blind && !__instance.HasEffect(StatusEffectType.Thorned))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BLIND_BRAMBLE_COMBO]; i++)
                    {
                        __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Thorned), allowKnockOnEffects: false);
                    }
                }
                else if (statusEffect.EffectType == StatusEffectType.Thorned)
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BLIND_BRAMBLE_COMBO]; i++)
                    {
                        __instance.ApplyStatusEffect(new StatusEffect(StatusEffectType.Blind, 2), allowKnockOnEffects: false);
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(Enemy),"ApplyStatusEffect")]
    public class EnemyApplyStatusEffectPatch : Singleton<EnemyApplyStatusEffectPatch>
    {
        public static Boolean Prefix(Enemy __instance, StatusEffectImmunity ____statusEffectImmunities, RelicManager ____relicManager, StatusEffectIconManager ____statusEffectUI, List<StatusEffect> ____statusEffects, StatusEffect statusEffect, bool allowKnockOnEffects = true)
        {
            if (statusEffect.EffectType == StatusEffectType.None || statusEffect.Intensity <= 0)
            {
                return false;
            }
            if (____statusEffectImmunities != null)
            {
                StatusEffectType[] immunities = ____statusEffectImmunities.Immunities;
                foreach (StatusEffectType statusEffectType in immunities)
                {
                    if (statusEffect.EffectType == statusEffectType)
                    {
                        return false;
                    }
                }
            }
            if (StatusEffect.IsStatusEffectDebuff(statusEffect.EffectType) && ____relicManager.AttemptUseRelic(RelicEffect.INCREASE_NEGATIVE_STATUS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASE_NEGATIVE_STATUS]; i++)
                {
                    statusEffect.Intensity++;
                }
            }
            foreach (StatusEffect statusEffect2 in ____statusEffects)
            {
                if (statusEffect2.EffectType == statusEffect.EffectType)
                {
                    statusEffect2.Intensity = Mathf.Min(statusEffect2.Intensity + statusEffect.Intensity, 999);

                    //CheckForPoisonAchievement
                    if (statusEffect2.EffectType == StatusEffectType.Poison && statusEffect2.Intensity >= 50)
                    {
                        Achievement achievement = AchievementData.Achievements[AchievementData.AchievementId.POISON_AMOUNT_ON_ENEMY];
                        if (!achievement.Unlocked && AchievementManager.AchievementsOn)
                        {
                            AchievementManager.Instance.UnlockAchievement(achievement);
                            PersistentPlayerData.Instance.QueuedClassUnlocks.Add(Class.Roundrel);
                        }
                    }

                    if (____statusEffectUI != null)
                    {
                        ____statusEffectUI.UpdateStatusEffects(__instance.StatusEffects);
                    }
                    if (allowKnockOnEffects)
                    {
                        EnemyCheckForKnockOnEffectsPatch.Prefix(__instance, ____relicManager, statusEffect);
                    }
                    return false;
                }
            }
            ____statusEffects.Add(statusEffect);
            if (allowKnockOnEffects)
            {
                EnemyCheckForKnockOnEffectsPatch.Prefix(__instance, ____relicManager, statusEffect);
            }
            if (____statusEffectUI != null)
            {
                ____statusEffectUI.UpdateStatusEffects(__instance.StatusEffects);
            }
            return false;
        }
    }
}
