using Leopotam.Ecs;
using LeopotamGroup.Pooling;
using Network.Sessions;
using Players;
using Ships;
using Ships.Flight;

namespace Cleaning
{
    [EcsInject]
    public class CleaningSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;

        private EcsFilter<UnityComponent> _unityComponents;
        private EcsFilter<RigidBodyComponent> _rigidComponents;
        private EcsFilter<EngineEffectsComponent> _engineEffects;
        
        private EcsFilter<RemoveMarkComponent, ShipComponent> _shipsToRemove;
        private EcsFilter<RemoveMarkComponent> _markedToRemove;

        private EcsFilter<RemovePlayerEvent> _removePlayerEvents;
        private EcsFilter<SendNetworkDataEvent> _sendDataEvents;
        private EcsFilter<SwitchEngineEvent> _switchEngineEvents;
    
        public void Initialize()
        {
        
        }

        public void Run()
        {
            _removePlayerEvents.RemoveAllEntities();
            _sendDataEvents.RemoveAllEntities();
            _switchEngineEvents.RemoveAllEntities();

            for (int i = 0; i < _shipsToRemove.EntitiesCount; i++)
            {
                int shipEntity = _shipsToRemove.Entities[i];
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
                    _localConfig.Data.ShipContainer.Recycle(unity.Transform.GetComponent<IPoolObject>());
                    unity.Transform = null;
                }
            }

            for (int i = 0; i < _markedToRemove.EntitiesCount; i++)
            {
                _ecsWorld.RemoveEntity(_markedToRemove.Entities[i]);
            }
        }

        public void Destroy()
        {
            _localConfig.Data.ShipContainer = null;
        
            for (int i = 0; i < _unityComponents.EntitiesCount; i++)
            {
                _unityComponents.Components1[i].Transform = null;
            }

            for (int i = 0; i < _rigidComponents.EntitiesCount; i++)
            {
                _rigidComponents.Components1[i].Rigidbody2D = null;
            }

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