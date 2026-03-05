using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.LoanAction;
using InterfaceAdapters.Game.IncomeTrack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class IncomeTrackView : MonoBehaviour
{
	private IIncomeTrackController _controller;

	private Transform _incomeTrack;

	private Transform _scrollContent;

	private ScrollRect _scrollRect;

	private Scrollbar _scrollbar;

	private Transform _arrowToTarget;

	private Transform _currentPlayerIncomeToken;

	private Transform _targetPlayerIncomeToken;

	private void Awake()
	{
		PrepareReferences();
	}

	public void ShowCurrentIncomeOfPlayers(Dictionary<PlayerColor, int> IncomeLevels, Dictionary<PlayerColor, int> IncomePositions, GameObject _playerIncomeTokenPrefab, PlayerColor _curPlayerColor, Dictionary<PlayerColor, PlayerAvatar> PlayerAvatars, IIncomeTrackController controller)
	{
		_controller = controller;
		InjectTooltipFunctions();
		foreach (KeyValuePair<PlayerColor, int> IncomeLevel in IncomeLevels)
		{
			AddTokenToTrack(IncomeLevel.Value, IncomePositions[IncomeLevel.Key], IncomeLevel.Key, PlayerAvatars[IncomeLevel.Key], _playerIncomeTokenPrefab);
			if (IncomeLevel.Key == _curPlayerColor)
			{
				ScrollToPlayerToken(1046.5f, -516f, IncomeLevel.Value, 22f);
			}
		}
	}

	public void ShowLoanView(LoanActionViewModel loanActionViewModel, GameObject _playerIncomeTokenPrefab, GameObject _targetPlayerIncomeTokenPrefab, GameObject arrowToTargetPrefab, IIncomeTrackController controller)
	{
		_controller = controller;
		foreach (KeyValuePair<PlayerColor, int> incomeLevel in loanActionViewModel.IncomeLevels)
		{
			Transform transform = AddTokenToTrack(incomeLevel.Value, loanActionViewModel.IncomePositions[incomeLevel.Key], incomeLevel.Key, loanActionViewModel.PlayerAvatars[incomeLevel.Key], _playerIncomeTokenPrefab);
			if (incomeLevel.Key == loanActionViewModel.PlayerColor)
			{
				_currentPlayerIncomeToken = transform;
				continue;
			}
			transform.GetComponent<Image>().color *= (Color)new Vector4(1f, 1f, 1f, 0.3f);
			transform.GetChild(0).GetComponent<Image>().color *= (Color)new Vector4(1f, 1f, 1f, 0.3f);
		}
		_targetPlayerIncomeToken = AddTokenToTrack(loanActionViewModel.TargetIncomeLevel, loanActionViewModel.TargetIncomePosition, loanActionViewModel.PlayerColor, loanActionViewModel.PlayerAvatar, _targetPlayerIncomeTokenPrefab);
		_targetPlayerIncomeToken.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		_arrowToTarget = Object.Instantiate(arrowToTargetPrefab, new Vector3(_targetPlayerIncomeToken.position.x, _targetPlayerIncomeToken.position.y + 200f, 0f), Quaternion.Euler(0f, 0f, 90f), _currentPlayerIncomeToken).transform;
		ScrollToPlayerToken(1294.7f, -391.9f, loanActionViewModel.CurrentIncomeLevel, 26f);
		InjectTooltipFunctions();
		ShowLoanViewAnimation();
	}

	private void ScrollToPlayerToken(float contentPositionYDifference, float contentPositionYMax, int level, float middleFieldCountDifference)
	{
		_scrollContent.localPosition = new Vector3(_scrollContent.localPosition.x, contentPositionYMax - Mathf.Clamp((float)level / middleFieldCountDifference, 0f, 1f) * contentPositionYDifference, _scrollContent.localPosition.z);
	}

	private Transform AddTokenToTrack(int level, int position, PlayerColor playerColor, PlayerAvatar avatar, GameObject prefab)
	{
		GameObject gameObject = null;
		int num = _incomeTrack.childCount - level - 11;
		Transform child = _incomeTrack.GetChild(num);
		int num2 = 2;
		if (num >= 31)
		{
			num2 = 1;
		}
		for (int i = num2; i < child.childCount; i++)
		{
			Transform child2 = child.GetChild(i);
			if (child2.Find("Number").GetComponent<TextMeshProUGUI>().text.Equals(position.ToString()))
			{
				gameObject = Object.Instantiate(prefab, child2);
				gameObject.transform.localPosition = child2.Find("Number").transform.localPosition;
				if (gameObject.transform.childCount > 0)
				{
					gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + avatar);
					gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + playerColor);
					PlayerTokenView component = gameObject.GetComponent<PlayerTokenView>();
					component._onPointerEnter = ShowTooltipPlayerToken;
					component._onPointerExit = _controller.OnMouseExit;
					component.playerColor = playerColor;
				}
			}
			StackPlayerTokens(child2);
		}
		return gameObject.transform;
	}

	private void StackPlayerTokens(Transform field)
	{
		switch (field.childCount)
		{
		case 2:
			field.GetChild(1).localPosition = new Vector3(0f, 0f, 0f);
			break;
		case 3:
			field.GetChild(1).localPosition = new Vector3(0f, 3f, 0f);
			field.GetChild(2).localPosition = new Vector3(0f, -3f, 0f);
			break;
		case 4:
			field.GetChild(1).localPosition = new Vector3(0f, 6f, 0f);
			field.GetChild(2).localPosition = new Vector3(0f, 0f, 0f);
			field.GetChild(3).localPosition = new Vector3(0f, -6f, 0f);
			break;
		case 5:
			field.GetChild(1).localPosition = new Vector3(0f, 9f, 0f);
			field.GetChild(2).localPosition = new Vector3(0f, 3f, 0f);
			field.GetChild(3).localPosition = new Vector3(0f, -3f, 0f);
			field.GetChild(4).localPosition = new Vector3(0f, -9f, 0f);
			break;
		}
	}

	private void ShowLoanViewAnimation()
	{
		Sequence s = DOTween.Sequence();
		s.AppendInterval(10f * UIProperties.FrameDuration);
		s.Append(_arrowToTarget.DOLocalMoveY(-60f, 10f * UIProperties.FrameDuration));
		s.Insert(15f * UIProperties.FrameDuration, _targetPlayerIncomeToken.DOScale(1.5f, 6f * UIProperties.FrameDuration).SetEase(Ease.InCubic));
		s.Append(_targetPlayerIncomeToken.DOScale(1f, 9f * UIProperties.FrameDuration).SetEase(Ease.OutCubic));
	}

	protected void PrepareReferences()
	{
		_incomeTrack = base.transform.Find("Scroll View/Viewport/Content/Track");
		_scrollContent = base.transform.Find("Scroll View/Viewport/Content");
		_scrollbar = base.transform.Find("Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();
		_scrollRect = base.transform.Find("Scroll View").GetComponent<ScrollRect>();
	}

	public IAnimation CreateLoanActionAnimation(LoanActionController controller, LoanActionViewModel loanActionViewModel, Button buttonTakeLoan, float delay)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			ScrollToPlayerToken(1294.7f, -391.9f, loanActionViewModel.CurrentIncomeLevel, 26f);
			_scrollbar.enabled = false;
			_scrollRect.enabled = false;
			buttonTakeLoan.enabled = false;
		});
		sequence.AppendInterval(delay);
		sequence.AppendCallback(delegate
		{
			if (_targetPlayerIncomeToken == null || _arrowToTarget == null)
			{
				Debug.LogError("Missing references to key transforms in Loan Animation");
			}
			else
			{
				_arrowToTarget.SetParent(base.transform);
			}
		});
		sequence.Append(_targetPlayerIncomeToken.GetComponent<Image>().DOFade(0f, 5f * UIProperties.FrameDuration)).Join(_currentPlayerIncomeToken.DOScale(1.34f, 4f * UIProperties.FrameDuration));
		sequence.Insert(delay + 5f * UIProperties.FrameDuration, _currentPlayerIncomeToken.DOScale(1f, 8f * UIProperties.FrameDuration)).Join(_currentPlayerIncomeToken.GetComponent<Image>().DOFade(0f, 8f * UIProperties.FrameDuration));
		sequence.AppendCallback(delegate
		{
			_currentPlayerIncomeToken.position = _targetPlayerIncomeToken.position;
			_currentPlayerIncomeToken.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		});
		sequence.Append(_currentPlayerIncomeToken.DOScale(1.2f, 6f * UIProperties.FrameDuration)).Join(_currentPlayerIncomeToken.GetComponent<Image>().DOFade(1f, 4f * UIProperties.FrameDuration));
		sequence.Insert(delay + 22f * UIProperties.FrameDuration, _currentPlayerIncomeToken.DOScale(1f, 9f * UIProperties.FrameDuration));
		sequence.AppendInterval(15f * UIProperties.FrameDuration);
		sequence.AppendCallback(delegate
		{
			controller.HideIncomePanel();
		});
		return new DOTweenAnimationSequence(sequence);
	}

	private void InjectTooltipFunctions()
	{
		for (int i = 0; i < _incomeTrack.childCount; i++)
		{
			int num = ((i < 31) ? 1 : 0);
			IncomeTrackMoneyIncome component = _incomeTrack.GetChild(i).GetChild(num).GetComponent<IncomeTrackMoneyIncome>();
			component.ActionOnHover = ShowTooltipMoneyIncome;
			component.ActionOnHoverExit = _controller.OnMouseExit;
			for (int j = 1 + num; j < _incomeTrack.GetChild(i).childCount; j++)
			{
				IncomeTrackMoneyTrackField component2 = _incomeTrack.GetChild(i).GetChild(j).GetChild(0)
					.GetComponent<IncomeTrackMoneyTrackField>();
				component2.ActionOnHover = ShowTooltipMoneyTrackField;
				component2.ActionOnHoverExit = _controller.OnMouseExit;
			}
		}
	}

	private void ShowTooltipMoneyIncome()
	{
		_controller.OnMouseEnter("Income", Input.mousePosition.x, Input.mousePosition.y);
	}

	private void ShowTooltipMoneyTrackField()
	{
		_controller.OnMouseEnter("IncomePoints", Input.mousePosition.x, Input.mousePosition.y);
	}

	private void ShowTooltipPlayerToken(PlayerColor playerColor, Vector3 tokenPosition)
	{
		_controller.OnMouseEnter("Token", tokenPosition.x, tokenPosition.y, playerColor);
	}
}
