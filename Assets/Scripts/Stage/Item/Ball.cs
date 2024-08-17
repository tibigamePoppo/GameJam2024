using DG.Tweening;
using Interface;
using Manager;
using Singleton.Effect;
using System.Collections.Generic;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace Stage
{

    public class Ball : MonoBehaviour
    {
        private Vector3 direction;
        private Vector3 normal;
        private Rigidbody rb;
        [SerializeField]
        private int _startHp = 3;
        private int _hp;
        private int _bound = 0;
        private List<GameObject> damaged = new List<GameObject>();
        private GameObject _lastReflectObject = null;
        private IngameManager _ingameManager;
        void Start()
        {
            _hp = _startHp;
            _ingameManager = FindObjectOfType<IngameManager>();
            rb = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            direction = rb.velocity;
            var hits = Physics.SphereCastAll
            (
                origin: transform.position,
                radius: 0.6f,
                direction: transform.forward,
                maxDistance: 2,
                layerMask: LayerMask.GetMask("Jumpable", "Wall", "StageOut")
            );
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.layer.Equals(9))
                    {
                        GetDamange(_hp);
                    }
                    else if (hit.collider.gameObject != _lastReflectObject)
                    {
                        Reflect(hit.collider.gameObject);
                        _lastReflectObject = hit.collider.gameObject;
                    }
                }
            }
        }
        private void Reflect(GameObject other)
        {
            if (other.CompareTag("Stage"))
            {
                normal = GetNormal(other, other.layer.Equals(6));
                Vector3 result = Vector3.Reflect(direction, normal);
                rb.velocity = result;
                direction = result;
                if (damaged.Contains(other)) return;
                if (other.TryGetComponent(out IDamagable damagable))
                {
                    GetDamange(1);
                    damagable.Damage(1);
                    EffectEmiter.Instance.EmitEffect(EffectType.WallHit, other.transform.position);
                    damaged.Add(other.gameObject);
                }
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                GetDamange(_hp);
            }
        }
        private Vector3 GetNormal(GameObject target, bool edge)
        {
            //外の壁が判定が抜けやすいので特別な処理
            if(LayerMask.LayerToName(target.layer) == "Jumpable")
            {
                switch(target.name)
                {
                    case "Roof":
                        return -transform.up;
                    case "Floow":
                        return transform.up;
                    case "Right":
                        return -transform.right;
                    case "Left":
                        return transform.right;
                }
            }
            var diffPosition = target.transform.position - transform.position;
            var diffSize = target.transform.localScale / 2;
            //Debug.Log($"diffPosition {diffPosition} : diffSize {diffSize} : target {target.name}");
            float xVector = Mathf.Abs(diffPosition.x / diffSize.x);
            float yVector = Mathf.Abs(diffPosition.y / diffSize.y);
            float zVector =  Mathf.Abs(diffPosition.z / diffSize.z);
            Vector3 normalVector = Vector3.zero;
            float maxVector = Mathf.Max(xVector, yVector, zVector);
            float effectiveVector = edge ? maxVector * 0.75f : maxVector;//有効な成分
                                                                         //Debug.Log($"target = {target.name}, diffPosition = {diffPosition},diffSize = {diffSize}, xVector = {xVector}, yVector = {yVector}, zVector = {zVector}");
            if (effectiveVector <= yVector)  //y成分が一番大きい
            {
                normalVector += diffPosition.y > 0 ? -Vector3.up : Vector3.up;
            }
            if (effectiveVector <= zVector)  //z成分が一番大きい
            {
                normalVector += diffPosition.z > 0 ? -Vector3.forward : Vector3.forward;
            }
            if (effectiveVector <= xVector)  //x 成分が一番大きい
            {
                normalVector += diffPosition.x > 0 ? Vector3.left : -Vector3.left;
            }
            normalVector = normalVector.normalized;
            return normalVector;
        }
        private void GetDamange(int damage)
        {
            _hp = _hp - damage > 0 ? _hp - damage : 0;
            if (_hp <= 0)
            {
                EffectEmiter.Instance.EmitEffect(EffectType.HammerHit, transform.position);
                Destroy(gameObject);
            }
        }
        public void PlayerBound()
        {
            _ingameManager.AddScore(100 * _bound);
            _bound++;
            _hp = _startHp;
        }

        /*
        void OnDrawGizmos()
        {
            //　Capsuleのレイを疑似的に視覚化
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.zero, 0.6f);
        }
        */
    }
}