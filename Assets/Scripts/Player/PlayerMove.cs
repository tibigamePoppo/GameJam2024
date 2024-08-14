using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using static Unity.VisualScripting.Member;
using Audio;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private float _playerMoveSpeed;
        [SerializeField]
        private float _playerSlideSpeed;
        [SerializeField]
        private float _playerJumpPower;
        [SerializeField] private LayerMask groundLayer;

        private CharacterController _characterController;
        private Rigidbody _rigidbody;
        private PlayerState _playerState;
        private CancellationTokenSource _source;
        private PlayerStatus _playerStatus;

        private bool UsedSecondJump = false;

        private bool _isPlaying;
        Vector3 _moveDirection = Vector3.zero;
        int _sideDirection = 0;

        public float PlayerMoveSpeed { get => _playerMoveSpeed; }

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
            _playerState = GetComponent<PlayerState>();
            _playerStatus = GetComponent<PlayerStatus>();
            _source = new CancellationTokenSource();

            _playerState.OnChangePlayerState
                .Where(x => x != StateType.Idle)
                .Subscribe(x => {
                    _isPlaying = true;
                });
            _playerStatus.OnChangeMoveSpeed
                .Subscribe(x =>
                {
                    _playerMoveSpeed = x;
                }).AddTo(this);
            _playerStatus.OnChangeMoveSpeed
                .First()
                .Subscribe(x =>
                {
                    _moveDirection.z = _playerMoveSpeed;
                }).AddTo(this);

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.DownAttack)
                .Subscribe(x => {
                    MoveDown();
                });

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Attack)
                .Subscribe(x => {
                    _moveDirection.y = _moveDirection.y < 1 ? _moveDirection.y + 1: _moveDirection.y;
                });

            //ジャンプの処理
            this.UpdateAsObservable()
                .Where(_ => _isPlaying && _playerState.GetPlayerState != StateType.Attack && _isPlaying)
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .ThrottleFirst(TimeSpan.FromSeconds(0.4))
                .Subscribe(_ =>
                {
                    if (_playerState.GetPlayerState == StateType.Dash && isGrounded())
                    {
                        _playerState.ChangeState(StateType.Jump);
                        _moveDirection.y = _playerJumpPower;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                    }
                    else if (_playerState.GetPlayerState == StateType.Jump && !isGrounded())
                    {
                        _playerState.ChangeState(StateType.SecondJump);
                        _moveDirection.y = _playerJumpPower;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                    }
                }).AddTo(this);

            //ジャンプ後の着地の判定
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Jump)
                .Delay(TimeSpan.FromSeconds(0.5f))
                .Subscribe(x => {
                    GroundCheck(_source.Token);
                });

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dead)
                .Subscribe(x => {
                    _isPlaying = false;
                });
            //キャラクターの加速

            this.UpdateAsObservable()
                .Where(_ => _isPlaying)
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    _playerStatus.AddSpeed(0.1f);
                    _moveDirection.z = _playerMoveSpeed;
                }).AddTo(this);
        }
        public void MoveDown()
        {
            _moveDirection.y = _moveDirection.y > -2 ? -2 : _moveDirection.y - 2;
        }
        private void Update()
        {
            if (!_isPlaying) return;
            _moveDirection.x = 0;
            if (Input.GetKey(KeyCode.A))
            {
                _moveDirection.x -= _playerSlideSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _moveDirection.x += _playerSlideSpeed;
            }

            _moveDirection.y += Physics.gravity.y * Time.deltaTime;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }
        private async UniTask GroundCheck(CancellationToken token)
        {
            await UniTask.WaitUntil(() => isGrounded(), cancellationToken : token);
            if (_playerState.GetPlayerState == StateType.Jump || _playerState.GetPlayerState == StateType.SecondJump || _playerState.GetPlayerState == StateType.DownAttack)
            {
                _playerState.ChangeState(StateType.Dash);
            }
        }
        private bool isGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
        }
        private void OnDestroy()
        {
            _source.Cancel();
            _source.Dispose();
        }
    }

}
