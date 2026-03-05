using System;

namespace BrassApplication.UseCases.Common.LoadScene;

public class LoadSceneUseCase : IUseCase, ILoadSceneInput
{
	private readonly ILoadSceneOutput _presenter;

	private bool _isBoardUpdated;

	private bool _isTutorialReady;

	private const int _tipCount = 25;

	private Random _random;

	public LoadSceneUseCase(ILoadSceneOutput presenter)
	{
		_presenter = presenter;
		_random = new Random();
	}

	public void GenerateGameTip()
	{
		int tipLocalizationId;
		bool wasLastTip;
		if (!_presenter.GetWasLastTipNewGlobal())
		{
			tipLocalizationId = _random.Next(1, 26);
			wasLastTip = true;
		}
		else
		{
			tipLocalizationId = _presenter.GetLastTipIdGlobal();
			wasLastTip = false;
		}
		_presenter.DisplayTip(tipLocalizationId, wasLastTip);
	}

	public void SetIsBoardUpdated(bool isBoardUpdated, bool isTutorial = false)
	{
		_isBoardUpdated = isBoardUpdated;
		if (!isTutorial)
		{
			CheckStandardStatusAndHide();
		}
		else
		{
			CheckTutorialStatusAndHide();
		}
	}

	public void SetIsTutorialReady(bool isTutorialReady)
	{
		_isTutorialReady = isTutorialReady;
		CheckTutorialStatusAndHide();
	}

	private void CheckStandardStatusAndHide()
	{
		if (_isBoardUpdated)
		{
			_presenter.HideView();
		}
	}

	private void CheckTutorialStatusAndHide()
	{
		if (_isBoardUpdated && _isTutorialReady)
		{
			_presenter.HideView();
		}
	}
}
