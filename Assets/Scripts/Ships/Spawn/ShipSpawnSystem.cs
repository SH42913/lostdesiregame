using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using Ships.Flight;
using UnityEngine;

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
            _createEvents.RemoveAllEntities();
        }

        private void SpawnShips()
        {
            for (int i = 0; i < _createEvents.EntitiesCount; i++)
            {
                long playerKey = _createEvents.Components1[i].PlayerId;
                int localPlayerEntity = FindLocalPlayerEntity(playerKey);
                
                if(localPlayerEntity < 0 || !_ecsWorld.IsEntityExists(localPlayerEntity)) continue;
                if(_ecsWorld.GetComponent<AssignedShipComponent>(localPlayerEntity) != null) continue;

                Transform shipObject = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipObject.gameObject.SetActive(true);
                shipObject.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                
                ShipMarkComponent shipMark;
                PositionComponent position;
                int shipEntity = _ecsWorld.CreateEntityWith(out shipMark, out position);
                shipMark.PlayerId = playerKey;
                
                position.PositionX = shipObject.position.x;
                position.PositionY = shipObject.position.y;
                position.Rotation = shipObject.eulerAngles.z;

                var ownedBy = _ecsWorld.AddComponent<OwnedByPlayerComponent>(shipEntity);
                ownedBy.LocalPlayerEntity = localPlayerEntity;
                ownedBy.PlayerId = playerKey;
                
                _ecsWorld.AddComponent<AssignedShipComponent>(localPlayerEntity).LocalShipEntity = shipEntity;
                
                shipObject.GetComponent<EntityBehaviour>().AttachToEntity(shipEntity);
                _ecsWorld.SendComponentToNetwork<ShipMarkComponent>(shipEntity);
                _ecsWorld.SendComponentToNetwork<PositionComponent>(shipEntity);
            }
        }

        private int FindLocalPlayerEntity(long playerKey)
        {
            for (int i = 0; i < _players.EntitiesCount; i++)
            {
                if (_players.Components1[i].Id == playerKey) return _players.Entities[i];
            }

            return -1;
        }
    }
}