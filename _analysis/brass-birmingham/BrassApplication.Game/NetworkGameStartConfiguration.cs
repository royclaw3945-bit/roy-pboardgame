using System;

namespace BrassApplication.Game;

public class NetworkGameStartConfiguration : ICloneable
{
	public int TurnTimeoutInSeconds { get; set; }

	public bool IsPrivate { get; set; }

	public string PrivateGamePassword { get; set; }

	public string GameName { get; set; }

	public object Clone()
	{
		return new NetworkGameStartConfiguration
		{
			TurnTimeoutInSeconds = TurnTimeoutInSeconds,
			IsPrivate = IsPrivate,
			PrivateGamePassword = PrivateGamePassword,
			GameName = GameName
		};
	}

	public override bool Equals(object obj)
	{
		if (obj is NetworkGameStartConfiguration startGameStartConfiguration)
		{
			return Equals(startGameStartConfiguration);
		}
		return false;
	}

	public bool Equals(NetworkGameStartConfiguration startGameStartConfiguration)
	{
		if (startGameStartConfiguration == null)
		{
			return false;
		}
		if (TurnTimeoutInSeconds == startGameStartConfiguration.TurnTimeoutInSeconds && IsPrivate == startGameStartConfiguration.IsPrivate && PrivateGamePassword.Equals(startGameStartConfiguration.PrivateGamePassword))
		{
			return GameName.Equals(startGameStartConfiguration.GameName);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return TurnTimeoutInSeconds.GetHashCode() ^ IsPrivate.GetHashCode() ^ PrivateGamePassword.GetHashCode() ^ GameName.GetHashCode();
	}
}
