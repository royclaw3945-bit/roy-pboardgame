using System;

namespace InterfaceAdapters.Common;

public interface IFadeView : IBaseView
{
	IFadeView FadeIn();

	IFadeView FadeOut();

	IFadeView Show();

	IFadeView Hide();

	IFadeView SetColor(float r, float g, float b, float a);

	IFadeView SetDuration(float newDuration);

	IFadeView OnComplete(Action action);

	IAnimation GetAnimation();
}
