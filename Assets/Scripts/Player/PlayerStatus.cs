using Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int _startHP;
    private int _currentHp;
    void Start()
    {
        _currentHp = _startHP;
    }
    public void Damage(int damage) 
    { 
        _currentHp = damage > 0 ? _currentHp - damage : 0;
        Debug.Log($"ダメージを受けた。現在体力は{_currentHp}");
        if (_currentHp <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
