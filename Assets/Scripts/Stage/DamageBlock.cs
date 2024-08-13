using Interface;
using Singleton.Effect;
using UnityEngine;

public class DamageBlock : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int _damage;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.TryGetComponent(out IDamagable damagable))
        {
            EffectEmiter.Instance.EmitEffect(EffectType.WallHit, other.ClosestPointOnBounds(this.transform.position));
            damagable.Damage(_damage);
        }
    }
    [SerializeField]
    private int _hp;

    public void Damage(int damage)
    {
        _hp = _hp - damage;
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
