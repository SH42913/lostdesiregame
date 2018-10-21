using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using UnityEngine;

namespace Ships.Flight
{
    [EcsInject]
    public class ShipFlightSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<SwitchEngineEvent> _switchEvents;
        private EcsFilter<PlayerComponent> _players;
        private EcsFilter<RigidBodyComponent, EnginesComponent> _engines;
        
        public void Run()
        {
            if(_localConfig.Data.ClientType == ClientType.CLIENT) return;
            
            for (int i = 0; i < _switchEvents.EntitiesCount; i++)
            {
                int playerEntity = GetLocalPlayer(_switchEvents.Components1[i].PlayerId);
                if(playerEntity < 0 || !_ecsWorld.IsEntityExists(playerEntity)) continue;

                var assignedShip = _ecsWorld.GetComponent<AssignedShipComponent>(playerEntity);
                if(assignedShip == null) continue;
                
                int shipEntity = assignedShip.LocalShipEntity;
                if(!_ecsWorld.IsEntityExists(shipEntity)) continue;

                var engines = _ecsWorld.GetComponent<EnginesComponent>(shipEntity);
                var rigid = _ecsWorld.GetComponent<RigidBodyComponent>(shipEntity);
                if(engines == null || rigid == null) continue;

                var switchEvent = _switchEvents.Components1[i];
                if (switchEvent.Enable)
                {
                    engines.EnabledEngines = engines.EnabledEngines | switchEvent.Direction;
                }
                else
                {
                    engines.EnabledEngines = engines.EnabledEngines & ~switchEvent.Direction;
                }
                _ecsWorld.SendComponentToNetwork<EnginesComponent>(shipEntity);
            }

            for (int i = 0; i < _engines.EntitiesCount; i++)
            {
                Rigidbody2D rigid = _engines.Components1[i].Rigidbody2D;
                EngineDirection enabledEngines = _engines.Components2[i].EnabledEngines;

                if (enabledEngines.HasFlag(EngineDirection.FORWARD))
                {
                    Vector2 forceDirection = rigid.transform.rotation * Vector2.up;
                    rigid.AddForce(_engines.Components2[i].ForwardForce * forceDirection);
                }
            }
        }

        private int GetLocalPlayer(long playerId)
        {
            for (int i = 0; i < _players.EntitiesCount; i++)
            {
                if(_players.Components1[i].Id != playerId) continue;

                return _players.Entities[i];
            }

            return -1;
        }
    }
}