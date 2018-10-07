using Leopotam.Ecs;
using UnityEngine.UI;

namespace Dialogs.ClientTypeDialog
{
    public class ClientTypeDialogBehaviourComponent : AbstractBehaviourComponent
    {
        public Button ServerButton;
        public Button ConnectButton;
        
        private void OnEnable()
        {
            ServerButton.onClick.AddListener(SendClientTypeIsServer);
            ConnectButton.onClick.AddListener(SendClientTypeIsClient);
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            world.AddComponent<ClientTypeDialogComponent>(entity);
        }

        public void SendClientTypeIsServer()
        {
            var world = EcsWorld.Active;
            world.CreateEntityWith<ChangeClientTypeEvent>().ClientType = ClientType.SERVER;
        }

        public void SendClientTypeIsClient()
        {
            var world = EcsWorld.Active;
            world.CreateEntityWith<ChangeClientTypeEvent>().ClientType = ClientType.CLIENT;
        }
    }
}