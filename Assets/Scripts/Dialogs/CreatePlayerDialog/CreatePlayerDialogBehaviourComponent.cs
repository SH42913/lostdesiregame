using Leopotam.Ecs;
using Players;
using UnityEngine.UI;

namespace Dialogs.CreatePlayerDialog
{
	public class CreatePlayerDialogBehaviourComponent : AbstractBehaviourComponent
	{
		public InputField NicknameField;
		public Button CreateButton;

		private void OnEnable()
		{
			CreateButton.onClick.AddListener(() =>
			{
				EcsWorld.Active.CreateEntityWith<CreatePlayerEvent>();
			});
		}

		public override void AttachComponentToEntity(EcsWorld world, int entity)
		{
			world.AddComponent<CreatePlayerDialogComponent>(entity).NicknameField = NicknameField;
		}
	}
}
