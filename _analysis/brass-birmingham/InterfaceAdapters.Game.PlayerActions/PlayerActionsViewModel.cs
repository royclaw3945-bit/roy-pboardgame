namespace InterfaceAdapters.Game.PlayerActions;

public class PlayerActionsViewModel
{
	public bool Is1stActionAvailable;

	public bool Is2ndActionAvailable;

	public bool Is2ndActionPresent { get; set; }

	public override bool Equals(object obj)
	{
		if (obj is PlayerActionsViewModel playerActionsViewModel)
		{
			return Equals(playerActionsViewModel);
		}
		return false;
	}

	public bool Equals(PlayerActionsViewModel playerActionsViewModel)
	{
		if (playerActionsViewModel == null)
		{
			return false;
		}
		if (Is1stActionAvailable == playerActionsViewModel.Is1stActionAvailable && Is2ndActionAvailable == playerActionsViewModel.Is2ndActionAvailable)
		{
			return Is2ndActionPresent == playerActionsViewModel.Is2ndActionPresent;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Is1stActionAvailable.GetHashCode() ^ Is2ndActionAvailable.GetHashCode() ^ Is2ndActionPresent.GetHashCode();
	}
}
