using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using UnityEngine;

namespace Ships
{
    [EcsInject]
    public class ShipSpawnSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<CreateShipEvent> _createEvents;
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
                if(_ecsWorld.GetComponent<CanControlComponent>(localPlayerEntity) != null) continue;

                Transform shipObject = _localConfig.Data.ShipContainer.Get().PoolTransform;
                shipObject.gameObject.SetActive(true);
                shipObject.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                int shipEntity = shipObject.GetComponent<EntityBehaviour>().Entity;

                shipObject.GetComponent<Rigidbody2D>().angularVelocity = 10;
                
                ShipMarkComponent shipMark = _ecsWorld.AddComponent<ShipMarkComponent>(shipEntity);
                shipMark.PlayerId = playerKey;
                
                PositionComponent position = _ecsWorld.AddComponent<PositionComponent>(shipEntity);
                position.PositionX = shipObject.position.x;
                position.PositionY = shipObject.position.y;
                position.Rotation = shipObject.eulerAngles.z;

                _ecsWorld.AddComponent<ControlableByComponent>(shipEntity).LocalPlayerEntity = localPlayerEntity;
                _ecsWorld.AddComponent<CanControlComponent>(localPlayerEntity).LocalShipEntity = shipEntity;
                
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