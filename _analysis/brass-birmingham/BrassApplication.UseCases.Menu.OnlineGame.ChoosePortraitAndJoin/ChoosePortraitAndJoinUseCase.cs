using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;

public class ChoosePortraitAndJoinUseCase : IChoosePortraitAndJoinInput, IUseCase
{
	private IChoosePortraitAndJoinOutput _presenter;

	private PlayerAvatar _currentAvatar;

	private int _currentAvatarIndex;

	private Action<PlayerAvatar> _joinGameAction;

	private List<PlayerAvatar> _availableAvatars;

	public ChoosePortraitAndJoinUseCase(IChoosePortraitAndJoinOutput presenter)
	{
		_presenter = presenter;
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}

	public void JoinGame()
	{
		_presenter.HideView();
		_joinGameAction(_currentAvatar);
	}

	public void NextPortraitClicked()
	{
		_currentAvatarIndex++;
		if (_currentAvatarIndex >= _availableAvatars.Count)
		{
			_currentAvatarIndex = 0;
		}
		_currentAvatar = _availableAvatars[_currentAvatarIndex];
		_presenter.SetPortrait(_currentAvatar);
	}

	public void PreviousPortraitClicked()
	{
		_currentAvatarIndex--;
		if (_currentAvatarIndex < 0)
		{
			_currentAvatarIndex = _availableAvatars.Count - 1;
		}
		_currentAvatar = _availableAvatars[_currentAvatarIndex];
		_presenter.SetPortrait(_currentAvatar);
	}

	public void SetUp(IEnumerable<PlayerAvatar> availableAvatars, Action<PlayerAvatar> joinGame, PlayerColor color)
	{
		_joinGameAction = joinGame;
		_availableAvatars = availableAvatars.ToList();
		_currentAvatar = _availableAvatars[0];
		_currentAvatarIndex = 0;
		_presenter.SetView(_currentAvatar, color);
	}
}
