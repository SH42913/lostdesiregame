using Dialogs.ConnectToDialog;
using Dialogs.CreatePlayerDialog;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;
using UnityIntegration;
using World;

namespace Dialogs.StartConnectionDialog
{
    public class StartConnectionDialogBehaviourComponent : AbstractBehaviourComponent
    {
        private const string LocalAddressKey = "LOCAL_ADDRESS";
        private const string LocalPortKey = "LOCAL_PORT";
        
        public InputField LocalAddress;
        public InputField LocalPort;
        public Button ServerButton;
        public Button ConnectButton;
        
        private void OnEnable()
        {
            Load();
            
            ServerButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<ApplyLocalAddressEvent>();
                EcsWorld.Active.CreateEntityWith<CreateWorldEvent>();
                Save();
            });
            ConnectButton.onClick.AddListener(() =>
            {
                EcsWorld.Active.CreateEntityWith<ApplyLocalAddressEvent>();
                EcsWorld.Active.CreateEntityWith<ShowConnectToDialogEvent>();
                Save();
            });
        }

        public override void AttachComponentToEntity(EcsWorld world, int entity)
        {
            var dialog = world.AddComponent<StartConnectionDialogComponent>(entity);
            dialog.LocalAddress = LocalAddress;
            dialog.LocalPort = LocalPort;
        }

        private void Load()
        {
            LocalAddress.text = PlayerPrefs.GetString(LocalAddressKey);
            LocalPort.text = PlayerPrefs.GetString(LocalPortKey);
        }

        private void Save()
        {   
            PlayerPrefs.SetString(LocalAddressKey, LocalAddress.text);
            PlayerPrefs.SetString(LocalPortKey, LocalPort.text);
        }
    }
}