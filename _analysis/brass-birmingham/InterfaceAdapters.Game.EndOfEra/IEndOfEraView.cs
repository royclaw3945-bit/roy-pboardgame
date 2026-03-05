using System;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.EndOfEra;

public interface IEndOfEraView : IBaseView
{
	void UpdateEndOfEraView(EndOfEraViewModel endOfEraViewModel);

	void SetOnOkButtonClicked(Action onOkButtonClicked);

	void SetOnSkipButtonClicked(Action onSkipButtonClicked);

	void SetSkipButtonInteractable(bool skipButtonInteractable);

	void MorphSkipButtonToOkButton();

	void HideScoreWindow();

	void SetEndOfEraPhaseText(int phase);

	IAnimation ShowEndOfCanalEraTextAnimation(bool isIntroductoryGame);

	IAnimation ShowEndOfRailEraTextAnimation();

	void ShowScoreWindow();
}
