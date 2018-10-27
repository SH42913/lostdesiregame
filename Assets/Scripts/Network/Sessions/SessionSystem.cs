using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using World;

namespace Network.Sessions
{
    [EcsInject]
    public class SessionSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<SessionComponent, LocalMarkComponent> _localSession;

        private EcsFilter<SendNetworkDataEvent> _sendEvents;
        private EcsFilter<CreateLocalSessionEvent> _createEvent;
        private EcsFilter<RemoveSessionEvent> _removeEvents;
        
        public void Run()
        {
            if (_createEvent.EntitiesCount > 0)
            {
                _createEvent.RemoveAllEntities();

                CreateLocalSession();
            }

            if (_sendEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _localSession.EntitiesCount; i++)
                {
                    _ecsWorld.SendComponentToNetwork<SessionComponent>(_localSession.Entities[i]);
                }
            }

            for (int i = 0; i < _removeEvents.EntitiesCount; i++)
            {
                int entityToRemove = _removeEvents.Components1[i].LocalEntity;
                if(!_ecsWorld.IsEntityExists(entityToRemove)) continue;

                var playerComponent = _ecsWorld.GetComponent<PlayerComponent>(entityToRemove);
                if (playerComponent != null)
                {
                    _ecsWorld.CreateEntityWith<RemovePlayerEvent>().PlayerId = playerComponent.Id;
                }
                _ecsWorld.RemoveEntity(entityToRemove);
            }
            _removeEvents.RemoveAllEntities();
        }

        private void CreateLocalSession()
        {
            if (_localSession.EntitiesCount > 0) return;
            SessionComponent session;
            LocalMarkComponent local;
            _ecsWorld.CreateEntityWith(out session, out local);
            session.Address = _networkConfig.Data.LocalAddress;
            session.Port = _networkConfig.Data.LocalPort;
        }
    }
}