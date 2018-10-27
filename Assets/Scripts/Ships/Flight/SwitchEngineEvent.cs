using Leopotam.Ecs.Net;

namespace Ships.Flight
{
    [EcsNetComponentUid(4)]
    public class SwitchEngineEvent
    {
        public long SessionId;
        public EngineDirection Direction;
        public bool Enable;

        public static void NewToOldConverter(SwitchEngineEvent newEvent, SwitchEngineEvent oldEvent)
        {
            oldEvent.SessionId = newEvent.SessionId;
            oldEvent.Direction = newEvent.Direction;
            oldEvent.Enable = newEvent.Enable;
        }
    }
}