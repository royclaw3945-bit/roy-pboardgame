using System;
using System.Threading.Tasks;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Game.Board;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSlotView : UIBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private BuildingSlot _buildingSlot;

	private Image _borderImage;

	private Image _industryImage;

	private Image _industryTokenImage;

	private Image _outlineObject;

	private Image _outlineBiggerObject;

	private TextMeshProUGUI _victoryPoint;

	private Image _resource;

	private Image _links;

	private Sprite _backgroundSprite;

	private PlayerColor _industryColor;

	private AudioSource _audioSource;

	private Action<BuildingSlot> _actionOnClick = delegate
	{
	};

	private Action<string, float, float> _actionOnHover;

	private Action _actionOnRegionClick = delegate
	{
	};

	private Action _actionOnHoverExit = delegate
	{
	};

	private bool isTooltipVisible;

	public AudioClip IndustryBuiltClip;

	public AudioClip IndustryFlippedClip;

	private new void Awake()
	{
		PrepareReferences();
	}

	private void LateUpdate()
	{
		if (isTooltipVisible && (Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.15f || Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.15f))
		{
			isTooltipVisible = false;
			_actionOnHoverExit?.Invoke();
		}
	}

	public void UpdateSlot(BuildingSlotViewModel viewModel, bool highlightUpdate = false)
	{
		_buildingSlot = viewModel.BuildingSlot;
		_outlineObject.color = new Color(1f, 1f, 1f, viewModel.Highlighted ? 1 : 0);
		_outlineBiggerObject.gameObject.SetActive(value: false);
		_backgroundSprite = Resources.Load<Sprite>("Board/Industries/Board" + viewModel.BackgroundImageName);
		if (_buildingSlot.BuildedIndustry != null)
		{
			_industryColor = _buildingSlot.BuildedIndustry.Color;
			if (_buildingSlot.BuildedIndustry.IsFlipped && _buildingSlot.BuildedIndustry.VPPerLink > 0)
			{
				_links.gameObject.SetActive(value: true);
				_links.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconVpLink" + _buildingSlot.BuildedIndustry.VPPerLink);
			}
			else
			{
				_links.gameObject.SetActive(value: false);
			}
		}
		else if (_links.gameObject.activeSelf)
		{
			_links.gameObject.SetActive(value: false);
		}
		base.gameObject.name = _buildingSlot.UniqueId;
		if (!highlightUpdate)
		{
			_industryTokenImage.color = new Color(1f, 1f, 1f, viewModel.Build ? 1 : 0);
			if (viewModel.Build)
			{
				UpdateResources(viewModel.ImageName, viewModel.ResourceCount);
			}
			else
			{
				_industryImage.sprite = _backgroundSprite;
				_resource.gameObject.SetActive(value: false);
			}
		}
		_actionOnHover = viewModel.ActionOnHover;
		_actionOnHoverExit = viewModel.ActionOnHoverExit;
		_actionOnRegionClick = viewModel.ActionOnRegionClick;
		_borderImage.raycastTarget = true;
		if (viewModel.IsInteractive)
		{
			_actionOnClick = viewModel.ActionOnClick;
		}
		else
		{
			_actionOnClick = OnClickRegion;
		}
		if (viewModel.HighlightResource)
		{
			StartHighlightResourceAnimation();
		}
	}

	public void UpdateResources(string imageName, int resourceCount)
	{
		_resource.gameObject.SetActive(value: true);
		string text = (imageName.Contains("Coal") ? "Coal" : (imageName.Contains("Iron") ? "Iron" : (imageName.Contains("Brewery") ? "Beer" : "")));
		_industryTokenImage.sprite = Resources.Load<Sprite>("Board/Industries/" + imageName);
		if (resourceCount > 0)
		{
			_resource.sprite = ((text != "") ? Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIcon" + text + resourceCount) : null);
			_resource.rectTransform.sizeDelta = GetResourceSize(text, resourceCount);
		}
		else
		{
			_resource.gameObject.SetActive(value: false);
		}
	}

	private Vector2 GetResourceSize(string type, int count)
	{
		Vector2 result = Vector2.zero;
		if (type == "Beer")
		{
			result = ((count != 1) ? new Vector2(216f, 237f) : new Vector2(134f, 187f));
		}
		else
		{
			switch (count)
			{
			case 1:
				result = new Vector2(106f, 119f);
				break;
			case 2:
				result = new Vector2(260f, 239f);
				break;
			case 3:
				result = new Vector2(326f, 239f);
				break;
			case 4:
				result = new Vector2(326f, 278f);
				break;
			case 5:
				result = new Vector2(392f, 278f);
				break;
			case 6:
				result = new Vector2(392f, 317f);
				break;
			}
		}
		return result;
	}

	private void StartHighlightResourceAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_outlineBiggerObject.gameObject.SetActive(value: true);
			_outlineObject.DOFade(1f, 0f);
			_outlineBiggerObject.DOFade(1f, 0f);
			_outlineObject.transform.localScale = 2.25f * Vector3.one;
			_outlineObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			_outlineBiggerObject.transform.localScale = 3.2f * Vector3.one;
			_outlineBiggerObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -45f));
		});
		sequence.Append(_outlineObject.transform.DOScale(1.4f, 20f * UIProperties.FrameDuration)).Join(_outlineBiggerObject.transform.DOScale(1.4f, 20f * UIProperties.FrameDuration)).Join(_outlineObject.transform.DORotate(Vector3.zero, 20f * UIProperties.FrameDuration))
			.Join(_outlineBiggerObject.transform.DORotate(Vector3.zero, 20f * UIProperties.FrameDuration));
		sequence.AppendCallback(delegate
		{
			_outlineBiggerObject.gameObject.SetActive(value: false);
		});
		sequence.Play();
	}

	public void StartShowPointsAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_victoryPoint.text = _victoryPoint.text.Replace("1", _buildingSlot.BuildedIndustry.VPForBuilding.ToString());
			_victoryPoint.transform.SetParent(base.transform.parent.parent);
			_victoryPoint.transform.SetAsLastSibling();
		});
		sequence.Append(_victoryPoint.DOFade(1f, 1f)).Join(_victoryPoint.transform.DOMoveY(base.transform.position.y + 100f, 1f));
		sequence.Append(_victoryPoint.transform.DOScale(2f, 1f)).Join(_victoryPoint.DOFade(0f, 1f));
		sequence.OnComplete(delegate
		{
			_victoryPoint.alpha = 0f;
			_victoryPoint.text = _victoryPoint.text.Replace(_buildingSlot.BuildedIndustry.VPForBuilding.ToString(), "1");
			_victoryPoint.transform.localPosition = Vector3.zero;
			_victoryPoint.transform.localScale = Vector3.one;
			_victoryPoint.transform.SetParent(base.transform);
			_victoryPoint.transform.SetAsLastSibling();
		});
		sequence.Play();
	}

	public void StartRemoveIndustryAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_industryTokenImage.transform.SetParent(base.transform.parent.parent);
			_industryTokenImage.transform.SetAsLastSibling();
			_resource.transform.SetParent(base.transform.parent.parent);
			_resource.transform.SetAsLastSibling();
			_industryImage.sprite = _backgroundSprite;
		});
		sequence.Append(_industryTokenImage.transform.DOScale(2f, 1f)).Join(_industryTokenImage.DOFade(0f, 1f)).Join(_resource.transform.DOScale(2f, 1f))
			.Join(_resource.DOFade(0f, 1f));
		sequence.OnComplete(delegate
		{
			_industryTokenImage.color = new Color(_industryTokenImage.color.r, _industryTokenImage.color.g, _industryTokenImage.color.b, 0f);
			_resource.color = new Color(_resource.color.r, _resource.color.g, _resource.color.b, 1f);
			_industryTokenImage.transform.localScale = Vector3.one;
			_resource.transform.localScale = Vector3.one;
			_industryTokenImage.transform.SetParent(base.transform);
			_industryTokenImage.transform.SetSiblingIndex(1);
			_resource.transform.SetParent(base.transform);
			_resource.transform.SetSiblingIndex(5);
			_resource.gameObject.SetActive(value: false);
		});
		sequence.Play();
	}

	public PlayerColor GetIndustryColor()
	{
		return _industryColor;
	}

	public bool IsBuilt()
	{
		if ((bool)_industryTokenImage)
		{
			return _industryTokenImage.color.a == 1f;
		}
		return false;
	}

	public bool IsFlipped()
	{
		return _buildingSlot.BuildedIndustry.IsFlipped;
	}

	public bool IsLevelOne()
	{
		return _buildingSlot.BuildedIndustry.Level == 1;
	}

	private void PrepareReferences()
	{
		_borderImage = GetComponent<Image>();
		_industryImage = base.transform.Find("Icon").GetComponent<Image>();
		_industryTokenImage = base.transform.Find("IndustryToken").GetComponent<Image>();
		_outlineObject = base.transform.Find("Outline").GetComponent<Image>();
		_outlineBiggerObject = base.transform.Find("OutlineBigger").GetComponent<Image>();
		_victoryPoint = base.transform.Find("VictoryPoint").GetComponent<TextMeshProUGUI>();
		_resource = base.transform.Find("Resource").GetComponent<Image>();
		_links = base.transform.Find("Links").GetComponent<Image>();
		_audioSource = base.transform.GetComponent<AudioSource>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_actionOnClick(_buildingSlot);
	}

	public void OnClickRegion(BuildingSlot slot)
	{
		_actionOnRegionClick();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_actionOnHover?.Invoke(_buildingSlot.UniqueId, base.transform.position.x, base.transform.position.y);
		TooltipDisappearDelay();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isTooltipVisible = false;
		_actionOnHoverExit?.Invoke();
	}

	public async void TooltipDisappearDelay()
	{
		await Task.Delay(1000);
		isTooltipVisible = true;
	}

	public void PlayIndustryBuiltSound()
	{
		_audioSource.PlayOneShot(IndustryBuiltClip);
	}

	public void PlayIndustryFlippedSound()
	{
		_audioSource.PlayOneShot(IndustryFlippedClip);
	}
}
