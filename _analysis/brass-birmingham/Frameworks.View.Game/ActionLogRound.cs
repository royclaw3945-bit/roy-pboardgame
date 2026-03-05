using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class ActionLogRound : MonoBehaviour
{
	private TextMeshProUGUI textEra;

	private TextMeshProUGUI textRound;

	private Image iconEra;

	public void SetTextEra(string text)
	{
		textEra.text = text;
	}

	public void SetTextRound(string text)
	{
		textRound.text = text;
	}

	public void SetIconEra(Sprite sprite)
	{
		iconEra.sprite = sprite;
	}

	public void PrepareReferences()
	{
		textEra = base.transform.Find("TextEra").GetComponent<TextMeshProUGUI>();
		textRound = base.transform.Find("TextRound").GetComponent<TextMeshProUGUI>();
		iconEra = base.transform.Find("IconEra").GetComponent<Image>();
	}
}
