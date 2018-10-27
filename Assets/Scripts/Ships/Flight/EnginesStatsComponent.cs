using Leopotam.Ecs.Net;

namespace Ships.Flight
{
    [EcsNetComponentUid(8)]
    public class EnginesStatsComponent
    {
        public float ForwardForce;
        public float BackwardForce;
        public float StrafeForce;
        public float TurnTorque;

        public static void NewToOldConverter(EnginesStatsComponent newComp, EnginesStatsComponent oldComp)
        {
            oldComp.ForwardForce = newComp.ForwardForce;
            oldComp.BackwardForce = newComp.BackwardForce;
            oldComp.StrafeForce = newComp.StrafeForce;
            oldComp.TurnTorque = newComp.TurnTorque;
        }
    }
}