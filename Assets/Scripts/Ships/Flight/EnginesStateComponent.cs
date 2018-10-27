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
    public class EnginesStateComponent
    {
        public EngineDirection EnabledEngines;

        public static void NewToOldConverter(EnginesStateComponent newComp, EnginesStateComponent oldComp)
        {
            oldComp.EnabledEngines = newComp.EnabledEngines;
        }
    }
}