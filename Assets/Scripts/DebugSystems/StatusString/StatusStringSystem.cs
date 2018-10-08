using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;

namespace DebugSystems.StatusString
{
    [EcsInject]
    public class StatusStringSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        
        private EcsFilterSingle<LocalGameConfig> _localConfig;
        private EcsFilterSingle<EcsNetworkConfig> _networkConfig;

        private EcsFilter<SessionComponent> _sessions;
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
                   " Sessions: " + _sessions.EntitiesCount;
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