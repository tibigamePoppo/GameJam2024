using UnityEngine;
using UniRx;
using DG.Tweening;

namespace Player
{
    public class CameraShake : MonoBehaviour
    {
        private PlayerStatus _status;
        [SerializeField]
        private GameObject _camera;
        void Start()
        {
            _status = GetComponent<PlayerStatus>();
            _status.OnChangeCurrentHp
                .Subscribe(_ => _camera.transform.DOShakePosition(0.5f, 0.5f));

        }
    }
}