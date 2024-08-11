using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        private float _playerMoveSpeed;
        [SerializeField]
        private float _playerSlideSpeed;
        [SerializeField]
        private float _playerJumpPower;
        private CharacterController _characterController;
        private Rigidbody _rigidbody;
        private PlayerState _playerState;

        private bool UsedSecondJump = false;

        private bool _isPlaying;
        Vector3 _moveDirection = Vector3.zero;
        int _sideDirection = 0;
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
            _playerState = GetComponent<PlayerState>();

            _moveDirection.z = _playerMoveSpeed;
            _playerState.OnChangePlayerState
                .Where(x => x != StateType.Idle)
                .Subscribe(x => {
                    _isPlaying = true;
                });

            this.UpdateAsObservable()
                .Where(_ => _isPlaying)
                .Where(_ => Input.GetKey(KeyCode.Space))
                .ThrottleFirst(TimeSpan.FromSeconds(0.5))
                .Subscribe(_ =>
                {
                    if(_playerState.GetPlayerState == StateType.Dash)
                    {
                        _playerState.ChangeState(StateType.Jump);
                        _moveDirection.y = _playerJumpPower;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                        Debug.Log($"Jump! {_moveDirection.y}");
                    }
                    else if (_playerState.GetPlayerState == StateType.Jump)
                    {
                        _playerState.ChangeState(StateType.SecondJump);
                        _moveDirection.y = _playerJumpPower;
                        _characterController.Move(_moveDirection * Time.deltaTime);
                    }
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

            _moveDirection.y += Physics.gravity.y * Time.deltaTime;

            if (_characterController.isGrounded)
            {
                _moveDirection.y = -0.5f;
                if (_playerState.GetPlayerState == StateType.Jump || _playerState.GetPlayerState == StateType.SecondJump)
                {
                    _playerState.ChangeState(StateType.Dash);
                }
            }
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }

}
