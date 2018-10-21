using Leopotam.Ecs.Net;

namespace Ships.Flight
{
    public enum EngineDirection
    {
        FORWARD,
        BACK,
        TURN_LEFT,
        TURN_RIGHT
    }
    
    [EcsNetComponentUid(4)]
    public class SwitchEngineEvent
    {
        public EngineDirection Direction;
        public bool Enable;
    }
}