using System.Collections.Generic;
using Cleaning;
using ControlledCamera;
using DebugSystems.StatusString;
using Dialogs;
using Dialogs.ConnectToDialog;
using Dialogs.CreatePlayerDialog;
using Dialogs.StartConnectionDialog;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Leopotam.Ecs.Net.Implementations.JsonSerializator;
using Leopotam.Ecs.Net.Implementations.TcpRetranslator;
using LeopotamGroup.Pooling;
using Network;
using Network.Sessions;
using Players;
using Ships;
using Ships.Flight;
using Ships.Spawn;
using Ships.Update;
using UnityEngine;
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
            .AddConnectionSystems()
            .AddDialogsSystems()
            .AddShipSystems()
            .AddCameraSystems()
            .AddWorldSystems()
            .AddDebugSystems();
            
        AddNetworkProcessingSystems(_systems);
        _networkProcessingSystems = null;
        
        _systems.Add(new CleaningSystem());
        _systems.Initialize ();
        GenerateStartEvents();
        
#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create (_systems);
#endif
    }

    private void Update () 
    {
        CheckKey(KeyCode.W, EngineDirection.FORWARD);
        CheckKey(KeyCode.S, EngineDirection.BACKWARD);
        CheckKey(KeyCode.A, EngineDirection.STRAFE_LEFT);
        CheckKey(KeyCode.D, EngineDirection.STRAFE_RIGHT);
        CheckKey(KeyCode.Q, EngineDirection.TURN_LEFT);
        CheckKey(KeyCode.E, EngineDirection.TURN_RIGHT);
        
        _systems.Run ();
    }

    private void CheckKey(KeyCode key, EngineDirection direction)
    {
        if (Input.GetKeyDown(key))
        {
            var switchEvent = _world.SendEventToNetwork<SwitchEngineEvent>();
            switchEvent.Direction = direction;
            switchEvent.Enable = true;
            switchEvent.PlayerId = _world
                .GetFilter<EcsFilter<PlayerComponent, LocalMarkComponent>>()
                .Components1[0]
                .Id;
        }
        if (Input.GetKeyUp(key))
        {
            var switchEvent = _world.SendEventToNetwork<SwitchEngineEvent>();
            switchEvent.Direction = direction;
            switchEvent.Enable = false;
            switchEvent.PlayerId = _world
                .GetFilter<EcsFilter<PlayerComponent, LocalMarkComponent>>()
                .Components1[0]
                .Id;
        }
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
            new NetworkComponentProcessSystem<RemoveMarkComponent>((newComp, oldComp) => { }),
            new NetworkEventProcessSystem<SwitchEngineEvent>(SwitchEngineEvent.NewToOldConverter),
            new NetworkComponentProcessSystem<EnginesComponent>(EnginesComponent.NewToOldConverter)
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

    public static EcsSystems AddDebugSystems(this EcsSystems systems)
    {
        return systems
            .Add(new StatusStringSystem());
    }
}