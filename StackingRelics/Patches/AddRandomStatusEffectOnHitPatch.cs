using Battle.Attacks.AttackBehaviours;
using Battle.Enemies;
using Battle.StatusEffects;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics.Patches
{
    [HarmonyPatch(typeof(AddRandomStatusEffectOnHit), "AffectEnemy")]
    public class AddRandomStatusEffectOnHitAffectEnemyPatch : Singleton<AddRandomStatusEffectOnHitAffectEnemyPatch>
    {
        public static Boolean Prefix(StatusEffectType[] ___potentialEffects, Enemy enemy, RelicManager rm)
        {
            if (rm.AttemptUseRelic(RelicEffect.RANDOM_STATUS_EFFECT_ON_HIT))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.RANDOM_STATUS_EFFECT_ON_HIT]; i++)
                {
                    StatusEffectType effectType = ___potentialEffects[UnityEngine.Random.Range(0, ___potentialEffects.Length)];
                    enemy.ApplyStatusEffect(new StatusEffect(effectType));
                }
            }
            return false;
        }
    }
}
