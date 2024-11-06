using Game.Messages;
using Game.Services;
using GameLovers.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views
{
	public class BackgroundShootView : MonoBehaviour, IPointerClickHandler
	{
		private IGameServices _gameServices;

		private void Awake()
		{
			_gameServices = MainInstaller.Resolve<IGameServices>();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_gameServices.MessageBrokerService.Publish(new OnShootMessage());
		}
	}
}
