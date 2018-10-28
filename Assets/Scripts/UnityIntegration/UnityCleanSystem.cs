﻿using Leopotam.Ecs;

namespace UnityIntegration
{
    [EcsInject]
    public class UnityCleanSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<UnityComponent> _unityComponents;
        private EcsFilter<RigidBodyComponent> _rigidbodyComponents;
        
        public void Initialize()
        {
            
        }

        public void Destroy()
        {
            for (int i = 0; i < _unityComponents.EntitiesCount; i++)
            {
                _unityComponents.Components1[i].Transform = null;
            }

            for (int i = 0; i < _rigidbodyComponents.EntitiesCount; i++)
            {
                _rigidbodyComponents.Components1[i].Rigidbody2D = null;
            }
        }
    }
}