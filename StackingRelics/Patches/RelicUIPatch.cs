using DG.Tweening;
using HarmonyLib;
using PeglinUI.LoadoutManager;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace StackingRelics.Patches.RelicPatches
{
    [HarmonyPatch(typeof(RelicUI),"UpdateRelicCountdown")]
    internal class RelicUIUpdateRelicCountdownPatch : Singleton<RelicUIUpdateRelicCountdownPatch>
    {
        public static Boolean Prefix(Relic relic, int countdown)
        {
            if (RelicManage.relicQuantity.ContainsKey(relic.effect))
            {
                RelicManage.relicIcons[relic.effect][RelicManage.currentRelicAdd].SetCountdownText(countdown.ToString());                  
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicUI), "AddRelic")]
    internal class RelicUIAddRelicPatch : Singleton<RelicUIAddRelicPatch>
    {
        public static Boolean Prefix(RelicUI __instance, GameObject ___relicIconPrefab, Dictionary<RelicEffect, GameObject> ____animatedRelicPrefabs, Dictionary<RelicEffect, RelicIcon> ___icons, ImageCarousel ____imageCarousel, Relic toAdd)
        {
            GameObject original = ((!____animatedRelicPrefabs.ContainsKey(toAdd.effect)) ? ___relicIconPrefab : ____animatedRelicPrefabs[toAdd.effect]);
            GameObject gameObject = UnityEngine.Object.Instantiate(original);
            RelicIcon component = gameObject.GetComponent<RelicIcon>();
            component.SetRelic(toAdd);
            Debug.LogWarning($"Setting icon {component} to relic {toAdd}.");
            if (!___icons.ContainsKey(toAdd.effect))
            {
                ___icons.Add(toAdd.effect, component);
            }
            gameObject.transform.SetParent(__instance.gameObject.transform, worldPositionStays: false);
            gameObject.GetComponent<Image>().DOFade(1f, 0.5f).From(0f);
            ____imageCarousel.AddElement(gameObject);
            if (!RelicManage.relicIcons.ContainsKey(toAdd.effect))
            {
                RelicManage.relicIcons.Add(toAdd.effect, new Dictionary<int, RelicIcon>());
            }
            RelicManage.relicIcons[toAdd.effect].Add(RelicManage.currentRelicAdd2, component);
            return false;
        }
    }
    [HarmonyPatch(typeof(RelicUI),"UseRelic")]
    internal class RelicUIUseRelicPatch : Singleton<RelicUIUseRelicPatch>
    {
        public static Boolean Prefix(AudioSource ____audioSource, Dictionary<RelicEffect, RelicIcon> ___icons, Relic relic)
        {
            if (___icons.ContainsKey(relic.effect))
            {
                
                RelicManage.relicIcons[relic.effect][RelicManage.currentRelicAdd].Flash();
                if (RelicManager.relicCountdownValues.ContainsKey(relic.effect))
                {
                    RelicManage.relicIcons[relic.effect][RelicManage.currentRelicAdd].ResetCountdownText();
                }
            }
            if (relic.useSfx != null)
            {
                ____audioSource.PlayOneShot(relic.useSfx);
            }
            return false;
        }
    }
}
