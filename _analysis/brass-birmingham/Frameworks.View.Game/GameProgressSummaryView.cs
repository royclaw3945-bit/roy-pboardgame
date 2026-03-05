using System;
using BoardGameRules.Entities.GamePhases;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameProgressSummary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class GameProgressSummaryView : BaseView, IGameProgressSummaryView, IBaseView
{
	public Sprite _railEraImage;

	public Sprite _canalEraImage;

	private GameProgressSummaryController _controller;

	private TextMeshProUGUI _turnCountText;

	private TextMeshProUGUI _currentEraText;

	private TextMeshProUGUI _deckStatusText;

	private TextMeshProUGUI _wildIndustryCardCountText;

	private TextMeshProUGUI _wildLocationCardCountText;

	private Transform _gameLogButton;

	private Transform _earningTrackButton;

	private Image _currentEraImage;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is GameProgressSummaryController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateGameProgressSummaryView(GameProgressSummaryViewModel gameProgressSummaryViewModel)
	{
		_turnCountText.text = gameProgressSummaryViewModel.RoundCount + "/" + gameProgressSummaryViewModel.MaxRound;
		_currentEraText.text = gameProgressSummaryViewModel.CurrentEraTranslation;
		_deckStatusText.text = gameProgressSummaryViewModel.DeckCount + "/" + gameProgressSummaryViewModel.DeckSize;
		_wildIndustryCardCountText.text = string.Concat(gameProgressSummaryViewModel.WildIndustryCardCount);
		_wildLocationCardCountText.text = string.Concat(gameProgressSummaryViewModel.WildLocationCardCount);
		if (gameProgressSummaryViewModel.CurrentEra.Equals(GameEra.CanalEra))
		{
			_currentEraImage.sprite = _canalEraImage;
		}
		else
		{
			_currentEraImage.sprite = _railEraImage;
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_turnCountText = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/TurnCountText").GetComponent<TextMeshProUGUI>();
		_currentEraText = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/CurrentEraText").GetComponent<TextMeshProUGUI>();
		_wildIndustryCardCountText = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/WildIndustryCardCountText").GetComponent<TextMeshProUGUI>();
		_wildLocationCardCountText = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/WildLocationCardCountText").GetComponent<TextMeshProUGUI>();
		_deckStatusText = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/DeckStatusText").GetComponent<TextMeshProUGUI>();
		_currentEraImage = base.transform.Find("SafeArea/Popup/EraAndCardsInfo/CurrentEraIcon").GetComponent<Image>();
		_gameLogButton = base.transform.Find("SafeArea/Popup/Buttons/ButtonGameLog");
		_earningTrackButton = base.transform.Find("SafeArea/Popup/Buttons/ButtonEarningTrack");
	}

	public void OnEarningsTrackClicked()
	{
		_controller.OnEarningsTrackClicked();
	}

	public void OnGameLogButtonClicked()
	{
		_controller.OnGameLogClicked();
	}

	public void OnMouseEnterDeck()
	{
		Vector3 position = _deckStatusText.transform.position;
		_controller.OnMouseEnter("Deck", position.x, position.y);
	}

	public void OnMouseEnterWildIndustry()
	{
		Vector3 position = _wildIndustryCardCountText.transform.position;
		_controller.OnMouseEnter("WildIndustry", position.x, position.y);
	}

	public void OnMouseEnterWildLocation()
	{
		Vector3 position = _wildLocationCardCountText.transform.position;
		_controller.OnMouseEnter("WildLocation", position.x, position.y);
	}

	public void OnMouseEnterEarningTrack()
	{
		Vector3 position = _earningTrackButton.position;
		_controller.OnMouseEnter("EarningTrack", position.x, position.y);
	}

	public void OnMouseEnterGameLog()
	{
		Vector3 position = _gameLogButton.position;
		_controller.OnMouseEnter("GameLog", position.x, position.y);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}
}
