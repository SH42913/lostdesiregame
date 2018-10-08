using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;

namespace Players
{
    [EcsInject]
    public class PlayerSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<SessionComponent, LocalSessionMarkComponent> _localSession;
        private EcsFilter<PlayerComponent, LocalSessionMarkComponent> _localPlayer;

        private EcsFilter<SendBaseInfo> _sendEvents;
        private EcsFilter<CreatePlayerEvent> _createEvents;
        private EcsFilter<RemovePlayerEvent> _removeEvents;
        
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
                _ecsWorld.SendComponentToNetwork<PlayerComponent>(localSessionEntity);
            }
        }
    }
}