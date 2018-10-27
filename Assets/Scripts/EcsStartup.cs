using System.Collections.Generic;
using ControlledCamera;
using DebugSystems.StatusString;
using Dialogs;
using Dialogs.ConnectToDialog;
using Dialogs.CreatePlayerDialog;
using Dialogs.StartConnectionDialog;
using InputSystems;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Leopotam.Ecs.Net.Implementations.JsonSerializator;
using Leopotam.Ecs.Net.Implementations.TcpRetranslator;
using LeopotamGroup.Pooling;
using Network;
using Network.Sessions;
using Players;
using Ships;
using Ships.Effects;
using Ships.Flight;
using Ships.Spawn;
using UnityEngine;
using UnityIntegration;
using World;

internal sealed class EcsStartup : MonoBehaviour {
    private EcsWorld _world;
    private EcsSystems _systems;
    private List<IEcsSystem> _networkProcessingSystems;

    [SerializeField]
    private PoolContainer _shipContainer;

    private void OnEnable () 
    {
        _world = new EcsWorld ();
        var networkConfig = EcsFilterSingle<EcsNetworkConfig>.Create(_world);
        var localConfig = EcsFilterSingle<LocalGameConfig>.Create(_world);
        localConfig.ShipContainer = _shipContainer;
        localConfig.SessionIdToLocalEntity = new Dictionary<long, int>();
        
        networkConfig.EcsNetworkListener = new TcpRetranslator();
        networkConfig.Serializator = new JsonSerializator();
        
#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create (_world);
#endif
        
        _systems = new EcsSystems (_world);
        
        _systems.Add(new RetranslatorSystem());
        
        CreateNetworkProcessingSystems();
        AddNetworkProcessingSystems(_systems);
            
        _systems
            .AddControlSystems()
            .AddDialogsSystems()
            .AddWorldSystems()
            .AddConnectionSystems()
            .AddShipSystems()
            .AddCameraSystems()
            .AddDebugSystems();
            
        AddNetworkProcessingSystems(_systems);
        _networkProcessingSystems = null;

        _systems.AddCleanSystems();
        _systems.Initialize ();
        GenerateStartEvents();
        
#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create (_systems);
#endif
    }

    private void Update () 
    {   
        _systems.Run ();
    }

    private void GenerateStartEvents()
    {
        _world.CreateEntityWith<ShowStartConnectionDialogEvent>();
    }

    private void OnDisable () 
    {
        _systems.Dispose ();
        _systems = null;
        _world.Dispose ();
        _world = null;
    }

    private void CreateNetworkProcessingSystems()
    {
        _networkProcessingSystems = new List<IEcsSystem>
        {
            new NetworkComponentProcessSystem<WorldComponent>(WorldComponent.NewToOldConverter),
            new NetworkComponentProcessSystem<SessionComponent>(SessionComponent.NewToOldConverter),
            new NetworkComponentProcessSystem<PlayerComponent>(PlayerComponent.NewToOldConverter),
            new NetworkEventProcessSystem<SpawnShipEvent>(SpawnShipEvent.NewToOldConverter),
            new NetworkComponentProcessSystem<ShipComponent>(ShipComponent.NewToOldConverter),
            new NetworkComponentProcessSystem<PositionComponent>(PositionComponent.NewToOldConverter),
            new NetworkComponentProcessSystem<DestroyedShipMarkComponent>(DestroyedShipMarkComponent.NewToOldConverter),
            new NetworkEventProcessSystem<SwitchEngineEvent>(SwitchEngineEvent.NewToOldConverter),
            new NetworkComponentProcessSystem<EnginesStatsComponent>(EnginesStatsComponent.NewToOldConverter),
            new NetworkComponentProcessSystem<EnginesStateComponent>(EnginesStateComponent.NewToOldConverter)
        };
    }
    
    private void AddNetworkProcessingSystems(EcsSystems systems)
    {
        foreach (IEcsSystem processingSystem in _networkProcessingSystems)
        {
            systems.Add(processingSystem);
        }
    }
}

public static class EcsWorldExtensions
{
    public static EcsSystems AddControlSystems(this EcsSystems systems)
    {
        return systems.Add(new KeyboardListenerSystem());
    }
    
    public static EcsSystems AddDialogsSystems(this EcsSystems systems)
    {
        return systems
            .Add(new BaseDialogSystem())
            .Add(new StartConnectionDialogSystem())
            .Add(new ConnectToDialogSystem())
            .Add(new CreatePlayerDialogSystem());
    }

    public static EcsSystems AddWorldSystems(this EcsSystems systems)
    {
        return systems
            .Add(new PlayerSystem())
            .Add(new WorldSystem());
    }

    public static EcsSystems AddConnectionSystems(this EcsSystems systems)
    {
        return systems
            .Add(new ConnectedClientSystem())
            .Add(new DisconnectedClientSystem())
            .Add(new SessionSystem());
    }

    public static EcsSystems AddShipSystems(this EcsSystems systems)
    {
        return systems
            .Add(new ShipSpawnSystem())
            .Add(new ShipFlightSystem())
            .Add(new ShipFlightEffectsSystem())
            .Add(new ShipUpdateSystem());
    }

    public static EcsSystems AddCameraSystems(this EcsSystems systems)
    {
        return systems
            .Add(new ControlledCameraSystem());
    }

    public static EcsSystems AddCleanSystems(this EcsSystems systems)
    {
        return systems.Add(new SessionCleanSystem())
            .Add(new PlayerCleanSystem())
            .Add(new ShipCleanSystem())
            .Add(new UnityCleanSystem());
    }

    public static EcsSystems AddDebugSystems(this EcsSystems systems)
    {
        return systems
            .Add(new StatusStringSystem());
    }
}