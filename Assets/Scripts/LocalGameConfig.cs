using System.Collections.Generic;
using Leopotam.Ecs.Net;

public enum ClientType
{
    SERVER,
    CLIENT
}

public class LocalGameConfig
{
    public ClientType ClientType;
}