using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

using Unity.Mathematics;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class JobTesting : MonoBehaviour
{

    [SerializeField] private bool useJobs;

    [SerializeField] private Transform pfZombie;
    private List<Zombie> zombieList;
    
    
    public class Zombie
    {
        public Transform transform;
        public float moveY;
    }

    public void Start()
    {
        zombieList = new List<Zombie>();
        for (int i = 0; i < 1000; i++)
        {
            Transform zombieTransform = Instantiate(pfZombie, new Vector3(Random.Range(-8, 8), Random.Range(-5, 5), 0), Quaternion.identity);
            zombieList.Add(new Zombie
            {
                transform = zombieTransform,
                moveY = Random.Range(1f, 2f)
            });
        }
    }

    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            // NativeArray<float3> positionArray = new NativeArray<float3>(zombieList.Count, Allocator.TempJob);
            NativeArray<float> moveArray = new NativeArray<float>(zombieList.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(zombieList.Count);
            
            for (int i = 0; i < zombieList.Count; i++)
            {
                // positionArray[i] = zombieList[i].transform.position;
                moveArray[i] = zombieList[i].moveY;
                transformAccessArray.Add(zombieList[i].transform);
            }
            /*
            ReallyToughParallelJob reallyToughParallelJob = new ReallyToughParallelJob
            {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveArray,
            };
            */
            ReallyToughParalleleJobTransform reallyToughParalleleJobTransform = new ReallyToughParalleleJobTransform
            {
                deltaTime = Time.deltaTime,
                moveYArray = moveArray,
            };
            //for transform job
            JobHandle jobHandle = reallyToughParalleleJobTransform.Schedule(transformAccessArray);
            jobHandle.Complete();
            
            // for parallel job
            // JobHandle jobHandle = reallyToughParallelJob.Schedule(zombieList.Count, 100);
            // jobHandle.Complete();

            for (int i = 0; i < zombieList.Count; i++)
            {
                // zombieList[i].transform.position = positionArray[i];
                zombieList[i].moveY = moveArray[i];
            }

            // positionArray.Dispose();
            moveArray.Dispose();
            transformAccessArray.Dispose();
        }
        else
        {
            foreach (var zombie in zombieList)
            {
                zombie.transform.position += new Vector3(0, zombie.moveY * Time.deltaTime);
                if (zombie.transform.position.y > 5f)
                {
                    zombie.moveY = -math.abs(zombie.moveY);
                }
                if (zombie.transform.position.y < -5f)
                {
                    zombie.moveY = +math.abs(zombie.moveY);
                }
                float value = 0f;
                for (int i = 0; i < 1000; i++)
                {
                    value = math.exp10(math.sqrt(value));
                }
            }
        }
        
        /*
        if (useJobs)
        {
            NativeList<JobHandle> jobHandlesList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = ReallyToughTaskJob();
                jobHandlesList.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandlesList);
            jobHandlesList.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }
        */
        
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000) + "ms");
    }

    void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
}

[BurstCompile]
public struct ReallyToughJob: IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor
{
    public NativeArray<float3> positionArray;
    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;
    
    public void Execute(int index)
    {
        positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0);
        if (positionArray[index].y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (positionArray[index].y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughParalleleJobTransform : IJobParallelForTransform
{
    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;
    
    public void Execute(int index, TransformAccess transform)
    {
        transform.position += new Vector3(0, moveYArray[index] * deltaTime, 0);
        if (transform.position.y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (transform.position.y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
    
