using Assets.Src.Presenters;
using Game.Controllers;
using Game.Logic;
using Game.Messages;
using Game.Services;
using GameLovers.Services;
using GameLovers.StatechartMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Analytics;

namespace Assets.Src.StateMachines
{
	public class MainMenuState
	{
		private static readonly IStatechartEvent PLAY_CLICKED_EVENT = new StatechartEvent("Play Button Clicked Event");

		private readonly IGameUiService _uiService;
		private readonly IGameServices _services;
		private readonly IGameDataProviderLocator _gameDataProvider;
		private readonly Action<IStatechartEvent> _statechartTrigger;

		public MainMenuState(IInstaller installer, Action<IStatechartEvent> statechartTrigger)
		{
			_uiService = installer.Resolve<IGameUiService>();
			_services = installer.Resolve<IGameServices>();
			_gameDataProvider = installer.Resolve<IGameDataProviderLocator>();
			_statechartTrigger = statechartTrigger;
		}

		public void Setup(IStateFactory stateFactory)
		{
			var initial = stateFactory.Initial("Initial");
			var final = stateFactory.Final("Final");
			var mainScreen = stateFactory.State("Main Screen");

			initial.Transition().Target(mainScreen);
			initial.OnExit(SubscribeEvents);

			mainScreen.OnEnter(OpenMainScreenUi);
			mainScreen.Event(PLAY_CLICKED_EVENT).Target(final);
			mainScreen.OnExit(CloseMainScreenUi);

			final.OnEnter(UnsubscribeEvents);
		}

		private void SubscribeEvents()
		{
			_services.MessageBrokerService.Subscribe<OnPlayClickedMessage>(OnPlayClickedMessage);
		}

		private void UnsubscribeEvents()
		{
			_services.MessageBrokerService.UnsubscribeAll(this);
		}

		private void OpenMainScreenUi()
		{
			_ = _uiService.OpenUiAsync<MainMenuPresenter>();
		}

		private void CloseMainScreenUi()
		{
			_uiService.CloseUi<MainMenuPresenter>();
		}

		private void OnPlayClickedMessage(OnPlayClickedMessage messagage)
		{
			_statechartTrigger(PLAY_CLICKED_EVENT);
		}
	}
}
