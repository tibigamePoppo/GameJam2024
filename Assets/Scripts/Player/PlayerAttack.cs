using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Unity.Burst.CompilerServices;
using System.Threading;
using Audio;
using static Unity.VisualScripting.Member;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField]
        private GameObject _boxcollderPosition;
        private PlayerState _playerState;
        private PlayerStatus _playerStatus;
        private Vector3 halfExtents = new Vector3(0.8f, 0.8f, 1f); // 各軸についてのボックスサイズの半分

        private bool _isPlaying;
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _playerStatus = GetComponent<PlayerStatus>();
            _playerState.OnChangePlayerState
                .Where(x => x != StateType.Idle)
                .Subscribe(x =>
                {
                    _isPlaying = true;
                });
            this.UpdateAsObservable()
                .Where(_ => _isPlaying)
                .Where(_ => Input.GetKey(KeyCode.W))
                .Subscribe(_ =>
                {
                    if (_playerState.GetPlayerState != StateType.Attack)
                    {
                        _playerState.ChangeState(StateType.Attack);
                        AttackAsync().Forget();
                    }

                }).AddTo(this);

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dead)
                .Subscribe(x => {
                    _isPlaying = false;
                });

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Attack)
                .Delay(TimeSpan.FromSeconds(0.65f))
                .Subscribe(_ =>
                {
                    _playerState.ChangeState(StateType.Dash);
                });
        }
        private async UniTaskVoid AttackAsync()
        {
            var elapsedTime = 0.0f;
            var findAttack = false;
            while (1.5f > elapsedTime)
            {
                var center = _boxcollderPosition.transform.position;
                var hits = Physics.BoxCastAll
                (
                    center: center,
                    halfExtents: halfExtents,
                    direction: transform.forward,
                    orientation: Quaternion.identity,
                    maxDistance: 2,
                    layerMask: LayerMask.GetMask("Attackable")
                );
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.TryGetComponent(out Rigidbody rigidbody))
                    {
                        Vector3 shotDirection =  hit.transform.position -transform.position - new Vector3(0, 1.5f, -5);
                        shotDirection.y = Math.Abs(shotDirection.y);
                        shotDirection = Vector3.Normalize(shotDirection);
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.AddForce(shotDirection * (10 + _playerStatus.GetMoveSpeed * 1.5f), ForceMode.Impulse);
                        Debug.Log($"HIT!! {hit.transform.position} ");
                        findAttack = true;
                    }
                }
                if (findAttack) break;  // 攻撃対象への処理が完了したらタスク終了
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }
        void OnDrawGizmos()
        {
            var center = _boxcollderPosition.transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, halfExtents * 2);
        }
    }

}