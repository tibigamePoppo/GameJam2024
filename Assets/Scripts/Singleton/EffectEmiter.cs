using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Singleton.Effect
{
    public class EffectEmiter : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> effects = new List<GameObject>();
        public static EffectEmiter Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

        }
        public void EmitEffect(EffectType type, Vector3 emitPosition)
        {
            var emitEffect = effects.First(e => e.name.Equals(type.ToString()));
            var effect = Instantiate(emitEffect,emitPosition,Quaternion.identity);
            Destroy(effect,10f);
        }
    }
}