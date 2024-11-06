using Game.Services;
using Game.ViewControllers;
using GameLovers.Services;
using System;
using UnityEngine;

namespace Game.Controllers
{
	public interface IPieceSpawnerController { }

	public class PieceSpawnerController : IPieceSpawnerController, IGameController
	{
		private IGameServices _services;
		private PieceViewController _piecePrefab;
		private IObjectPool<PieceViewController> _objectPool;
		private float _nextTick;

		public PieceSpawnerController(IGameServices services)
		{
			_services = services;
		}

		public void Enable()
		{
			if(_piecePrefab == null)
			{
				// TODO: Change this to load from an addressaable instead
				_piecePrefab = GameObject.FindFirstObjectByType<PieceViewController>(FindObjectsInactive.Include);
				_objectPool = new GameObjectPool<PieceViewController>(10, _piecePrefab);
			}

			_services.TickService.SubscribeOnUpdate(OnUpdate);
		}

		public void Disable()
		{
			_objectPool.DespawnAll();
			_services.TickService.UnsubscribeOnUpdate(OnUpdate);
		}

		private void OnUpdate(float deltaTime)
		{
			if (Time.time < _nextTick) return;

			_nextTick = Time.time + Freya.Random.Value;

			_objectPool.Spawn();
		}
	}
}
