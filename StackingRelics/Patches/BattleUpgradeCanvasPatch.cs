using Battle.StatusEffects;
using Battle;
using Cruciball;
using Currency;
using HarmonyLib;
using PeglinUI;
using PeglinUI.PostBattle;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Worldmap;
using I2.Loc;
using System.Globalization;
using Coffee.UIEffects;
using DG.Tweening;
using StackingRelics.Patches.RelicPatches;
using Battle.Attacks;
using PeglinUI.ControllerSupport;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using static PeglinUI.PostBattle.BattleUpgradeCanvas;

namespace StackingRelics.Patches
{
    
    [HarmonyPatch(typeof(BattleUpgradeCanvas), "SetUpPostBattleOptions")]
    public class BattleUpgradeCanvasSetUpPostBattleOptionsPatch : Singleton<BattleUpgradeCanvasSetUpPostBattleOptionsPatch>
    {
        public static Boolean Prefix(BattleUpgradeCanvas __instance, Color ____upgradeButtonDefaultColor, Button ____healButton, Button ____maxHPButton, Button ____skipButton, Button ____upgradeButton, PopulateSuggestionOrbs ____suggestionOrbs, int ____currentUpgradeOrbCost, int ____currentAddOrbCost, int ____currentHealCost, CruciballManager ____cruciballManager, RelicManager ____relicManager, PlayerHealthController ____playerHealthController, DeckManager ____deckManager)
        {
            if (____deckManager == null)
            {
                Debug.LogWarning("BattleUpgradeCanvas::OnEnable(): No DeckManager set. Did you leave this enabled when it shouldn't be?");
                __instance.gameObject.SetActive(value: false);
                return false;
            }
            if (____playerHealthController == null)
            {
                Debug.LogError("BattleUpgradeCanvas::OnEnable(): No PlayerHealthController set");
                return false;
            }
            if (ReleaseDemoManager.ReleaseDemoActive && StaticGameData.currentNode != null && StaticGameData.currentNode.RoomType == RoomType.BOSS)
            {
                __instance.gameObject.SetActive(value: false);
                return false;
            }
            PlayerHealthController playerHealthController = GameObject.FindWithTag("Player")?.GetComponent<PlayerHealthController>();
            if (playerHealthController != null)
            {
                if (____relicManager.AttemptUseRelic(RelicEffect.HEAL_WITH_BALLWARK))
                {
                    PlayerStatusEffectController componentInChildren = playerHealthController.GetComponentInChildren<PlayerStatusEffectController>();
                    if (componentInChildren != null && componentInChildren.HasEffect(StatusEffectType.Ballwark))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.HEAL_WITH_BALLWARK]; i++)
                        {
                            playerHealthController.Heal(4f);
                        }
                    }
                }
                if (StaticGameData.currentNode != null && StaticGameData.currentNode.RoomType == RoomType.BOSS)
                {
                    if (____cruciballManager.LessHealingFromBosses())
                    {
                        playerHealthController.Heal(Mathf.Ceil(playerHealthController.MaxHealth / 2f));
                    }
                    else
                    {
                        playerHealthController.HealToFull();
                    }
                }
            }
            else
            {
                Debug.LogError("PlayerHealthController reference missing after battle!");
            }
            if (____relicManager.RelicEffectActive(RelicEffect.DOUBLE_COINS_AND_PRICES))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_COINS_AND_PRICES]; i++)
                {
                    ____currentHealCost *= 2;
                    ____currentAddOrbCost *= 2;
                    ____currentUpgradeOrbCost *= 2;
                }
            }
            ____suggestionOrbs.Initialize(____currentAddOrbCost);
            ____upgradeButton.GetComponentInChildren<SimpleCostText>().SetGold(____currentUpgradeOrbCost);

            //CheckForUpgradeButtonEnabled
            UIEffect buttonUIEffect = ____upgradeButton.GetComponent<UIEffect>();
            UnityEngine.UI.Image component = ____upgradeButton.GetComponent<UnityEngine.UI.Image>();
            float duration = 0.35f;
            if (____deckManager.GetUpgradeableOrbs().Count <= 0)
            {
                DOTween.To(() => buttonUIEffect.effectFactor, delegate (float x)
                {
                    buttonUIEffect.effectFactor = x;
                }, 1f, duration);
                component.DOColor(Color.white, duration);
            }
            else
            {
                DOTween.To(() => buttonUIEffect.effectFactor, delegate (float x)
                {
                    buttonUIEffect.effectFactor = x;
                }, 0f, duration);
                component.DOColor(____upgradeButtonDefaultColor, duration);
            }

            ____skipButton.gameObject.SetActive(value: true);
            ____maxHPButton.gameObject.SetActive(____relicManager.RelicEffectActive(RelicEffect.INC_MAX_HP_IF_FULL_HP));
            ____maxHPButton.GetComponentInChildren<SimpleCostText>(includeInactive: true).SetGold(____currentHealCost);

            //CheckIfHealthChanged
            if (____playerHealthController.CurrentHealth >= ____playerHealthController.MaxHealth)
            {
                ____healButton.gameObject.SetActive(value: false);
            }
            else
            {
                ____healButton.gameObject.SetActive(value: true);
                ____healButton.GetComponentInChildren<Localize>().Term = "Menu/post_battle_heal_percent";
                LocalizationParamsManager componentInChildren = ____healButton.GetComponentInChildren<LocalizationParamsManager>();
                componentInChildren.SetParameterValue("POST_BATTLE_HEAL_PERCENT", Mathf.RoundToInt(____playerHealthController.endOfBattleHealPercent * 100f).ToString(CultureInfo.InvariantCulture));
                componentInChildren.SetParameterValue("POST_BATTLE_HEAL_AMOUNT", ____playerHealthController.endOfBattleHealAmount.ToString());
                ____healButton.GetComponentInChildren<SimpleCostText>().SetGold(____currentHealCost);
            }

            return false;
        }
    }
}
