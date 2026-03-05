using System;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Replay;

public interface IReplaySummaryView : IBaseView
{
	void UpdateReplaySummaryView(ActionLogViewModel actionLogViewModel, Action onClickAction);
}
