using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Ships
{
    [EcsIgnoreInFilter]
    [EcsNetComponentUid(-1)]
    public class DestroyedShipMarkComponent
    {
        public static void NewToOldConverter(DestroyedShipMarkComponent newComp, DestroyedShipMarkComponent oldComp)
        {
            
        }
    }
}