using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Network.Sessions
{
    [EcsInject]
    public class SessionCleanSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private LocalGameConfig _localConfig;

        private EcsFilter<CreateLocalSessionEvent> _createSessionEvents;
        private EcsFilter<RemoveSessionEvent> _removeSessionEvents;
        private EcsFilter<RefreshNetworkDataEvent> _sendDataEvents;
        
        public void Run()
        {
            _createSessionEvents.RemoveAllEntities();
            _sendDataEvents.RemoveAllEntities();

            for (int i = 0; i < _removeSessionEvents.EntitiesCount; i++)
            {
                long sessionId = _removeSessionEvents.Components1[i].SessionId;
                int sessionLocalEntity = _localConfig.SessionIdToLocalEntity[sessionId];
                
                _ecsWorld.RemoveEntity(sessionLocalEntity);
                _localConfig.SessionIdToLocalEntity.Remove(sessionId);
            }
            _removeSessionEvents.RemoveAllEntities();
        }
    }
}