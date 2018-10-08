using System.Net;
using Dialogs.CreatePlayerDialog;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;

namespace Dialogs.ConnectToDialog
{
    [EcsInject]
    public class ConnectToDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<DialogComponent, ConnectToDialogComponent> _dialogs;
        private EcsFilter<ShowConnectToDialogEvent> _showEvents;
        private EcsFilter<TryToConnectEvent> _connectEvents;

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            if (_showEvents.EntitiesCount > 0)
            {
                _showEvents.RemoveAllEntities();
                
                for (int i = 0; i < _dialogs.EntitiesCount; i++)
                {
                    _dialogs.Components1[i].Content.SetActive(true);
                }
            }
            
            if(_connectEvents.EntitiesCount <= 0) return;
            _connectEvents.RemoveAllEntities();

            string remoteAddress = null;
            short remotePort = 0;
            bool created = false;
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                ConnectToDialogComponent dialog = _dialogs.Components2[i];
                
                IPAddress ip;
                short port;
                if(!IPAddress.TryParse(dialog.RemoteAddress.text, out ip) ||
                   !short.TryParse(dialog.RemotePort.text, out port)) continue;

                remoteAddress = dialog.RemoteAddress.text;
                remotePort = short.Parse(dialog.RemotePort.text);
                created = true;
            }
            
            if(!created) return;

            _localConfig.Data.ClientType = ClientType.CLIENT;

            ConnectToEvent connect;
            _ecsWorld.CreateEntityWith(out connect);
            connect.Address = remoteAddress;
            connect.Port = remotePort;

            _ecsWorld.CreateEntityWith<CreateLocalSessionEvent>();
            _ecsWorld.CreateEntityWith<ShowCreatePlayerDialogEvent>();

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
                component.RemoteAddress = null;
                component.RemotePort = null;
            }
        }
    }
}