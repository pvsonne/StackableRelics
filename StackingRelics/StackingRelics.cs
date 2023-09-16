using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using Relics;
using UnityEngine;

namespace StackingRelics
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("peglin.exe")]
    public class StackingRelics : BaseUnityPlugin
    {
        private const string modGUID = "Patrick0331.StackingRelics";
        private const string modName = "StackingRelics by Patrick0331";
        private const string modVersion = "0.0.0.1";
        private readonly Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {modGUID} is loaded!");
        }

        
    }

    
}
