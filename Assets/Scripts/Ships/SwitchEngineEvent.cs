using Leopotam.Ecs.Net;

namespace Ships
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