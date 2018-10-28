using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace World
{
    [EcsNetComponentUid(0)]
    [EcsIgnoreInFilter]
    public class WorldComponent
    {
        public static void NewToOldConverter(WorldComponent newWorld, WorldComponent oldWorld)
        {
            
        }
    }
}