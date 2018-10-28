using Leopotam.Ecs.Net;
using UnityEngine;

[EcsNetComponentUid(5)]
public class PositionComponent
{
    public float PositionX;
    public float PositionY;
    public float Rotation;

    public static void NewToOldConverter(PositionComponent newPosition, PositionComponent oldPosition)
    {
        oldPosition.PositionX = newPosition.PositionX;
        oldPosition.PositionY = newPosition.PositionY;
        oldPosition.Rotation = newPosition.Rotation;
    }
}