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
            var playerMove = GetComponent<PlayerMove>();
            this.UpdateAsObservable()
                .Where(_ => state.GetPlayerState != StateType.Idle)
                .Subscribe(_ => Manager.AddScore(Time.deltaTime * playerMove.PlayerMoveSpeed)).AddTo(this);
        }
    }
}