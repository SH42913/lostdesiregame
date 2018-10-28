using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;

namespace DebugSystems.StatusString
{
    [EcsInject]
    public class StatusStringSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        
        private LocalGameConfig _localConfig;
        private EcsNetworkConfig _networkConfig;

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
            return " LocalAddress: " + _networkConfig.LocalAddress + ":" + _networkConfig.LocalPort +
                   " Type: " + _localConfig.ClientType +
                   " IsRunning: " + _networkConfig.EcsNetworkListener.IsRunning +
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