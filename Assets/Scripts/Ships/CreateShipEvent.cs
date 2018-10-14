using Leopotam.Ecs.Net;

namespace Ships
{
    [EcsNetComponentUid(3)]
    public class CreateShipEvent
    {
        public long PlayerId;

        public static void NewToOldConverter(CreateShipEvent newEvent, CreateShipEvent oldEvent)
        {
            oldEvent.PlayerId = newEvent.PlayerId;
        }
    }
}