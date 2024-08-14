using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using System.Reflection;
using UnityEngine.UIElements;
using UniRx.Triggers;
using Config;

namespace Stage
{
    public class StageGenerater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _stageObject;
        [SerializeField]
        private GameObject _wallObject;
        [SerializeField]
        private GameObject _ballObject;
        [SerializeField]
        private int _stageMaxCount;
        private int _stageCount;
        private GameObject _player;
        private Vector3 _instancePosition = new Vector3(0,-0.25f,0);
        private Queue<List<GameObject>> _stageInstantiateObjects = new Queue<List<GameObject>>();
        private const int STAGELENGTH = 20;
       
        void Start()
        {
            for (int i = 0; i < _stageMaxCount; i++)
            {
                InstantiateStageBlock();
            }
            _player = GameObject.FindWithTag("Player");
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_player.transform.position.z >= (_stageCount - 5) * STAGELENGTH)
                    {
                        InstantiateStageBlock();
                    }
                }).AddTo(this);
        }
        private void InstantiateStageBlock()
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(Instantiate(_stageObject, _instancePosition, Quaternion.identity));
            _instancePosition.z += STAGELENGTH;
            int randomValue = UnityEngine.Random.Range(0, 2);
            int wallCount = (_stageCount + 1) % 9 > 7 || (_stageCount + 1) % 9 == 0 ? (_stageCount + 1) % 9 : (_stageCount + 1 + randomValue) % 9;
            if (_stageCount == 0) wallCount = 0;//Å‰‚Í•Ç‚ªâ‘Î‚É—N‚©‚È‚¢
            var walls = SetWal(wallCount, _stageCount * STAGELENGTH);
            list = list.Concat(walls).ToList();
            _stageInstantiateObjects.Enqueue(list);
            _stageCount++;
            if(_stageInstantiateObjects.Count > 6)
            {
                List<GameObject> DestoryObjects = _stageInstantiateObjects.Dequeue();
                foreach (GameObject DestoryObject in DestoryObjects)
                {
                    Destroy(DestoryObject);
                }
            }

        }
        private List<GameObject> SetWal(int index,int xLength)
        {
            List<GameObject> list = new List<GameObject>();
            var wallPosition = RandomInts(index);
            for (int i = 0; i < index; i++)
            {
                Vector3 instantiatePosition = new Vector3(-2 + wallPosition[i] % 3 * 2, 1 + wallPosition[i] / 3 * 2, xLength + 5);
                bool SpawnBall = UnityEngine.Random.Range(0, 100) < ConfigParameter.BallDropPersent;
                if (SpawnBall)
                {
                    Instantiate(_ballObject, instantiatePosition, Quaternion.identity);
                }
                else
                {
                    list.Add(Instantiate(_wallObject, instantiatePosition, Quaternion.identity));
                }
            }
            return list;
        }
        private int[] RandomInts(int size)
        {
            int[] ints = new int[size];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = -1;
            }
            for (int i = 0;i < ints.Length;i++)
            {
                ints[i] = RandomValue(0, 9, ints);
            }
            return ints;
        }

        private int RandomValue(int min, int max, int[] withoutInts)
        {
            int r = UnityEngine.Random.Range(min, max);
            return withoutInts.Any(x => x == r) ? RandomValue(min,max,withoutInts) : r;
        }
    }
}