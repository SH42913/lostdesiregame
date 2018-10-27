using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using Players;
using Ships.Flight;
using UnityEngine;
using UnityIntegration;

namespace Ships.Spawn
{
    [EcsInject]
    public class ShipSpawnSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<SpawnShipEvent> _createEvents;
        private EcsFilter<PlayerComponent> _players;
        
        public void Run()
        {
            if (_localConfig.Data.ClientType == ClientType.SERVER)
            {
                SpawnShips();
            }
        }

        private void SpawnShips()
        {
            for (int i = 0; i < _createEvents.EntitiesCount; i++)
            {
                long sessionId = _createEvents.Components1[i].SessionId;
                int sessionLocalEntity = GetSessionEntity(sessionId);
                
                if(sessionLocalEntity < 0 || !_ecsWorld.IsEntityExists(sessionLocalEntity)) continue;
                if(_ecsWorld.GetComponent<AssignedShipComponent>(sessionLocalEntity) != null) continue;

                Transform shipObject = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipObject.gameObject.SetActive(true);
                shipObject.position = new Vector3(Random.Range(-25, 25), 0);
                shipObject.rotation = Quaternion.identity;
                
                ShipComponent ship;
                PositionComponent position;
                int shipEntity = _ecsWorld.CreateEntityWith(out ship, out position);
                ship.SessionId = sessionId;
                
                position.PositionX = shipObject.position.x;
                position.PositionY = shipObject.position.y;
                position.Rotation = shipObject.eulerAngles.z;

                var ownedBy = _ecsWorld.AddComponent<OwnedBySessionComponent>(shipEntity);
                ownedBy.SessionId = sessionId;
                
                _ecsWorld.AddComponent<AssignedShipComponent>(sessionLocalEntity).LocalShipEntity = shipEntity;
                
                shipObject.GetComponent<EntityBehaviour>().AttachToEntity(shipEntity);
                _ecsWorld.SendComponentToNetwork<ShipComponent>(shipEntity);
                _ecsWorld.SendComponentToNetwork<PositionComponent>(shipEntity);
                _ecsWorld.SendComponentToNetwork<EnginesStatsComponent>(shipEntity);
            }
        }

        private int GetSessionEntity(long sessionId)
        {
            if (!_localConfig.Data.SessionIdToLocalEntity.ContainsKey(sessionId)) return -1;

            return _localConfig.Data.SessionIdToLocalEntity[sessionId];
        }
    }
}