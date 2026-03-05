using BrassApplication.Utils;
using UnityEngine;

namespace Frameworks.Utils;

public class VersionReader : MonoBehaviour, IVersionReader
{
	public string PublicVersion { get; private set; }

	public string BuildVersion { get; private set; }

	public void Awake()
	{
		PublicVersion = "n/a";
		BuildVersion = "n/a";
		TextAsset textAsset = Resources.Load<TextAsset>("Version/PublicVersion");
		if (!(textAsset == null))
		{
			TextAsset textAsset2 = Resources.Load<TextAsset>("Version/BuildVersion");
			if (!(textAsset2 == null))
			{
				PublicVersion = textAsset.text;
				BuildVersion = textAsset2.text;
			}
		}
	}
}
