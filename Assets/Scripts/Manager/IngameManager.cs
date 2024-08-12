using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
namespace Manager
{
    public enum IngameType
    {
        Ready,
        Ingame,
        Pause
    }
    public class IngameManager : MonoBehaviour
    {
        ReactiveProperty<IngameType> _state = new ReactiveProperty<IngameType>();
        public IObservable<IngameType> OnChangeIngameState { get { return _state; } }
        ReactiveProperty<float> _score = new ReactiveProperty<float>(0);
        public IObservable<float> Score { get { return _score; } }

        void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.Escape))
                .Subscribe(_ =>
                {
                    if(_state.Value == IngameType.Ingame)
                    {
                        ChangeState(IngameType.Pause);
                    }
                    else
                    if (_state.Value == IngameType.Pause)
                    {
                        ChangeState(IngameType.Ingame);
                    }
                }).AddTo(this);
            ChangeState(IngameType.Ingame);
        }

        public void ChangeState(IngameType state)
        {
            _state.Value = state;
        }
        public void AddScor(float value)
        {

        }
    }

}