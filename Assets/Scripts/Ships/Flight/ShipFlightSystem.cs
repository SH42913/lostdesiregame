using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Players;
using UnityEngine;
using UnityIntegration;

namespace Ships.Flight
{
    [EcsInject]
    public class ShipFlightSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<SwitchEngineEvent> _switchEvents;
        private EcsFilter<PlayerComponent> _players;
        private EcsFilter<RigidBodyComponent, EnginesStateComponent, EnginesStatsComponent> _engines;
        
        public void Run()
        {
            if(_localConfig.Data.ClientType == ClientType.CLIENT) return;
            
            SwitchEngines();
            ApplyForces();
        }

        private void SwitchEngines()
        {
            for (int i = 0; i < _switchEvents.EntitiesCount; i++)
            {
                int sessionEntity = GetSessionEntity(_switchEvents.Components1[i].SessionId);
                if(sessionEntity < 0 || !_ecsWorld.IsEntityExists(sessionEntity)) continue;

                var assignedShip = _ecsWorld.GetComponent<AssignedShipComponent>(sessionEntity);
                if(assignedShip == null) continue;
                
                int shipEntity = assignedShip.LocalShipEntity;
                if(!_ecsWorld.IsEntityExists(shipEntity)) continue;

                var engines = _ecsWorld.GetComponent<EnginesStateComponent>(shipEntity);
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
                _ecsWorld.SendComponentToNetwork<EnginesStateComponent>(shipEntity);
            }
        }

        private void ApplyForces()
        {
            for (int i = 0; i < _engines.EntitiesCount; i++)
            {
                Rigidbody2D rigid = _engines.Components1[i].Rigidbody2D;
                EnginesStatsComponent enginesStats = _engines.Components3[i];
                EngineDirection enabledEngines = _engines.Components2[i].EnabledEngines;

                if (enabledEngines.HasFlag(EngineDirection.FORWARD))
                {
                    Vector2 forceDirection = rigid.transform.rotation * Vector2.up;
                    rigid.AddForce(enginesStats.ForwardForce * forceDirection);
                }

                if (enabledEngines.HasFlag(EngineDirection.BACKWARD))
                {
                    Vector2 forceDirection = rigid.transform.rotation * Vector2.down;
                    rigid.AddForce(enginesStats.BackwardForce * forceDirection);
                }

                if (enabledEngines.HasFlag(EngineDirection.TURN_LEFT))
                {
                    rigid.AddTorque(enginesStats.TurnTorque);
                }

                if (enabledEngines.HasFlag(EngineDirection.TURN_RIGHT))
                {
                    rigid.AddTorque(-enginesStats.TurnTorque);
                }

                if (enabledEngines.HasFlag(EngineDirection.STRAFE_LEFT))
                {
                    Vector2 forceDirection = rigid.transform.rotation * Vector2.left;
                    rigid.AddForce(enginesStats.ForwardForce * forceDirection);
                }

                if (enabledEngines.HasFlag(EngineDirection.STRAFE_RIGHT))
                {
                    Vector2 forceDirection = rigid.transform.rotation * Vector2.right;
                    rigid.AddForce(enginesStats.ForwardForce * forceDirection);
                }
            }
        }

        private int GetSessionEntity(long sessionId)
        {
            if (!_localConfig.Data.SessionIdToLocalEntity.ContainsKey(sessionId)) return -1;

            return _localConfig.Data.SessionIdToLocalEntity[sessionId];
        }
    }
}