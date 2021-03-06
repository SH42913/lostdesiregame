﻿using Leopotam.Ecs;
using UnityEngine.UI;
using UnityIntegration;

namespace DebugSystems.StatusString
{
    public class StatusStringBehaviourComponent : AbstractBehaviourComponent
    {
        public Text StatusText;
        
        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            world.AddComponent<StatusStringComponent>(entity).StatusText = StatusText;
        }
    }
}