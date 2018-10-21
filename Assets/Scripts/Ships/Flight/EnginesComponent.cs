using System;
using Leopotam.Ecs.Net;

namespace Ships.Flight
{
    [Flags]
    public enum EngineDirection
    {
        FORWARD = 1,
        BACKWARD = 2,
        TURN_LEFT = 4,
        TURN_RIGHT = 8,
        STRAFE_LEFT = 16,
        STRAFE_RIGHT = 32
    }
    
    [EcsNetComponentUid(7)]
    public class EnginesComponent
    {
        public EngineDirection EnabledEngines;
        
        public float ForwardForce;
        public float BackwardForce;
        public float StrafeForce;
        public float TurnTorque;

        public static void NewToOldConverter(EnginesComponent newComp, EnginesComponent oldComp)
        {
            oldComp.EnabledEngines = newComp.EnabledEngines;
            oldComp.ForwardForce = newComp.ForwardForce;
            oldComp.BackwardForce = newComp.BackwardForce;
            oldComp.StrafeForce = newComp.StrafeForce;
            oldComp.TurnTorque = newComp.TurnTorque;
        }
    }
}