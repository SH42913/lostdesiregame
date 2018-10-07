using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Dialogs.StartServerDialog
{
    [EcsInject]
    public class StartServerDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;
        private EcsFilter<DialogComponent, StartServerDialogComponent> _dialogs;
        private EcsFilter<StartServerEvent> _startEvents;

        private EcsFilter<ShowStartServerDialogEvent> _showEvents;

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
            
            if(_startEvents.EntitiesCount <= 0) return;

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                StartServerDialogComponent dialog = _dialogs.Components2[i];
                if(string.IsNullOrEmpty(dialog.Address.text) || string.IsNullOrEmpty(dialog.Port.text)) continue;
                
                _networkConfig.Data.LocalAddress = dialog.Address.text;
                _networkConfig.Data.LocalPort = short.Parse(dialog.Port.text);
            }
            _startEvents.RemoveAllEntities();
            
            if(string.IsNullOrEmpty(_networkConfig.Data.LocalAddress)) return;
            _ecsWorld.CreateEntityWith<StartListenerEvent>();

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _ecsWorld.CreateEntityWith<HideDialogEvent>().DialogEntity = _dialogs.Entities[i];
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components1[i].Content = null;
                _dialogs.Components2[i].Address = null;
                _dialogs.Components2[i].Port = null;
            }
        }
    }
}