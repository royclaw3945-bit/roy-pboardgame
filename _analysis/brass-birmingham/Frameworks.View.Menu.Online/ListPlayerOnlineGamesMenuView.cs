using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Online;
using InterfaceAdapters.Menu.OnlineGameMenu;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class ListPlayerOnlineGamesMenuView : BaseView, IListPlayerOnlineGamesMenuView, IBaseView
{
	private IGamesListView _gamesListView;

	private IGamesListView _joinableGamesListView;

	private Transform _filtersContainer;

	private ListPlayerOnlineGamesMenuController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ListPlayerOnlineGamesMenuController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateFilters()
	{
		_gamesListView.RemoveAllGames();
		_joinableGamesListView.RemoveAllGames();
		(bool, bool, bool, bool) tuple = (false, false, false, false);
		for (int i = 0; i < _filtersContainer.childCount; i++)
		{
			Toggle component = _filtersContainer.GetChild(i).GetChild(0).GetComponent<Toggle>();
			switch (i)
			{
			case 0:
				tuple.Item1 = component.isOn;
				break;
			case 1:
				tuple.Item2 = component.isOn;
				break;
			case 2:
				tuple.Item3 = component.isOn;
				break;
			case 3:
				tuple.Item4 = component.isOn;
				break;
			}
		}
		_controller.UpdateFilters(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
	}

	public void UpdateFiltersVisuals(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		for (int i = 0; i < _filtersContainer.childCount; i++)
		{
			Toggle component = _filtersContainer.GetChild(i).GetChild(0).GetComponent<Toggle>();
			switch (i)
			{
			case 0:
				component.isOn = show2Players;
				break;
			case 1:
				component.isOn = show3Players;
				break;
			case 2:
				component.isOn = show4Players;
				break;
			case 3:
				component.isOn = showPrivate;
				break;
			}
		}
	}

	public void UpdatePlayerGamesView(ListPlayerOnlineGamesMenuViewModel viewModel)
	{
		_gamesListView.UpdatePlayerGamesList(viewModel.PlayerGamesList);
	}

	public void UpdateOpenGamesView(ListPlayerOnlineGamesMenuViewModel viewModel)
	{
		_joinableGamesListView.UpdateOpenGamesList(viewModel.OpenGamesList);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_gamesListView = base.transform.Find("SafeArea/Popup/OngoingGamesContainer/YourGamesContainer").GetComponent<IGamesListView>();
		_joinableGamesListView = base.transform.Find("SafeArea/Popup/OngoingGamesContainer/OpenGamesContainer").GetComponent<IGamesListView>();
		_filtersContainer = base.transform.Find("SafeArea/Popup/Filters/FiltersContainer");
	}

	public void OnLogoutClicked()
	{
		_controller.LogoutClicked();
	}

	public void OnCreateGameClicked()
	{
		_controller.CrateGameClicked();
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackClicked();
		}
	}
}
