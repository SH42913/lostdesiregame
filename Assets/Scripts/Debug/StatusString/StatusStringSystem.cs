using Leopotam.Ecs;
using Leopotam.Ecs.Net;

namespace Debug.StatusString
{
    [EcsInject]
    public class StatusStringSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;
        private EcsFilter<StatusStringComponent> _statusStrings;
        
        public void Initialize()
        {
            
        }

        public void Run()
        {
            for (int i = 0; i < _statusStrings.EntitiesCount; i++)
            {
                _statusStrings.Components1[i].StatusText.text = GetStatusString();
            }
        }

        private string GetStatusString()
        {
            return " LocalAddress: " + _networkConfig.Data.LocalAddress + ":" + _networkConfig.Data.LocalPort +
                   " Type: " + _localConfig.Data.ClientType +
                   " IsRunning: " + _networkConfig.Data.EcsNetworkListener.IsRunning +
                   " ClientsCount: " + _localConfig.Data.ConnectedClients.Count;
        }

        public void Destroy()
        {
            for (int i = 0; i < _statusStrings.EntitiesCount; i++)
            {
                _statusStrings.Components1[i].StatusText = null;
            }
        }
    }
}