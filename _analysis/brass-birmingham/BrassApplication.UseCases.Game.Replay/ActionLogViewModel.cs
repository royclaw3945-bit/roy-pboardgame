using System.Collections.Generic;

namespace BrassApplication.UseCases.Game.Replay;

public class ActionLogViewModel
{
	public List<ActionLogRoundViewModel> Rounds { get; set; }

	public string ActionTranslation { get; set; }
}
