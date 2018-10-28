using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using UnityEngine;

namespace Network
{
    [EcsInject]
    public class DisconnectedClientSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<SessionComponent> _sessions;
        
        private EcsFilter<ClientDisconnectedEvent> _disconnectedEvents;
        
        public void Run()
        {
            for (int i = 0; i < _disconnectedEvents.EntitiesCount; i++)
            {
                ClientInfo disconnected = _disconnectedEvents.Components1[i].DisconnectedClient;

                RemoveClientInfo(disconnected.Address, disconnected.Port);
            }
        }

        private void RemoveClientInfo(string address, short port)
        {
            for (int i = 0; i < _sessions.EntitiesCount; i++)
            {
                SessionComponent session = _sessions.Components1[i];
                if(session.Address != address || session.Port != port) continue;

                _ecsWorld.CreateEntityWith<RemoveSessionEvent>().SessionId = session.Id;
                return;
            }
        }
    }
}