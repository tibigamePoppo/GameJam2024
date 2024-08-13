using Cysharp.Threading.Tasks.Triggers;
using Interface;
using Player;
using Singleton.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 normal;
    private Rigidbody rb;
    [SerializeField]
    private int _hp = 3;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        direction = rb.velocity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stage"))
        {
            normal = other.ClosestPointOnBounds(this.transform.position).normalized;
            Vector3 result = Vector3.Reflect(direction, normal);
            rb.velocity = result;
            direction = rb.velocity;
            if (other.gameObject.TryGetComponent(out IDamagable damagable))
            {
                GetDamange(1);
                damagable.Damage(1);
                EffectEmiter.Instance.EmitEffect(EffectType.WallHit, other.ClosestPointOnBounds(this.transform.position));
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            GetDamange(_hp);
        }
    }
    /*

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"colisio {collision.transform.tag}");
        if (collision.gameObject.CompareTag("Stage"))
        {
            normal = collision.contacts[0].normal;
            Vector3 result = Vector3.Reflect(direction, normal);
            rb.velocity = result;
            direction = rb.velocity;

        }
    }*/
    private void GetDamange(int damage)
    {
        _hp = _hp - damage  > 0 ? _hp - damage : 0;
        if(_hp <= 0)
        {
            EffectEmiter.Instance.EmitEffect(EffectType.HammerHit,transform.position);
            Destroy(gameObject);
        }
    }
}
