using Manager;
using TMPro;
using UniRx;
using System.Linq;
using UnityEngine;
using Player;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Presenter
{
    public class InGamePresenter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _tapToStart;
        [SerializeField]
        private TextMeshProUGUI _gameOver;
        [SerializeField]
        private TextMeshProUGUI _retry;
        [SerializeField]
        private GameObject[] Hps;
        IngameManager _ingameManager;
        PlayerStatus _playerStatus;
        void Start()
        {
            _tapToStart.DOFade(0, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _ingameManager = FindAnyObjectByType<IngameManager>();
            _playerStatus = FindAnyObjectByType<PlayerStatus>();
            _ingameManager.Score
                .Subscribe(x => _scoreText.text = $"SCORE : {x.ToString("f1")}").AddTo(this);
            _ingameManager.OnChangeIngameState
                .Where(state => state == IngameType.Ingame)
                .Subscribe(x => _tapToStart.gameObject.SetActive(false)).AddTo(this);
            _ingameManager.OnChangeIngameState
                .Where(state => state == IngameType.GameOver)
                .Subscribe(x =>
                {
                    _gameOver.gameObject.SetActive(true);
                    _gameOver.text = $"GAME OVER\nSOCRE : {_ingameManager.GetScore.ToString("f1")}";
                }).AddTo(this);
            _ingameManager.OnChangeIngameState
                .Where(state => state == IngameType.GameOver)
                .Delay(TimeSpan.FromSeconds(1f))
                .Subscribe(x =>
                {
                    _gameOver.DOFade(0, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    _retry.gameObject.SetActive(true);
                    Retry().Forget();
                }).AddTo(this);

            //HPŠÖŒW
            int _currentHp = _playerStatus.GetCurrentHp;
            for (int i = 0; i < Hps.Length; i++)
            {
                if (i < _currentHp)
                {
                    Hps[i].SetActive(true);
                }
                else
                {
                    Hps[i].SetActive(false);
                }
            }
            _playerStatus.OnChangeCurrentHp
                .Where(hp => hp >= 0)
                .Skip(1)
                .Subscribe(hp =>
                {
                    for (int j = 0; j < Hps.Length; j++)
                    {
                        var targetObject = Hps[j];
                        if(j < hp)
                        {
                            targetObject.SetActive(true);
                            if(j == hp - 1)
                            {
                                targetObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBounce);
                            }
                        }
                        else
                        {
                            targetObject.transform.DOShakePosition(0.5f,strength:5).OnComplete(() => targetObject.SetActive(false));
                        }
                    }
                }).AddTo(this);

        }
        public async UniTask Retry()
        {
            await UniTask.WaitUntil(() => Input.anyKey);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}