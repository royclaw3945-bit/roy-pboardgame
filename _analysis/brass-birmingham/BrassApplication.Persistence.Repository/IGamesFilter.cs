namespace BrassApplication.Persistence.Repository;

public interface IGamesFilter
{
	string GameId { get; }

	bool PrivateGames { get; }

	bool TwoPlayers { get; }

	bool ThreePlayers { get; }

	bool FourPlayers { get; }

	bool ShowOnlyOpenGames { get; }

	bool ShowExceptOpenGames { get; }

	string ShowOnlyGamesThatBelongToPlayerId { get; }

	string ShowOnlyGamesThatDoNotBelongToPlayerId { get; }
}
