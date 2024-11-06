using Game.Services;
using System.Collections.Generic;

namespace Game.Controllers
{
	public interface IGameControllerLocator
	{
		public IGameplayController GameplayController { get; }
		public IPieceSpawnerController PieceSpawnerController { get; }
		bool IsEnabled { get; }
	}

	public class GameControllerLocator : IGameControllerLocator, IGameController
	{
		private readonly List<IGameController> _controllers = new List<IGameController>();

		public IGameplayController GameplayController { get; }
		public IPieceSpawnerController PieceSpawnerController { get; }
		public bool IsEnabled { get; private set; }

		public GameControllerLocator(IGameServices services)
		{
			var gameplayController = new GameplayController(services);
			var pieceSpawnerController = new PieceSpawnerController(services);

			GameplayController = gameplayController;
			PieceSpawnerController = pieceSpawnerController;

			_controllers.Add(gameplayController);
			_controllers.Add(pieceSpawnerController);
		}

		public void Enable()
		{
			foreach (var controller in _controllers)
			{
				controller.Enable();
			}
			IsEnabled = true;
		}

		public void Disable()
		{
			IsEnabled = false;
			foreach (var controller in _controllers)
			{
				controller.Disable();
			}
		}

		public IGameController GetController<T>() where T : IGameController
		{
			throw new System.NotImplementedException();
		}
	}
}
