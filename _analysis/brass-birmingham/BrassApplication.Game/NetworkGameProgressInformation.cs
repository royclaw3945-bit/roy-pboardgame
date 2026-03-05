using System;

namespace BrassApplication.Game;

public class NetworkGameProgressInformation : ICloneable
{
	public enum GameStatus
	{
		InProgress,
		Open,
		Finished,
		AbortedPlayerLeft,
		AbortedTimeout
	}

	public int TimeLeftForTurn { get; set; }

	public string CurrentPlayerId { get; set; }

	public GameStatus CurrentGameStatus { get; set; }

	public object Clone()
	{
		return new NetworkGameProgressInformation
		{
			TimeLeftForTurn = TimeLeftForTurn,
			CurrentPlayerId = CurrentPlayerId,
			CurrentGameStatus = CurrentGameStatus
		};
	}

	public override bool Equals(object obj)
	{
		if (obj is NetworkGameProgressInformation metadata)
		{
			return Equals(metadata);
		}
		return false;
	}

	public bool Equals(NetworkGameProgressInformation metadata)
	{
		if (metadata == null)
		{
			return false;
		}
		if (TimeLeftForTurn == metadata.TimeLeftForTurn && CurrentPlayerId == metadata.CurrentPlayerId)
		{
			return CurrentGameStatus == metadata.CurrentGameStatus;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return TimeLeftForTurn ^ CurrentPlayerId.GetHashCode() ^ CurrentGameStatus.GetHashCode();
	}
}
