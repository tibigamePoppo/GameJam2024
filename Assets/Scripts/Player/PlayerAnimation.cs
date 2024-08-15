using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Audio;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        Animator _animator;
        PlayerState _playerState;
        [SerializeField]
        GameObject _hammer;
        Vector3 standardPosition = new Vector3(-0.45f, 0.1f, 0.1f);
        Vector3 standardRotation = new Vector3(0, 20, 60f);
        Vector3 downPosition = new Vector3(-0.2f,0.2f,0.2f);
        Vector3 downRotation = new Vector3(55, -15, -13.5f);
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dash)
                .First()
                .Subscribe(_ =>
                {
                    hammerRotation(true);
                    _animator.SetBool("Start",true);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dash)
                .Skip(1)
                .Subscribe(_ =>
                {
                    hammerRotation(true);
                    _animator.SetBool("SecondJump", false);
                    _animator.SetBool("DownSword", false);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Jump)
                .Subscribe(_ =>
                {
                    _animator.SetTrigger("Jump");
                    SEManager.Instance.ShotSE(SEType.Jump);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.SecondJump)
                .Subscribe(_ =>
                {
                    _animator.SetBool("SecondJump",true);
                    SEManager.Instance.ShotSE(SEType.SecondJump);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Attack)
                .Subscribe(_ =>
                {
                    _animator.SetTrigger("Sword");
                    SEManager.Instance.ShotSE(SEType.Attack);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.DownAttack)
                .Subscribe(_ =>
                {
                    hammerRotation(false);
                    _animator.SetBool("DownSword", true);
                    SEManager.Instance.ShotSE(SEType.Attack);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.WideAttack)
                .Subscribe(_ =>
                {
                    _animator.SetTrigger("WideSword");
                    SEManager.Instance.ShotSE(SEType.Attack);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dead)
                .Subscribe(_ =>
                {
                    _animator.SetTrigger("Dead");
                    SEManager.Instance.ShotSE(SEType.Dead);
                }).AddTo(this);
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _animator.SetFloat("Height",transform.position.y);
                }).AddTo(this);
        }

        private void hammerRotation(bool standard)
        {
            if (standard) {
                _hammer.transform.localPosition = standardPosition;
                _hammer.transform.localRotation = Quaternion.Euler(standardRotation);
            }
            else {
                _hammer.transform.localPosition = downPosition;
                _hammer.transform.localRotation = Quaternion.Euler(downRotation);
            }
        }
    }
}
