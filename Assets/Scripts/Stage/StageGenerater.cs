using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using System.Reflection;
using UnityEngine.UIElements;
using UniRx.Triggers;

namespace Stage
{
    public class StageGenerater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _stageObject;
        [SerializeField]
        private GameObject _wallObject;
        [SerializeField]
        private int _stageMaxCount;
        private int _stageCount;
        private GameObject _player;
        private Vector3 _instancePosition = Vector3.zero;
        private Queue<List<GameObject>> _stageInstantiateObjects = new Queue<List<GameObject>>();
       
        void Start()
        {
            for (int i = 0; i < _stageMaxCount; i++)
            {
                /*
                Instantiate(_stageObject,_instancePosition,Quaternion.identity);
                SetWal(_stageCount + 1, _stageCount * 12);
                _instancePosition.z += 12;
                _stageCount++;
                */
                InstantiateStageBlock();
            }
            _player = GameObject.FindWithTag("Player");
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (_player.transform.position.z >= (_stageCount - 5) * 12)
                    {
                        InstantiateStageBlock();
                    }
                }).AddTo(this);
        }
        private void InstantiateStageBlock()
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(Instantiate(_stageObject, _instancePosition, Quaternion.identity));
            _instancePosition.z += 12;
            var walls = SetWal((_stageCount + 1) % 9, _stageCount * 12);
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
                Vector3 instantiatePosition = new Vector3(-2 + wallPosition[i] % 3 * 2, 1 + wallPosition[i] / 3 * 2,xLength);
                list.Add(Instantiate(_wallObject, instantiatePosition, Quaternion.identity));
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