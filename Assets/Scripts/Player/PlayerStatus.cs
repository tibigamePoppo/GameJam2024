using Audio;
using Interface;
using Player;
using Singleton.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerStatus : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private int _startHP;
        [SerializeField]
        private float _playerMoveSpeed = 5;
        private const int MAXHP = 18;
        private PlayerState _playerState;
        private ReactiveProperty<int> _currentHp = new ReactiveProperty<int>();
        public IObservable<int> OnChangeCurrentHp { get { return _currentHp; } }
        public int GetCurrentHp { get { return _currentHp.Value; } }
        private ReactiveProperty<float> _moveSpeed = new ReactiveProperty<float>();
        public IObservable<float> OnChangeMoveSpeed{ get { return _moveSpeed; } }
        public float GetMoveSpeed {  get { return _moveSpeed.Value; } }
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _moveSpeed.Value = _playerMoveSpeed;
            _currentHp.Value = _startHP;
        }
        public void Damage(int damage)
        {
            _currentHp.Value = damage > 0 ? _currentHp.Value - damage : 0;
            //Debug.Log($"ダメージを受けた。現在体力は{_currentHp.Value}");
            if (_currentHp.Value <= 0)
            {
                Debug.Log("DEAD");
                _playerState.ChangeState(StateType.Dead);
            }
            else
            {
                SEManager.Instance.ShotSE(SEType.Damage);
            }
        }
        public void Heal(int healValue)
        {
            _currentHp.Value = _currentHp.Value + healValue < MAXHP ? _currentHp.Value + healValue : _currentHp.Value;
            //Debug.Log($"回復した！現在体力は{_currentHp.Value}");
        }
        public void AddSpeed(float add)
        {
            _moveSpeed.Value += add;
        }
    }
}