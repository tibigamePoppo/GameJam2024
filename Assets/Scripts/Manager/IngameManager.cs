using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using unityroom.Api;
namespace Manager
{
    public enum IngameType
    {
        OutGame,
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
            OnChangeIngameState
                .Where(x => x == IngameType.GameOver)
                .First()
                .Subscribe(_ =>
                {
                    UnityroomApiClient.Instance.SendScore(1, _score.Value, ScoreboardWriteMode.Always);
                }).AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyUp(KeyCode.Space) && _state.Value == IngameType.OutGame)
                .First()
                .Subscribe(_ => ChangeState(IngameType.Ready)).AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => Input.anyKeyDown && _state.Value == IngameType.Ready)
                .First()
                .Subscribe(_ => ChangeState(IngameType.Ingame)).AddTo(this);
            ChangeState(IngameType.OutGame);
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