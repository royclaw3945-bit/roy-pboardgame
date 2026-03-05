using System;
using System.Collections.Generic;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Game;
using I2.Loc;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Tutorials.PlayersSummary;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Tutorial;

public class TutorialPlayersSummaryView : PlayersSummaryView, ITutorialPlayersSummaryView, IPlayersSummaryView, IBaseView
{
	private bool _showOnlyYellowPlayer = true;

	private bool _showOnlyYellowPlayerNextValue = true;

	public void HideOtherPlayers()
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			PlayerView component = _playersContainer.GetChild(i).GetComponent<PlayerView>();
			if (!component.IsSet || component.PlayerColor != PlayerColor.Yellow)
			{
				_playersContainer.GetChild(i).gameObject.SetActive(value: false);
			}
		}
	}

	public void ShowAllSetPlayers()
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			_playersContainer.GetChild(i).gameObject.SetActive(_playersContainer.GetChild(i).GetComponent<PlayerView>().IsSet);
		}
	}

	public void ShowAllPlayersAfterNextEndTurnAnimation()
	{
		_showOnlyYellowPlayerNextValue = false;
	}

	private int GetYellowPlayerChildId()
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			if (_playersContainer.GetChild(i).GetComponent<PlayerView>().PlayerColor == PlayerColor.Yellow)
			{
				return i;
			}
		}
		throw new InvalidOperationException("There is no yellow player in player summary!");
	}

	public void GrowYellowPlayerPortrait()
	{
		Transform child = _playersContainer.GetChild(GetYellowPlayerChildId());
		child.GetComponent<LayoutElement>().ignoreLayout = true;
		child.DOScale(2f, 0.3f);
		child.DOLocalMoveY(-108f, 0.3f);
	}

	public void ShrinkYellowPlayerPortrait()
	{
		Transform portrait = _playersContainer.GetChild(GetYellowPlayerChildId());
		Sequence s = DOTween.Sequence();
		s.Append(portrait.DOScale(1f, 0.3f)).Join(portrait.DOLocalMoveY(-53.44f, 0.3f));
		s.AppendInterval(0.3f);
		s.AppendCallback(delegate
		{
			portrait.GetComponent<LayoutElement>().ignoreLayout = false;
		});
	}

	public override void UpdatePlayersSummaryView(List<PlayerViewModel> playerViewModels)
	{
		base.UpdatePlayersSummaryView(playerViewModels);
		ShowAllSetPlayers();
		if (_showOnlyYellowPlayer)
		{
			HideOtherPlayers();
		}
	}

	public override IAnimation CreateHideViewAnimation()
	{
		_isViewVisible = false;
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = false;
		});
		sequence.AppendCallback(delegate
		{
			HideViewCompleted();
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}

	public override IAnimation CreateEndOfRoundAnimation(RoundEnded roundEndedEvent)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			_playersContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
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
			ShowAllSetPlayers();
		});
		sequence.AppendCallback(delegate
		{
			_showOnlyYellowPlayer = _showOnlyYellowPlayerNextValue;
		});
		sequence.AppendCallback(delegate
		{
			RectTransform component = _playersContainer.GetChild(1).GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(-300f, component.sizeDelta.y * -1.5f - _playersContainer.GetComponent<VerticalLayoutGroup>().spacing);
		});
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
			_playersContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
		});
		return new DOTweenAnimationSequence(sequence);
	}
}
