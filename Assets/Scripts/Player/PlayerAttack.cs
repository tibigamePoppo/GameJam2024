using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Stage;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField]
        private GameObject _boxcollderPosition;
        private PlayerState _playerState;
        private PlayerStatus _playerStatus;
        private Vector3 normalAttackSize = new Vector3(0.8f, 0.8f, 0.8f); // 通常攻撃の当たり判定の大きさ
        private Vector3 jumpAttackSize = new Vector3(0.6f, 1.2f, 0.8f); // 空中攻撃の当たり判定の大きさ
        private Vector3 wideAttackSize = new Vector3(1.2f, 0.6f, 0.8f); // 範囲攻撃の当たり判定の大きさ
        private Vector3 halfExtents = new Vector3(0.8f, 0.8f, 0.8f); // 各軸についてのボックスサイズの半分
        private Vector3 gizmoExtents = new Vector3(0.8f, 0.8f, 0.8f); // 各軸についてのボックスサイズの半分
        private bool _isAttacking = false;

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
                .ThrottleFirst(TimeSpan.FromSeconds(0.4))
                .Subscribe(_ =>
                {
                    if (_playerState.GetPlayerState == StateType.Attack)
                    {
                        halfExtents = wideAttackSize;
                        _playerState.ChangeState(StateType.WideAttack);
                        AttackAsync().Forget();
                    }
                    else if (_playerState.GetPlayerState != StateType.WideAttack
                    && _playerState.GetPlayerState != StateType.DownAttack
                    && !_isAttacking)
                    {
                        halfExtents = normalAttackSize;
                        _playerState.ChangeState(StateType.Attack);
                        AttackAsync().Forget();
                    }

                }).AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => _isPlaying)
                .Where(_ => Input.GetKey(KeyCode.S))
                .Where(_ => _playerState.GetPlayerState == StateType.Jump || _playerState.GetPlayerState == StateType.SecondJump)
                .Subscribe(_ =>
                {
                    halfExtents = jumpAttackSize;
                    _playerState.ChangeState(StateType.DownAttack);
                    AttackAsync().Forget();
                }).AddTo(this);

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dead)
                .Subscribe(x =>
                {
                    _isPlaying = false;
                });

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Attack)
                .Delay(TimeSpan.FromSeconds(0.65f))
                .Subscribe(x =>
                {
                    if (_playerState.GetPlayerState != StateType.WideAttack)
                    {
                        _playerState.ChangeState(StateType.Dash);
                    }
                });
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.WideAttack)
                .Delay(TimeSpan.FromSeconds(0.8f))
                .Subscribe(x =>
                {
                    _playerState.ChangeState(StateType.Dash);
                });
        }
        private async UniTaskVoid AttackAsync()
        {
            _isAttacking = true;
            var elapsedTime = 0.0f;
            var findAttack = false;
            gizmoExtents = halfExtents;
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
                        Vector3 shotDirection = hit.transform.position - transform.position - new Vector3(0, 1.5f, -5);
                        shotDirection.y = Math.Abs(shotDirection.y);
                        shotDirection = Vector3.Normalize(shotDirection);
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.AddForce(shotDirection * (10 + _playerStatus.GetMoveSpeed * 1.5f), ForceMode.Impulse);
                        hit.collider.GetComponent<Ball>().PlayerBound();
                        findAttack = true;
                    }
                }
                if (findAttack) break;  // 攻撃対象への処理が完了したらタスク終了
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            gizmoExtents = Vector3.zero;
            _isAttacking = false;
        }
        /*
        void OnDrawGizmos()
        {
            var center = _boxcollderPosition.transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, gizmoExtents * 2);
        }*/
    }
}