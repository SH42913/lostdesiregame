using Leopotam.Ecs;
using UnityIntegration;

namespace ControlledCamera
{
    public class ControlledCameraBehaviourComponent : AbstractBehaviourComponent
    {
        public float StartHeight;
        
        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            world.AddComponent<ControlledCameraComponent>(entity).Height = StartHeight;
        }
    }
}