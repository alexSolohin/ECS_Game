using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref MoveSpeedComponent speedComponent) =>
        {
            translation.Value.y += speedComponent.moveSpeed * Time.DeltaTime;
            if (translation.Value.y >= 5f)
            {
                speedComponent.moveSpeed = -Mathf.Abs(speedComponent.moveSpeed);
            }
            if (translation.Value.y < -5f)
            {
                speedComponent.moveSpeed = +Mathf.Abs(speedComponent.moveSpeed);
            }
        });
    }
}
