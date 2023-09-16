using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingRelics
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    var go = new GameObject(typeof(T).Name);
                    GameObject.DontDestroyOnLoad(go);
                    _instance = go.AddComponent(typeof(T)) as T;
                }
                return _instance;
            }
        }
        private static T _instance;

        public static void EnsureInstanceCreated()
        {
            var throwAway = Instance;
        }
    }
}
