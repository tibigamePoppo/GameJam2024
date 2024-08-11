using Interface;
using UnityEngine;

public class DamageBlock : MonoBehaviour
{
    [SerializeField]
    private int _damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(_damage);
        }
    }
}
