using Battle;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using Battle.Enemies;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(TargetingManager), "HandlePlayerHealed")]
    public class TargetingManagerHandlePlayerHealedPatch : Singleton<TargetingManagerHandlePlayerHealedPatch>
    {
        public static Boolean Prefix(TargetingManager __instance, Enemy ____target, RelicManager ____relicManager, float healAmount)
        {
            Debug.Log("Registered a Heal");
            if (healAmount > 0f && ____relicManager.RelicEffectActive(RelicEffect.DAMAGE_TARGETED_ON_HEAL))
            {
                Debug.Log("Entered if statement");
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DAMAGE_TARGETED_ON_HEAL]; i++) {
                    float damage = healAmount * 3f;
                    damage = Mathf.FloorToInt(damage);
                    Debug.Log($"Damage = {damage}");
                    if (____target == null)
                    {
                        __instance.AutoSelect();
                        if (____target == null)
                        {
                            return false;
                        }
                    }
                    if (____target.CurrentHealth <= 0f)
                    {
                        __instance.AutoSelect();
                        if (____target == null)
                        {
                            return false;
                        }
                    }
                    Debug.Log("Damaging target");
                    ____target.Damage(damage, screenshake: false, 0.25f);
                    if (____target.CurrentHealth == 0f)
                    {
                        if (____target != null)
                        {
                            ____target.ToggleTargetedUI(on: false);
                            ____target = null;
                        }
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(TargetingManager), "HandlePegActivated")]
    public class TargetingManagerHandlePegActivatedPatch : Singleton<TargetingManagerHandlePegActivatedPatch>
    {
        public static Boolean Prefix(TargetingManager __instance, ref Enemy ____target, RelicManager ____relicManager, Peg.PegType pegType, Peg peg)
        {
            if (____relicManager.RelicEffectActive(RelicEffect.DAMAGE_TARGETED_PEG_HITS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DAMAGE_TARGETED_PEG_HITS]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.DAMAGE_TARGETED_PEG_HITS) ? RelicManage.relicRemainingCountdowns[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i] : RelicManager.relicCountdownValues[RelicEffect.DAMAGE_TARGETED_PEG_HITS]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i] = RelicManager.relicCountdownValues[RelicEffect.DAMAGE_TARGETED_PEG_HITS];
                        MyDamageTargetedEnemy(__instance, ref ____target);
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.DAMAGE_TARGETED_PEG_HITS][i], num);
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.LIFESTEAL_PEG_HITS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.LIFESTEAL_PEG_HITS]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.LIFESTEAL_PEG_HITS) ? RelicManage.relicRemainingCountdowns[RelicEffect.LIFESTEAL_PEG_HITS][i] : RelicManager.relicCountdownValues[RelicEffect.LIFESTEAL_PEG_HITS]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.LIFESTEAL_PEG_HITS][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.LIFESTEAL_PEG_HITS][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.LIFESTEAL_PEG_HITS][i] = RelicManager.relicCountdownValues[RelicEffect.LIFESTEAL_PEG_HITS];
                        if (MyDamageTargetedEnemy(__instance, ref ____target))
                        {
                            PlayerHealthController.OnExternalHealRequested?.Invoke(1f);
                        }

                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.LIFESTEAL_PEG_HITS][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.LIFESTEAL_PEG_HITS][i], num);
                }
            }
            return false;
        }
        public static bool MyDamageTargetedEnemy(TargetingManager __instance, ref Enemy ____target)
        {
            float damage = 1f;
            if (____target == null)
            {
                __instance.AutoSelect();
                if (____target == null)
                {
                    return false;
                }
            }
            if (____target.CurrentHealth <= 0f)
            {
                __instance.AutoSelect();
                if (____target == null)
                {
                    return false;
                }
            }
            ____target.Damage(damage, screenshake: false, 0.25f);
            if (____target.CurrentHealth == 0f)
            {
                if (____target != null)
                {
                    ____target.ToggleTargetedUI(on: false);
                    ____target = null;
                }
            }
            return true;
        }
    }
}
