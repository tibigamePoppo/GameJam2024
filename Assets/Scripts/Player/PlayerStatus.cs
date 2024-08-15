using Audio;
using Interface;
using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerStatus : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private int _startHP;
        [SerializeField]
        private float _playerMoveSpeed = 5;//�����ړ����x
        private const int MAXHP = 18;//HP�̍ő�l
        private PlayerState _playerState;//���݂̏�Ԃ��Ǘ�����X�N���v�g
        private ReactiveProperty<int> _currentHp = new ReactiveProperty<int>();
        private ReactiveProperty<float> _moveSpeed = new ReactiveProperty<float>();
        public IObservable<int> OnChangeCurrentHp { get { return _currentHp; } }
        public IObservable<float> OnChangeMoveSpeed { get { return _moveSpeed; } }
        public int GetCurrentHp { get { return _currentHp.Value; } }
        public float GetMoveSpeed { get { return _moveSpeed.Value; } }
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _moveSpeed.Value = _playerMoveSpeed;
            _currentHp.Value = _startHP;
        }
        public void Damage(int damage)
        {
            _currentHp.Value = damage > 0 ? _currentHp.Value - damage : 0;
            if (_currentHp.Value <= 0)
            {
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
            SEManager.Instance.ShotSE(SEType.Get);
        }
        public void AddSpeed(float add)
        {
            _moveSpeed.Value += add;
        }
    }
}