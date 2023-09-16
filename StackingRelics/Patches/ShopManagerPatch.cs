using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using HarmonyLib;
using PeglinUI;
using Relics;
using Scenarios.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using StackingRelics.Patches.RelicPatches;
using UnityEngine.UI;
using Rewired.Integration.UnityUI;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(ShopManager), "OnEnable")]
    public class ShopManagerOnEnablePatch : Singleton<ShopManagerOnEnablePatch>
    {
        public static Boolean Prefix(ShopManager __instance, int ___removeOrbCostIncrease, RemoveOrbButton ___removeOrbButton, Button ___exitStoreButton, GameObject ___relicContainer, IPurchasableItem[] ____purchasableRelics, float ___percentChanceOfRareRelic, int ___relicsOffered, IPurchasableItem[] ____purchasableOrbs, RewiredEventSystem ____rewiredEventSystem, Selectable ____firstSelectable, GameObject ___orbContainer, GameObject ___purchasablePrefab, int ___orbsOffered, DeckManager ___deckManager, List<ShopItem> ___relicItems, List<ShopItem> ___orbItems, float ___introFadeTime, ShopUIMove ___shopUI, Image ___fadeCurtain, int ___removeOrbCostBase, RelicManager ___relicManager)
        {
            PauseMenu.PauseBlock = true;
            TweenerCore<Color, Color, ColorOptions> tweenerCore = ___fadeCurtain.DOFade(0f, ___introFadeTime);
            tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, (TweenCallback)delegate
            {
                ___shopUI.gameObject.SetActive(value: true);
                ___shopUI.MoveIn();
                ___fadeCurtain.enabled = false;
            });
            int removeOrbCost = ___removeOrbCostBase + StaticGameData.StoreNumberOfOrbsRemoved * ___removeOrbCostIncrease;
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.DOUBLE_COINS_AND_PRICES))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_COINS_AND_PRICES]; i++)
                {
                    removeOrbCost *= 2;
                }
            }
            __instance.removeOrbButton.Initialize(__instance, removeOrbCost);
            //CreateRandomOrbOffer
            ShopManagerCreateRandomOrbOfferPatch.Prefix(__instance, ___orbItems, ____purchasableOrbs, ____rewiredEventSystem, ____firstSelectable, ___orbContainer, ___purchasablePrefab, ___orbsOffered, ___relicManager, ___deckManager);
            //CreateRandomRelicOffer
            ShopManagerCreateRandomRelicOfferPatch.Prefix(__instance, ___relicItems, ____purchasableRelics, ___percentChanceOfRareRelic, ___relicsOffered, ____rewiredEventSystem, ___relicContainer, ___purchasablePrefab, ___relicManager);
            //SetupNavigation
            for (int i = 0; i < ___orbItems.Count; i++)
            {
                Navigation navigation = default(Navigation);
                navigation.mode = Navigation.Mode.Explicit;
                Navigation navigation2 = navigation;
                if (i > 0)
                {
                    navigation2.selectOnLeft = ___orbItems[i - 1].selectable;
                }
                if (___orbItems.Count > i + 1)
                {
                    navigation2.selectOnRight = ___orbItems[i + 1].selectable;
                }
                else
                {
                    navigation2.selectOnRight = ___exitStoreButton;
                }
                int index = Mathf.Clamp(i, 0, ___relicItems.Count - 1);
                navigation2.selectOnDown = ___relicItems[index].selectable;
                ___orbItems[i].selectable.navigation = navigation2;
            }
            for (int j = 0; j < ___relicItems.Count; j++)
            {
                Navigation navigation = default(Navigation);
                navigation.mode = Navigation.Mode.Explicit;
                Navigation navigation3 = navigation;
                if (j > 0)
                {
                    navigation3.selectOnLeft = ___relicItems[j - 1].selectable;
                }
                if (___relicItems.Count > j + 1)
                {
                    navigation3.selectOnRight = ___relicItems[j + 1].selectable;
                }
                else
                {
                    navigation3.selectOnRight = ___exitStoreButton;
                }
                int index2 = Mathf.Clamp(j, 0, ___orbItems.Count - 1);
                navigation3.selectOnUp = ___orbItems[index2].selectable;
                navigation3.selectOnDown = ___removeOrbButton.selectable;
                ___relicItems[j].selectable.navigation = navigation3;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(ShopManager), "CreateRandomRelicOffer")]
    public class ShopManagerCreateRandomRelicOfferPatch : Singleton<ShopManagerCreateRandomRelicOfferPatch>
    {
        public static Boolean Prefix(ShopManager __instance, List<ShopItem> ___relicItems, IPurchasableItem[] ____purchasableRelics, float ___percentChanceOfRareRelic, int ___relicsOffered, RewiredEventSystem ____rewiredEventSystem, GameObject ___relicContainer, GameObject ___purchasablePrefab, RelicManager ___relicManager)
        {
            Relic[] multipleCommonRelicsWithRareChance = ___relicManager.GetMultipleCommonRelicsWithRareChance(___relicsOffered, ___percentChanceOfRareRelic);
            if (multipleCommonRelicsWithRareChance.Length == 0)
            {
                return false;
            }
            int num = 0;
            Relic[] array = multipleCommonRelicsWithRareChance;
            foreach (Relic relic in array)
            {
                if (!(relic == null))
                {
                    ShopItem component = UnityEngine.Object.Instantiate(___purchasablePrefab, ___relicContainer.transform).GetComponent<ShopItem>();
                    int costMultiplier = 1;
                    if (RelicManage.relicQuantity.ContainsKey(RelicEffect.DOUBLE_COINS_AND_PRICES))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_COINS_AND_PRICES]; i++)
                        {
                            costMultiplier *= 2;
                        }
                    }
                    PurchasableRelic purchasableRelic = new PurchasableRelic(relic, ___relicManager, costMultiplier);
                    component.Initialize(purchasableRelic, __instance);
                    component.GetComponentInChildren<ArrowSelection>().rewiredEventSystem = ____rewiredEventSystem;
                    ____purchasableRelics[num++] = purchasableRelic;
                    ___relicItems.Add(component);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(ShopManager), "CreateRandomOrbOffer")]
    public class ShopManagerCreateRandomOrbOfferPatch : Singleton<ShopManagerCreateRandomOrbOfferPatch>
    {
        public static Boolean Prefix(ShopManager __instance, List<ShopItem> ___orbItems, IPurchasableItem[] ____purchasableOrbs, RewiredEventSystem ____rewiredEventSystem, Selectable ____firstSelectable, GameObject ___orbContainer, GameObject ___purchasablePrefab, int ___orbsOffered, RelicManager ___relicManager, DeckManager ___deckManager)
        {
            for (int i = 0; i < ___orbsOffered; i++)
            {
                List<GameObject> randomOrbPool;

                float value = UnityEngine.Random.value;
                if (value <= 0.6f)
                {
                    randomOrbPool = ___deckManager.CommonOrbPool;
                }
                else if (value <= 0.9f)
                {
                    randomOrbPool = ___deckManager.UncommonOrbPool;
                }
                else randomOrbPool = ___deckManager.RareOrbPool;

                int index = UnityEngine.Random.Range(0, randomOrbPool.Count);
                ShopItem component = UnityEngine.Object.Instantiate(___purchasablePrefab, ___orbContainer.transform).GetComponent<ShopItem>();
                int costMultiplier = 1;
                if (RelicManage.relicQuantity.ContainsKey(RelicEffect.DOUBLE_COINS_AND_PRICES))
                {
                    for (int j = 0; j < RelicManage.relicQuantity[RelicEffect.DOUBLE_COINS_AND_PRICES]; j++)
                    {
                        costMultiplier *= 2;
                    }
                }
                
                PurchasableOrb purchasableOrb = new PurchasableOrb(randomOrbPool[index], ___deckManager, costMultiplier);
                component.Initialize(purchasableOrb, __instance);
                if (i == 0)
                {
                    ____firstSelectable = component.GetComponentInChildren<Selectable>(includeInactive: true);
                }
                component.GetComponentInChildren<ArrowSelection>().rewiredEventSystem = ____rewiredEventSystem;
                ____purchasableOrbs[i] = purchasableOrb;
                ___orbItems.Add(component);
            }
            return false;
        }
    }
}
