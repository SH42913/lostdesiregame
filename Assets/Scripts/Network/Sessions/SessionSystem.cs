using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using UnityEngine;
using World;

namespace Network.Sessions
{
    [EcsInject]
    public class SessionSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private LocalGameConfig _localConfig;
        private EcsNetworkConfig _networkConfig;

        private EcsFilter<SessionComponent>.Exclude<LocalMarkComponent, RemoteMarkComponent> _undefinedSessions;
        private EcsFilter<SessionComponent, LocalMarkComponent> _localSession;

        private EcsFilter<RefreshNetworkDataEvent> _sendEvents;
        private EcsFilter<CreateLocalSessionEvent> _createEvent;
        private EcsFilter<RemoveSessionEvent> _removeEvents;
        
        public void Run()
        {
            for (int i = 0; i < _undefinedSessions.EntitiesCount; i++)
            {
                long remoteSessionId = _undefinedSessions.Components1[i].Id;
                int localSessionEntity = _undefinedSessions.Entities[i];

                _ecsWorld.AddComponent<RemoteMarkComponent>(localSessionEntity);
                _localConfig.SessionIdToLocalEntity.Add(remoteSessionId, localSessionEntity);
            }
            
            if (_createEvent.EntitiesCount > 0)
            {
                CreateLocalSession();
            }

            if (_sendEvents.EntitiesCount > 0)
            {
                SendLocalSession();
            }
        }

        private void CreateLocalSession()
        {
            if (_localSession.EntitiesCount > 0) return;
            
            SessionComponent session;
            LocalMarkComponent local;
            int sessionEntity = _ecsWorld.CreateEntityWith(out session, out local);
            session.Address = _networkConfig.LocalAddress;
            session.Port = _networkConfig.LocalPort;
            session.Id = _networkConfig.Random.NextInt64();
            
            _localConfig.SessionIdToLocalEntity.Add(session.Id, sessionEntity);
        }

        private void SendLocalSession()
        {
            for (int i = 0; i < _localSession.EntitiesCount; i++)
            {
                _ecsWorld.SendComponentToNetwork<SessionComponent>(_localSession.Entities[i]);
            }
        }
    }
}