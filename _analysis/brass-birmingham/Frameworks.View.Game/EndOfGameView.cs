using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.EndOfGame;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class EndOfGameView : BaseView, IEndOfGameView, IBaseView
{
	private EndOfGameController _controller;

	private Transform _playersContainer;

	private Action _onReviewButtonClicked = delegate
	{
	};

	private Action _onExitGameButtonClicked = delegate
	{
	};

	private Button _reviewButton;

	private Button _exitGameButton;

	private GameObject _canalEraCategory;

	private GameObject _moneyCategory;

	private GameObject _incomeCategory;

	private GameObject _higherLevelIndustriesCategory;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is EndOfGameController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.Return, this);
		base.InputRegistration.RegisterKey(KeyEnum.Space, this);
	}

	public void SetOnReviewButtonClicked(Action onReviewButtonClicked)
	{
		_onReviewButtonClicked = onReviewButtonClicked;
	}

	public void SetOnExitGameButtonClicked(Action onReviewButtonClicked)
	{
		_onExitGameButtonClicked = onReviewButtonClicked;
	}

	public void UpdateEndOfGameView(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place)> endOfGamePlayersScore)
	{
		_moneyCategory.SetActive(value: false);
		_incomeCategory.SetActive(value: false);
		_higherLevelIndustriesCategory.SetActive(value: false);
		_canalEraCategory.SetActive(value: true);
		endOfGamePlayersScore.Sort(delegate((PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place) p1, (PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place) p2)
		{
			if (p1.Rest.Item2 > p2.Rest.Item2)
			{
				return 1;
			}
			return (p1.Rest.Item2 != p2.Rest.Item2) ? (-1) : 0;
		});
		for (int num = 1; num < _playersContainer.childCount; num++)
		{
			Transform child = _playersContainer.GetChild(num);
			if (num - 1 < endOfGamePlayersScore.Count)
			{
				child.GetComponent<EndOfGamePlayerView>().UpdatePlayer(endOfGamePlayersScore[num - 1].color, endOfGamePlayersScore[num - 1].avatar, endOfGamePlayersScore[num - 1].playerName, endOfGamePlayersScore[num - 1].vpFromCanalEra, endOfGamePlayersScore[num - 1].vpFromLinksInLastEra, endOfGamePlayersScore[num - 1].vpFromIndustriesInLastEra, endOfGamePlayersScore[num - 1].vpFromBonusesInLastEra, endOfGamePlayersScore[num - 1].Rest.Item1);
			}
			else
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}

	public void UpdateEndOfIntroductoryGameView(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)> endOfGamePlayersScore)
	{
		_moneyCategory.SetActive(value: true);
		_incomeCategory.SetActive(value: true);
		_higherLevelIndustriesCategory.SetActive(value: true);
		_canalEraCategory.SetActive(value: false);
		endOfGamePlayersScore.Sort(delegate((PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries) p1, (PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries) p2)
		{
			if (p1.Rest.Item1 < p2.Rest.Item1)
			{
				return 1;
			}
			return (p1.Rest.Item1 != p2.Rest.Item1) ? (-1) : 0;
		});
		for (int num = 1; num < _playersContainer.childCount; num++)
		{
			Transform child = _playersContainer.GetChild(num);
			if (num - 1 < endOfGamePlayersScore.Count)
			{
				child.GetComponent<EndOfGamePlayerView>().UpdatePlayerIntroductory(endOfGamePlayersScore[num - 1].color, endOfGamePlayersScore[num - 1].avatar, endOfGamePlayersScore[num - 1].playerName, endOfGamePlayersScore[num - 1].vpFromLinksInLastEra, endOfGamePlayersScore[num - 1].vpFromIndustriesInLastEra, endOfGamePlayersScore[num - 1].vpFromBonusesInLastEra, endOfGamePlayersScore[num - 1].totalVP, endOfGamePlayersScore[num - 1].Rest.Item2, endOfGamePlayersScore[num - 1].Rest.Item3, endOfGamePlayersScore[num - 1].Rest.Item4);
			}
			else
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}

	private void OnReviewButtonClicked()
	{
		_onReviewButtonClicked();
	}

	private void OnExitGameButtonClicked()
	{
		_onExitGameButtonClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Return || keyEnum == KeyEnum.Escape || keyEnum == KeyEnum.Space)
		{
			OnReviewButtonClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_playersContainer = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Players");
		_canalEraCategory = _playersContainer.Find("Categories/VerticalLayout/CanalEra").gameObject;
		_moneyCategory = _playersContainer.Find("Categories/VerticalLayout/Money").gameObject;
		_incomeCategory = _playersContainer.Find("Categories/VerticalLayout/Income").gameObject;
		_higherLevelIndustriesCategory = _playersContainer.Find("Categories/VerticalLayout/HigherIndustries").gameObject;
		_reviewButton = base.transform.Find("SafeArea/Popup/ReviewButton").GetComponent<Button>();
		_reviewButton.onClick.AddListener(OnReviewButtonClicked);
		_exitGameButton = base.transform.Find("SafeArea/Popup/ExitGameButton").GetComponent<Button>();
		_exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
	}
}
