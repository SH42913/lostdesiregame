using Leopotam.Ecs;
using Leopotam.Ecs.Net;
using Network.Sessions;
using Ships.Flight;
using UnityEngine;

namespace InputSystems
{
    [EcsInject]
    public class KeyboardListenerSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<SessionComponent, LocalMarkComponent> _localSessions;
        
        public void Run()
        {
            if(_localSessions.EntitiesCount <= 0) return;
            
            CheckKey(KeyCode.W, EngineDirection.FORWARD);
            CheckKey(KeyCode.S, EngineDirection.BACKWARD);
            CheckKey(KeyCode.A, EngineDirection.STRAFE_LEFT);
            CheckKey(KeyCode.D, EngineDirection.STRAFE_RIGHT);
            CheckKey(KeyCode.Q, EngineDirection.TURN_LEFT);
            CheckKey(KeyCode.E, EngineDirection.TURN_RIGHT);
        }

        private void CheckKey(KeyCode key, EngineDirection direction)
        {
            if (Input.GetKeyDown(key))
            {
                var switchEvent = _ecsWorld.SendEventToNetwork<SwitchEngineEvent>();
                switchEvent.Direction = direction;
                switchEvent.Enable = true;
                switchEvent.SessionId = _localSessions.Components1[0].Id;
            }
            
            if (Input.GetKeyUp(key))
            {
                var switchEvent = _ecsWorld.SendEventToNetwork<SwitchEngineEvent>();
                switchEvent.Direction = direction;
                switchEvent.Enable = false;
                switchEvent.SessionId = _localSessions.Components1[0].Id;
            }
        }
    }
}