using Game.Messages;
using Game.Services;
using GameLovers.Services;
using GameLovers.UiService;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Src.Presenters
{
	/// <summary>
	/// This Presenter handles the Main Menu UI by:
	/// - Showing the Main Menu button to start the game
	/// - Showing game instructions and objectives about the game for the player to plat
	/// </summary>
	public class MainMenuPresenter : UiPresenter
	{
		[SerializeField] private Button _playButton;

		private IGameServices _services;

		private void Awake()
		{
			_services = MainInstaller.Resolve<IGameServices>();

			_playButton.onClick.AddListener(OnPlayButtonClicked);
		}

		private void OnPlayButtonClicked()
		{
			_services.MessageBrokerService.Publish(new OnPlayClickedMessage());
		}
	}
}
