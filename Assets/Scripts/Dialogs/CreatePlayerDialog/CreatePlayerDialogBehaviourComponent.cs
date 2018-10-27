using Leopotam.Ecs;
using Players;
using UnityEngine;
using UnityEngine.UI;
using UnityIntegration;

namespace Dialogs.CreatePlayerDialog
{
	public class CreatePlayerDialogBehaviourComponent : AbstractBehaviourComponent
	{
		private const string LastNicknameKey = "LAST_NICKNAME";
		
		public InputField NicknameField;
		public Button CreateButton;

		private void OnEnable()
		{
			Load();
			
			CreateButton.onClick.AddListener(() =>
			{
				EcsWorld.Active.CreateEntityWith<CreatePlayerEvent>();
				Save();
			});
		}

		public override void AttachComponentToEntity(EcsWorld world, int entity)
		{
			world.AddComponent<CreatePlayerDialogComponent>(entity).NicknameField = NicknameField;
		}

		private void Load()
		{
			NicknameField.text = PlayerPrefs.GetString(LastNicknameKey);
		}

		private void Save()
		{   
			PlayerPrefs.SetString(LastNicknameKey, NicknameField.text);
		}
	}
}
