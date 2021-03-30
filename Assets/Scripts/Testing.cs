using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = UnityEngine.Random;


public class Testing : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    private void Start()
    {
        //set entity manager
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        //new extension for bounds
        AABBExtensions.ToAABB(mesh.bounds);
        
        //create architype for diffrent entities
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(MoveSpeedComponent)
        );
        
        //create array with size of entities you want
        NativeArray<Entity> entityArray = new NativeArray<Entity>(100000, Allocator.Temp);
        
        entityManager.CreateEntity(entityArchetype, entityArray);

        //work with elements of arra entities 
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            
            //set data component (component is another script set in gameObject)
            entityManager.SetComponentData(entity, new LevelComponent{ level = Random.Range(10, 20) });
            entityManager.SetComponentData(entity, new MoveSpeedComponent{moveSpeed = Random.Range(1f, 2f)});
            entityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(Random.Range(-4f, 4f),
                                    Random.Range(-4, 4f), 0)
            });
            
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
        }

        //free
        entityArray.Dispose();
    }
}
