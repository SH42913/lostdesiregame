using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using World;

namespace Network
{
    [EcsInject]
    public class ConnectedClientSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        
        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<SessionComponent, LocalMarkComponent> _localSession;
        
        private EcsFilter<ClientConnectedEvent> _connectedEvents;
        
        public void Run()
        {
            if (_connectedEvents.EntitiesCount > 0)
            {
                UnityEngine.Debug.Log("Connected");
                _ecsWorld.CreateEntityWith<SendNetworkDataEvent>();
            }
        }
    }
}