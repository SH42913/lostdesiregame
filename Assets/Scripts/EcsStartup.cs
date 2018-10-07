using System.Collections.Generic;
using Connections;
using Debug.StatusString;
using Dialogs;
using Dialogs.ConnectToDialog;
using Dialogs.StartConnectionDialog;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Leopotam.Ecs.Net.Implementations.JsonSerializator;
using Leopotam.Ecs.Net.Implementations.TcpRetranslator;
using UnityEngine;

internal sealed class EcsStartup : MonoBehaviour {
    private EcsWorld _world;
    private EcsSystems _systems;

    private void OnEnable () {
        _world = new EcsWorld ();
        var networkConfig = EcsFilterSingle<EcsNetworkConfig>.Create(_world);
        var localConfig = EcsFilterSingle<LocalGameConfig>.Create(_world);
        
        networkConfig.EcsNetworkListener = new TcpRetranslator();
        networkConfig.Serializator = new JsonSerializator();
        
        localConfig.ConnectedClients = new List<ClientInfo>();
        
#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create (_world);
#endif
        
        _systems = new EcsSystems (_world);
        _systems
            .Add(new RetranslatorSystem())
            .AddNetworkProcessingSystems()
            .AddConnectionSystems()
            .AddDialogsSystems()
            .AddDebugSystems()
            .AddNetworkProcessingSystems()
            .Initialize ();

        GenerateStartEvents();
        
#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create (_systems);
#endif
    }

    private void Update () {
        _systems.Run ();
    }

    private void GenerateStartEvents()
    {
        _world.CreateEntityWith<ShowStartConnectionDialogEvent>();
    }

    private void OnDisable () {
        _systems.Dispose ();
        _systems = null;
        _world.Dispose ();
        _world = null;
    }
}

public static class EcsWorldExtensions
{
    public static EcsSystems AddDialogsSystems(this EcsSystems systems)
    {
        return systems
            .Add(new BaseDialogSystem())
            .Add(new StartConnectionDialogSystem())
            .Add(new ConnectToDialogSystem());
    }

    public static EcsSystems AddConnectionSystems(this EcsSystems systems)
    {
        return systems
            .Add(new ConnectedClientSystem())
            .Add(new DisconnectedClientSystem());
    }

    public static EcsSystems AddDebugSystems(this EcsSystems systems)
    {
        return systems
            .Add(new StatusStringSystem());
    }
    
    public static EcsSystems AddNetworkProcessingSystems(this EcsSystems systems)
    {
        return systems;
    }
}