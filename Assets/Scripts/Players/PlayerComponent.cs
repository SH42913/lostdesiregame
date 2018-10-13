using Leopotam.Ecs.Net;

namespace Players
{
    [EcsNetComponentUid(2)]
    public class PlayerComponent
    {
        public string Name;
        public long Id;

        public static void NewToOldConverter(PlayerComponent newComponent, PlayerComponent oldComponent)
        {
            oldComponent.Name = newComponent.Name;
            oldComponent.Id = newComponent.Id;
        }
    }
}