using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics.Settings
{
    public class PTSSettingsManager : Singleton<PTSSettingsManager>
    {

        static readonly Color backgroundColor = new Color(0.8f, 0.45f, 0.2f, 0.2f);
        static readonly Color textColor = new Color(0.2f, 0.1961f, 0.2392f, 1.0f);

        public static SettingBoolean randomOrder = new SettingBoolean("RandomOrder", "Randomized Order?", true, backgroundColor);
        public static SettingString relicOne = new SettingString("RelicOne", "Relic 1 ", "", "Enter relic here", backgroundColor, textColor);
        public static SettingString relicTwo = new SettingString("RelicTwo", "Relic 2 ", "", "Enter relic here", backgroundColor, textColor);
        public static SettingString relicThree = new SettingString("RelicThree", "Relic 3 ", "", "Enter relic here", backgroundColor, textColor);

        public static List<SettingString> threeRelics = new List<SettingString>();
        public static int counter = 0;

        private SettingBase[] settings =
        {
            randomOrder,
            relicOne,
            relicTwo,
            relicThree
        };

        
        
        private void Awake()
        {
            LoadSettings();
        }

        private void Start()
        {
            this.InitializePreviousSettings();
            this.SaveSettings();
        }

        public void ApplySettings()
        {
            this.SaveSettings();
            this.InitializePreviousSettings();
        }

        public void LoadSettings()
        {
            foreach (SettingBase setting in settings) { setting.LoadValue(); }
        }

        public void SaveSettings()
        {
            foreach (SettingBase setting in settings) { setting.SaveValue(); }
            PlayerPrefs.Save();

            threeRelics.Clear();
            if (relicOne != "") 
            { 
                threeRelics.Add(relicOne);
            }
            if (relicTwo != "")
            {
                threeRelics.Add(relicTwo);
            }
            if (relicThree != "")
            {
                threeRelics.Add(relicThree);
            }
            counter = UnityEngine.Random.Range(0,threeRelics.Count);
            
        }

        public void InitializePreviousSettings()
        {
            foreach (SettingBase setting in settings) { setting.InitializePreviousValue(); }
        }

        public void RevertSettings()
        {
            foreach (SettingBase setting in settings) { setting.RevertValue(); }
            this.ApplySettings();
        }

        public void ShowSettings()
        {
            foreach (SettingBase setting in settings) { setting.ShowValue(); }
        }

        public void AddApplyActions()
        {
            foreach (SettingBase setting in settings) { setting.AddApplyAction(); }
        }

        public void RemoveApplyActions()
        {
            foreach (SettingBase setting in settings) { setting.RemoveApplyAction(); }
        }

        public void CreateUI()
        {
            foreach (SettingBase setting in settings) { setting.CreateUI(); }
        }
    }
}
