using Battle;
using Battle.Enemies;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Battle.PlayerHealthController;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(PlayerHealthController), "CheckForRelicOnRefreshPotion")]
    public class PlayerHealthControllerCheckForRelicOnRefreshPotionPatch : Singleton<PlayerHealthControllerCheckForRelicOnRefreshPotionPatch>
    {
        public static Boolean Prefix(PlayerHealthController __instance, RelicManager ____relicManager)
        {
            if (____relicManager.AttemptUseRelic(RelicEffect.HEAL_ON_REFRESH_POTION))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.HEAL_ON_REFRESH_POTION]; i++)
                {
                    __instance.Heal(____relicManager.REFRESH_POTION_HEAL_AMOUNT);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerHealthController), "HandlePegActivated")]
    public class PlayerHealthControllerHandlePegActivatedPatch : Singleton<PlayerHealthControllerHandlePegActivatedPatch>
    {
        public static Boolean Prefix(PlayerHealthController __instance, RelicManager ____relicManager)
        {
            if (____relicManager.RelicEffectActive(RelicEffect.HEAL_ON_PEG_HITS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.HEAL_ON_PEG_HITS]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.HEAL_ON_PEG_HITS) ? RelicManage.relicRemainingCountdowns[RelicEffect.HEAL_ON_PEG_HITS][i] : RelicManager.relicCountdownValues[RelicEffect.HEAL_ON_PEG_HITS]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.HEAL_ON_PEG_HITS][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.HEAL_ON_PEG_HITS][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.HEAL_ON_PEG_HITS][i] = RelicManager.relicCountdownValues[RelicEffect.HEAL_ON_PEG_HITS];
                        __instance.Heal(1f);
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.HEAL_ON_PEG_HITS][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.HEAL_ON_PEG_HITS][i], num);
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG) ? RelicManage.relicRemainingCountdowns[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i] : RelicManager.relicCountdownValues[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i] = RelicManager.relicCountdownValues[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG];
                        __instance.DealSelfDamage(RelicManager.SKULL_WAND_SELF_DMG, blockable: true, DamageSource.OTHER_SELF);
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG][i], num);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerHealthController), "CheckForRelicOnReload")]
    public class PlayerHealthControllerCheckForRelicOnReloadPatch : Singleton<PlayerHealthControllerCheckForRelicOnReloadPatch>
    {
        public static Boolean Prefix(PlayerHealthController __instance, ref int ____reloadCount, RelicManager ____relicManager, int deckSize)
        {
            if (____reloadCount > 0 && ____relicManager.AttemptUseRelic(RelicEffect.HEAL_ON_RELOAD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.HEAL_ON_RELOAD]; i++)
                {
                    __instance.Heal(6f);
                }
            }
            ____reloadCount++;
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerHealthController), "AttemptDamageReturn")]
    public class PlayerHealthControllerAttemptDamageReturnPatch : Singleton<PlayerHealthControllerAttemptDamageReturnPatch>
    {
        public static Boolean Prefix(FloatVariable ____playerHealth, RelicManager ____relicManager, float damage)
        {
            if (____playerHealth.Value > 0f && ____relicManager.AttemptUseRelic(RelicEffect.DAMAGE_RETURN))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DAMAGE_RETURN]; i++)
                {
                    Enemy.OnAllEnemiesDamaged?.Invoke(damage * RelicManager.DAMAGE_RETURN_MODIFIER);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerHealthController), "AdjustMaxHealth")]
    public class PlayerHealthControllerAdjustMaxHealthPatch : Singleton<PlayerHealthControllerAdjustMaxHealthPatch>
    {
        public static Boolean Prefix(GameObject ___HealParticleAnim, float ___HealAnimThreshold, FloatVariable ____playerHealth, FloatVariable ____maxPlayerHealth, RelicManager ____relicManager, float amount)
        {
            if (amount > 0f && ____relicManager.AttemptUseRelic(RelicEffect.INCREASE_MAX_HP_GAIN))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASE_MAX_HP_GAIN]; i++)
                {
                    amount += 1f;
                }
            }
            ____maxPlayerHealth.Add(amount);
            ____playerHealth.Add(amount);
            if (amount > ___HealAnimThreshold)
            {
                ___HealParticleAnim.SetActive(value: true);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerHealthController), "endOfBattleHealPercent", MethodType.Getter)]
    public class PlayerHealthControllerEndOfBattleHealPercentPatch : Singleton<PlayerHealthControllerEndOfBattleHealPercentPatch>
    {
        public static Boolean Prefix(ref float __result, RelicManager ____relicManager, CruciballManager ____cruciballManager, float ____baseEndOfBattleHealPercent)
        {
            float num = ____cruciballManager.GetModifiedPostBattleHeal(____baseEndOfBattleHealPercent);
            if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_END_BATTLE_HEAL))
            {
                num += (float)DeckManager.completeDeck.Count * 0.01f * RelicManage.relicQuantity[RelicEffect.ADDITIONAL_END_BATTLE_HEAL];
            }
            __result = num;
            return false;
        }
    }
}
