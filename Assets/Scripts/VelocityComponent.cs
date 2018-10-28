using Leopotam.Ecs.Net;

[EcsNetComponentUid(9)]
public class VelocityComponent
{
    public float VelocityX;
    public float VelocityY;
    public float AngularVelocity;

    public static void NewToOldConverter(VelocityComponent newComp, VelocityComponent oldComp)
    {
        oldComp.VelocityX = newComp.VelocityX;
        oldComp.VelocityY = newComp.VelocityY;
        oldComp.AngularVelocity = newComp.AngularVelocity;
    }
}