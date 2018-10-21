using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using Ships;
using Ships.Spawn;

namespace Players
{
    [EcsInject]
    public class PlayerSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<SessionComponent, LocalSessionMarkComponent> _localSession;
        private EcsFilter<PlayerComponent, LocalSessionMarkComponent> _localPlayer;
        private EcsFilter<OwnedByPlayerComponent> _ownedObjects;

        private EcsFilter<SendNetworkDataEvent> _sendEvents;
        private EcsFilter<CreatePlayerEvent> _createEvents;
        
        public void Run()
        {
            for (int i = 0; i < _createEvents.EntitiesCount; i++)
            {
                CreateLocalPlayer(_createEvents.Components1[i].Nickname);
            }
            _createEvents.RemoveAllEntities();

            if (_sendEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _localPlayer.EntitiesCount; i++)
                {
                    _ecsWorld.SendComponentToNetwork<PlayerComponent>(_localPlayer.Entities[i]);
                }
            }
        }

        private void CreateLocalPlayer(string name)
        {
            if(_localPlayer.EntitiesCount > 0) return;

            for (int i = 0; i < _localSession.EntitiesCount; i++)
            {
                int localSessionEntity = _localSession.Entities[i];
                var player = _ecsWorld.AddComponent<PlayerComponent>(localSessionEntity);
                player.Name = name;
                player.Id = _networkConfig.Data.Random.NextInt64();
                _ecsWorld.SendComponentToNetwork<PlayerComponent>(localSessionEntity);
                _ecsWorld.SendEventToNetwork<SpawnShipEvent>().PlayerId = player.Id;
            }
        }
    }
}