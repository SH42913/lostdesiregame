using Leopotam.Ecs.Net;

namespace Ships
{
    [EcsNetComponentUid(6)]
    public class ShipComponent
    {
        public long SessionId;
        
        public static void NewToOldConverter(ShipComponent newComponent, ShipComponent oldComponent)
        {
            oldComponent.SessionId = newComponent.SessionId;
        }
    }
}