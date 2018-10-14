using Leopotam.Ecs.Net;

namespace Ships
{
    [EcsNetComponentUid(6)]
    public class ShipMarkComponent
    {
        public long PlayerId;
        
        public static void NewToOldConverter(ShipMarkComponent newComponent, ShipMarkComponent oldComponent)
        {
            oldComponent.PlayerId = newComponent.PlayerId;
        }
    }
}