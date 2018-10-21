using Leopotam.Ecs;
using LeopotamGroup.Pooling;
using Network.Sessions;
using Players;
using Ships;

namespace Cleaning
{
    [EcsInject]
    public class CleaningSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<UnityComponent> _unityComponents;
        private EcsFilter<RemoveMarkComponent, UnityComponent, ShipMarkComponent> _shipsToRemove;
        private EcsFilter<RemoveMarkComponent> _markedToRemove;

        private EcsFilter<RemovePlayerEvent> _removePlayerEvents;
        private EcsFilter<SendNetworkDataEvent> _sendDataEvents;
    
        public void Initialize()
        {
        
        }

        public void Run()
        {
            _removePlayerEvents.RemoveAllEntities();
            _sendDataEvents.RemoveAllEntities();

            for (int i = 0; i < _shipsToRemove.EntitiesCount; i++)
            {
                UnityComponent unity = _shipsToRemove.Components2[i];
                _localConfig.Data.ShipContainer.Recycle(unity.Transform.GetComponent<IPoolObject>());
                unity.Transform = null;
            }

            for (int i = 0; i < _markedToRemove.EntitiesCount; i++)
            {
                _ecsWorld.RemoveEntity(_markedToRemove.Entities[i]);
            }
        }

        public void Destroy()
        {
            _localConfig.Data.ShipContainer = null;
        
            for (int i = 0; i < _unityComponents.EntitiesCount; i++)
            {
                _unityComponents.Components1[i].Transform = null;
            }
        }
    }
}