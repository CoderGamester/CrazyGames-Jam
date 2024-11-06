using Game.Messages;
using Game.Services;
using Game.ViewControllers;
using GameLovers;
using GameLovers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Controllers
{
	public interface IGameplayController
	{
		IObservableFieldReader<int> Ammo { get; }
	}

	public class GameplayController : IGameplayController, IGameController
	{
		private IGameServices _services;
		private IObservableField<int> _ammo;
		private IObjectPool<PieceViewController> _objectPool;
		private float _nextTick;

		public IObservableFieldReader<int> Ammo => _ammo;

		public GameplayController(IGameServices services)
		{
			_services = services;
		}

		public void Enable()
		{
			_ammo = new ObservableField<int>(50);

			_services.MessageBrokerService.Subscribe<OnPieceHitMessage>(OnPieceHitMessage);
			_services.MessageBrokerService.Subscribe<OnShootMessage>(OnShootMessage);
		}

		public void Disable()
		{
			_services.MessageBrokerService.Unsubscribe<OnPieceHitMessage>(OnPieceHitMessage);
			_services.MessageBrokerService.Unsubscribe<OnShootMessage>(OnShootMessage);

			_ammo = null;
		}

		private void OnPieceHitMessage(OnPieceHitMessage message)
		{
			OnShootMessage(new OnShootMessage());
		}

		private void OnShootMessage(OnShootMessage message)
		{
			_ammo.Value--;

			if (_ammo.Value <= 0)
			{
				_services.MessageBrokerService.Publish(new OnGameOverMessage());
			}
		}
	}
}
