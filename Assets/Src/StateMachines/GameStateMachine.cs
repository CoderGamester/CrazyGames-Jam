using GameLovers.Services;
using GameLovers.StatechartMachine;
using Game.Logic;
using Game.Services;
using System;
using Assets.Src.StateMachines;

namespace Game.StateMachines
{
	/// <summary>
	/// The State Machine that controls the entire flow of the game
	/// </summary>
	public class GameStateMachine
	{
		private readonly IStatechart _stateMachine;
		private readonly IGameServices _services;
		private readonly IGameUiService _uiService;
		private readonly InitialLoadingState _initialLoadingState;
		private readonly GameplayState _gameplayState;
		private readonly MainMenuState _mainMenuState;

		/// <inheritdoc cref="IStateMachine.LogsEnabled"/>
		public bool LogsEnabled
		{
			get => _stateMachine.LogsEnabled;
			set => _stateMachine.LogsEnabled = value;
		}

		public GameStateMachine(IInstaller installer)
		{
			_services = installer.Resolve<IGameServices>();
			_uiService = installer.Resolve<IGameUiService>();

			_initialLoadingState = new InitialLoadingState(installer);
			_gameplayState = new GameplayState(installer, Trigger);
			_mainMenuState = new MainMenuState(installer, Trigger);
			_stateMachine = new Statechart(Setup);
		}

		/// <inheritdoc cref="IStatechart.LogsEnabled"/>
		public void Run()
		{
			_stateMachine.Run();
		}

		private void Trigger(IStatechartEvent eventTrigger)
		{
			_stateMachine.Trigger(eventTrigger);
		}

		private void Setup(IStateFactory stateFactory)
		{
			var initial = stateFactory.Initial("Initial");
			var final = stateFactory.Final("Final");
			var initialLoading = stateFactory.Nest("Initial Loading");
			var mainMenu = stateFactory.Nest("Main Menu");
			var game = stateFactory.Nest("Game");

			initial.Transition().Target(initialLoading);
			initial.OnExit(SubscribeEvents);
			
			initialLoading.Nest(_initialLoadingState.Setup).Target(mainMenu);

			mainMenu.Nest(_mainMenuState.Setup).Target(game);

			game.Nest(_gameplayState.Setup).Target(mainMenu);
			
			final.OnEnter(UnsubscribeEvents);
		}

		private void SubscribeEvents()
		{
			// Add any events to subscribe
		}

		private void UnsubscribeEvents()
		{
			//
		}
	}
}