using Leopotam.Ecs;
using UnityEngine;

[EcsIgnoreInFilter]
public class FakeComponent
{
    
}

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

    private int _entity;

    private void OnEnable()
    {
        _world = EcsWorld.Active;
        
        FakeComponent fake;
        _entity = _world.CreateEntityWith(out fake);
        
        foreach (AbstractBehaviourComponent component in _components)
        {
            component.AttachComponentToEntity(_world, _entity);
        }
        
        _world.RemoveComponent<FakeComponent>(_entity);
    }

    private void OnDisable()
    {
        if(!_removeOnDisable) return;
        
        _world.RemoveEntity(_entity);
    }
}