using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Connections
{
    [EcsInject]
    public class DisconnectedClientSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilter<ClientDisconnectedEvent> _disconnectedEvents;
        private object _locker = new object();

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            for (int i = 0; i < _disconnectedEvents.EntitiesCount; i++)
            {
                List<ClientInfo> connected = _localConfig.Data.ConnectedClients;
                ClientInfo disconnected = _disconnectedEvents.Components1[i].DisconnectedClient;

                lock (_locker)
                {
                    foreach (ClientInfo info in connected)
                    {
                        if (info.Address == disconnected.Address && info.Port == disconnected.Port)
                        {
                            connected.Remove(info);
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            _localConfig.Data.ConnectedClients = null;
        }
    }
}