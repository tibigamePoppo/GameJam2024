using Player;
using Singleton.Effect;
using UnityEngine;

namespace Stage.Item
{
    public class HealOrb : MonoBehaviour
    {
        private const int HEALVALUE = 1;
        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.root.TryGetComponent(out PlayerStatus status))
            {
                status.Heal(HEALVALUE);
                EffectEmiter.Instance.EmitEffect(EffectType.GetHealOrb, other.ClosestPointOnBounds(this.transform.position));
                Destroy(gameObject,1f);
            }
        }
    }
}