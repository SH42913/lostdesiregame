using Leopotam.Ecs.Net;

namespace Ships
{
    [EcsNetComponentUid(6)]
    public class ShipComponent
    {
        public long PlayerId;
        
        public static void NewToOldConverter(ShipComponent newComponent, ShipComponent oldComponent)
        {
            oldComponent.PlayerId = newComponent.PlayerId;
        }
    }
}