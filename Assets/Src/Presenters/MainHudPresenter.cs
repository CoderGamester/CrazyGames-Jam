using GameLovers;
using GameLovers.Services;
using GameLovers.UiService;
using Game.Ids;
using Game.Logic;
using Game.Services;
using TMPro;
using UnityEngine;
using Game.Views;
using UnityEngine.UI;
using System;
using Game.Messages;
using Game.Controllers;

namespace Game.Presenters
{
	/// <summary>
	/// This Presenter handles the Main HUD UI by:
	/// - Showing the HUD visual status
	/// </summary>
	public class MainHudPresenter : UiPresenter
	{
		[SerializeField] private TimerView _timer;
		[SerializeField] private TextMeshProUGUI _version;
		[SerializeField] private TextMeshProUGUI _softCurrencyText;
		[SerializeField] private TextMeshProUGUI _hardCurrencyText;
		[SerializeField] private TextMeshProUGUI _ammotText;
		[SerializeField] private Button _gameOverButton;

		private IGameDataProviderLocator _dataProvider;
		private IGameServices _services;
		private IGameControllerLocator _gameController;

		private void Awake()
		{
			_dataProvider = MainInstaller.Resolve<IGameDataProviderLocator>();
			_services = MainInstaller.Resolve<IGameServices>();
			_gameController = MainInstaller.Resolve<IGameControllerLocator>();

			_timer.Init(_services);
			_gameOverButton.onClick.AddListener(GameOverClicked);
		}

		private void Start()
		{
			_version.text = 
				$"internal = v{VersionServices.VersionInternal}\n" +
				$"external = v{VersionServices.VersionExternal}\n" +
				$"build number = {VersionServices.BuildNumber}";
		}

		protected override void OnOpened()
		{
			_dataProvider.CurrencyDataProvider.Currencies.InvokeObserve(GameId.SoftCurrency, OnSoftCurrencyUpdated);
			_dataProvider.CurrencyDataProvider.Currencies.InvokeObserve(GameId.HardCurrency, OnHardCurrencyUpdated);
			_gameController.GameplayController.Ammo.InvokeObserve(OnAmmoUpdated);
		}

		private void OnAmmoUpdated(int oldValue, int newValue)
		{
			_ammotText.text = $"Ammo: {newValue.ToString()}";
		}

		private void OnSoftCurrencyUpdated(GameId currency, int amountBefore, int amountAfter, ObservableUpdateType updateType)
		{
			_softCurrencyText.text = $"SC: {amountAfter.ToString()}";
		}

		private void OnHardCurrencyUpdated(GameId currency, int amountBefore, int amountAfter, ObservableUpdateType updateType)
		{
			_hardCurrencyText.text = $"HC: {amountAfter.ToString()}";
		}

		private void GameOverClicked()
		{
			_services.MessageBrokerService.Publish(new OnGameOverMessage());
		}
	}
}