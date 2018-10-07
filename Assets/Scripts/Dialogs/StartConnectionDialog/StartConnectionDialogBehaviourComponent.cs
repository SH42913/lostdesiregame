using Dialogs.ConnectToDialog;
using Leopotam.Ecs;
using UnityEngine.UI;
using World;

namespace Dialogs.StartConnectionDialog
{
    public class StartConnectionDialogBehaviourComponent : AbstractBehaviourComponent
    {
        public InputField LocalAddress;
        public InputField LocalPort;
        public Button ServerButton;
        public Button ConnectButton;
        
        private void OnEnable()
        {
            ServerButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<ApplyLocalAddressEvent>();
                EcsWorld.Active.CreateEntityWith<CreateWorldEvent>();
            });
            ConnectButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<ApplyLocalAddressEvent>();
                EcsWorld.Active.CreateEntityWith<ShowConnectToDialogEvent>();
            });
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var dialog = world.AddComponent<StartConnectionDialogComponent>(entity);
            dialog.LocalAddress = LocalAddress;
            dialog.LocalPort = LocalPort;
        }
    }
}