using BoardGameRules.Entities.Board.Links;

namespace BrassApplication.UseCases.Game.Actions;

public interface ILinkSelectedDelegate
{
	void LinkSelected(Link link);
}
