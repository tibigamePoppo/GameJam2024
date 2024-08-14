using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using unityroom.Api;
namespace Manager
{
    public enum IngameType
    {
        Ready,
        Ingame,
        Pause,
        GameOver
    }
    public class IngameManager : MonoBehaviour
    {
        ReactiveProperty<IngameType> _state = new ReactiveProperty<IngameType>();
        public IObservable<IngameType> OnChangeIngameState { get { return _state; } }
        ReactiveProperty<float> _score = new ReactiveProperty<float>(0);
        public IObservable<float> Score { get { return _score; } }

        public float GetScore { get => _score.Value; }

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
            OnChangeIngameState
                .Where(x => x == IngameType.GameOver)
                .First()
                .Subscribe(_ =>
                {
                    UnityroomApiClient.Instance.SendScore(1, 123.45f, ScoreboardWriteMode.Always);
                }).AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => Input.anyKey)
                .First()
                .Subscribe(_ => ChangeState(IngameType.Ingame)).AddTo(this);
            ChangeState(IngameType.Ready);
        }

        public void ChangeState(IngameType state)
        {
            _state.Value = state;
        }
        public void AddScore(float value)
        {
            _score.Value += value;
        }
    }

}