using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameProgressDetails;
using InterfaceAdapters.Routing;
using UnityEngine;

namespace Frameworks.View.Game;

public class GameProgressDetailsView : BaseView, IGameProgressDetailsView, IBaseView
{
	private GameProgressDetailsController _controller;

	private IncomeTrackView _incomeTrackView;

	[SerializeField]
	private GameObject _playerToken;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is GameProgressDetailsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateGameProgressDetailsView(GameProgressDetailsViewModel gameProgressDetailsViewModel)
	{
		_incomeTrackView.ShowCurrentIncomeOfPlayers(gameProgressDetailsViewModel.IncomeLevels, gameProgressDetailsViewModel.IncomePositions, _playerToken, gameProgressDetailsViewModel.CurPlayerColor, gameProgressDetailsViewModel.PlayerAvatars, _controller);
	}

	public void OnBackButtonClicked()
	{
		_controller.OnBackButtonClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackButtonClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_incomeTrackView = base.transform.Find("SafeArea/Popup/IncomeTrackSlot/IncomeTrackPrefab").GetComponent<IncomeTrackView>();
	}
}
