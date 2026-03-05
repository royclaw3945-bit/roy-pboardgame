using System;
using BoardGameRules.Entities.Game;

namespace BrassApplication.Game;

public class BrassApplicationGame
{
	private readonly GameMetadata _metadata;

	public BoardGameRulesGame Game { get; }

	public GameMetadata Metadata
	{
		get
		{
			_metadata.GameProgressInformation = Game.GameProgressInformation;
			if (_metadata.GameType.Equals(GameType.Network) && _metadata.GameProgressInformation != null)
			{
				_metadata.NetworkGameProgressInformation.CurrentPlayerId = _metadata.GetPlayerMetadataByColor(_metadata.GameProgressInformation.CurrentPlayer.Color).PlayerNetworkId;
			}
			return _metadata;
		}
	}

	public BrassApplicationGame(BoardGameRulesGame boardGameRulesGame, GameMetadata gameMetadata)
	{
		Game = boardGameRulesGame ?? throw new ArgumentException("Board Game Rules can't be null");
		_metadata = gameMetadata ?? throw new ArgumentException("Game Metadata can't ben ull");
	}

	public BrassApplicationGame(BrassApplicationGame copy)
	{
		Game = copy.Game;
		_metadata = copy.Metadata;
	}

	public void UpdatePlayerLastSeenRevisionAfterUndoOrCancel()
	{
		_metadata.PlayerMetadata.Find((PlayerMetadata pm) => pm.PlayerStartConfiguration.Color == Game.GameProgressInformation.CurrentPlayer.Color).LastSeenEventRevision = Game.Revision;
	}
}
