using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using LeopotamGroup.Pooling;
using Ships.Effects;
using Ships.Flight;
using Ships.Spawn;
using UnityIntegration;

namespace Ships
{
    [EcsInject]
    public class ShipCleanSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private LocalGameConfig _localConfig;

        private EcsFilter<ShipComponent, DestroyedShipMarkComponent> _destroyedShips;
        private EcsFilter<EngineEffectsComponent> _engineEffects;
        
        private EcsFilter<SwitchEngineEvent> _switchEngineEvents;
        private EcsFilter<SpawnShipEvent> _createEvents;

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            _switchEngineEvents.RemoveAllEntities();
            _createEvents.RemoveAllEntities();
            
            DestroyShips();
        }

        private void DestroyShips()
        {
            for (int i = 0; i < _destroyedShips.EntitiesCount; i++)
            {
                int shipEntity = _destroyedShips.Entities[i];
                var engine = _ecsWorld.GetComponent<EngineEffectsComponent>(shipEntity);
                if (engine != null)
                {
                    engine.ForwardEngines = null;
                    engine.BackwardEngines = null;
                    engine.StrafeLeftEngines = null;
                    engine.StrafeRightEngines = null;
                    engine.TurnLeftEngines = null;
                    engine.TurnRightEngines = null;
                }

                var rigid = _ecsWorld.GetComponent<RigidBodyComponent>(shipEntity);
                if (rigid != null)
                {
                    rigid.Rigidbody2D = null;
                }
                
                var unity = _ecsWorld.GetComponent<UnityComponent>(shipEntity);
                if(unity != null)
                {
                    _localConfig.ShipContainer.Recycle(unity.Transform.GetComponent<IPoolObject>());
                    unity.Transform = null;
                }
                
                _ecsWorld.RemoveEntity(shipEntity);
                _ecsWorld.RemoveNetworkEntity(shipEntity);
            }
        }

        public void Destroy()
        {
            ClearEngineEffects();
        }

        private void ClearEngineEffects()
        {
            for (int i = 0; i < _engineEffects.EntitiesCount; i++)
            {
                _engineEffects.Components1[i].ForwardEngines = null;
                _engineEffects.Components1[i].BackwardEngines = null;
                _engineEffects.Components1[i].TurnLeftEngines = null;
                _engineEffects.Components1[i].TurnRightEngines = null;
                _engineEffects.Components1[i].StrafeLeftEngines = null;
                _engineEffects.Components1[i].StrafeRightEngines = null;
            }
        }
    }
}