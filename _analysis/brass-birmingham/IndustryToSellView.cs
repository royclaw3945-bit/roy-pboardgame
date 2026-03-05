using UnityEngine;
using UnityEngine.UI;

public class IndustryToSellView : MonoBehaviour
{
	private Image _tokenImage;

	public virtual void Awake()
	{
		PrepareReferences();
	}

	public virtual void SetIndustry(string imagePath)
	{
		_tokenImage.sprite = Resources.Load<Sprite>("Board/Industries/" + imagePath);
	}

	protected virtual void PrepareReferences()
	{
		_tokenImage = base.transform.Find("BlackBackground/Token").GetComponent<Image>();
	}
}
