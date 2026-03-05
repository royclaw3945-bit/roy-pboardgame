using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.ChoosePortraitAndColor;

public class ChoosePortraitAndColorUseCase : IChoosePortraitAndColorInput, IUseCase
{
	private IChoosePortraitAndColorOutput _presenter;

	private PlayerColor? _startingColor;

	private PlayerAvatar? _startingAvatar;

	public ChoosePortraitAndColorUseCase(IChoosePortraitAndColorOutput presenter)
	{
		_presenter = presenter;
	}

	public void BackClicked()
	{
		_startingColor = null;
		_startingAvatar = null;
		_presenter.HideView();
	}

	public void NextColorClicked(PlayerColor color, bool isOnlineGame = false)
	{
		if (!_startingColor.HasValue)
		{
			_startingColor = color;
		}
		_presenter.NextColorFor(_startingColor.Value, color, isOnlineGame);
	}

	public void PreviousColorClicked(PlayerColor color, bool isOnlineGame = false)
	{
		if (!_startingColor.HasValue)
		{
			_startingColor = color;
		}
		_presenter.PreviousColorFor(_startingColor.Value, color, isOnlineGame);
	}

	public void NextPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false)
	{
		if (!_startingAvatar.HasValue)
		{
			_startingAvatar = avatar;
		}
		_presenter.NextPortraitFor(_startingAvatar.Value, avatar, isOnlineGame);
	}

	public void PreviousPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false)
	{
		if (!_startingAvatar.HasValue)
		{
			_startingAvatar = avatar;
		}
		_presenter.PreviousPortraitFor(_startingAvatar.Value, avatar, isOnlineGame);
	}
}
