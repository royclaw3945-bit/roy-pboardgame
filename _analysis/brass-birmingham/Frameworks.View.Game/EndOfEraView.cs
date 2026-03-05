using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.EndOfEra;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class EndOfEraView : BaseView, IEndOfEraView, IBaseView
{
	public enum EndOfEraPhase
	{
		ScoringLinks,
		ScoringIndustries,
		RemoveIndustries,
		ScoringMoney,
		ScoringIncome,
		ScoringTwoPlusIndustries,
		Empty
	}

	private Transform _playersContainer;

	private Transform _popup;

	private TextMeshProUGUI _endOfEraText;

	private TextMeshProUGUI _endOfEraHeadlineText;

	private TextMeshProUGUI _endOfEraActionText;

	private TextMeshProUGUI _endOfEraPhaseText;

	private Button _okButton;

	private Button _skipButton;

	private Action _onOkButtonClicked = delegate
	{
	};

	private Action _onSkipButtonClicked = delegate
	{
	};

	private EndOfEraController _controller;

	private Image _background;

	private AudioSource _audioSource;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is EndOfEraController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateEndOfEraView(EndOfEraViewModel endOfEraViewModel)
	{
		List<EndOfEraPlayerViewModel> playersStats = endOfEraViewModel.PlayersStats;
		playersStats.Sort(delegate(EndOfEraPlayerViewModel p1, EndOfEraPlayerViewModel p2)
		{
			if (p1.SumVP < p2.SumVP)
			{
				return 1;
			}
			return (p1.SumVP != p2.SumVP) ? (-1) : 0;
		});
		Dictionary<PlayerColor, PlayerAvatar> playerAvatars = _controller.GetPlayerAvatars();
		Dictionary<PlayerColor, string> playerNames = _controller.GetPlayerNames();
		for (int num = 1; num < _playersContainer.childCount; num++)
		{
			Transform child = _playersContainer.GetChild(num);
			if (num - 1 < playersStats.Count)
			{
				playersStats[num - 1].PlayerAvatar = playerAvatars[playersStats[num - 1].PlayerColor];
				playersStats[num - 1].PlayerName = playerNames[playersStats[num - 1].PlayerColor];
				child.GetComponent<EndOfEraPlayerView>().UpdatePlayer(playersStats[num - 1]);
			}
			else
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetOnOkButtonClicked(Action onOkButtonClicked)
	{
		_onOkButtonClicked = onOkButtonClicked;
	}

	public void SetOnSkipButtonClicked(Action onSkipButtonClicked)
	{
		_onSkipButtonClicked = onSkipButtonClicked;
		_skipButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = _controller.GetLocalizationSkipToResults();
	}

	public void SetSkipButtonInteractable(bool skipButtonInteractable)
	{
		_skipButton.interactable = skipButtonInteractable;
	}

	public void MorphSkipButtonToOkButton()
	{
		_onSkipButtonClicked = _onOkButtonClicked;
		_skipButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = _controller.GetLocalizationOk();
	}

	public void HideScoreWindow()
	{
		_endOfEraText.gameObject.SetActive(value: false);
		_endOfEraActionText.gameObject.SetActive(value: false);
		_endOfEraPhaseText.gameObject.SetActive(value: false);
		_popup.gameObject.SetActive(value: false);
	}

	public void ShowScoreWindow()
	{
		_popup.gameObject.SetActive(value: true);
	}

	public IAnimation ShowEndOfCanalEraTextAnimation(bool isIntroductoryGame)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_audioSource.Play();
			_endOfEraText.text = _controller.GetLocalizationEndOfCanalEra();
			_endOfEraHeadlineText.text = "∙ <I>" + _endOfEraText.text + "</I> ∙";
			_endOfEraText.gameObject.SetActive(value: true);
			_background.gameObject.SetActive(value: true);
			_endOfEraText.rectTransform.localPosition = Vector3.zero;
			_endOfEraText.alpha = 0f;
			_background.color *= new Color(1f, 1f, 1f, 0f);
			_endOfEraActionText.text = _controller.GetLocalizationEndOfCanalEra();
			_endOfEraActionText.gameObject.SetActive(value: true);
			_endOfEraActionText.alpha = 0f;
			_endOfEraPhaseText.gameObject.SetActive(value: true);
			_endOfEraPhaseText.alpha = 0f;
		});
		sequence.Append(_background.DOFade(1f, 0.5f)).Join(_endOfEraText.DOFade(1f, 0.5f));
		sequence.Append(_endOfEraActionText.DOFade(1f, 0.5f));
		sequence.Append(_endOfEraPhaseText.DOFade(1f, 0.5f));
		sequence.AppendInterval(1f);
		sequence.Append(_background.DOFade(0f, 0.5f)).Join(_endOfEraText.DOFade(0f, 0.5f));
		sequence.OnKill(delegate
		{
			_background.color *= new Color(1f, 1f, 1f, 0f);
			_endOfEraText.alpha = 0f;
			_endOfEraActionText.alpha = 1f;
			_endOfEraPhaseText.alpha = 1f;
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation ShowEndOfRailEraTextAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_audioSource.Play();
			_endOfEraText.text = _controller.GetLocalizationEndOfRailroadEra();
			_endOfEraHeadlineText.text = "∙ <I>" + _endOfEraText.text + "</I> ∙";
			_endOfEraText.gameObject.SetActive(value: true);
			_endOfEraText.rectTransform.localPosition = Vector3.zero;
			_endOfEraText.alpha = 0f;
			_background.color *= new Color(1f, 1f, 1f, 0f);
			_endOfEraActionText.text = _endOfEraText.text;
			_endOfEraActionText.gameObject.SetActive(value: true);
			_endOfEraActionText.alpha = 0f;
			_endOfEraPhaseText.gameObject.SetActive(value: true);
			_endOfEraPhaseText.alpha = 0f;
		});
		sequence.Append(_background.DOFade(1f, 0.5f)).Join(_endOfEraText.DOFade(1f, 0.5f));
		sequence.Append(_endOfEraActionText.DOFade(1f, 0.5f));
		sequence.Append(_endOfEraPhaseText.DOFade(1f, 0.5f));
		sequence.Append(_endOfEraText.DOFade(1f, 0.5f));
		sequence.AppendInterval(1f);
		sequence.Append(_background.DOFade(0f, 0.5f)).Join(_endOfEraText.DOFade(0f, 0.5f));
		sequence.OnKill(delegate
		{
			_background.color *= new Color(1f, 1f, 1f, 0f);
			_endOfEraText.alpha = 0f;
			_endOfEraActionText.alpha = 1f;
			_endOfEraPhaseText.alpha = 1f;
			_audioSource.Stop();
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public void SetEndOfEraPhaseText(int phase)
	{
		switch ((EndOfEraPhase)phase)
		{
		case EndOfEraPhase.ScoringIndustries:
			_endOfEraPhaseText.text = _controller.GetLocalizationScoringIndustries();
			break;
		case EndOfEraPhase.ScoringLinks:
			_endOfEraPhaseText.text = _controller.GetLocalizationScoringLinks();
			break;
		case EndOfEraPhase.RemoveIndustries:
			_endOfEraPhaseText.text = _controller.GetLocalizationRemoveIndustries();
			break;
		case EndOfEraPhase.ScoringMoney:
			_endOfEraPhaseText.text = _controller.GetLocalizationScoringMoney() + "\n4 <sprite name=\"BbCommonIconsAtlas_3\"> = 1 <sprite name=\"BbCommonIconsAtlas_0\">";
			break;
		case EndOfEraPhase.ScoringIncome:
			_endOfEraPhaseText.text = _controller.GetLocalizationScoringIncome() + "\n4 <sprite name=\"BbCommonIconsAtlas_2\"> = 1 <sprite name=\"BbCommonIconsAtlas_0\">";
			break;
		case EndOfEraPhase.ScoringTwoPlusIndustries:
			_endOfEraPhaseText.text = _controller.GetLocalizationScoringTwoPlusIndustries();
			break;
		case EndOfEraPhase.Empty:
			_endOfEraPhaseText.text = "";
			break;
		}
	}

	private void OnOkButtonClicked()
	{
		_onOkButtonClicked();
	}

	private void OnSkipButtonClicked()
	{
		_onSkipButtonClicked();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_popup = base.transform.Find("SafeArea/Popup");
		_background = base.transform.Find("SafeArea/Background").GetComponent<Image>();
		_playersContainer = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Players");
		_endOfEraText = base.transform.Find("SafeArea/EndOfEraText").GetComponent<TextMeshProUGUI>();
		_endOfEraHeadlineText = base.transform.Find("SafeArea/Popup/EndOfEraHeadlineText").GetComponent<TextMeshProUGUI>();
		_endOfEraActionText = base.transform.Find("SafeArea/ActionMenu/EndOfEraActionText").GetComponent<TextMeshProUGUI>();
		_endOfEraPhaseText = base.transform.Find("SafeArea/ActionMenu/EndOfEraPhaseText").GetComponent<TextMeshProUGUI>();
		_okButton = base.transform.Find("SafeArea/Popup/OkButton").GetComponent<Button>();
		_skipButton = base.transform.Find("SafeArea/ActionMenu/SkipAction").gameObject.GetComponent<Button>();
		_okButton.onClick.AddListener(OnOkButtonClicked);
		_skipButton.onClick.AddListener(OnSkipButtonClicked);
		_audioSource = GetComponent<AudioSource>();
	}
}
