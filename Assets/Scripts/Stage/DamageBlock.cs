using Config;
using Interface;
using Singleton.Effect;
using UnityEngine;

public class DamageBlock : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int _damage;
    [SerializeField]
    private GameObject _healOrb;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.TryGetComponent(out IDamagable damagable) && other.transform.root.CompareTag("Player"))
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
            bool SpawnHealOrb = UnityEngine.Random.Range(0, 100) < ConfigParameter.HealDropPerennt;
            if (SpawnHealOrb)
            {
                Instantiate(_healOrb,transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
