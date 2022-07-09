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
            //设置transform
            TransformAccessArray transfromArray = new TransformAccessArray(cubes);
            MyJob job = new MyJob() { position = position };
            JobHandle jobHandle = job.Schedule(transfromArray);
            //等待工作线程结束
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
            //工作线程中设置坐标
            transform.position = position[index];
        }
    }
}


