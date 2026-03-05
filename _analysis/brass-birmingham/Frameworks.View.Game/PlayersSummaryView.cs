using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using I2.Loc;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayersSummary;
using TMPro;
using UnityEngine;

namespace Frameworks.View.Game;

public class PlayersSummaryView : BaseView, IPlayersSummaryView, IBaseView
{
	private PlayersSummaryController _controller;

	protected Transform _playersContainer;

	protected CanvasGroup _backgroundCanvasGroup;

	protected CanvasGroup _textCanvasGroup;

	protected TextMeshProUGUI _endOfEraText;

	protected TextMeshProUGUI _endOfRoundText;

	protected List<PlayerViewModel> _lastPlayerViewModels = new List<PlayerViewModel>();

	public AudioClip EndOfRoundClip;

	public AudioClip IncomeIncreasedClip;

	public AudioClip VictoryPointScoredClip;

	public AudioClip MoneySpentClip;

	public AudioClip PlayerChangedClip;

	private AudioSource _audioSource;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is PlayersSummaryController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public virtual void UpdatePlayersSummaryView(List<PlayerViewModel> playerViewModels)
	{
		_lastPlayerViewModels.Clear();
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			if (playerViewModels.Count > i)
			{
				playerViewModels[i].ActionOnHover = OnMouseEnter;
				playerViewModels[i].ActionOnHoverExit = OnMouseExit;
				_lastPlayerViewModels.Add(playerViewModels[i]);
				_playersContainer.GetChild(i).GetComponent<PlayerView>().SetPlayer(playerViewModels[i]);
				if (playerViewModels[i].IsCurrent)
				{
					CreateShowCurrentPlayerAnimation(playerViewModels[i].Color).Play();
				}
			}
			else
			{
				_playersContainer.GetChild(i).gameObject.SetActive(value: false);
			}
		}
	}

	public IAnimation CreateShowCurrentPlayerAnimation(PlayerColor curPlayerColor)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			_audioSource.PlayOneShot(PlayerChangedClip);
			Sequence s = DOTween.Sequence();
			RectTransform rectTransform = null;
			for (int i = 0; i < _playersContainer.childCount; i++)
			{
				rectTransform = _playersContainer.GetChild(i).GetComponent<RectTransform>();
				s.Insert(0f, rectTransform.DOAnchorPosX(_playersContainer.GetChild(i).GetComponent<PlayerView>().PlayerColor.Equals(curPlayerColor) ? 35 : 0, UIProperties.AnimationDuration));
			}
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public virtual IAnimation CreateEndOfRoundAnimation(RoundEnded roundEndedEvent)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			_audioSource.PlayOneShot(EndOfRoundClip);
		});
		sequence.Append(_backgroundCanvasGroup.DOFade(1f, UIProperties.AnimationDuration).OnComplete(delegate
		{
			_backgroundCanvasGroup.interactable = true;
			_backgroundCanvasGroup.blocksRaycasts = true;
			_endOfEraText.text = ((roundEndedEvent.GameEra == GameEra.CanalEra) ? "<sprite name=\"BbEraIconsAtlas_0\">" : "<sprite name=\"BbEraIconsAtlas_1\">");
			_endOfRoundText.text = LocalizationManager.GetTranslation("UI/END_OF_ROUND").ToUpper();
		}));
		sequence.Append(_textCanvasGroup.DOFade(1f, 5f * UIProperties.FrameDuration));
		sequence.AppendInterval(2f);
		float num = sequence.Duration();
		sequence.Append(_textCanvasGroup.DOFade(0f, 5f * UIProperties.FrameDuration));
		RectTransform rectTransform = null;
		for (int num2 = 0; num2 < _playersContainer.childCount; num2++)
		{
			rectTransform = _playersContainer.GetChild(num2).GetComponent<RectTransform>();
			sequence.Insert(num, CreateHidePlayerAnimation(rectTransform));
			num += (float)(_playersContainer.childCount - num2 - 1) * UIProperties.FrameDuration;
		}
		ReorderViewModels(roundEndedEvent.PlayersOrder);
		sequence.AppendCallback(delegate
		{
			UpdatePlayersWithoutAnimationsView(_lastPlayerViewModels);
		});
		sequence.AppendInterval(10f * UIProperties.FrameDuration);
		num = sequence.Duration();
		for (int num3 = 0; num3 < _playersContainer.childCount; num3++)
		{
			rectTransform = _playersContainer.GetChild(num3).GetComponent<RectTransform>();
			sequence.Insert(num, CreateShowPlayerAnimation(rectTransform)).Join(rectTransform.GetComponent<PlayerView>().CreateChangeMoneySpentAnimation("0", MoneySpentClip));
			num += (float)((num3 == 0) ? 9 : 5) * UIProperties.FrameDuration;
		}
		sequence.OnKill(delegate
		{
			FadeOutAndDisable(_backgroundCanvasGroup);
			FadeOutAndDisable(_textCanvasGroup);
			_audioSource.Stop();
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateShuffleNewPlayersOrderAnimation(RoundStarted roundStartedEvent)
	{
		Sequence sequence = DOTween.Sequence();
		if (_backgroundCanvasGroup.alpha < 1f)
		{
			sequence.Append(_backgroundCanvasGroup.DOFade(1f, UIProperties.AnimationDuration).OnComplete(delegate
			{
				_backgroundCanvasGroup.interactable = true;
				_backgroundCanvasGroup.blocksRaycasts = true;
			}));
		}
		sequence.AppendCallback(delegate
		{
			_endOfEraText.text = ((roundStartedEvent.GameEra == GameEra.CanalEra) ? "<sprite name=\"BbEraIconsAtlas_0\">" : "<sprite name=\"BbEraIconsAtlas_1\">");
			_endOfRoundText.text = LocalizationManager.GetTranslation("UI/ROUND") + " " + roundStartedEvent.RoundNumber + "/" + roundStartedEvent.MaxRounds;
		});
		sequence.Append(_textCanvasGroup.DOFade(1f, 5f * UIProperties.FrameDuration));
		sequence.AppendInterval(2f);
		sequence.OnKill(delegate
		{
			FadeOutAndDisable(_backgroundCanvasGroup);
			FadeOutAndDisable(_textCanvasGroup);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateChangeMoneyAnimation(string newAmount, PlayerColor playerColor)
	{
		return CreateChangeValueAnimation(playerColor, delegate(Sequence sequence, PlayerView view)
		{
			sequence.Append(view.CreateChangeMoneyAnimation(newAmount, MoneySpentClip));
		});
	}

	public IAnimation CreateChangeMoneySpentAnimation(string newAmount, PlayerColor playerColor, string newMoneySpent)
	{
		return CreateChangeValueAnimation(playerColor, delegate(Sequence sequence, PlayerView view)
		{
			sequence.Append(view.CreateChangeMoneyAnimation(newAmount, MoneySpentClip));
			sequence.Join(view.CreateChangeMoneySpentAnimation(newMoneySpent, null));
		});
	}

	public IAnimation CreateChangeVPAnimation(string newAmount, PlayerColor playerColor)
	{
		return CreateChangeValueAnimation(playerColor, delegate(Sequence sequence, PlayerView view)
		{
			sequence.Append(view.CreateChangeVPAnimation(newAmount, VictoryPointScoredClip));
		});
	}

	public IAnimation CreateChangeIncomeAnimation(string newIncomeLevel, string newIncomeMarker, PlayerColor playerColor)
	{
		return CreateChangeValueAnimation(playerColor, delegate(Sequence sequence, PlayerView view)
		{
			sequence.Append(view.CreateChangeIncomeAnimation(newIncomeLevel, IncomeIncreasedClip));
			sequence.Join(view.CreateChangeIncomeMarkerAnimation(newIncomeMarker, null));
		});
	}

	private IAnimation CreateChangeValueAnimation(PlayerColor playerColor, Action<Sequence, PlayerView> actionWhenFound)
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			PlayerView component = _playersContainer.GetChild(i).GetComponent<PlayerView>();
			if (component.PlayerColor.Equals(playerColor))
			{
				actionWhenFound(sequence, component);
			}
		}
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateVictoryPointsForMoneyAnimation(int amount, PlayerColor playerColor)
	{
		Sequence doTweenSequence = DOTween.Sequence();
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			PlayerView component = _playersContainer.GetChild(i).GetComponent<PlayerView>();
			if (component.PlayerColor.Equals(playerColor))
			{
				doTweenSequence = component.CreateVictoryPointsForMoneyAnimation(amount);
				break;
			}
		}
		return new DOTweenAnimationSequence(doTweenSequence);
	}

	public IAnimation CreateVictoryPointsForIncomeAnimation(int amount, PlayerColor playerColor)
	{
		Sequence doTweenSequence = DOTween.Sequence();
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			PlayerView component = _playersContainer.GetChild(i).GetComponent<PlayerView>();
			if (component.PlayerColor.Equals(playerColor))
			{
				doTweenSequence = component.CreateVictoryPointsForIncomeAnimation(amount);
				break;
			}
		}
		return new DOTweenAnimationSequence(doTweenSequence);
	}

	protected Sequence CreateHidePlayerAnimation(RectTransform playerRT)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(playerRT.DOAnchorPosX(35f, 16f * UIProperties.FrameDuration).SetEase(Ease.OutQuart));
		sequence.Append(playerRT.DOAnchorPosX(-300f, 10f * UIProperties.FrameDuration).SetEase(Ease.OutQuart));
		return sequence;
	}

	protected Sequence CreateShowPlayerAnimation(RectTransform playerRT)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(playerRT.DOAnchorPosX(35f, 4f * UIProperties.FrameDuration).SetEase(Ease.OutQuart));
		sequence.Append(playerRT.DOAnchorPosX(0f, 13f * UIProperties.FrameDuration).SetEase(Ease.OutQuart));
		return sequence;
	}

	protected void ReorderViewModels(IEnumerable<PlayerColor> newOrder)
	{
		List<PlayerViewModel> list = new List<PlayerViewModel>();
		foreach (PlayerColor playerColor in newOrder)
		{
			list.Add(_lastPlayerViewModels.First((PlayerViewModel vm) => vm.Color.Equals(playerColor)));
		}
		_lastPlayerViewModels = list;
	}

	protected void UpdatePlayersWithoutAnimationsView(List<PlayerViewModel> playerViewModels)
	{
		for (int i = 0; i < playerViewModels.Count; i++)
		{
			_playersContainer.GetChild(i).GetComponent<PlayerView>().SetPlayer(playerViewModels[i]);
		}
	}

	public void OnMouseEnter(string element, float x, float y)
	{
		_controller.OnMouseEnter(element, x + (float)(Screen.width / 15), y);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_playersContainer = base.transform.Find("SafeArea/Popup/PlayersContainer");
		_backgroundCanvasGroup = base.transform.Find("SafeArea/Popup/Background").GetComponent<CanvasGroup>();
		_textCanvasGroup = base.transform.Find("SafeArea/Popup/Background/Text").GetComponent<CanvasGroup>();
		_endOfEraText = base.transform.Find("SafeArea/Popup/Background/Text/EraText").GetComponent<TextMeshProUGUI>();
		_endOfRoundText = base.transform.Find("SafeArea/Popup/Background/Text/EndOfRoundText").GetComponent<TextMeshProUGUI>();
		_audioSource = GetComponent<AudioSource>();
	}
}
