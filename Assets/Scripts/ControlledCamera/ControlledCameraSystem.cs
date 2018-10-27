using Leopotam.Ecs;
using UnityEngine;

namespace ControlledCamera
{
    [EcsInject]
    public class ControlledCameraSystem : IEcsRunSystem
    {
        private EcsFilter<ControlledCameraComponent, UnityComponent> _cameras;

        private EcsFilter<CameraFollowTargetComponent, UnityComponent> _targets;
        
        public void Run()
        {
            for (int i = 0; i < _cameras.EntitiesCount; i++)
            {
                float height = _cameras.Components1[i].Height;

                Vector3 newPosition = SelectTargetPosition();
                
                Transform cameraTransform = _cameras.Components2[i].Transform;
                cameraTransform.position = new Vector3(newPosition.x, newPosition.y, -height);
                cameraTransform.rotation = SelectTargetRotation();
            }
        }

        private Vector3 SelectTargetPosition()
        {
            return _targets.EntitiesCount > 0 
                ? _targets.Components2[_targets.EntitiesCount - 1].Transform.position 
                : Vector3.zero;
        }

        private Quaternion SelectTargetRotation()
        {
            return _targets.EntitiesCount > 0 
                ? _targets.Components2[_targets.EntitiesCount - 1].Transform.rotation 
                : Quaternion.identity;
        }
    }
}