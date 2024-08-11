using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private PlayerState _playerState;

        private bool _isPlaying;
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _playerState.OnChangePlayerState
                .Where(x => x != StateType.Idle)
                .Subscribe(x => {
                _isPlaying = true;
            });
            this.UpdateAsObservable()
                .Where(_ => _isPlaying)
                .Where(_ => Input.GetKey(KeyCode.W))
                .Subscribe(_ =>
                {
                    if (_playerState.GetPlayerState != StateType.Attack)
                    {
                        Debug.Log("Attack");
                        _playerState.ChangeState(StateType.Attack);
                    }
                    
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Attack)
                .Delay(TimeSpan.FromSeconds(1f))
                .Subscribe(_ => {
                    _playerState.ChangeState(StateType.Dash);
                });
        }

        void Update()
        {

        }
    }
}