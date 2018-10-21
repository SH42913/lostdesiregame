using Leopotam.Ecs.Net;

namespace Ships.Flight
{
    [EcsNetComponentUid(4)]
    public class SwitchEngineEvent
    {
        public long PlayerId;
        public EngineDirection Direction;
        public bool Enable;

        public static void NewToOldConverter(SwitchEngineEvent newEvent, SwitchEngineEvent oldEvent)
        {
            oldEvent.PlayerId = newEvent.PlayerId;
            oldEvent.Direction = newEvent.Direction;
            oldEvent.Enable = newEvent.Enable;
        }
    }
}