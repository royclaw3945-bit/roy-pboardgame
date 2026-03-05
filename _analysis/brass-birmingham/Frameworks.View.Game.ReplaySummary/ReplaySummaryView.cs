using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Replay;
using DG.Tweening;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game.ReplaySummary;

public class ReplaySummaryView : BaseView, IReplaySummaryView, IBaseView
{
	private Transform _container;

	private ScrollRect _scrollRect;

	private Action _closeWindow;

	[SerializeField]
	private GameObject _roundTextPrefab;

	[SerializeField]
	private GameObject _playerPrefab;

	public override IController Controller { get; set; }

	public void UpdateReplaySummaryView(ActionLogViewModel actionLogViewModel, Action onClickAction)
	{
		_closeWindow = onClickAction;
		foreach (ActionLogRoundViewModel round in actionLogViewModel.Rounds)
		{
			ActionLogRound component = UnityEngine.Object.Instantiate(_roundTextPrefab, _container).GetComponent<ActionLogRound>();
			component.PrepareReferences();
			component.SetTextEra(round.Era);
			component.SetTextRound(round.Turn);
			if (round.Era == "Canal Era")
			{
				component.SetIconEra(Resources.Load<Sprite>("GameLog/BbIconCanalEraFlat"));
			}
			foreach (KeyValuePair<PlayerColor, List<Tuple<ActionLogRoundViewModel.TextEventType, string>>> action in round.Actions)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_playerPrefab, _container);
				List<string> list = new List<string>();
				foreach (Tuple<ActionLogRoundViewModel.TextEventType, string> item in action.Value)
				{
					list.Add(item.Item2);
				}
				gameObject.GetComponent<PlayerActionLogView>().UpdatePlayer(action.Key, round.Avatars[action.Key], list, actionLogViewModel.ActionTranslation);
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(_container.parent.GetComponent<RectTransform>());
		DOTween.Sequence().AppendCallback(delegate
		{
			_scrollRect.verticalNormalizedPosition = 1f;
		}).Play();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_container = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/Panel");
		_scrollRect = base.transform.Find("SafeArea/Popup/Scroll View").GetComponent<ScrollRect>();
	}

	public void OnBackButtonClicked()
	{
		_closeWindow();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackButtonClicked();
		}
	}
}
