using Leopotam.Ecs;
using UnityEngine;

public abstract class AbstractBehaviourComponent : MonoBehaviour
{
    public abstract void AttachComponentToEntity(EcsWorld world, int entity);
}

public class EntityBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool _removeOnDisable;
    
    [SerializeField]
    private AbstractBehaviourComponent[] _components;

    private EcsWorld _world;
    public int Entity { get; private set; }

    private void OnEnable()
    {
        _world = EcsWorld.Active;
        
        UnityComponent unityComponent;
        Entity = _world.CreateEntityWith(out unityComponent);
        unityComponent.Transform = transform;
        
        foreach (AbstractBehaviourComponent component in _components)
        {
            component.AttachComponentToEntity(_world, Entity);
        }
    }

    private void OnDisable()
    {
        if(!_removeOnDisable) return;

        _world.GetComponent<UnityComponent>(Entity).Transform = null;
        _world.RemoveEntity(Entity);
    }
}