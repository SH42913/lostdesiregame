using System;
using ControlledCamera;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network;
using Network.Sessions;
using UnityEngine;
using UnityIntegration;

namespace Ships
{
    [EcsInject]
    public class ShipUpdateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<PositionComponent, UnityComponent, ShipComponent>.Exclude<DestroyedShipMarkComponent> _ships;
        private EcsFilter<PositionComponent, ShipComponent>.Exclude<UnityComponent, DestroyedShipMarkComponent> _shipsWithoutTransform;
        private EcsFilter<ShipComponent>.Exclude<LocalMarkComponent, RemoteMarkComponent> _newShips;

        private EcsFilter<RefreshNetworkDataEvent> _sendEvents;
        private EcsFilter<RemoveSessionEvent> _removeSessionEvents;
        
        public void Run()
        {
            for (int i = 0; i < _newShips.EntitiesCount; i++)
            {
                if (_newShips.Components1[i].SessionId == _localConfig.Data.LocalSessionId)
                {
                    _ecsWorld.AddComponent<LocalMarkComponent>(_newShips.Entities[i]);
                    _ecsWorld.AddComponent<CameraFollowTargetComponent>(_newShips.Entities[i]);
                }
                else
                {
                    _ecsWorld.AddComponent<RemoteMarkComponent>(_newShips.Entities[i]);
                }
            }
            
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

            for (int i = 0; i < _removeSessionEvents.EntitiesCount; i++)
            {
                long sessionId = _removeSessionEvents.Components1[i].SessionId;
                if(!_localConfig.Data.SessionIdToLocalEntity.ContainsKey(sessionId)) continue;
                
                int sessionEntity = _localConfig.Data.SessionIdToLocalEntity[sessionId];
                
                var assignedShip = _ecsWorld.GetComponent<AssignedShipComponent>(sessionEntity);
                if(assignedShip == null) continue;
                
                int shipEntity = assignedShip.LocalShipEntity;
                if(!_ecsWorld.IsEntityExists(shipEntity)) continue;
                
                _ecsWorld.AddComponent<DestroyedShipMarkComponent>(shipEntity);
                _ecsWorld.SendComponentToNetwork<DestroyedShipMarkComponent>(shipEntity);
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
                    _ecsWorld.SendComponentToNetwork<ShipComponent>(shipEntity);
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