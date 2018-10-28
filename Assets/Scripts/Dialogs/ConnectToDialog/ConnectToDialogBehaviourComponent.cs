using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;
using UnityIntegration;

namespace Dialogs.ConnectToDialog
{
    public class ConnectToDialogBehaviourComponent : AbstractBehaviourComponent
    {
        private const string RemoteAddressKey = "REMOTE_ADDRESS";
        private const string RemotePortKey = "REMOTE_PORT";
        
        public InputField RemoteAddress;
        public InputField RemotePort;

        public Button ConnectButton;

        private void OnEnable()
        {
            Load();
            
            ConnectButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<TryToConnectEvent>();
                Save();
            });
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var dialog = world.AddComponent<ConnectToDialogComponent>(entity);
            dialog.RemoteAddress = RemoteAddress;
            dialog.RemotePort = RemotePort;
        }

        private void Load()
        {
            RemoteAddress.text = PlayerPrefs.GetString(RemoteAddressKey);
            RemotePort.text = PlayerPrefs.GetString(RemotePortKey);
        }

        private void Save()
        {   
            PlayerPrefs.SetString(RemoteAddressKey, RemoteAddress.text);
            PlayerPrefs.SetString(RemotePortKey, RemotePort.text);
        }
    }
}