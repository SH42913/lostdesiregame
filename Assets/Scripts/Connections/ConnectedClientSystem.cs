using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Connections
{
    [EcsInject]
    public class ConnectedClientSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilter<ClientConnectedEvent> _connectedEvents;
        
        public void Run()
        {
            for (int i = 0; i < _connectedEvents.EntitiesCount; i++)
            {
                _localConfig.Data.ConnectedClients.Add(_connectedEvents.Components1[i].ConnectedClient);
            }
        }
    }
}