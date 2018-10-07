using System.Net;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Dialogs.StartConnectionDialog
{
    [EcsInject]
    public class StartConnectionDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;
        
        private EcsFilter<DialogComponent, StartConnectionDialogComponent> _dialogs;
        
        private EcsFilter<ApplyLocalAddressEvent> _changeEvents;
        private EcsFilter<ShowStartConnectionDialogEvent> _showEvents;

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
            
            if(_changeEvents.EntitiesCount <= 0) return;
            _changeEvents.RemoveAllEntities();

            bool created = false;
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                StartConnectionDialogComponent dialog = _dialogs.Components2[i];
                IPAddress ip;
                short port;
                if(!IPAddress.TryParse(dialog.LocalAddress.text, out ip) ||
                   !short.TryParse(dialog.LocalPort.text, out port)) continue;

                _networkConfig.Data.LocalAddress = dialog.LocalAddress.text;
                _networkConfig.Data.LocalPort = port;
                created = true;
            }
            if(!created) return;

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _ecsWorld.CreateEntityWith<HideDialogEvent>().DialogEntity = _dialogs.Entities[i];
            }
            _ecsWorld.CreateEntityWith<StartListenerEvent>();
        }

        public void Destroy()
        {
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components2[i].LocalAddress = null;
                _dialogs.Components2[i].LocalPort = null;
            }
        }
    }
}