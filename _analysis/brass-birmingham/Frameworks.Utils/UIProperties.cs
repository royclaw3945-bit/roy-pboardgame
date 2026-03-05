namespace Frameworks.Utils;

public static class UIProperties
{
	public static readonly float AnimationDuration = 0.3f;

	public static readonly float DefaultFadeViewFadeValue = 0.94f;

	public static readonly float FrameDuration = 1f / 30f;

	public static float CurrentAnimationSpeed = 1f;

	public static readonly float DefaultAnimationSpeed = 1f;

	public static readonly float FastForwardAnimationSpeed = 2f;

	public static string GetRomanNumber(int arabicNumber)
	{
		return arabicNumber switch
		{
			1 => "I", 
			2 => "II", 
			3 => "III", 
			4 => "IV", 
			5 => "V", 
			6 => "VI", 
			7 => "VII", 
			8 => "VIII", 
			_ => "I", 
		};
	}
}
