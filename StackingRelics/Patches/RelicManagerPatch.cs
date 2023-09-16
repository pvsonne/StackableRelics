using HarmonyLib;
using Relics;
using Saving;
using StackingRelics.Patches.RelicPatches;
using StackingRelics.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static Relics.RelicManager;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetRelicOfRarity))]
    internal class RelicManagerGetCommonRelicPatch : Singleton<RelicManagerGetCommonRelicPatch>
    {
        public static bool Prefix(ref Relic __result, List<Relic> ___CommonRelicPool, List<Relic> ___RareRelicPool, List<Relic> ___BossRelicPool, List<Relic> ___RareScenarioRelicPool)
        {

            List<Relic> allRelics = new List<Relic>();
            for (int i = 0; i < ___CommonRelicPool.Count; i++)
            {
                allRelics.Add(___CommonRelicPool[i]);
            }
            for (int i = 0; i < ___RareRelicPool.Count; i++)
            {
                allRelics.Add(___RareRelicPool[i]);
            }
            for (int i = 0; i < ___BossRelicPool.Count; i++)
            {
                allRelics.Add(___BossRelicPool[i]);
            }
            for (int i = 0; i < ___RareScenarioRelicPool.Count; i++)
            {
                allRelics.Add(___RareScenarioRelicPool[i]);
            }
            for (int i = 0; i < allRelics.Count; i++)
            {
                if (allRelics[i].englishDisplayName == PTSSettingsManager.threeRelics[PTSSettingsManager.counter])
                {
                    __result = allRelics[i];
                    if (PTSSettingsManager.randomOrder)
                    {
                        int randInt = UnityEngine.Random.Range(0, PTSSettingsManager.threeRelics.Count);
                        PTSSettingsManager.counter = randInt;
                    }
                    else
                    {
                        PTSSettingsManager.counter++;
                        if (PTSSettingsManager.counter > PTSSettingsManager.threeRelics.Count - 1) PTSSettingsManager.counter = 0;
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetMultipleRelicsOfRarity))]
    internal class RelicManagerGetMultipleRelicsOfRarityPatch : Singleton<RelicManagerGetMultipleRelicsOfRarityPatch>
    {
        public static bool Prefix(ref Relic[] __result, List<Relic> ___UnavailableRelicPool, List<Relic> ___CommonRelicPool, List<Relic> ___RareRelicPool, List<Relic> ___BossRelicPool, List<Relic> ___RareScenarioRelicPool, int number, RelicRarity rarity, bool fallback = true)
        {
            List<Relic> list = new List<Relic>();
            List<Relic> allRelics = new List<Relic>();


            for (int i = 0; i < ___CommonRelicPool.Count; i++)
            {
                allRelics.Add(___CommonRelicPool[i]);
            }
            for (int i = 0; i < ___RareRelicPool.Count; i++)
            {
                allRelics.Add(___RareRelicPool[i]);
            }
            for (int i = 0; i < ___BossRelicPool.Count; i++)
            {
                allRelics.Add(___BossRelicPool[i]);
            }
            for (int i = 0; i < ___RareScenarioRelicPool.Count; i++)
            {
                allRelics.Add(___RareScenarioRelicPool[i]);
            }
            for (int i = 0; i < ___UnavailableRelicPool.Count; i++)
            {
                allRelics.Add(___UnavailableRelicPool[i]);
            }

            if (list.Count == number)
            {
                __result = list.ToArray();
                return false;
            }
            while (list.Count < number)
            {
                for (int i = 0; i < allRelics.Count; i++)
                {
                    if (allRelics[i].englishDisplayName == PTSSettingsManager.threeRelics[PTSSettingsManager.counter])
                    {

                        list.Add(allRelics[i]);
                        if (PTSSettingsManager.randomOrder)
                        {
                            int randInt = UnityEngine.Random.Range(0, PTSSettingsManager.threeRelics.Count);
                            PTSSettingsManager.counter = randInt;
                        }
                        else
                        {
                            PTSSettingsManager.counter++;
                            if (PTSSettingsManager.counter > PTSSettingsManager.threeRelics.Count - 1) PTSSettingsManager.counter = 0;
                        }
                    }
                }
            }
            __result = list.ToArray();
            
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetMultipleCommonRelicsWithRareChance))]
    internal class RelicManagerGetMultipleCommonRelicsWithRareChancePatch : Singleton<RelicManagerGetMultipleCommonRelicsWithRareChancePatch>
    {
        public static Boolean Prefix(ref Relic[] __result, RelicManager __instance, int totalRelics, float rareRelicChance)
        {
            List<Relic> list = new List<Relic>();
            
            list.AddRange(__instance.GetMultipleRelicsOfRarity(totalRelics, RelicRarity.COMMON));
            
            __result = list.ToArray();
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.AddRelic))]
    internal class RelicManagerAddRelicPatch : Singleton<RelicManagerAddRelicPatch>
    {
        public static Boolean Prefix(RelicManager __instance, Dictionary<RelicEffect, int> ___relicUsesPerBattleCounts, DeckManager ____deckManager, FloatVariable ____playerHealth, FloatVariable ____maxPlayerHealth, Dictionary<RelicEffect, int> ____orderOfRelicsObtained, int ____orderCounter,Dictionary<RelicEffect, Relic> ____ownedRelics, Relic relic)
        {
            Debug.Log($"Adding relic effect: {relic.effect}.");

            float num = 0f;
            switch (relic.effect)
            {
                case RelicEffect.MAX_HEALTH_SMALL:
                    num = 15f;
                    if (____ownedRelics.ContainsKey(RelicEffect.INCREASE_MAX_HP_GAIN))
                    {
                        num += 1f;
                    }
                    ____maxPlayerHealth.Add(num);
                    ____playerHealth.Add(num);
                    break;
                case RelicEffect.MAX_HEALTH_MEDIUM:
                    num = 25f;
                    if (____ownedRelics.ContainsKey(RelicEffect.INCREASE_MAX_HP_GAIN))
                    {
                        num += 1f;
                    }
                    ____maxPlayerHealth.Add(num);
                    ____playerHealth.Add(num);
                    break;
                case RelicEffect.MAX_HEALTH_LARGE:
                    num = 50f;
                    if (____ownedRelics.ContainsKey(RelicEffect.INCREASE_MAX_HP_GAIN))
                    {
                        num += 1f;
                    }
                    ____maxPlayerHealth.Add(num);
                    ____playerHealth.Add(num);
                    break;
                case RelicEffect.ADD_ORBS_AND_UPGRADE:
                    ____deckManager.RelicModifyDeck(relic);
                    break;
            }

            
            if (RelicManage.relicQuantity.ContainsKey(relic.effect))
            {
                RelicManage.relicQuantity[relic.effect] ++;
                RelicManage.relicStorage[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, relic);
                RelicManage.EffectChanger(ref __instance, relic.effect);
                if (relicCountdownValues.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingCountdowns.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingCountdowns.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding countdown");
                    RelicManage.relicRemainingCountdowns[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, relicCountdownValues[relic.effect]);
                }
                if (___relicUsesPerBattleCounts.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingUsesPerBattle.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingUsesPerBattle.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding per battle");
                    RelicManage.relicRemainingUsesPerBattle[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, ___relicUsesPerBattleCounts[relic.effect]);
                }
                if (relicUsesPerRunCounts.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingUsesPerRun.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingUsesPerRun.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding per run");
                    RelicManage.relicRemainingUsesPerRun[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, relicUsesPerRunCounts[relic.effect]);
                }
                if (!RelicManage.relicOrder.ContainsKey(relic.effect))
                {
                    RelicManage.relicOrder.Add(relic.effect, new Dictionary<int, int>());
                }
                RelicManage.relicOrder[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, RelicManage.relicCount);
                RelicManage.relicCount++;
                
                RelicManage.currentRelicAdd2 = RelicManage.relicQuantity[relic.effect] - 1;
                RelicManager.OnRelicAdded(relic);
                return false;
            }
            else
            {
                RelicManage.relicQuantity.Add(relic.effect, 1);
                RelicManage.relicStorage.Add(relic.effect, new Dictionary<int, Relic>());
                RelicManage.relicStorage[relic.effect].Add(0,relic);
                if (relicCountdownValues.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingCountdowns.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingCountdowns.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding countdown");
                    RelicManage.relicRemainingCountdowns[relic.effect].Add(0, relicCountdownValues[relic.effect]);
                }
                if (___relicUsesPerBattleCounts.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingUsesPerBattle.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingUsesPerBattle.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding per battle");
                    RelicManage.relicRemainingUsesPerBattle[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, ___relicUsesPerBattleCounts[relic.effect]);
                }
                if (relicUsesPerRunCounts.ContainsKey(relic.effect))
                {
                    if (!RelicManage.relicRemainingUsesPerRun.ContainsKey(relic.effect))
                    {
                        RelicManage.relicRemainingUsesPerRun.Add(relic.effect, new Dictionary<int, int>());
                    }
                    Debug.Log("Adding per run");
                    RelicManage.relicRemainingUsesPerRun[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, relicUsesPerRunCounts[relic.effect]);
                }
                if (!RelicManage.relicOrder.ContainsKey(relic.effect))
                {
                    RelicManage.relicOrder.Add(relic.effect, new Dictionary<int, int>());
                }
                RelicManage.relicOrder[relic.effect].Add(RelicManage.relicQuantity[relic.effect] - 1, RelicManage.relicCount);
                RelicManage.relicCount++;
            }
            


            ____ownedRelics.Add(relic.effect, relic);
            ____orderOfRelicsObtained.Add(relic.effect, ____orderCounter);
            ____orderCounter++;
            RelicManage.currentRelicAdd2 = 0;
            RelicManager.OnRelicAdded(relic);

            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetRemainingCountdownForRelic))]
    internal class RelicManagerGetRemainingCountdownForRelicPatch : Singleton<RelicManagerGetRemainingCountdownForRelicPatch>
    {
        /*public static Boolean Prefix(int __result, RelicEffect effect)
        {
            if (RelicManage.relicRemainingCountdowns.ContainsKey(effect))
            {
                __result = RelicManage.relicRemainingCountdowns[effect];
            }
            __result = 0;
            return false;
        }*/
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.SetRemainingCountdownForRelic))]
    internal class RelicManagerSetRemainingCountdownForRelicPatch : Singleton<RelicManagerSetRemainingCountdownForRelicPatch>
    {
        public static Boolean Prefix(RelicEffect effect, int countdown)
        {
            for (int i = 0; i < RelicManage.relicQuantity[effect]; i++)
            {
                if (RelicManage.relicRemainingCountdowns[effect][i] + 1 == countdown)
                {
                    RelicManage.relicRemainingCountdowns[effect][i] = countdown;
                    OnCountdownDecremented?.Invoke(RelicManage.relicStorage[effect][i], countdown);
                }
            }
            
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.AttemptUseRelic))]
    internal class RelicManagerAttemptUseRelicPatch : Singleton<RelicManagerAttemptUseRelicPatch>
    {
        public static Boolean Prefix(ref bool __result, RelicManager __instance, Dictionary<RelicEffect, int> ____relicRemainingUsesPerRun, Dictionary<RelicEffect, int> ___relicUsesPerBattleCounts, Dictionary<RelicEffect, int> ____relicRemainingUsesPerBattle, Dictionary<RelicEffect, int> ____relicRemainingCountdowns, Dictionary<RelicEffect, Relic> ____ownedRelics, RelicEffect re)
        {
            if (____ownedRelics == null)
            {
                __instance.Reset();
            }
            if (____ownedRelics.ContainsKey(re))
            {
                if (relicCountdownValues.ContainsKey(re))
                {
                    __result = false;
                    for (int i = 0; i < RelicManage.relicQuantity[re]; i++)
                    {
                        RelicManage.currentRelicAdd = i;
                        int num = (RelicManage.relicRemainingCountdowns.ContainsKey(re) ? RelicManage.relicRemainingCountdowns[re][i] : relicCountdownValues[re]);
                        if (--num <= 0)
                        {
                            OnRelicUsed?.Invoke(RelicManage.relicStorage[re][i]);
                            RelicManage.relicRemainingCountdowns[re][i] = relicCountdownValues[re];
                            __result = true;
                            continue;
                        }
                        RelicManage.relicRemainingCountdowns[re][i] = num;
                        OnCountdownDecremented(RelicManage.relicStorage[re][i], num);
                        
                    }
                    return false;

                }
                if (___relicUsesPerBattleCounts.ContainsKey(re))
                {
                    __result = false;
                    for (int i = 0; i < RelicManage.relicQuantity[re]; i++)
                    {
                        RelicManage.currentRelicAdd = i;
                        int num = (RelicManage.relicRemainingUsesPerBattle.ContainsKey(re) ? RelicManage.relicRemainingUsesPerBattle[re][i] : ___relicUsesPerBattleCounts[re]);
                        if (num == 1)
                        {
                            OnRelicUsed?.Invoke(RelicManage.relicStorage[re][i]);
                            __result = true;
                            RelicManage.relicRemainingUsesPerBattle[re][i]--;
                            RelicManage.relicIcons[re][i].ApplyGrayscale();
                            break;
                        }
                    }
                    return false;

                }
                if (relicUsesPerRunCounts.ContainsKey(re))
                {
                    int num3 = (____relicRemainingCountdowns.ContainsKey(re) ? ____relicRemainingCountdowns[re] : relicUsesPerRunCounts[re]);
                    if (--num3 >= 0)
                    {
                        OnRelicUsed?.Invoke(____ownedRelics[re]);
                        ____relicRemainingUsesPerRun[re] = num3;
                        OnCountdownDecremented?.Invoke(____ownedRelics[re], num3);
                        if (num3 == 0)
                        {
                            OnRelicDisabled?.Invoke(____ownedRelics[re]);
                        }
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                for (int i = 0; i < RelicManage.relicQuantity[re]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    Relic relic = ____ownedRelics[re];
                    OnRelicUsed(relic);
                }
                __result = true;
                return false;
            }
            __result = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.LoadRelicData))]
    internal class RelicManagerLoadRelicDataPatch : Singleton<RelicManagerLoadRelicDataPatch>
    {
        public static Boolean Prefix(RelicManager __instance, RelicSet ____globalRelics)
        {
            RelicManage.loadData(__instance, ____globalRelics);
            
            return false;
        }
    }

}
