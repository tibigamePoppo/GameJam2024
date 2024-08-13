using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Manager;

namespace Player
{
    public class PlayerScoreCounter : MonoBehaviour
    {
        void Start()
        {
            var Manager = FindObjectOfType<IngameManager>();
            var state = GetComponent<PlayerState>();
            var playerStatus = GetComponent<PlayerStatus>();
            this.UpdateAsObservable()
                .Where(_ => state.GetPlayerState != StateType.Idle && state.GetPlayerState != StateType.Dead)
                .Subscribe(_ => Manager.AddScore(Time.deltaTime * playerStatus.GetMoveSpeed)).AddTo(this);
        }
    }
}