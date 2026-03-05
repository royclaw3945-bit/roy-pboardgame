using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Replay;
using DG.Tweening;
using Frameworks.View.Common;
using Frameworks.View.Game.ReplaySummary;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameLogView;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class GameLogView : BaseView, IGameLogView, IBaseView
{
	private GameLogController _controller;

	private Transform _container;

	private GameObject _logEmptyTextMesh;

	private ScrollRect _scrollRect;

	[SerializeField]
	private GameObject _roundTextPrefab;

	[SerializeField]
	private GameObject _playerPrefab;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is GameLogController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateGameLogView(ActionLogViewModel actionLogViewModel)
	{
		if (actionLogViewModel.Rounds.Count != 0)
		{
			_logEmptyTextMesh.SetActive(value: false);
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
					List<string> list = new List<string>();
					foreach (Tuple<ActionLogRoundViewModel.TextEventType, string> item in action.Value)
					{
						list.Add(item.Item2);
					}
					UnityEngine.Object.Instantiate(_playerPrefab, _container).GetComponent<PlayerActionLogView>().UpdatePlayer(action.Key, round.Avatars[action.Key], list, actionLogViewModel.ActionTranslation);
				}
			}
		}
		else
		{
			_logEmptyTextMesh.SetActive(value: true);
		}
		Sequence s = DOTween.Sequence();
		s.AppendInterval(0.05f);
		s.AppendCallback(delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_container.parent.GetComponent<RectTransform>());
		});
		s.Append(DOTween.To(() => _scrollRect.verticalNormalizedPosition, delegate(float x)
		{
			_scrollRect.verticalNormalizedPosition = x;
		}, 0f, 0.2f)).Play();
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
		_container = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/Panel");
		_logEmptyTextMesh = _container.Find("LogEmptyText").gameObject;
		_scrollRect = base.transform.Find("SafeArea/Popup/Scroll View").GetComponent<ScrollRect>();
	}
}
