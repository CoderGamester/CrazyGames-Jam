using System;
using System.Threading.Tasks;
using GameLovers.Services;
using GameLovers.StatechartMachine;
using Game.Ids;
using Game.Services;
using UnityEngine;
using Game.Presenters;
using Game.Messages;
using Game.Commands;
using Game.Logic;
using Game.Controllers;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Game.StateMachines
{
	/// <summary>
	/// This object contains the behaviour logic for the Gameplay State in the <seealso cref="GameStateMachine"/>
	/// </summary>
	public class GameplayState
	{
		public static readonly IStatechartEvent GAME_OVER_EVENT = new StatechartEvent("Game Over Event");
		public static readonly IStatechartEvent GAME_COMPLETE_EVENT = new StatechartEvent("Game Complete Event");

		private static readonly IStatechartEvent RESTART_CLICKED_EVENT = new StatechartEvent("Restart Button Clicked Event");
		private static readonly IStatechartEvent MENU_CLICKED_EVENT = new StatechartEvent("Menu Clicked Event");

		private readonly IGameUiService _uiService;
		private readonly IGameServices _services;
		private readonly IGameDataProviderLocator _gameDataProvider;
		private readonly IGameController _gameController;
		private readonly Action<IStatechartEvent> _statechartTrigger;

		public GameplayState(IInstaller installer, Action<IStatechartEvent> statechartTrigger)
		{
			_uiService = installer.Resolve<IGameUiService>();
			_services = installer.Resolve<IGameServices>();
			_gameDataProvider = installer.Resolve<IGameDataProviderLocator>();
			_gameController = installer.Resolve<IGameController>();
			_statechartTrigger = statechartTrigger;
		}

		public void Setup(IStateFactory stateFactory)
		{
			var initial = stateFactory.Initial("Initial");
			var final = stateFactory.Final("Final");
			var gameplayLoading = stateFactory.TaskWait("Gameplay Loading");
			var gameStateCheck = stateFactory.Choice("GameOver Check");
			var gameplay = stateFactory.State("Gameplay");
			var gameOver = stateFactory.State("GameOver");

			initial.Transition().Target(gameplayLoading);
			initial.OnExit(SubscribeEvents);
			
			gameplayLoading.WaitingFor(LoadGameplayAssets).Target(gameStateCheck);

			gameStateCheck.OnEnter(GameInit);
			gameStateCheck.Transition().Condition(IsGameOver).Target(gameOver);
			gameStateCheck.Transition().Target(gameplay);

			gameplay.OnEnter(OpenGameplayUi);
			gameplay.Event(GAME_OVER_EVENT).Target(gameOver);
			gameplay.Event(GAME_COMPLETE_EVENT).Target(gameOver);
			gameplay.OnExit(CloseGameplayUi);

			gameOver.OnEnter(GameOver);
			gameOver.OnEnter(OpenGameOverUi);
			gameOver.Event(RESTART_CLICKED_EVENT).OnTransition(RestartGame).Target(gameStateCheck);
			gameOver.Event(MENU_CLICKED_EVENT).Target(final);
			gameOver.OnExit(CloseGameOverUi);

			final.OnEnter(UnloadAssets);
			final.OnEnter(UnsubscribeEvents);
		}

		private void SubscribeEvents()
		{
			_services.MessageBrokerService.Subscribe<OnGameOverMessage>(OnGameOverMessage);
			_services.MessageBrokerService.Subscribe<OnGameCompleteMessage>(OnGameCompleteMessage);
			_services.MessageBrokerService.Subscribe<OnGameRestartClickedMessage>(OnGameRestartClickedMessage);
			_services.MessageBrokerService.Subscribe<OnMenuClickedMessage>(OnMenutClickedMessage);
		}

		private void UnsubscribeEvents()
		{
			_services.MessageBrokerService.UnsubscribeAll(this);
		}

		private void OnGameOverMessage(OnGameOverMessage message)
		{
			_statechartTrigger(GAME_OVER_EVENT);
		}

		private void OnGameRestartClickedMessage(OnGameRestartClickedMessage message)
		{
			_statechartTrigger(RESTART_CLICKED_EVENT);
		}

		private void OnMenutClickedMessage(OnMenuClickedMessage message)
		{
			_statechartTrigger(MENU_CLICKED_EVENT);
		}

		private void GameInit()
		{
			_gameController.Enable();
			_services.MessageBrokerService.Publish(new OnGameInitMessage());
		}

		private void RestartGame()
		{
			_services.CommandService.ExecuteCommand(new RestartGameCommand());
		}

		public void OnGameCompleteMessage(OnGameCompleteMessage message)
		{
			_statechartTrigger(GAME_COMPLETE_EVENT);
		}

		private bool IsGameOver()
		{
			return false;
		}

		private void OpenGameplayUi()
		{
			_ = _uiService.OpenUiAsync<MainHudPresenter>();
		}

		private void CloseGameplayUi()
		{
			_uiService.CloseUi<MainHudPresenter>();
		}

		private void OpenGameOverUi()
		{
			_ = _uiService.OpenUiAsync<GameOverScreenPresenter>();
		}

		private void CloseGameOverUi()
		{
			_uiService.CloseUi<GameOverScreenPresenter>();
		}

		private void GameOver()
		{
			_gameController.Disable();
		}

		private async Task LoadGameplayAssets()
		{
			var tasks = new List<Task>
			{
				_uiService.LoadGameUiSet(UiSetId.GameplayUi, 0.8f),
				_services.AssetResolverService.LoadAllAssets<GameId, GameObject>()
			};

			await SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
			await Task.WhenAll(tasks);

			GC.Collect();
			await Resources.UnloadUnusedAssets();
		}

		private void UnloadAssets()
		{
			SceneManager.UnloadSceneAsync("Game");
		}
	}
}