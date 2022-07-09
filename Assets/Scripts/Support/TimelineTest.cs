using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class TimelineTest : MonoBehaviour
{
    public Transform[] cubes;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NativeArray<Vector3> position = new NativeArray<Vector3>(cubes.Length, Allocator.Persistent);
            for(int i = 0; i < position.Length; i++)
            {
                position[i] = Vector3.one * i;
            }
            //����transform
            TransformAccessArray transfromArray = new TransformAccessArray(cubes);
            MyJob job = new MyJob() { position = position };
            JobHandle jobHandle = job.Schedule(transfromArray);
            //�ȴ������߳̽���
            jobHandle.Complete();
            transfromArray.Dispose();
            position.Dispose();
        }
    }

    struct MyJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> position;
        public void Execute(int index, TransformAccess transform)
        {
            //�����߳�����������
            transform.position = position[index];
        }
    }
}


