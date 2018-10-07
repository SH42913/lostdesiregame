using Leopotam.Ecs;
using UnityEngine.UI;

namespace Dialogs.ConnectToDialog
{
    public class ConnectToDialogBehaviourComponent : AbstractBehaviourComponent
    {
        public InputField LocalAddress;
        public InputField LocalPort;
        public InputField RemoteAddress;
        public InputField RemotePort;

        public Button ConnectButton;

        private void OnEnable()
        {
            ConnectButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<ConnectToDialogEvent>();
            });
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var dialog = world.AddComponent<ConnectToDialogComponent>(entity);
            dialog.LocalAddress = LocalAddress;
            dialog.LocalPort = LocalPort;
            dialog.RemoteAddress = RemoteAddress;
            dialog.RemotePort = RemotePort;
        }
    }
}