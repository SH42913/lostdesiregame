using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Players
{
    [EcsInject]
    public class PlayerCleanSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<CreatePlayerEvent> _createEvents;
        
        public void Run()
        {
            _createEvents.RemoveAllEntities();
        }
    }
}