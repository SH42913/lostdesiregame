﻿using System;
using Leopotam.Ecs;
using UnityEngine;

namespace UnityIntegration
{
    public class EntityBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool _createOnEnable = true;
    
        [SerializeField]
        private bool _removeOnDisable = true;

        [SerializeField]
        private bool _checkForRigidBody;
    
        [SerializeField]
        private AbstractBehaviourComponent[] _components;

        private EcsWorld _world = EcsWorld.Active;
        
        public int Entity { get; private set; } = -1;

        public void AttachToEntity(int entity)
        {
#if DEBUG
            if (Entity > 0)
            {
                throw new Exception("Object already attached to Entity " + Entity);
            }
#endif
        
            var unityComponent = _world.AddComponent<UnityComponent>(entity);
            unityComponent.Transform = transform;
        
            AddComponentsToEntity(entity);
        }

        private void OnEnable()
        {
            if(!_createOnEnable) return;
            _world = EcsWorld.Active;
        
            UnityComponent unityComponent;
            Entity = _world.CreateEntityWith(out unityComponent);
            unityComponent.Transform = transform;
        
            AddComponentsToEntity(Entity);
        }

        private void OnDisable()
        {
            if(!_removeOnDisable || Entity < 0) return;

            _world.GetComponent<UnityComponent>(Entity).Transform = null;
            _world.RemoveEntity(Entity);
            Entity = -1;
        }

        private void AddComponentsToEntity(int entity)
        {
            if (_checkForRigidBody)
            {
                var rigid = GetComponent<Rigidbody2D>();
                if (rigid == null)
                {
                    throw new Exception("RigidBody is not exist on this GO");
                }
                _world.AddComponent<RigidBodyComponent>(entity).Rigidbody2D = rigid;
            }
        
            foreach (AbstractBehaviourComponent component in _components)
            {
                component.AttachComponentToEntity(_world, entity);
            }
        }
    }
}