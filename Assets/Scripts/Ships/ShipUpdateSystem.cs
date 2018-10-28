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

        private LocalGameConfig _localConfig;

        private EcsFilter<PositionComponent, UnityComponent, ShipComponent>.Exclude<DestroyedShipMarkComponent> _unityShips;
        private EcsFilter<VelocityComponent, RigidBodyComponent, ShipComponent>.Exclude<DestroyedShipMarkComponent> _rigidShips;
        private EcsFilter<PositionComponent, ShipComponent>.Exclude<UnityComponent, DestroyedShipMarkComponent> _nonUnityShips;
        private EcsFilter<ShipComponent>.Exclude<LocalMarkComponent, RemoteMarkComponent> _newShips;

        private EcsFilter<RefreshNetworkDataEvent> _sendEvents;
        private EcsFilter<RemoveSessionEvent> _removeSessionEvents;
        
        public void Run()
        {
            for (int i = 0; i < _newShips.EntitiesCount; i++)
            {
                if (_newShips.Components1[i].SessionId == _localConfig.LocalSessionId)
                {
                    _ecsWorld.AddComponent<LocalMarkComponent>(_newShips.Entities[i]);
                    _ecsWorld.AddComponent<CameraFollowTargetComponent>(_newShips.Entities[i]);
                }
                else
                {
                    _ecsWorld.AddComponent<RemoteMarkComponent>(_newShips.Entities[i]);
                }
            }
            
            for (int i = 0; i < _nonUnityShips.EntitiesCount; i++)
            {
                int shipEntity = _nonUnityShips.Entities[i];

                Transform shipTransform = _localConfig.ShipContainer.Get().PoolTransform;
                shipTransform.gameObject.SetActive(true);
                shipTransform.GetComponent<EntityBehaviour>().AttachToEntity(shipEntity);
            }

            switch (_localConfig.ClientType)
            {
                case ClientType.SERVER:
                    UpdatePositionsOnServer();
                    UpdateVelocityOnServer();
                    break;
                case ClientType.CLIENT:
                    UpdatePositionsOnClient();
                    UpdateVelocityOnClient();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < _removeSessionEvents.EntitiesCount; i++)
            {
                long sessionId = _removeSessionEvents.Components1[i].SessionId;
                if(!_localConfig.SessionIdToLocalEntity.ContainsKey(sessionId)) continue;
                
                int sessionEntity = _localConfig.SessionIdToLocalEntity[sessionId];
                
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
            for (int i = 0; i < _unityShips.EntitiesCount; i++)
            {
                PositionComponent position = _unityShips.Components1[i];
                Transform shipTransform = _unityShips.Components2[i].Transform;

                shipTransform.position = new Vector3(position.PositionX, position.PositionY);
                shipTransform.rotation = Quaternion.Euler(0, 0, position.Rotation);
            }
        }

        private void UpdateVelocityOnClient()
        {
            for (int i = 0; i < _rigidShips.EntitiesCount; i++)
            {
                VelocityComponent velocity = _rigidShips.Components1[i];
                Rigidbody2D shipRigid = _rigidShips.Components2[i].Rigidbody2D;

                shipRigid.velocity = new Vector2(velocity.VelocityX, velocity.VelocityY);
                shipRigid.angularVelocity = velocity.AngularVelocity;
            }
        }

        private void UpdatePositionsOnServer()
        {
            if (_sendEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _unityShips.EntitiesCount; i++)
                {
                    int shipEntity = _unityShips.Entities[i];
                    _ecsWorld.SendComponentToNetwork<ShipComponent>(shipEntity);
                    _ecsWorld.SendComponentToNetwork<PositionComponent>(shipEntity);
                }
            }
            
            for (int i = 0; i < _unityShips.EntitiesCount; i++)
            {
                PositionComponent position = _unityShips.Components1[i];
                Transform shipTransform = _unityShips.Components2[i].Transform;

                position.Rotation = shipTransform.rotation.eulerAngles.z;
                position.PositionX = shipTransform.position.x;
                position.PositionY = shipTransform.position.y;

                _ecsWorld.SendComponentToNetwork<PositionComponent>(_unityShips.Entities[i]);
            }
        }

        private void UpdateVelocityOnServer()
        {
            if (_sendEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _rigidShips.EntitiesCount; i++)
                {
                    int shipEntity = _rigidShips.Entities[i];
                    _ecsWorld.SendComponentToNetwork<VelocityComponent>(shipEntity);
                }
            }
            
            for (int i = 0; i < _rigidShips.EntitiesCount; i++)
            {
                VelocityComponent velocity = _rigidShips.Components1[i];
                Rigidbody2D rigid = _rigidShips.Components2[i].Rigidbody2D;

                velocity.VelocityX = rigid.velocity.x;
                velocity.VelocityY = rigid.velocity.y;
                velocity.AngularVelocity = rigid.angularVelocity;

                _ecsWorld.SendComponentToNetwork<VelocityComponent>(_rigidShips.Entities[i]);
            }
        }
    }
}