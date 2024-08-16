using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using Config;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private float _playerMoveSpeed;
        private float _basePlayerMoveSpeed;
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
        float time = 0;
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
                .Subscribe(x =>
                {
                    _isPlaying = true;
                });
            _playerStatus.OnChangeMoveSpeed
                .Skip(1)
                .Subscribe(x =>
                {
                    ConfigParameter.globalSpeed = 1 + (_playerMoveSpeed - _basePlayerMoveSpeed) / 50;
                    _playerMoveSpeed = x;
                }).AddTo(this);
            _playerStatus.OnChangeMoveSpeed
                .First()
                .Subscribe(x =>
                {
                    _playerMoveSpeed = x;
                    _basePlayerMoveSpeed = _playerMoveSpeed;
                    _moveDirection.z = _playerMoveSpeed;
                }).AddTo(this);

            _playerState.OnChangePlayerState
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case StateType.Attack:
                            _moveDirection.y = _moveDirection.y < 1 ? _moveDirection.y + 1 : _moveDirection.y;
                            break;
                        case StateType.DownAttack:
                            _moveDirection.y = _moveDirection.y > -6 ? -6 : _moveDirection.y - 6;
                            break;
                        case StateType.WideAttack:
                            _moveDirection.y = _moveDirection.y > -1 ? _moveDirection.y : -0.5f;
                            break;
                    }
                });


            //ジャンプの処理
            this.UpdateAsObservable()
                .Where(_ => _isPlaying
                            && (_playerState.GetPlayerState != StateType.Dash
                            || _playerState.GetPlayerState != StateType.Jump))
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {
                    if (_playerState.GetPlayerState == StateType.Dash && isGrounded())
                    {
                        time = 1;
                        _playerState.ChangeState(StateType.Jump);
                        _moveDirection.y = _playerJumpPower * ConfigParameter.globalSpeed;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                    }
                    else if (_playerState.GetPlayerState == StateType.Jump && !isGrounded())
                    {
                        time = 1;
                        _playerState.ChangeState(StateType.SecondJump);
                        _moveDirection.y = _moveDirection.y > (_playerJumpPower * ConfigParameter.globalSpeed) / 2 ? _moveDirection.y + (_playerJumpPower * ConfigParameter.globalSpeed) / 2 : _playerJumpPower * ConfigParameter.globalSpeed;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                    }
                }).AddTo(this);

            //ジャンプ後の着地の判定
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Jump)
                .Delay(TimeSpan.FromSeconds(0.5f))
                .Subscribe(x =>
                {
                    GroundCheck(_source.Token);
                });

            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dead)
                .Subscribe(x =>
                {
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
            if(!isGrounded())
            {
                time += Time.deltaTime * 6.6f * Mathf.Pow(ConfigParameter.globalSpeed, 3.2f);
            }
            if (_playerState.GetPlayerState != StateType.WideAttack && _moveDirection.y >= 0)
            {
                _moveDirection.y -= _playerJumpPower * Time.deltaTime * time;
                if(_moveDirection.y < 0) time = 1;
            }
            else if(_playerState.GetPlayerState != StateType.WideAttack)
            {
                _moveDirection.y -= _playerJumpPower * Time.deltaTime * time;
            }
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
        private async UniTask GroundCheck(CancellationToken token)
        {
            await UniTask.WaitUntil(() => isGrounded(), cancellationToken: token);
            time = 1;
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
