using Manager;
using TMPro;
using UniRx;
using System.Linq;
using UnityEngine;
using Player;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;
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
        private TextMeshProUGUI _gameOver;
        [SerializeField]
        private TextMeshProUGUI _retry;
        [SerializeField]
        private GameObject _descriptionPanel;
        [SerializeField]
        private Image _descriptionButton;
        [SerializeField]
        private GameObject[] Hps;
        IngameManager _ingameManager;
        PlayerStatus _playerStatus;
        void Start()
        {
            _tapToStart.DOFade(0, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _descriptionButton.DOFade(0, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _ingameManager = FindAnyObjectByType<IngameManager>();
            _playerStatus = FindAnyObjectByType<PlayerStatus>();
            _ingameManager.OnChangeIngameState
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case IngameType.Ready:
                            _descriptionPanel.SetActive(false);
                            _tapToStart.DOFade(1, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).From(0);
                            DOTween.Sequence()
                                .AppendCallback(() =>
                                {
                                    _tapToStart.text = "3";
                                })
                                .AppendInterval(1)
                                .AppendCallback(() =>
                                {
                                    _tapToStart.text = "2";
                                })
                                .AppendInterval(1)
                                .AppendCallback(() =>
                                {
                                    _tapToStart.text = "1";
                                })
                                .AppendInterval(1)
                                .AppendCallback(() =>
                                {
                                    _ingameManager.ChangeState(IngameType.Ingame);
                                });
                            _descriptionButton.gameObject.SetActive(false);
                            break;
                        case IngameType.Ingame:
                            _tapToStart.gameObject.SetActive(false);
                            break;
                        case IngameType.GameOver:
                            _gameOver.gameObject.SetActive(true);
                            _gameOver.text = $"GAME OVER\nSOCRE : {_ingameManager.GetScore.ToString("f1")}";
                            DOTween.Sequence()
                                .AppendCallback(() =>
                                {
                                    _retry.gameObject.SetActive(true);
                                    Retry().Forget();
                                })
                                .Append(_retry.DOFade(0, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo))
                                .SetDelay(1);
                            break;
                    }
                }).AddTo(this);

            _ingameManager.Score
                 .Subscribe(x => _scoreText.text = $"SCORE : {x.ToString("f1")}").AddTo(this);
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
                        if (j < hp)
                        {
                            targetObject.SetActive(true);
                            if (j == hp - 1)
                            {
                                targetObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBounce);
                            }
                        }
                        else
                        {
                            targetObject.transform.DOShakePosition(0.5f, strength: 5).OnComplete(() => targetObject.SetActive(false));
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