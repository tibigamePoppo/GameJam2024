using Singleton.Effect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Audio
{
    public class SEManager : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> seList = new List<AudioClip>();
        public static SEManager Instance;
        AudioSource _audioSource;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        public void ShotSE(SEType type)
        {
            if(type == SEType.Attack)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        type = SEType.Attack1;
                        break;
                    case 1:
                        type = SEType.Attack2;
                        break;
                    case 2:
                        type = SEType.Attack3;
                        break;
                    default:
                        break;
                }
            }
            else if (type == SEType.Damage)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        type = SEType.Damage1;
                        break;
                    case 1:
                        type = SEType.Damage2;
                        break;
                    case 2:
                        type = SEType.Damage3;
                        break;
                    default:
                        break;
                }
            }
            else if (type == SEType.Dead)
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        type = SEType.Dead1;
                        break;
                    case 1:
                        type = SEType.Dead2;
                        break;
                    default:
                        break;
                }
            }
            var se = seList.First(e => e.name.Equals(type.ToString()));
            _audioSource.PlayOneShot(se);
        }
    }
}