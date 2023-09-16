using Battle.Pachinko;
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
    [HarmonyPatch(typeof(PersistentOrb), "modifiedPersistLevel", MethodType.Getter)]
    public class PersistentOrbModifiedPersistLevelPatch : Singleton<PersistentOrbModifiedPersistLevelPatch> 
    {
        public static Boolean Prefix(ref int __result, RelicManager ____relicManager, ref int ___basePersistLevel)
        {
            __result = ___basePersistLevel;
            if (____relicManager.RelicEffectActive(RelicEffect.ALL_ORBS_PERSIST))
            {
                __result += (RelicManage.relicQuantity[RelicEffect.ALL_ORBS_PERSIST]);
            }
            return false;
        }
    }
}
