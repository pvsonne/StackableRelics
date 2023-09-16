using Battle;
using Battle.Attacks;
using Battle.Attacks.DamageModifiers;
using Battle.Pachinko.BallBehaviours;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Relics;
using StackingRelics.Patches.RelicPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(Attack),"GetModifiedDamagePerPeg")]
    public class AttackGetModifiedDamagePerPegPatch : Singleton<AttackGetModifiedDamagePerPegPatch>
    {
        public static Boolean Prefix(ref float __result, Attack __instance,int ____randomCritMod, int ____randomDmgMod, string ___locNameString, string ___id, PlayerHealthController ____playerHealthController, bool ___applyUniqueBuff, int ____critPegsOnBoardAtShotStart, DeckManager ____deckManager, AttackManager ____attackManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController, RelicManager ____relicManager, float ____previousOrbCritDamageMod, float ____previousOrbRegularDamageMod, float ___DamagePerPeg, float ___CritDamagePerPeg, int critCount = 0)
        {
            AttackDamageModifiableRules component = __instance.GetComponent<AttackDamageModifiableRules>();
            if (component != null)
            {
                if (critCount > 0 && component.critDamageNonMod)
                {
                    __result = ___CritDamagePerPeg;
                    return false;
                }
                if (critCount <= 0 && component.baseDamageNonMod)
                {
                    __result = ___DamagePerPeg;
                    return false;
                }
            }
            float num = 0f;
            float num2 = 0f;
            num += ____previousOrbRegularDamageMod;
            num2 += ____previousOrbCritDamageMod;
            if (____relicManager != null)
            {
                if (____relicManager.RelicEffectActive(RelicEffect.INCREASE_STRENGTH_SMALL))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.INCREASE_STRENGTH_SMALL]; i++)
                    {
                        num += 1f;
                        num2 += 1f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.CONFUSION_RELIC) && ____playerStatusEffectController != null && ____playerStatusEffectController.HasNegativeStatusEffect())
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CONFUSION_RELIC]; i++)
                    {
                        num += 2f;
                        num2 += 4f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.NO_DISCARD]; i++)
                    {
                        num += 2f;
                        num2 += 2f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.AIM_LIMITER))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.AIM_LIMITER]; i++)
                    {
                        num += 2f;
                        num2 += 4f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.MATRYOSHKA]; i++)
                    {
                        num -= 2f;
                        num2 -= 2f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.NON_CRIT_BONUS_DMG))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.NON_CRIT_BONUS_DMG]; i++)
                    {
                        num += 1f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.CRIT_BONUS_DMG))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CRIT_BONUS_DMG]; i++)
                    {
                        num2 += 1f;
                    }
                }
                if (__instance.IsStone)
                {
                    if (____relicManager.RelicEffectActive(RelicEffect.BASIC_STONE_BONUS_DAMAGE))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BASIC_STONE_BONUS_DAMAGE]; i++)
                        {
                            num += 1f;
                            num2 += 2f;
                        }
                    }
                    if (____cruciballManager.WeakStones())
                    {
                        num += 0f;
                        num2 += -1f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.ALL_ORBS_BUFF))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_ORBS_BUFF]; i++)
                    {
                        num -= RelicManager.ALL_ORBS_BUFF_DMG_REDUCTION;
                        num2 -= RelicManager.ALL_ORBS_BUFF_CRIT_REDUCTION;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.REDUCE_CRIT))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REDUCE_CRIT]; i++)
                    {
                        num += 2f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.HEDGE_BETS) && ____attackManager != null)
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.HEDGE_BETS]; i++)
                    {
                        num += (float)____critPegsOnBoardAtShotStart;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.REDUCE_REFRESH))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REDUCE_REFRESH]; i++)
                    {
                        num2 += 4f;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.ALL_IN_RELIC) && ____attackManager != null)
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_IN_RELIC]; i++)
                    {
                        int critPegsOnBoardAtShotStart = ____critPegsOnBoardAtShotStart;
                        num -= (float)critPegsOnBoardAtShotStart;
                        num2 += (float)critPegsOnBoardAtShotStart;
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.ADJACENCY_BONUS) && BattleController.BattleActive)
                {
                    int num3 = 0;
                    GameObject[] array = ____deckManager.shuffledDeck.ToArray();
                    int num4 = 0;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].GetComponent<Attack>().id == ___id)
                        {
                            num4 = i;
                            break;
                        }
                    }
                    for (int j = num4; j < array.Length; j++)
                    {
                        Attack component2 = array[j].GetComponent<Attack>();
                        if (!(component2.id == ___id))
                        {
                            if (!(component2.locNameString == ___locNameString))
                            {
                                break;
                            }
                            num3++;
                        }
                    }
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADJACENCY_BONUS]; i++)
                    {
                        num += (float)num3;
                        num2 += (float)(num3 * 2);
                    }
                }
                if (____relicManager.RelicEffectActive(RelicEffect.RANDOMLY_ROLL_DAMAGE) && BattleController.BattleActive)
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.RANDOMLY_ROLL_DAMAGE]; i++)
                    {
                        num += (float)____randomDmgMod;
                        num2 += (float)____randomCritMod;
                    }
                }
                if (___applyUniqueBuff)
                {
                    num += 1f;
                    num2 += 2f;
                }
            }
            if (____playerStatusEffectController != null)
            {
                StatusEffectDamageBooster[] components = __instance.GetComponents<StatusEffectDamageBooster>();
                float num5 = 1f;
                float num6 = 1f;
                float num7 = 1f;
                if (components != null && components.Length != 0)
                {
                    StatusEffectDamageBooster[] array2 = components;
                    foreach (StatusEffectDamageBooster statusEffectDamageBooster in array2)
                    {
                        switch (statusEffectDamageBooster.typeToBoost)
                        {
                            case StatusEffectType.Balance:
                                num7 = statusEffectDamageBooster.multiplier;
                                break;
                            case StatusEffectType.Strength:
                                num5 = statusEffectDamageBooster.multiplier;
                                break;
                            case StatusEffectType.Finesse:
                                num6 = statusEffectDamageBooster.multiplier;
                                break;
                        }
                    }
                }
                num += (float)____playerStatusEffectController.EffectStrength(StatusEffectType.Strength) * num5;
                num2 += (float)____playerStatusEffectController.EffectStrength(StatusEffectType.Finesse) * num6;
                float num8 = (float)____playerStatusEffectController.EffectStrength(StatusEffectType.Balance) * num7;
                num += num8;
                num2 += num8;
            }
            if (__instance.gameObject != null)
            {
                AttackBaseDamageModifier[] components2 = __instance.GetComponents<AttackBaseDamageModifier>();
                if (components2 != null && components2.Length != 0)
                {
                    AttackBaseDamageModifier[] array3 = components2;
                    foreach (AttackBaseDamageModifier attackBaseDamageModifier in array3)
                    {
                        num += attackBaseDamageModifier.GetDamageMod(____deckManager, ____relicManager, ____playerHealthController, ____playerStatusEffectController);
                        num2 += attackBaseDamageModifier.GetDamageMod(____deckManager, ____relicManager, ____playerHealthController, ____playerStatusEffectController, 1);
                    }
                }
            }
            if (component != null)
            {
                float num9 = num2;
                if (component.regularBoostAppliesToCrit && num > 0f)
                {
                    num2 += num;
                }
                if (component.critBoostAppliesToRegular && num9 > 0f)
                {
                    num += num9;
                }
            }
            float num10 = Mathf.Clamp(___DamagePerPeg + num, 0f, 999f);
            if (critCount > 0)
            {
                float num11 = Mathf.Max(___CritDamagePerPeg + num2, 0f);
                if (critCount > 1 && ____relicManager != null && ____relicManager.RelicEffectActive(RelicEffect.CRITS_STACK))
                {
                    float tempNum11 = num11;
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CRITS_STACK]; i++)
                    {
                        num11 += Mathf.Max((tempNum11 - num10) * (float)(critCount - 1), 0f);
                    }
                }
                __result = Mathf.Min(num11, 999f);
                
                return false;
            }
            __result = num10;
            return false;
        }
    }
    [HarmonyPatch(typeof(Attack), "GetDamage")]
    public class AttackGetDamagePatch : Singleton<AttackGetDamagePatch>
    {
        public static Boolean Prefix(ref float __result, Attack __instance, RelicManager ____relicManager, AttackManager attackManager, float[] dmgValues, float dmgMult, int bonus, int critCount = 0, bool displayNegative = false)
        {
            float num = bonus;
            if (____relicManager.RelicEffectActive(RelicEffect.ALL_ATTACKS_ECHO))
            {
                num += (float)Mathf.FloorToInt(AttackManager.PreviousAttackDamage * RelicManager.ECHO_CHAMBER_PERCENT);
            }
            float modifiedDamagePerPeg = __instance.GetModifiedDamagePerPeg(critCount);
            foreach (float num2 in dmgValues)
            {
                num += Mathf.Ceil(num2 * modifiedDamagePerPeg);
            }
            if (critCount > 1 && ____relicManager.RelicEffectActive(RelicEffect.ANCIENT_FLEECE))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ANCIENT_FLEECE]; i++)
                {
                    num *= Mathf.Pow(1.3f, critCount - 1);
                }
            }
            num += __instance.GetFlatDamageBonus();
            num *= dmgMult;
            num = Mathf.FloorToInt(num);
            if (displayNegative)
            {
                __result = num;
            }
            __result = Mathf.Max(num, 0f);
            return false;
        }
    }
    [HarmonyPatch(typeof(Attack), "GetStatusEffects")]
    public class AttackGetStatusEffectsPatch : Singleton<AttackGetStatusEffectsPatch>
    {
        public static Boolean Prefix(ref List<StatusEffect> __result, RelicManager ____relicManager)
        {
            List<StatusEffect> list = new List<StatusEffect>();
            if (____relicManager.RelicEffectActive(RelicEffect.ATTACKS_DEAL_BLIND))
            {
                int blindAmount = RelicManage.relicQuantity[RelicEffect.ATTACKS_DEAL_BLIND] * 4;
                list.Add(new StatusEffect(StatusEffectType.Blind, blindAmount));
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.ATTACKS_DEAL_POISON))
            {
                int poisonAmount = RelicManage.relicQuantity[RelicEffect.ATTACKS_DEAL_POISON] * 2;
                list.Add(new StatusEffect(StatusEffectType.Poison, poisonAmount));
            }
            __result = list;
            return false;
        }
    }
}
