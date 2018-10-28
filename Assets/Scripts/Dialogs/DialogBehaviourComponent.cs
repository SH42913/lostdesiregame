using Leopotam.Ecs;
using UnityEngine;
using UnityIntegration;

namespace Dialogs
{
    public class DialogBehaviourComponent : AbstractBehaviourComponent
    {
        public GameObject Content;
        
        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            world.AddComponent<DialogComponent>(entity).Content = Content;
        }
    }
}