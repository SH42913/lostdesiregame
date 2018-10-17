using Leopotam.Ecs;

[EcsInject]
public class CleaningSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _ecsWorld;

    private EcsFilter<UnityComponent> _unityComponents;
    
    public void Initialize()
    {
        
    }

    public void Run()
    {
        
    }

    public void Destroy()
    {
        for (int i = 0; i < _unityComponents.EntitiesCount; i++)
        {
            _unityComponents.Components1[i].Transform = null;
        }
    }
}