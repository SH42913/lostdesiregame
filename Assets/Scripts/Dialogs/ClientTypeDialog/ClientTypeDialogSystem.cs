using System;
using Dialogs.ConnectToDialog;
using Dialogs.StartServerDialog;
using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Dialogs.ClientTypeDialog
{
    [EcsInject]
    public class ClientTypeDialogSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilter<DialogComponent, ClientTypeDialogComponent> _dialogs;
        private EcsFilter<ChangeClientTypeEvent> _changeEvents;

        private EcsFilter<ShowClientTypeDialogEvent> _showEvents;

        public void Run()
        {
            for (int eventIndex = 0; eventIndex < _changeEvents.EntitiesCount; eventIndex++)
            {
                _localConfig.Data.ClientType = _changeEvents.Components1[eventIndex].ClientType;
                for (int dialogIndex = 0; dialogIndex < _dialogs.EntitiesCount; dialogIndex++)
                {
                    _ecsWorld.CreateEntityWith<HideDialogEvent>().DialogEntity = _dialogs.Entities[dialogIndex];
                }

                switch (_localConfig.Data.ClientType)
                {
                    case ClientType.SERVER:
                        _ecsWorld.CreateEntityWith<ShowStartServerDialogEvent>();
                        break;
                    case ClientType.CLIENT:
                        _ecsWorld.CreateEntityWith<ShowConnectToDialogEvent>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _changeEvents.RemoveAllEntities();

            if(_showEvents.EntitiesCount <= 0) return;

            for (int i = 0; i < _dialogs.EntitiesCount; i++)
            {
                _dialogs.Components1[i].Content.SetActive(true);
            }
            _showEvents.RemoveAllEntities();
        }
    }
}