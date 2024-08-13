using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Interface;
using Player;
using Singleton.Effect;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using System.Linq;

public class Ball : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 normal;
    private Rigidbody rb;
    [SerializeField]
    private int _hp = 3;
    private List<GameObject> damaged = new List<GameObject>();
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
            normal = GetNormal(other.gameObject);
            Vector3 result = Vector3.Reflect(direction, normal);
            rb.velocity = result;
            direction = rb.velocity;
            if (damaged.Contains(other.gameObject)) return;
            if (other.gameObject.TryGetComponent(out IDamagable damagable))
            {
                GetDamange(1);
                damagable.Damage(1);
                EffectEmiter.Instance.EmitEffect(EffectType.WallHit, other.ClosestPointOnBounds(this.transform.position));
                damaged.Add(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            GetDamange(_hp);
        }
    }
    private Vector3 GetNormal(GameObject target)
    {
        var diffPosition = target.transform.position - transform.position;
        var diffSize = (target.transform.localScale) / 2;
        //Debug.Log($"diffPosition {diffPosition} : diffSize {diffSize} : target {target.name}");


        if(diffSize.x < diffPosition.x)
        {
            return Vector3.left;
        }
        else if(-diffSize.x > diffPosition.x)
        {
            return -Vector3.left;
        }
        else if (diffSize.y < diffPosition.y)
        {
            return -Vector3.up;
        }
        else if (-diffSize.y > diffPosition.y)
        {
            return Vector3.up;
        }
        else if (diffSize.z < diffPosition.z)
        {
            return -Vector3.forward;
        }
        else if (-diffSize.z > diffPosition.z)
        {
            return Vector3.forward;
        }
        else
        {
            return Vector3.zero;
        }
    }
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
