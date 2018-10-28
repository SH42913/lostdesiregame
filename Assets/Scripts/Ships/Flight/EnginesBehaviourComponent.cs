using Leopotam.Ecs;
using Ships.Effects;
using UnityEngine;
using UnityIntegration;

namespace Ships.Flight
{
    public class EnginesBehaviourComponent : AbstractBehaviourComponent
    {
        public float ForwardForce;
        public float BackwardForce;
        public float StrafeForce;
        public float TurnTorque;
        
        public ParticleSystem[] ForwardEngines;
        public ParticleSystem[] BackwardEngines;
        public ParticleSystem[] TurnLeftEngines;
        public ParticleSystem[] TurnRightEngines;
        public ParticleSystem[] StrafeLeftEngines;
        public ParticleSystem[] StrafeRightEngines;
    
        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var engines = world.AddComponent<EnginesStateComponent>(entity);
            engines.EnabledEngines = 0;
            
            var enginesStats = world.AddComponent<EnginesStatsComponent>(entity);
            enginesStats.ForwardForce = ForwardForce;
            enginesStats.BackwardForce = BackwardForce;
            enginesStats.StrafeForce = StrafeForce;
            enginesStats.TurnTorque = TurnTorque;

            var engineEffects = world.AddComponent<EngineEffectsComponent>(entity);
            engineEffects.ForwardEngines = ForwardEngines;
            engineEffects.BackwardEngines = BackwardEngines;
            engineEffects.TurnLeftEngines = TurnLeftEngines;
            engineEffects.TurnRightEngines = TurnRightEngines;
            engineEffects.StrafeLeftEngines = StrafeLeftEngines;
            engineEffects.StrafeRightEngines = StrafeRightEngines;
        }
    }
}