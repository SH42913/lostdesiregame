using System;
using Cleaning;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using Players;
using UnityEngine;

namespace Ships.Update
{
    [EcsInject]
    public class ShipUpdateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<PositionComponent, UnityComponent, ShipMarkComponent> _ships;
        private EcsFilter<PositionComponent, ShipMarkComponent>.Exclude<UnityComponent> _shipsWithoutTransform;

        private EcsFilter<SendNetworkDataEvent> _sendEvents;
        private EcsFilter<RemovePlayerEvent> _removePlayerEvents;
        
        public void Run()
        {
            for (int i = 0; i < _shipsWithoutTransform.EntitiesCount; i++)
            {
                int shipEntity = _shipsWithoutTransform.Entities[i];

                Transform shipTransform = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipTransform.gameObject.SetActive(true);
                shipTransform.GetComponent<EntityBehaviour>().AttachToEntity(shipEntity);
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

            for (int i = 0; i < _removePlayerEvents.EntitiesCount; i++)
            {
                RemoveShip(_removePlayerEvents.Components1[i].PlayerId);
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

        private void RemoveShip(long playerId)
        {
            for (int i = 0; i < _ships.EntitiesCount; i++)
            {
                if (_ships.Components3[i].PlayerId != playerId) continue;

                int shipEntity = _ships.Entities[i];
                bool isNew;
                _ecsWorld.EnsureComponent<RemoveMarkComponent>(shipEntity, out isNew);
                _ecsWorld.SendComponentToNetwork<RemoveMarkComponent>(shipEntity);
            }
        }
    }
}