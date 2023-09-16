using Battle;
using Battle.Attacks;
using Battle.Attacks.AttackBehaviours;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.Serialization;
using UnityEngine;


namespace StackingRelics.Patches
{
    public static class RelicManage
    {
        public static Dictionary<RelicEffect, int> relicQuantity = new Dictionary<RelicEffect, int>();
        public static Dictionary<RelicEffect, Dictionary<int, Relic>> relicStorage = new Dictionary<RelicEffect, Dictionary<int, Relic>>();
        
        public static Dictionary<RelicEffect, Dictionary<int, RelicIcon>> relicIcons = new Dictionary<RelicEffect, Dictionary<int, RelicIcon>>();

        public static Dictionary<RelicEffect, Dictionary<int, int>> relicOrder = new Dictionary<RelicEffect, Dictionary<int, int>>();
        public static int relicCount = 0;
        
        public static Dictionary<RelicEffect, Dictionary<int, int>> relicRemainingCountdowns = new Dictionary<RelicEffect, Dictionary<int, int>>();
        public static Dictionary<RelicEffect, Dictionary<int, int>> relicRemainingUsesPerBattle = new Dictionary<RelicEffect, Dictionary<int, int>>();
        public static Dictionary<RelicEffect, Dictionary<int, int>> relicRemainingUsesPerRun = new Dictionary<RelicEffect, Dictionary<int, int>>();

        public static int currentRelicAdd = 0;
        public static int currentRelicAdd2 = 0;

        public static void EffectChanger(ref RelicManager relicManager, RelicEffect re)
        {
            switch (re)
            {
                case RelicEffect.MATRYOSHKA:
                    //relicManager.MATRYOSHKA_MULTIBALL_LEVEL = 5;
                    break;
                case RelicEffect.ALL_ORBS_BUFF:
                    RelicManager.ALL_ORBS_BUFF_AMOUNT = relicQuantity[RelicEffect.ALL_ORBS_BUFF] * 2;
                    break;
                case RelicEffect.ALL_ATTACKS_ECHO:
                    RelicManager.ECHO_CHAMBER_PERCENT = relicQuantity[RelicEffect.ALL_ATTACKS_ECHO] * .1f;
                    break;
                case RelicEffect.BUFF_FIRST_PEG_HIT:
                    RelicManager.FIRST_PEG_HIT_BUFF_AMOUNT = relicQuantity[RelicEffect.BUFF_FIRST_PEG_HIT] * 10;
                    break;
            }
        }
        public class RelicManageSaveData : SaveObjectData
        {
            public static readonly string KEY = "RelicManage";
            public override string Name => KEY;
            [SerializeField]
            public Dictionary<RelicEffect, int> _relicQuantity;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, Relic>> _relicStorage;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, RelicIcon>> _relicIcons;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, int>> _relicOrder;
            [SerializeField]
            public int _relicCount;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, int>> _relicRemainingCountdowns;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, int>> _relicRemainingUsesPerBattle;
            [SerializeField]
            public Dictionary<RelicEffect, Dictionary<int, int>> _relicRemainingUsesPerRun;

            public RelicManageSaveData() : base(deleteOnLose: true, DataSerializer.SaveType.RUN)
            {
                _relicQuantity = relicQuantity;
                _relicStorage = relicStorage;
                _relicIcons = relicIcons;   
                _relicOrder = relicOrder;
                _relicCount = relicCount;
                _relicRemainingCountdowns = relicRemainingCountdowns;
                _relicRemainingUsesPerBattle = relicRemainingUsesPerBattle;
                _relicRemainingUsesPerRun = relicRemainingUsesPerRun;
            }
        }
        public static void saveData()
        {
            new RelicManageSaveData().Save();
        }
        public static void loadData(RelicManager relicManager, RelicSet globalRelics)
        {
            relicQuantity.Clear();
            relicStorage.Clear();
            relicIcons.Clear();
            relicOrder.Clear();
            relicCount = 0;
            relicRemainingCountdowns.Clear();
            relicRemainingUsesPerBattle.Clear();
            relicRemainingUsesPerRun.Clear();

            RelicManageSaveData relicManageSaveData = (RelicManageSaveData)DataSerializer.Load<SaveObjectData>(RelicManageSaveData.KEY,DataSerializer.SaveType.RUN);
            if (relicManageSaveData == null)
            {
                Debug.LogWarning($"relicManageSaveData null for some reason");
                return;
            }

            


            int doAllRelics = 0;
            while (doAllRelics < relicManageSaveData._relicCount)
            {
                foreach(RelicEffect re in relicManageSaveData._relicOrder.Keys)
                {
                    if (relicManageSaveData._relicOrder[re].ContainsValue(doAllRelics)){
                        foreach (int relicNum in relicManageSaveData._relicOrder[re].Keys)
                        {
                            if (relicManageSaveData._relicOrder[re][relicNum] == doAllRelics)
                            {
                                foreach (Relic relic in globalRelics.relics)
                                {
                                    if (relic.effect == re)
                                    {
                                        relicManager.AddRelic(relic);

                                        if (RelicManager.relicCountdownValues.ContainsKey(relic.effect))
                                        {
                                            relicIcons[re][relicNum].SetCountdownText(relicManageSaveData._relicRemainingCountdowns[re][relicNum].ToString());
                                            relicRemainingCountdowns[re][relicNum] = relicManageSaveData._relicRemainingCountdowns[re][relicNum];
                                        }

                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                doAllRelics++;
            }
        }
        public static void loadRelic(RelicEffect re, int relicNum)
        {
            currentRelicAdd2 = relicNum;
            RelicManager.OnRelicAdded(relicStorage[re][relicNum]);
        }
    }
    
}
