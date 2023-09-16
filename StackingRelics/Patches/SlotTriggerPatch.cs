using Battle.Pachinko.BallBehaviours;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(SlotTrigger), "OnTriggerEnter2D")]
    public class SlotTriggerOnTriggerEnter2DPatch : Singleton<SlotTriggerOnTriggerEnter2DPatch>
    {
        public static Boolean Prefix(Color ____originalColor, Color ___badColor, Color ___goodColor, ref float ____multiplier, ref int ___index, TMP_Text ____slotText, SpriteRenderer ___slotGradient, ref int ____portalUsageCount, ref bool ____isPortal, Collider2D collision)
        {
            PachinkoBall component = collision.GetComponent<PachinkoBall>();
            if (!(component != null))
            {
                return false;
            }
            TeleportToTop component2 = collision.GetComponent<TeleportToTop>();

            int numTeleports = 3;
            if (RelicManage.relicQuantity.ContainsKey(Relics.RelicEffect.SLOT_PORTAL))
            {
                numTeleports = RelicManage.relicQuantity[Relics.RelicEffect.SLOT_PORTAL] * 3;
            }
            if ((BattleController.AwaitingShotCompletion() && ____isPortal && ____portalUsageCount++ < numTeleports) || (component2 != null && component2.CanTeleport()))
            {
                Vector3 position = collision.transform.position;
                position.y = StaticGameData.battleYBounds[1];
                component.TeleportTo(position);
                if (____portalUsageCount >= numTeleports)
                {
                    ___slotGradient.DOFade(0f, 0.65f);
                }
                return false;
            }
            SlotTrigger.onSlotTriggerActivated(___index);
            if (BattleController.AwaitingShotCompletion() && ____slotText != null && ____slotText.isActiveAndEnabled && Math.Abs(____multiplier - 1f) > 0.05f)
            {
                Color endValue = ((____multiplier > 1f) ? ___goodColor : ___badColor);
                float interval = ((____multiplier > 1f) ? 0.25f : 0.45f);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(____slotText.DOColor(endValue, 0.1f));
                sequence.AppendInterval(interval);
                sequence.Append(____slotText.DOColor(____originalColor, 0.1f));
                sequence.Play();
            }
            return false;
        }
    }
}
