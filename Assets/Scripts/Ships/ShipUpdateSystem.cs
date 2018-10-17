using System;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using UnityEngine;

namespace Ships
{
    [EcsInject]
    public class ShipUpdateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<PositionComponent, UnityComponent, ShipMarkComponent> _ships;
        private EcsFilter<PositionComponent, ShipMarkComponent>.Exclude<UnityComponent> _shipsWithoutTransform;

        private EcsFilter<SendBaseInfo> _sendEvents;
        
        public void Run()
        {
            for (int i = 0; i < _shipsWithoutTransform.EntitiesCount; i++)
            {
                int shipEntity = _shipsWithoutTransform.Entities[i];

                Transform shipTransform = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipTransform.gameObject.SetActive(true);
                
                _ecsWorld.AddComponent<UnityComponent>(shipEntity).Transform = shipTransform;
            }

            switch (_localConfig.Data.ClientType)
            {
                case ClientType.SERVER:
                    UpdatePositionsOnServer();
                    break;
                case ClientType.CLIENT:
                    UpdatePositionsOnClient();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdatePositionsOnClient()
        {
            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                PositionComponent position = _ships.Components1[i];
                Transform shipTransform = _ships.Components2[i].Transform;

                shipTransform.position = new Vector3(position.PositionX, position.PositionY);
                shipTransform.rotation = Quaternion.Euler(0, 0, position.Rotation);
            }
        }

        private void UpdatePositionsOnServer()
        {
            if (_sendEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _ships.EntitiesCount; i++)
                {
                    int shipEntity = _ships.Entities[i];
                    _ecsWorld.SendComponentToNetwork<ShipMarkComponent>(shipEntity);
                    _ecsWorld.SendComponentToNetwork<PositionComponent>(shipEntity);
                }
            }
            
            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                Transform shipTransform = _ships.Components2[i].Transform;
                PositionComponent position = _ships.Components1[i];

                position.Rotation = shipTransform.rotation.eulerAngles.z;
                position.PositionX = shipTransform.position.x;
                position.PositionY = shipTransform.position.y;

                _ecsWorld.SendComponentToNetwork<PositionComponent>(_ships.Entities[i]);
            }
        }
    }
}