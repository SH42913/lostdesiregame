using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using World;

namespace Network.Sessions
{
    [EcsInject]
    public class SessionSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<SessionComponent, LocalSessionMarkComponent> _localSession;

        private EcsFilter<SendBaseInfo> _sendEvents;
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
                
                _ecsWorld.RemoveEntity(entityToRemove);
            }
            _removeEvents.RemoveAllEntities();
        }

        private void CreateLocalSession()
        {
            if (_localSession.EntitiesCount > 0) return;
            SessionComponent session;
            LocalSessionMarkComponent localSession;
            _ecsWorld.CreateEntityWith(out session, out localSession);
            session.Address = _networkConfig.Data.LocalAddress;
            session.Port = _networkConfig.Data.LocalPort;
        }
    }
}