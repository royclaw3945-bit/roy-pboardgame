using System;
using System.Collections.Generic;
using Frameworks.View.Game;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameScene;
using InterfaceAdapters.Tutorials.Tutorial;
using UnityEngine;

namespace Frameworks.View.Tutorial;

public class TutorialGameSceneView : GameSceneView, ITutorialGameSceneView, IGameSceneView, IBaseView
{
	private readonly List<TutorialStagePopUpView> _stages = new List<TutorialStagePopUpView>();

	private Action _onClickAction = delegate
	{
	};

	public void MoveToTheNextStage(int stage)
	{
		IAnimation animation = _stages[Mathf.Max(stage - 1, 0)].Close();
		animation.OnComplete(delegate
		{
			_stages[Mathf.Max(stage - 1, 0)].gameObject.SetActive(value: false);
			if (stage < _stages.Count)
			{
				_stages[stage].Open().Play();
			}
		});
		animation.Play();
	}

	public void HideStage(int stage)
	{
		_stages[Mathf.Max(stage - 1, 0)].Close().Play();
	}

	public void SetOnClickAction(Action action)
	{
		_onClickAction = action;
	}

	public void OnClick()
	{
		_onClickAction();
	}

	public void MoveArrowFromStageToPosition(int stage, float? globalPositionX = null, float? globalPositionY = null, float offsetX = 0f, float offsetY = 0f)
	{
		Transform transform = base.transform.Find($"SafeArea/Popup/Stage{stage}/SafeArea/Popup/Arrow");
		if (globalPositionX.HasValue)
		{
			if (globalPositionY.HasValue)
			{
				transform.position = new Vector3(globalPositionX.Value + offsetX * (float)Screen.width / 100f, globalPositionY.Value + offsetY * (float)Screen.height / 100f, transform.position.z);
			}
			else
			{
				transform.position = new Vector3(globalPositionX.Value + offsetX * (float)Screen.width / 100f, transform.position.y, transform.position.z);
			}
		}
		else if (globalPositionY.HasValue)
		{
			transform.position = new Vector3(transform.position.x, globalPositionY.Value + offsetY * (float)Screen.height / 100f, transform.position.z);
		}
	}

	public void HideArrowFromStage(int stage)
	{
		Transform transform = base.transform.Find($"SafeArea/Popup/Stage{stage}/SafeArea/Popup/Arrow");
		if (transform != null)
		{
			transform.gameObject.SetActive(value: false);
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		Transform transform = base.transform.Find("SafeArea/Popup");
		for (int i = 1; i < transform.childCount; i++)
		{
			_stages.Add(transform.GetChild(i).gameObject.GetComponent<TutorialStagePopUpView>());
		}
	}
}
