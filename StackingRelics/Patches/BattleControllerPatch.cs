using Battle;
using HarmonyLib;
using Relics;
using System;
using UnityEngine;
using Cruciball;
using Data;
using Battle.Attacks.DamageModifiers;
using Battle.Attacks;
using Battle.Enemies;
using Battle.StatusEffects;
using static BattleController;
using System.Collections.Generic;
using Peglin.Achievements;
using UnityEngine.XR;
using System.Collections;
using Battle.Pachinko.BallBehaviours;
using Battle.Pachinko;

namespace StackingRelics.Patches
{

    [HarmonyPatch(typeof(BattleController), "CheckRelicsForStartingBombCount")]
    public class BattleControllerCheckRelicsForStartingBombCountPatch : Singleton<BattleControllerCheckRelicsForStartingBombCountPatch>
    {
        public static Boolean Prefix(BattleController __instance, PegManager ____pegManager, RelicManager ____relicManager)
        {
            int num = 0;
            int num2 = 0;
            if (____relicManager.AttemptUseRelic(RelicEffect.ADDITIONAL_STARTING_BOMBS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_STARTING_BOMBS]; i++)
                {
                    num += RelicManager.ADDITIONAL_BOMB_AMOUNT;
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.DOUBLE_BOMBS_ON_MAP))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_BOMBS_ON_MAP]; i++)
                {
                    num *= 2;
                    Bomb[] componentsInChildren = __instance.GetComponentsInChildren<Bomb>();
                    num += componentsInChildren.Length;
                }
            }
            if (num > 0)
            {
                ____pegManager.ConvertPegsToBombs(num);
            }
            if (num2 > 0)
            {
                ____pegManager.ConvertPegsToBombs(num2, rigged: true);
            }
            return false;


        }

    }
    [HarmonyPatch(typeof(BattleController), "MaxDiscardedShots", MethodType.Getter)]
    public class BattleControllerMaxDiscardedShotsPatch : Singleton<BattleControllerMaxDiscardedShotsPatch>
    {
        public static Boolean Prefix(ref int __result, RelicManager ____relicManager)
        {

            int num = 1;
            if (____relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.NO_DISCARD]; i++)
                {
                    num--;
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.ADDITIONAL_DISCARD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_DISCARD]; i++)
                {
                    num++;
                }
            }

            if (num < 0)
            {
                num = 0;
            }
            __result = num;
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "ArmBallForShot")]
    public class BattleControllerArmBallForShotPatch : Singleton<BattleControllerArmBallForShotPatch>
    {
        public static Boolean Prefix(PlayerHealthController ____playerHealthController, PegManager ____pegManager, GameObject ____activePachinkoBall, RelicManager ____relicManager)
        {
            int num = 0;
            if (____relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.MATRYOSHKA]; i++)
                {
                    num++;
                }
            }
            Multiball component = ____activePachinkoBall.GetComponent<Multiball>();
            if (component != null)
            {
                num += component.multiballLevel;
            }
            PachinkoBall component2 = ____activePachinkoBall.GetComponent<PachinkoBall>();
            component2.multiballLevel = num;
            component2.Arm();
            component2.SetTrajectorySimulationRadius();
            ____activePachinkoBall.GetComponent<TrajectorySimulation>().enabled = true;
            ____pegManager.RemoveSlimeTypeFromAllPegs(Peg.SlimeType.DamageReductionSlime);
            ____playerHealthController.damageReductionFromSlime = 0f;
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "Start")]
    public class BattleControllerStartPatch : Singleton<BattleControllerStartPatch>
    {
        public static void Prefix()
        {
            Debug.Log("Running Prefix.");
        }
        public static void Postfix(CruciballManager ____cruciballManager, PegManager ____pegManager, RelicManager ____relicManager)
        {
            int moltenNum = 0;
            if (____relicManager.AttemptUseRelic(RelicEffect.ADDITIONAL_BATTLE_GOLD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_BATTLE_GOLD] - 1; i++)
                {
                    moltenNum += 5;
                }
            }
            int doubleNum = 50;
            if (StaticGameData.dataToLoad != null)
            {
                MapDataBattle mapDataBattle = StaticGameData.dataToLoad as MapDataBattle;
                if (mapDataBattle != null)
                {
                    doubleNum = ____cruciballManager.GetAvailableBattleGold(mapDataBattle.grantedRelicRarity) + mapDataBattle.availableGoldModifier;
                }
            }
            int og5Gold = 5 * (____relicManager.AttemptUseRelic(RelicEffect.ADDITIONAL_BATTLE_GOLD) ? 1 : 0);
            doubleNum += moltenNum + og5Gold;
            int startingAmount = doubleNum;
            if (____relicManager.AttemptUseRelic(RelicEffect.DOUBLE_COINS_AND_PRICES))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_COINS_AND_PRICES] - 1; i++)
                {
                    doubleNum *= 2;
                    Debug.Log("Doubling Amount of Gold");
                }
                Debug.Log("Done Doubling Gold");
            }

            doubleNum -= startingAmount;
            ____pegManager.ApplyGoldToPegs(doubleNum + moltenNum);
        }

    }
    [HarmonyPatch(typeof(BattleController), "GetRiggedBombSelfDamage")]
    public class BattleControllerGetRiggedBombSelfDamagePatch : Singleton<BattleControllerGetRiggedBombSelfDamagePatch>
    {
        public static Boolean Prefix(ref float __result, GameObject ____activePachinkoBall, CruciballManager ____cruciballManager, RelicManager ____relicManager)
        {
            float num = (____cruciballManager.IncreasedRiggedDamage() ? 5 : 3);
            if (____activePachinkoBall != null)
            {
                ReduceRiggedDamageThisTurn component = ____activePachinkoBall.GetComponent<ReduceRiggedDamageThisTurn>();
                SummoningCirclePachinkoBall component2 = ____activePachinkoBall.GetComponent<SummoningCirclePachinkoBall>();
                if (component2 != null)
                {
                    component = component2.orbToSummon.GetComponent<ReduceRiggedDamageThisTurn>();
                }
                if (component != null)
                {
                    num -= (float)component.reductionAmount;
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.ALL_BOMBS_RIGGED))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_BOMBS_RIGGED] - 1; i++)
                {
                    num *= 2;
                }
            }
            __result = num;
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "GetBombDamage")]
    public class BattleControllerGetBombDamagePatch : Singleton<BattleControllerGetBombDamagePatch>
    {
        public static Boolean Prefix(ref float __result, float ____cruciballBombDamage, float ____cruciballRiggedBombDamage, float ____baseBombDamage, float ____baseRiggedBombDamage, RelicManager ____relicManager, CruciballManager ____cruciballManager, bool rigged = false)
        {
            float num = (rigged ? ____baseRiggedBombDamage : ____baseBombDamage);
            if (____cruciballManager.DecreasedBombDamage())
            {
                num = (rigged ? ____cruciballRiggedBombDamage : ____cruciballBombDamage);
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.ADDITIONAL_BOMB_DAMAGE))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_BOMB_DAMAGE]; i++)
                {
                    num += 10f;
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.ADDITIONAL_BOMB_DAMAGE2))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ADDITIONAL_BOMB_DAMAGE2]; i++)
                {
                    num += 15f;
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.BOMBS_RESPAWN))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BOMBS_RESPAWN]; i++)
                {
                    num += -15f;
                }
            }
            if (rigged && ____relicManager.AttemptUseRelic(RelicEffect.ALL_BOMBS_RIGGED))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.ALL_BOMBS_RIGGED]; i++)
                {
                    num *= 2f;
                }
            }
            __result = Mathf.Max(num, 0f);
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "OnBombDeath")]
    public class BattleControllerOnBombDeathPatch : Singleton<BattleControllerOnBombDeathPatch>
    {
        public static void Postfix(RelicManager ____relicManager, EnemyManager ____enemyManager, BombLob bomb)
        {
            foreach (Enemy enemy in ____enemyManager.Enemies)
            {
                if (enemy.CurrentHealth > 0f)
                {
                    if (____relicManager.AttemptUseRelic(RelicEffect.BOMBS_APPLY_BLIND))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BOMBS_APPLY_BLIND] - 1; i++)
                        {
                            enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.Blind));
                        }
                    }
                    if (____relicManager.AttemptUseRelic(RelicEffect.BOMBS_APPLY_POISON))
                    {
                        for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BOMBS_APPLY_POISON] - 1; i++)
                        {
                            enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.Poison));
                        }
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(BattleController), "HandlePachinkoBallBouncerBounce")]
    public class BattleControllerHandlePachinkoBallBouncerBouncePatch : Singleton<BattleControllerHandlePachinkoBallBouncerBouncePatch>
    {
        public static Boolean Prefix(BattleController __instance, BattleState ____battleState, RelicManager ____relicManager, Vector3 pos)
        {
            if ((____battleState != BattleState.NAVIGATION || ____relicManager.RelicEffectActive(RelicEffect.SUPER_BOOTS)) && ____relicManager.AttemptUseRelic(RelicEffect.BOUNCERS_COUNT))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BOUNCERS_COUNT]; i++)
                {
                    __instance.GrantAdditionalBasicPeg(pos);
                    if (____battleState == BattleState.NAVIGATION)
                    {
                        ____relicManager.AttemptUseRelic(RelicEffect.SUPER_BOOTS);
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "HandlePachinkoBallWallBounce")]
    public class BattleControllerHandlePachinkoBallWallBouncePatch : Singleton<BattleControllerHandlePachinkoBallWallBouncePatch>
    {
        public static Boolean Prefix(BattleController __instance, BattleState ____battleState, RelicManager ____relicManager, Vector3 pos)
        {
            if ((____battleState != BattleState.NAVIGATION || ____relicManager.RelicEffectActive(RelicEffect.SUPER_BOOTS)) && ____relicManager.AttemptUseRelic(RelicEffect.WALL_BOUNCES_COUNT))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.WALL_BOUNCES_COUNT]; i++)
                {
                    __instance.GrantAdditionalBasicPeg(pos);
                    if (____battleState == BattleState.NAVIGATION)
                    {
                        ____relicManager.AttemptUseRelic(RelicEffect.SUPER_BOOTS);
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "HandleCoinCollected")]
    public class BattleControllerHandleCoinCollectedPatch : Singleton<BattleControllerHandleCoinCollectedPatch>
    {
        public static Boolean Prefix(BattleController __instance, GameObject ____activePachinkoBall, int ____criticalHitCount, int ____damageBonus, List<float> ____damageAmounts, AttackManager ____attackManager, RelicManager ____relicManager, Vector3 position)
        {
            if (CurrentBattleState != BattleState.NAVIGATION)
            {
                if (____relicManager.RelicEffectActive(RelicEffect.CONVERT_COIN_TO_DAMAGE))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CONVERT_COIN_TO_DAMAGE]; i++)
                    {
                        RelicManage.currentRelicAdd = i;
                        int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.CONVERT_COIN_TO_DAMAGE) ? RelicManage.relicRemainingCountdowns[RelicEffect.CONVERT_COIN_TO_DAMAGE][i] : RelicManager.relicCountdownValues[RelicEffect.CONVERT_COIN_TO_DAMAGE]);
                        num--;
                        if (RelicManage.relicRemainingCountdowns[RelicEffect.CONVERT_COIN_TO_DAMAGE][i] <= 0)
                        {
                            RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.CONVERT_COIN_TO_DAMAGE][i]);
                            RelicManage.relicRemainingCountdowns[RelicEffect.CONVERT_COIN_TO_DAMAGE][i] = RelicManager.relicCountdownValues[RelicEffect.CONVERT_COIN_TO_DAMAGE];
                            TargetingManager.OnDamageTargetedEnemy?.Invoke(____attackManager.GetCurrentDamage(____damageAmounts.ToArray(), __instance.damageMultiplier, ____damageBonus, ____criticalHitCount));
                            continue;
                        }
                        RelicManage.relicRemainingCountdowns[RelicEffect.CONVERT_COIN_TO_DAMAGE][i] = num;
                        RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.CONVERT_COIN_TO_DAMAGE][i], num);
                    }
                }
                if (____relicManager.AttemptUseRelic(RelicEffect.COINS_TO_CRITS))
                {
                    for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.COINS_TO_CRITS]; i++)
                    {
                        BattleControllerActivateCritPatch.Prefix(ref ____criticalHitCount, ref ____activePachinkoBall, ref ____relicManager);
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "ActivateCrit")]
    public class BattleControllerActivateCritPatch : Singleton<BattleControllerActivateCritPatch>
    {
        public static Boolean Prefix(ref int ____criticalHitCount,ref GameObject ____activePachinkoBall,ref RelicManager ____relicManager)
        {
            ____criticalHitCount++;
            onCriticalHitActivated();
            if (____relicManager.AttemptUseRelic(RelicEffect.CRIT_DAMAGES_ENEMIES) && ____activePachinkoBall != null)
            {
                Attack attack = ____activePachinkoBall.GetComponent<Attack>();
                SummoningCirclePachinkoBall component = ____activePachinkoBall.GetComponent<SummoningCirclePachinkoBall>();
                if (component != null)
                {
                    attack = component.copiedOrbsAttack;
                }
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.CRIT_DAMAGES_ENEMIES]; i++)
                {
                    Enemy.OnAllEnemiesDamaged?.Invoke(attack.GetModifiedDamagePerPeg(____criticalHitCount) * (float)____criticalHitCount);
                }
            }
            if (____criticalHitCount >= 5 && ____relicManager.RelicEffectActive(RelicEffect.CRITS_STACK))
            {
                AchievementManager.Instance.UnlockAchievement(AchievementData.AchievementId.MULTIPLE_CRITS);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "AddPeg")]
    public class BattleControllerAddPegPatch : Singleton<BattleControllerAddPegPatch> 
    {
        public static Boolean Prefix(ref int ____damageBonus, ref List<float> ____damageAmounts, ref PlayerHealthController ____playerHealthController, ref RelicManager ____relicManager, float multiplier, int bonus = 0)
        {
            if (____playerHealthController.CurrentHealth / ____playerHealthController.MaxHealth <= RelicManager.LOW_HEALTH_DOUBLE_DAMAGE && ____relicManager.AttemptUseRelic(RelicEffect.LOW_HEALTH_INCREASED_DAMAGE))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.LOW_HEALTH_INCREASED_DAMAGE]; i++)
                {
                    multiplier *= 2f;
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG]; i++)
                {
                    multiplier *= 2f;
                }
            }
            ____damageAmounts.Add(multiplier);
            ____damageBonus += bonus;
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "HandleEnemyDestruction")]
    public class BattleControllerHandleEnemyDestructionPatch : Singleton<BattleControllerHandleEnemyDestructionPatch>
    {
        public static Boolean Prefix(BattleController __instance, ref bool ____victoryTriggeringEnemyKilled, ref bool ____forceEarlySpawn, List<Enemy> ____destroyedEnemies, TargetingManager ____targetingManager, PlayerHealthController ____playerHealthController, EnemyManager ____enemyManager, RelicManager ____relicManager, Enemy enemy)
        {
            SteamStatsManager.Instance?.StateChanged(StateChange.ENEMY_BEATEN);
            if (____destroyedEnemies.Contains(enemy))
            {
                return false;
            }
            ____destroyedEnemies.Add(enemy);
            if (enemy.GetComponent<WinWhenDestroyed>() != null)
            {
                ____victoryTriggeringEnemyKilled = true;
            }
            if (enemy.GetComponent<EnemySubcomponent>() == null && ____relicManager.AttemptUseRelic(RelicEffect.REFRESH_BOARD_ON_ENEMY_KILLED))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REFRESH_BOARD_ON_ENEMY_KILLED]; i++)
                {
                    __instance.ResetField();
                }
            }
            foreach (Enemy enemy2 in ____enemyManager.Enemies)
            {
                if (enemy == enemy2)
                {
                    ____targetingManager.RemoveDeadEnemyFromTargets(enemy);
                    ____enemyManager.RemoveEnemy(enemy);
                    enemy2.gameObject.SetActive(value: false);
                    if (____enemyManager.Enemies.Count == 0)
                    {
                        ____forceEarlySpawn = true;
                    }
                    break;
                }
            }
            if (!enemy.enemyTypes.HasFlag(Enemy.EnemyType.Minion) && ____relicManager.AttemptUseRelic(RelicEffect.GAIN_MAX_HP_ON_ENEMY_DEFEAT))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.GAIN_MAX_HP_ON_ENEMY_DEFEAT]; i++)
                {
                    ____playerHealthController.AdjustMaxHealth(1f);
                }
            }
            if (enemy.isActiveAndEnabled)
            {
                enemy.gameObject.SetActive(value: false);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "DealCheeseDamage")]
    public class BattleControllerDealCheeseDamagePatch : Singleton<BattleControllerDealCheeseDamagePatch>
    {
        public static Boolean Prefix(int ____reloadCount)
        {
            for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.DAMAGE_ENEMIES_ON_RELOAD]; i++)
            { 
                Enemy.OnAllEnemiesDamaged?.Invoke(RelicManager.RELOAD_DAMAGE * (float)____reloadCount);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "ResolveMagnetBehavior")]
    public class BattleControllerResolveMagnetBehaviorPatch : Singleton<BattleControllerResolveMagnetBehaviorPatch>
    {
        public static Boolean Prefix(RelicManager ____relicManager, GameObject ____activePachinkoBall)
        {
            MagnetBehavior component = ____activePachinkoBall.GetComponent<MagnetBehavior>();
            float num1 = 0;
            float num2 = 0;
            if (RelicManage.relicQuantity.ContainsKey(RelicEffect.PEG_MAGNET))
            {
                num1 = 4f * RelicManage.relicQuantity[RelicEffect.PEG_MAGNET];
                num2 = 8f * RelicManage.relicQuantity[RelicEffect.PEG_MAGNET];
            }
            if (____relicManager.RelicEffectActive(RelicEffect.PEG_MAGNET) && component == null)
            {
                ____activePachinkoBall.AddComponent<MagnetBehavior>().Initialize(num1, num2);
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
    [HarmonyPatch(typeof(BattleController), "ShotFired")]
    public class BattleControllerShotFiredPatch : Singleton<BattleControllerShotFiredPatch>
    {
        public static Boolean Prefix(BattleController __instance, DamageCountDisplay ____damageCountDisplay, ref int ____criticalHitCount, DeckManager ____deckManager, ref int ____remainingPachinkoBalls, ref int ____inflightRelicProcCount, ref float ____pachinkoShotTimer, ref float ____aimLimiterRotationDirection, ref float ____currentAimLimiterRotation, ref Vector2 ____previousNonRotatedAimVector, ref Vector2 ____lastShotAimVector, ref Vector2 ____previousAimVector, GameObject ____activePachinkoBall, ref int ____damageBonus, ref List<float> ____damageAmounts, PlayerHealthController ____playerHealthController, RelicManager ____relicManager, ref BattleState ____battleState, Vector2 aimVector)
        {
            GameObject ___activePachinkoBall = ____activePachinkoBall;
            if (____battleState == BattleState.NAVIGATION)
            {
                return false;
            }
            ____playerHealthController.HideSelfDamagePreview();
            if (____relicManager.RelicEffectActive(RelicEffect.MINIMUM_PEGS))
            {
                int quantity = RelicManage.relicQuantity[RelicEffect.MINIMUM_PEGS] * 5;
                for (int i = 0; i < quantity; i++)
                {
                    BattleControllerAddPegPatch.Prefix(ref ____damageBonus, ref ____damageAmounts, ref ____playerHealthController, ref ____relicManager, 1f);
                }
            }
            if (____relicManager.RelicEffectActive(RelicEffect.MULTIBALL_EVERY_X_SHOTS))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.MULTIBALL_EVERY_X_SHOTS]; i++)
                {
                    RelicManage.currentRelicAdd = i;
                    int num = (RelicManage.relicRemainingCountdowns.ContainsKey(RelicEffect.MULTIBALL_EVERY_X_SHOTS) ? RelicManage.relicRemainingCountdowns[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i] : RelicManager.relicCountdownValues[RelicEffect.MULTIBALL_EVERY_X_SHOTS]);
                    num--;
                    if (RelicManage.relicRemainingCountdowns[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i] <= 1)
                    {
                        RelicManager.OnRelicUsed?.Invoke(RelicManage.relicStorage[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i]);
                        RelicManage.relicRemainingCountdowns[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i] = RelicManager.relicCountdownValues[RelicEffect.MULTIBALL_EVERY_X_SHOTS];
                        SummoningCirclePachinkoBall component = ____activePachinkoBall.GetComponent<SummoningCirclePachinkoBall>();
                        if (component != null)
                        {
                            component.addDuplicationStationMultiball = true;
                        }
                        else
                        {
                            ___activePachinkoBall.GetComponent<PachinkoBall>().multiballLevel++;
                        }
                        continue;
                    }
                    RelicManage.relicRemainingCountdowns[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i] = num;
                    RelicManager.OnCountdownDecremented?.Invoke(RelicManage.relicStorage[RelicEffect.MULTIBALL_EVERY_X_SHOTS][i], num);
                }
            }
            ____battleState = BattleState.AWAITING_SHOT_COMPLETION;
            ____previousAimVector = aimVector;
            ____lastShotAimVector = aimVector;
            if (____relicManager.RelicEffectActive(RelicEffect.AIM_LIMITER))
            {
                PachinkoBall component2 = ___activePachinkoBall.GetComponent<PachinkoBall>();
                ____previousNonRotatedAimVector = component2.preRotatedAimVector;
                ____currentAimLimiterRotation += 8f * ____aimLimiterRotationDirection;
                if (____currentAimLimiterRotation >= 45f)
                {
                    ____currentAimLimiterRotation -= 90f;
                }
                else if (____currentAimLimiterRotation <= -45f)
                {
                    ____currentAimLimiterRotation += 90f;
                }
            }
            ____pachinkoShotTimer = 0f;
            ____inflightRelicProcCount = 0;
            ____remainingPachinkoBalls = 1;
            Attack component3 = ____activePachinkoBall.GetComponent<Attack>();
            PersistentOrb component4 = ____activePachinkoBall.GetComponent<PersistentOrb>();
            if (component3 != null && (component4 == null || component4.remainingPersistence == component4.modifiedPersistLevel - 1 || component4.modifiedPersistLevel <= 0))
            {
                if (component3.ExhaustOnFire)
                {
                    ____deckManager.RemoveOrbFromBattleDeck(____activePachinkoBall);
                }
                else if (component3.DestroyOnFire)
                {
                    ____deckManager.RemoveOrbFromBattleDeck(____activePachinkoBall);
                    ____deckManager.RemoveSpecifiedOrbFromDeck(____activePachinkoBall);
                }
            }
            AdjustMaxHPOnFire component5 = ____activePachinkoBall.GetComponent<AdjustMaxHPOnFire>();
            if (component5 != null)
            {
                ____playerHealthController.AdjustMaxHealth(component5.maxHPChange);
            }
            AttackDamagesSelf component6 = ____activePachinkoBall.GetComponent<AttackDamagesSelf>();
            if (component6 != null)
            {
                if (component6.selfDamage > 0f)
                {
                    ____playerHealthController.DealSelfDamage(component6.selfDamage, component6.blockable, PlayerHealthController.DamageSource.ORB_SELF);
                }
                else if (component6.selfDamage < 0f)
                {
                    ____playerHealthController.Heal(0f - component6.selfDamage);
                }
            }
            if (____activePachinkoBall.GetComponent<RefreshBoardOnFired>() != null)
            {
                __instance.ResetField();
            }
            if (____playerHealthController.playerHealthPercent <= RelicManager.GUARANTEED_CRIT_HEALTH_PERCENT && ____relicManager.AttemptUseRelic(RelicEffect.LOW_HEALTH_GUARANTEED_CRIT))
            {
                BattleControllerActivateCritPatch.Prefix(ref ____criticalHitCount, ref ____activePachinkoBall, ref ____relicManager);
                ____damageCountDisplay.DisplayCritText(____criticalHitCount, ____activePachinkoBall.transform.position + Vector3.up * 0.85f, ____criticalHitCount > 1 && ____relicManager.AttemptUseRelic(RelicEffect.CRITS_STACK));
            }
            ____deckManager.ModifyStatsOnFire(____activePachinkoBall);
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleController), "ShuffleDeck")]
    public class BattleControllerShuffleDeckPatch : Singleton<BattleControllerShuffleDeckPatch>
    {
        public static Boolean Prefix(BattleController __instance, DeckManager ____deckManager, ref int ___NumShotsDiscarded, RelicManager ____relicManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            if (____relicManager.AttemptUseRelic(RelicEffect.REFRESH_BOARD_ON_RELOAD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.REFRESH_BOARD_ON_RELOAD]; i++)
                {
                    __instance.ResetField();
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.STR_ON_RELOAD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.STR_ON_RELOAD]; i++)
                {
                    ____playerStatusEffectController.ApplyStatusEffect(new StatusEffect(StatusEffectType.Strength, 2));
                }
            }
            if (____relicManager.AttemptUseRelic(RelicEffect.BAL_ON_RELOAD))
            {
                for (int i = 0; i < RelicManage.relicQuantity[RelicEffect.BAL_ON_RELOAD]; i++)
                {
                    ____playerStatusEffectController.ApplyStatusEffect(new StatusEffect(StatusEffectType.Balance));
                }
            }
            ___NumShotsDiscarded = 0;
            __instance.OnDeckShuffled?.Invoke();
            OnReloadStarted?.Invoke();
            ____deckManager.ShuffleBattleDeck();
            if (____relicManager.AttemptUseRelic(RelicEffect.RANDOMLY_ROLL_DAMAGE))
            {
                foreach (GameObject item in ____deckManager.shuffledDeck)
                {
                    item.GetComponent<Attack>().SetRandomRolledDamage();
                }
            }
            foreach (GameObject item2 in ____deckManager.shuffledDeck)
            {
                item2.GetComponent<Attack>().ResetPreviousOrbDamageModifier();
            }
            return false;
        }
    }
    
}