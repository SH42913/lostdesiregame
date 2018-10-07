using Leopotam.Ecs;
using UnityEngine.UI;

namespace Dialogs.StartServerDialog
{
    public class StartServerDialogBehaviourComponent : AbstractBehaviourComponent
    {
        public InputField Address;
        public InputField Port;
        public Button StartButton;

        private void OnEnable()
        {
            StartButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<StartServerEvent>();
            });
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var dialog = world.AddComponent<StartServerDialogComponent>(entity);
            dialog.Address = Address;
            dialog.Port = Port;
        }
    }
}