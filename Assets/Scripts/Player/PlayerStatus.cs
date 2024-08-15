using Audio;
using Cysharp.Threading.Tasks;
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
        private float _playerMoveSpeed = 5;//初期移動速度
        private const int MAXHP = 18;//HPの最大値
        private PlayerState _playerState;//現在の状態を管理するスクリプト
        private ReactiveProperty<int> _currentHp = new ReactiveProperty<int>();
        private ReactiveProperty<float> _moveSpeed = new ReactiveProperty<float>();
        public IObservable<int> OnChangeCurrentHp { get { return _currentHp; } }
        public IObservable<float> OnChangeMoveSpeed { get { return _moveSpeed; } }
        public Subject<Unit> DamageEvent = new Subject<Unit>();
        public IObservable<Unit> OnDamage { get { return DamageEvent; } }

        public int GetCurrentHp { get { return _currentHp.Value; } }
        public float GetMoveSpeed { get { return _moveSpeed.Value; } }
        private bool _isArmor = false;
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _moveSpeed.Value = _playerMoveSpeed;
            _currentHp.Value = _startHP;
        }
        public void Damage(int damage)
        {
            if (_isArmor) return;
            _currentHp.Value = damage > 0 ? _currentHp.Value - damage : 0;
            if (_currentHp.Value <= 0)
            {
                DamageEvent.OnNext(default);
                _playerState.ChangeState(StateType.Dead);
            }
            else
            {
                DamageEvent.OnNext(default);
                SEManager.Instance.ShotSE(SEType.Damage);
            }
            Armor().Forget();
        }
        private async UniTaskVoid Armor()
        {
            _isArmor = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _isArmor = false;
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