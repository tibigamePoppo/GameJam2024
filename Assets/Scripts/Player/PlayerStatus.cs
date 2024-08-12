using Interface;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int _startHP;
    private ReactiveProperty<int> _currentHp = new ReactiveProperty<int>();
    public IObservable<int> OnChangeCurrentHp { get { return _currentHp; } }
    void Start()
    {
        _currentHp.Value = _startHP;
    }
    public void Damage(int damage) 
    { 
        _currentHp.Value = damage > 0 ? _currentHp.Value - damage : 0;
        Debug.Log($"ダメージを受けた。現在体力は{_currentHp.Value}");
        if (_currentHp.Value <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
