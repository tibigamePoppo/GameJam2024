using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Manager;

namespace Player
{
    public enum StateType
    {
        Idle,
        Dash,
        Jump,
        SecondJump,
        Attack
    }
    public class PlayerState : MonoBehaviour
    {
        ReactiveProperty<StateType> _state = new ReactiveProperty<StateType>();
        public IObservable<StateType> OnChangePlayerState { get { return _state; } }//_health(体力)が変化した際にイベントが発行
        public StateType GetPlayerState {  get { return _state.Value; } }

        private void Start()
        {
            var Manager = FindObjectOfType<IngameManager>();
            Manager.OnChangeIngameState
                .Where(x => x == IngameType.Ingame)
                .Subscribe(_ => ChangeState(StateType.Dash)).AddTo(this);
        }

        public void ChangeState(StateType state)
        {
            Debug.Log($"changeState {state}");
            _state.Value = state;
        }
        private void Update()
        {
            Debug.Log($"CurrentState is {GetPlayerState}");
        }
    }
}