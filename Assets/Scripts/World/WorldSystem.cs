using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace World
{
    [EcsInject]
    public class WorldSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<WorldComponent> _worlds;

        private EcsFilter<SendWorldEvent> _sendEvents;
        private EcsFilter<CreateWorldEvent> _createEvents;
        
        public void Run()
        {
            if (_createEvents.EntitiesCount > 0)
            {
                _createEvents.RemoveAllEntities();
                _ecsWorld.CreateEntityWith<WorldComponent>();
            }
            
            if(_sendEvents.EntitiesCount <= 0) return;
            _sendEvents.RemoveAllEntities();

            if (_localConfig.Data.ClientType == ClientType.CLIENT) return;
            for (int i = 0; i < _worlds.EntitiesCount; i++)
            {
                _ecsWorld.SendComponentToNetwork<WorldComponent>(_worlds.Entities[i]);
            }
            UnityEngine.Debug.Log("World mark for send");
        }
    }
}