namespace BrassApplication.Persistence.Repository;

public class GamesFilter : IGamesFilter
{
	public string GameId { get; }

	public bool PrivateGames { get; set; }

	public bool TwoPlayers { get; set; }

	public bool ThreePlayers { get; set; }

	public bool FourPlayers { get; set; }

	public bool ShowOnlyOpenGames { get; set; }

	public bool ShowExceptOpenGames { get; set; }

	public string ShowOnlyGamesThatBelongToPlayerId { get; set; }

	public string ShowOnlyGamesThatDoNotBelongToPlayerId { get; set; }
}
