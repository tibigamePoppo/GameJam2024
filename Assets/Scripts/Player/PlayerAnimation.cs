using System.Collections;
using System.Collections.Generic;
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
        void Start()
        {
            _playerState = GetComponent<PlayerState>();
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dash)
                .First()
                .Subscribe(_ =>
                {
                    _animator.SetBool("Start",true);
                }).AddTo(this);
            _playerState.OnChangePlayerState
                .Where(x => x == StateType.Dash)
                .Skip(1)
                .Subscribe(_ =>
                {
                    _animator.SetBool("SecondJump", false);
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
    }
}
