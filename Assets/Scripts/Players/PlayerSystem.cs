using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network;
using Network.Sessions;
using Ships;
using Ships.Spawn;

namespace Players
{
    [EcsInject]
    public class PlayerSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<SessionComponent, LocalMarkComponent> _localSession;
        private EcsFilter<PlayerComponent, LocalMarkComponent> _localPlayer;

        private EcsFilter<RefreshNetworkDataEvent> _sendEvents;
        private EcsFilter<CreatePlayerEvent> _createEvents;
        
        public void Run()
        {
            for (int i = 0; i < _createEvents.EntitiesCount; i++)
            {
                CreateLocalPlayer(_createEvents.Components1[i].Nickname);
            }

            if (_sendEvents.EntitiesCount > 0)
            {
                SendPlayer();
            }
        }

        private void CreateLocalPlayer(string name)
        {
            if(_localPlayer.EntitiesCount > 0) return;

            for (int i = 0; i < _localSession.EntitiesCount; i++)
            {
                int localSessionEntity = _localSession.Entities[i];
                long localSessionId = _localSession.Components1[i].Id;
                
                var player = _ecsWorld.AddComponent<PlayerComponent>(localSessionEntity);
                player.Name = name;
                
                _ecsWorld.SendComponentToNetwork<PlayerComponent>(localSessionEntity);
                _ecsWorld.SendEventToNetwork<SpawnShipEvent>().SessionId = localSessionId;

                _localConfig.Data.LocalSessionId = localSessionId;
            }
        }

        private void SendPlayer()
        {
            for (int i = 0; i < _localPlayer.EntitiesCount; i++)
            {
                _ecsWorld.SendComponentToNetwork<PlayerComponent>(_localPlayer.Entities[i]);
            }
        }
    }
}