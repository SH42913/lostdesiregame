﻿using Leopotam.Ecs.Net;

namespace Network.Sessions
{
    [EcsNetComponentUid(1)]
    public class SessionComponent
    {
        public string Address;
        public short Port;

        public static void NewToOldConverter(SessionComponent newSession, SessionComponent oldSession)
        {
            oldSession.Address = newSession.Address;
            oldSession.Port = newSession.Port;
        }
    }
}