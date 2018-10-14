using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using UnityEngine;

namespace Ships
{
    [EcsInject]
    public class ShipUpdateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<PositionComponent, TransformComponent, ShipMarkComponent> _ships;
        private EcsFilter<PositionComponent, ShipMarkComponent>.Exclude<TransformComponent> _shipsWithoutTransform;

        private EcsFilter<SendBaseInfo> _sendEvents;
        
        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            for (int i = 0; i < _shipsWithoutTransform.EntitiesCount; i++)
            {
                int shipEntity = _shipsWithoutTransform.Entities[i];

                Transform shipTransform = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipTransform.gameObject.SetActive(true);
                
                _ecsWorld.AddComponent<TransformComponent>(shipEntity).Transform = shipTransform;
            }

            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                PositionComponent position = _ships.Components1[i];
                Transform shipTransform = _ships.Components2[i].Transform;

                shipTransform.position = new Vector3(position.PositionX, position.PositionY);
                shipTransform.rotation = Quaternion.Euler(0, 0, position.Rotation);
            }
            
            if(_sendEvents.EntitiesCount <= 0) return;

            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                int shipEntity = _ships.Entities[i];
                _ecsWorld.SendComponentToNetwork<ShipMarkComponent>(shipEntity);
                _ecsWorld.SendComponentToNetwork<PositionComponent>(shipEntity);
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                _ships.Components2[i].Transform = null;
            }
        }
    }
}