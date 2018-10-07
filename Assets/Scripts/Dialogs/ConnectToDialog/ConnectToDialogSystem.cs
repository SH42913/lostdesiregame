using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Dialogs.ConnectToDialog
{
    [EcsInject]
    public class ConnectToDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<DialogComponent, ConnectToDialogComponent> _dialogs;
        private EcsFilter<ShowConnectToDialogEvent> _showEvents;
        private EcsFilter<ConnectToDialogEvent> _connectEvents;

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            if (_showEvents.EntitiesCount > 0)
            {
                for (int i = 0; i < _dialogs.EntitiesCount; i++)
                {
                    _dialogs.Components1[i].Content.SetActive(true);
                }

                _showEvents.RemoveAllEntities();
            }
            
            if(_connectEvents.EntitiesCount <= 0) return;

            string remoteAddress = null;
            short remotePort = 0;
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                ConnectToDialogComponent dialog = _dialogs.Components2[i];
                if(string.IsNullOrEmpty(dialog.LocalAddress.text) || string.IsNullOrEmpty(dialog.LocalPort.text) || 
                   string.IsNullOrEmpty(dialog.RemoteAddress.text) || string.IsNullOrEmpty(dialog.RemotePort.text)) continue;

                _networkConfig.Data.LocalAddress = dialog.LocalAddress.text;
                _networkConfig.Data.LocalPort = short.Parse(dialog.LocalPort.text);

                remoteAddress = dialog.RemoteAddress.text;
                remotePort = short.Parse(dialog.RemotePort.text);
            }
            _connectEvents.RemoveAllEntities();
            
            if(string.IsNullOrEmpty(remoteAddress)) return;
            _ecsWorld.CreateEntityWith<StartListenerEvent>();

            ConnectToEvent connect;
            _ecsWorld.CreateEntityWith(out connect);
            connect.Address = remoteAddress;
            connect.Port = remotePort;

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _ecsWorld.CreateEntityWith<HideDialogEvent>().DialogEntity = _dialogs.Entities[i];
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                ConnectToDialogComponent component = _dialogs.Components2[i];
                component.LocalAddress = null;
                component.LocalPort = null;
                component.RemoteAddress = null;
                component.RemotePort = null;
            }
        }
    }
}