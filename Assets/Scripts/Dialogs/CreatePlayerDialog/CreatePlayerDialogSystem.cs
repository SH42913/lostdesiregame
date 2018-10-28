using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using Players;

namespace Dialogs.CreatePlayerDialog
{
    [EcsInject]
    public class CreatePlayerDialogSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<DialogComponent, CreatePlayerDialogComponent> _dialogs;

        private EcsFilter<ShowCreatePlayerDialogEvent> _showEvents;
        private EcsFilter<CreatePlayerEvent> _createEvents;
        
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
            
            if(_createEvents.EntitiesCount <= 0) return;
            _createEvents.RemoveAllEntities();

            string nickName = null;
            bool created = false;
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                CreatePlayerDialogComponent dialog = _dialogs.Components2[i];

                if(string.IsNullOrEmpty(dialog.NicknameField.text)) continue;
                nickName = dialog.NicknameField.text;
                created = true;
            }
            
            if(!created) return;

            _ecsWorld.CreateEntityWith<CreatePlayerEvent>().Nickname = nickName;

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _ecsWorld.CreateEntityWith<HideDialogEvent>().DialogEntity = _dialogs.Entities[i];
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components2[i].NicknameField = null;
            }
        }
    }
}