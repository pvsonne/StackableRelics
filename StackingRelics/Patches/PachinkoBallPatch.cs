using Battle.Attacks;
using Battle.Pachinko.BallBehaviours;
using Battle.StatusEffects;
using HarmonyLib;
using Relics;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Rewired;
using static PachinkoBall;
using static PixelCrushers.DialogueSystem.UnityGUI.GUIProgressBar;
using UI;
using static UnityEngine.ParticleSystem.PlaybackState;
using System.Security.Policy;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(PachinkoBall), "SpawnMultiballFromLocation")]
    public class PachinkoBallSpawnMultiballFromLocationPatch : Singleton<PachinkoBallSpawnMultiballFromLocationPatch>
    {
        public static Boolean Prefix(ref GameObject __result, List<PachinkoBall> ____childMultiballs, PlayerStatusEffectController ____playerStatusEffectController, Vector2 ____previousAimVector, PredictionManager ____predictionManager, RelicManager ____relicManager, GameObject multiballGameObject, Vector3 fireFromPosition, Vector2 fireForce, Attack customAttack = null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(multiballGameObject.gameObject, fireFromPosition, Quaternion.identity);
            PachinkoBall component = gameObject.GetComponent<PachinkoBall>();
            gameObject.GetComponent<TrajectorySimulation>().enabled = false;
            component.Init(____relicManager, ____previousAimVector, ____predictionManager, ____playerStatusEffectController);
            if (customAttack != null)
            {
                int num = 0;
                Multiball component2 = multiballGameObject.GetComponent<Multiball>();
                if (component2 != null)
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.MATRYOSHKA]; i++)
                    {
                        num++;
                    }
                }
                component.multiballLevel = num;
            }
            else
            {
                customAttack = multiballGameObject.GetComponent<Attack>();
                component.multiballLevel = multiballGameObject.GetComponent<PachinkoBall>().multiballLevel;
            }
            component.GetComponent<Attack>().CopyValuesFromDeckAttack(customAttack);
            UnlimitedUpgrades component3 = multiballGameObject.GetComponent<UnlimitedUpgrades>();
            if (component3 != null)
            {
                gameObject.GetComponent<UnlimitedUpgrades>().timesUpgraded = component3.timesUpgraded;
            }

            PachinkoBallResolveMagnetBehaviorPatch.Prefix(____relicManager, gameObject);

            Rigidbody2D component4 = gameObject.GetComponent<Rigidbody2D>();
            component4.simulated = true;
            component4.gravityScale = component.GravityScale;
            component4.AddForce(fireForce);
            component.SetFiring();
            ____childMultiballs.Add(component);
            __result = gameObject;
            return false;
        }
    }
    [HarmonyPatch(typeof(PachinkoBall), "Init")]
    public class PachinkoBallInitPatch : Singleton<PachinkoBallInitPatch>
    {
        public static void Postfix(PachinkoBall __instance, Vector2 ____aimVector, float ____aimLimiterOffsetAngle, Vector2 ____preRotatedAimVector, bool ____refreshPegHitThisTurn, PlayerStatusEffectController ____playerStatusEffectController, PredictionManager ____predictionManager, Vector2 ____previousAimVector, RelicManager ____relicManager, RelicManager relicManager, Vector2 previousAimDir, PredictionManager predictionManager, PlayerStatusEffectController playerStatusEffectController)
        {
            
            if (____relicManager != null && ____relicManager.RelicEffectActive(RelicEffect.ALL_ORBS_MORBID) && __instance.GetComponent<ResetPegOnCollision>() == null)
            {
                MorbidPachinkoBehaviour component = __instance.gameObject.GetComponent<MorbidPachinkoBehaviour>();
                if (component == null)
                {
                    __instance.gameObject.AddComponent<MorbidPachinkoBehaviour>();

                }
                else
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_ORBS_MORBID] - 1; i++)
                    {
                        component.morbidLevel++;
                        Debug.Log("Adding 1 to morbid level");
                    }
                    Debug.Log($"Final morbid level: {component.morbidLevel}");
                }
            }
        }
    }
    [HarmonyPatch(typeof(PachinkoBall), "ResolveMagnetBehavior")]
    public class PachinkoBallResolveMagnetBehaviorPatch : Singleton<PachinkoBallResolveMagnetBehaviorPatch>
    {
        public static Boolean Prefix(RelicManager ____relicManager, GameObject orb)
        {
            MagnetBehavior component = orb.GetComponent<MagnetBehavior>();
            float num1 = 0;
            float num2 = 0;
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.PEG_MAGNET))
            {
                num1 = 4f * RelicManage.relicQuantity[RelicEffect.PEG_MAGNET];
                num2 = 8f * RelicManage.relicQuantity[RelicEffect.PEG_MAGNET];
            }
            if (____relicManager.RelicEffectActive(RelicEffect.PEG_MAGNET) && component == null)
            {
                orb.AddComponent<MagnetBehavior>().Initialize(num1, num2);
            }
            else if (____relicManager.RelicEffectActive(RelicEffect.PEG_MAGNET) && component != null)
            {
                component.Initialize(num1, num2);
            }
            else if (component != null)
            {
                component.Initialize(0f, 0f);
            }
            return false;
        }
    }
}
