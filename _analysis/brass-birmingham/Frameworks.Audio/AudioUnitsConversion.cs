using UnityEngine;

namespace Frameworks.Audio;

public class AudioUnitsConversion
{
	public static float DecibelToLinear(float volumeInDecibel)
	{
		return Mathf.Pow(10f, volumeInDecibel / 20f);
	}

	public static float LinearToDecibel(float volumeLinear)
	{
		if (volumeLinear != 0f)
		{
			return Mathf.Log10(volumeLinear) * 20f;
		}
		return -100f;
	}
}
