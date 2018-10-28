using Leopotam.Ecs;
using UnityEngine;

namespace UnityIntegration
{
    public abstract class AbstractBehaviourComponent : MonoBehaviour
    {
        public abstract void AttachComponentToEntity(EcsWorld world, int entity);
    }
}