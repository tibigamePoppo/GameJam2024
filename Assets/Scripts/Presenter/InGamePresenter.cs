using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace Presenter
{
    public class InGamePresenter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        IngameManager _ingameManager;
        void Start()
        {
            _ingameManager = FindAnyObjectByType<IngameManager>();
            _ingameManager.Score
                .Subscribe(x => _scoreText.text = $"SCORE : {x}").AddTo(this);
        }
    }
}