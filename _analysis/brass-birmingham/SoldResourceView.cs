using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoldResourceView : MonoBehaviour
{
	private Image _resourceImage;

	private TextMeshProUGUI _amountText;

	private void Awake()
	{
		PrepareReferences();
	}

	public void UpdateSoldResource(int amount, string type)
	{
		_amountText.text = "-" + amount;
		_resourceImage.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIcon" + type + amount);
	}

	private void PrepareReferences()
	{
		_resourceImage = base.transform.Find("ResourceImage").GetComponent<Image>();
		_amountText = base.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
	}
}
