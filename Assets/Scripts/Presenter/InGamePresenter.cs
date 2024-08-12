using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Presenter
{
    public class InGamePresenter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _tapToStart;
        [SerializeField]
        private GameObject[] Hps;
        IngameManager _ingameManager;
        PlayerStatus _playerStatus;
        void Start()
        {
            _ingameManager = FindAnyObjectByType<IngameManager>();
            _playerStatus = FindAnyObjectByType<PlayerStatus>();
            _ingameManager.Score
                .Subscribe(x => _scoreText.text = $"SCORE : {x.ToString("f1")}").AddTo(this);
            _ingameManager.OnChangeIngameState
                .Where(state => state == IngameType.Ingame)
                .Subscribe(x => _tapToStart.gameObject.SetActive(false)).AddTo(this);
            _playerStatus.OnChangeCurrentHp
                .Where(hp => hp >= 0)
                .Subscribe(hp =>
                {
                    for (int i = 0; i < Hps.Length; i++)
                    {
                        if(i < hp)
                        {
                            Hps[i].SetActive(true);
                        }
                        else
                        {
                            Hps[i].SetActive(false);
                        }
                    }
                }).AddTo(this);

        }
    }
}