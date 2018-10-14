using LeopotamGroup.Pooling;

public enum ClientType
{
    SERVER,
    CLIENT
}

public class LocalGameConfig
{
    public ClientType ClientType;
    public PoolContainer ShipContainer;
}