﻿using Leopotam.Ecs;
using UnityEngine;

namespace Ships.Flight
{
    [EcsInject]
    public class ShipFlightEffectsSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<EnginesComponent, EngineEffectsComponent> _engines;
        
        public void Run()
        {
            for (int i = 0; i < _engines.EntitiesCount; i++)
            {
                EngineDirection direction = _engines.Components1[i].EnabledEngines;

                SwitchEngine(direction, _engines.Components2[i].ForwardEngines, EngineDirection.FORWARD);
                SwitchEngine(direction, _engines.Components2[i].BackwardEngines, EngineDirection.BACKWARD);
                SwitchEngine(direction, _engines.Components2[i].TurnLeftEngines, EngineDirection.TURN_LEFT);
                SwitchEngine(direction, _engines.Components2[i].TurnRightEngines, EngineDirection.TURN_RIGHT);
                SwitchEngine(direction, _engines.Components2[i].StrafeLeftEngines, EngineDirection.STRAFE_LEFT);
                SwitchEngine(direction, _engines.Components2[i].StrafeRightEngines, EngineDirection.STRAFE_RIGHT);
            }
        }

        private void SwitchEngine(EngineDirection enabledEngines, ParticleSystem[] engines, EngineDirection target)
        {
            foreach (var engineEffect in engines)
            {
                var engineEffectEmission = engineEffect.emission;
                engineEffectEmission.enabled = enabledEngines.HasFlag(target);
            }
        }
    }
}