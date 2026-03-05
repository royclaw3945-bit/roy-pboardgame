using System;

namespace InterfaceAdapters.Common.SaveGame;

public interface ISaveGameView : IBaseView
{
	void ShowSaveInProgress();

	void ShowSaveFailed(Action onConfirm);
}
