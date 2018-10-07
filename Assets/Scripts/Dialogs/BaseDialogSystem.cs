using Leopotam.Ecs;

namespace Dialogs
{
    [EcsInject]
    public class BaseDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<DialogComponent> _dialogs;
        
        private EcsFilter<CloseAllDialogsEvent> _closeAllEvents;
        private EcsFilter<ShowDialogEvent> _showEvents;
        private EcsFilter<HideDialogEvent> _hideEvents;

        public void Initialize()
        {
            
        }
        
        public void Run()
        {
            for (int i = 0; i < _showEvents.EntitiesCount; i++)
            {
                int dialogEntity = _showEvents.Components1[i].DialogEntity;
                if(!_ecsWorld.IsEntityExists(dialogEntity)) continue;
                
                _ecsWorld.GetComponent<DialogComponent>(dialogEntity)?.Content.SetActive(true);
            }
            _showEvents.RemoveAllEntities();
            
            
            for (int i = 0; i < _hideEvents.EntitiesCount; i++)
            {
                int dialogEntity = _hideEvents.Components1[i].DialogEntity;
                if(!_ecsWorld.IsEntityExists(dialogEntity)) continue;
                
                _ecsWorld.GetComponent<DialogComponent>(dialogEntity)?.Content.SetActive(false);
            }
            _hideEvents.RemoveAllEntities();

            if (_closeAllEvents.EntitiesCount <= 0) return;
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components1[i].Content.SetActive(false);
            }
            _closeAllEvents.RemoveAllEntities();
        }

        public void Destroy()
        {
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components1[i].Content = null;
            }
        }
    }
}