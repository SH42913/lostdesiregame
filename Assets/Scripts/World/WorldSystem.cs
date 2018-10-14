using Dialogs.CreatePlayerDialog;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;

namespace World
{
    [EcsInject]
    public class WorldSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<WorldComponent> _worlds;

        private EcsFilter<SendBaseInfo> _sendEvents;
        private EcsFilter<CreateWorldEvent> _createEvents;

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            if (_createEvents.EntitiesCount > 0)
            {
                _createEvents.RemoveAllEntities();
                _ecsWorld.CreateEntityWith<WorldComponent>();
                _ecsWorld.CreateEntityWith<CreateLocalSessionEvent>();
                _ecsWorld.CreateEntityWith<ShowCreatePlayerDialogEvent>();
            }
            
            if(_sendEvents.EntitiesCount <= 0) return;
            _sendEvents.RemoveAllEntities();

            if (_localConfig.Data.ClientType == ClientType.CLIENT) return;
            for (int i = 0; i < _worlds.EntitiesCount; i++)
            {
                _ecsWorld.SendComponentToNetwork<WorldComponent>(_worlds.Entities[i]);
            }
        }

        public void Destroy()
        {
            _localConfig.Data.ShipContainer = null;
        }
    }
}