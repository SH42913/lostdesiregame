using Leopotam.Ecs.Net;

namespace Ships.Spawn
{
    [EcsNetComponentUid(3)]
    public class SpawnShipEvent
    {
        public long SessionId;

        public static void NewToOldConverter(SpawnShipEvent newEvent, SpawnShipEvent oldEvent)
        {
            oldEvent.SessionId = newEvent.SessionId;
        }
    }
}