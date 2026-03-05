using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Board;

namespace BrassApplication.UseCases.Tutorials.Board;

public interface ITutorialBoardInput : IBoardInput, IActionInProgressCancelDelegate
{
}
