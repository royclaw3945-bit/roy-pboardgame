using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Actions;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : BaseView, IBoardView, IBaseView
{
	[SerializeField]
	private float _maxZoomOutScale = 0.028f;

	protected BoardController _controller;

	private Transform _regionsContainer;

	protected Transform _borderRegionsContainer;

	protected Transform _linksContainer;

	private List<Transform> _buildingSlotsContainter = new List<Transform>();

	protected ZoomScrollRect _zoomScrollRect;

	protected readonly Dictionary<RegionName, RegionView> regionGameObjects = new Dictionary<RegionName, RegionView>();

	private readonly Dictionary<RegionName, BorderRegionView> borderRegionGameObjects = new Dictionary<RegionName, BorderRegionView>();

	public float scrollSpeed;

	public float zoomSpeed;

	public GameObject resourceSoldPrefab;

	public GameObject incomeMarkerPrefab;

	public AudioClip LinkHighlightClip;

	public AudioClip CoalConsumedClip;

	public AudioClip IronConsumedClip;

	public AudioClip BeerConsumedClip;

	public AudioClip ObsoleteIndustriesRemovedClip;

	private AudioSource _audioSource;

	[SerializeField]
	private List<Sprite> _dayBoardElements;

	[SerializeField]
	private List<Sprite> _nightBoardElements;

	private Transform _background;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is BoardController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.W, this);
		base.InputRegistration.RegisterKey(KeyEnum.S, this);
		base.InputRegistration.RegisterKey(KeyEnum.A, this);
		base.InputRegistration.RegisterKey(KeyEnum.D, this);
		base.InputRegistration.RegisterKey(KeyEnum.UpArrow, this);
		base.InputRegistration.RegisterKey(KeyEnum.DownArrow, this);
		base.InputRegistration.RegisterKey(KeyEnum.LeftArrow, this);
		base.InputRegistration.RegisterKey(KeyEnum.RightArrow, this);
		base.InputRegistration.RegisterKey(KeyEnum.PageUp, this);
		base.InputRegistration.RegisterKey(KeyEnum.PageDown, this);
		SetMapDayNight();
	}

	public void UpdateBoardView(BoardViewModel boardViewModel)
	{
		UpdateRegions(boardViewModel.RegionsViewModels);
		UpdateBorderRegions(boardViewModel.BorderRegionsViewModels);
		UpdateLinks(boardViewModel.Links);
	}

	public void HighlightBuildingSlots(BoardViewModel boardViewModel)
	{
		UpdateRegions(boardViewModel.RegionsViewModels, highlightUpdate: true);
	}

	public void HighlightMerchantSlots(IDictionary<RegionName, BorderRegionViewModel> borderRegions)
	{
		UpdateBorderRegions(borderRegions);
	}

	public void HighlightLinks(IDictionary<string, LinkViewModel> links, ILinkSelectedDelegate linkSelectedDelegate)
	{
		UpdateLinks(links, linkSelectedDelegate);
		_audioSource.PlayOneShot(LinkHighlightClip);
	}

	public void TurnOffRegionsActionOnClick()
	{
		foreach (KeyValuePair<RegionName, RegionView> regionGameObject in regionGameObjects)
		{
			regionGameObject.Value.TurnOffActionOnClick();
		}
	}

	public object GetRegionViewForGivenRegionName(RegionName regionName)
	{
		return regionGameObjects[regionName];
	}

	public IAnimation CreateLinkPlacedAnimation(string linkId, PlayerColor playerColor)
	{
		bool isCanalLink = linkId[0] == 'C';
		linkId = linkId.Remove(0, isCanalLink ? 5 : 4);
		Transform link = _linksContainer.Find(linkId);
		GameObject token = link.Find("LinkToken").gameObject;
		GameObject outline = link.Find("LinkToken/Outline").gameObject;
		foreach (Transform item in _linksContainer)
		{
			item.Find("LinkToken/Outline").gameObject.SetActive(value: false);
		}
		outline.SetActive(value: true);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(ScrollBoardToGivenLocation(token.transform.parent.GetComponent<RectTransform>()));
		sequence.AppendCallback(delegate
		{
			link.GetComponent<LinkView>().PlayPlaceLinkClip();
			token.SetActive(value: true);
			Image component = token.GetComponent<Image>();
			component.sprite = Resources.Load<Sprite>("Board/Links/Token" + (isCanalLink ? "CanalEra" : "RailEra") + playerColor);
			component.color = Color.white;
			token.transform.localScale = Vector3.one * 0.7f;
			outline.transform.position = token.transform.position;
			outline.SetActive(value: true);
			Sequence sequence2 = DOTween.Sequence();
			sequence2.AppendInterval(0.5f);
			sequence2.Append(token.transform.DOScale(Vector3.one * 1.2f, UIProperties.FrameDuration * 3f));
			sequence2.Append(token.transform.DOScale(Vector3.one * 1f, UIProperties.FrameDuration * 8f));
			sequence2.Insert(0.5f, outline.transform.DOScale(Vector3.one * 3f, UIProperties.FrameDuration * 6f));
			sequence2.Insert(0.5f, outline.GetComponent<Image>().DOFade(0f, UIProperties.FrameDuration * 6f));
			sequence2.OnKill(delegate
			{
				outline.SetActive(value: false);
			});
		});
		sequence.AppendInterval(0.5f + UIProperties.FrameDuration * 13f);
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateRemovePlayerLinksAnimation(PlayerColor player)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(1f);
		for (int i = 0; i < _linksContainer.childCount; i++)
		{
			LinkView linkObject = _linksContainer.GetChild(i).GetComponent<LinkView>();
			if (linkObject.GetLinkColor() == player && linkObject.isBuilt())
			{
				sequence.InsertCallback(1f, delegate
				{
					int pointsForLink = _controller.GetPointsForLink(linkObject.GetLinkId());
					linkObject.RemoveTokenAnimation(pointsForLink);
				});
			}
		}
		sequence.AppendInterval(0.5f);
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateIndustryBuiltAnimation(BuildingSlot slot, BaseIndustry industry, PlayerColor playerColor)
	{
		Sequence sequence = DOTween.Sequence();
		int index = 1;
		for (int i = 0; i < _buildingSlotsContainter.Count; i++)
		{
			sequence.Insert(0f, _buildingSlotsContainter[i].Find("Outline").GetComponent<Image>().DOFade(0f, 5f * UIProperties.FrameDuration));
			if (_buildingSlotsContainter[i].name == slot.UniqueId)
			{
				index = i;
			}
		}
		Transform chosenSlot = _buildingSlotsContainter[index];
		Transform token = UnityEngine.Object.Instantiate(chosenSlot.Find("IndustryToken"), chosenSlot.position, Quaternion.identity, chosenSlot.parent.parent);
		RectTransform component = token.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(320f, 320f);
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.position = chosenSlot.position;
		Transform glow = UnityEngine.Object.Instantiate(chosenSlot.Find("Glow"), chosenSlot.position, Quaternion.identity, chosenSlot.parent.parent);
		RectTransform component2 = glow.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 1f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.sizeDelta = new Vector2(320f, 320f);
		component2.position = chosenSlot.position;
		chosenSlot.Find("Outline").GetComponent<Image>().color *= new Color(1f, 1f, 1f, 0f);
		sequence.Append(ScrollBoardToGivenLocation(chosenSlot.parent.parent.parent.GetComponent<RectTransform>()));
		sequence.AppendCallback(chosenSlot.GetComponent<BuildingSlotView>().PlayIndustryBuiltSound);
		Image realToken = chosenSlot.Find("IndustryToken").GetComponent<Image>();
		Image resource = chosenSlot.Find("Resource").GetComponent<Image>();
		Color color = token.GetComponent<Image>().color;
		token.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0f);
		color = glow.GetComponent<Image>().color;
		glow.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0f);
		sequence.Append(glow.DOScale(Vector3.one * 1f, 0f * UIProperties.FrameDuration));
		sequence.Append(glow.DORotate(new Vector3(0f, 0f, 33f), 0f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(Vector3.one * 2.31f, 0f * UIProperties.FrameDuration));
		sequence.Append(token.DORotate(new Vector3(0f, 0f, 33f), 0f * UIProperties.FrameDuration));
		sequence.Append(token.DOLocalMoveY(token.localPosition.y + 350f, 0f * UIProperties.FrameDuration));
		sequence.Append(glow.DOLocalMoveY(glow.localPosition.y + 350f, 0f * UIProperties.FrameDuration));
		string color2 = playerColor.ToString();
		string building = industry.BuildingType.ToString();
		token.GetComponent<Image>().sprite = Resources.Load<Sprite>("Board/Industries/" + color2 + building);
		sequence.AppendCallback(delegate
		{
			glow.gameObject.SetActive(value: true);
		});
		sequence.AppendInterval(7f * UIProperties.FrameDuration);
		sequence.Append(glow.GetComponent<Image>().DOFade(1f, 7f * UIProperties.FrameDuration).SetEase(Ease.InOutQuad));
		sequence.Join(glow.DOScale(Vector3.one * 2.31f, 7f * UIProperties.FrameDuration));
		sequence.Join(realToken.DOFade(0f, 7f * UIProperties.FrameDuration));
		sequence.Join(resource.DOFade(0f, 7f * UIProperties.FrameDuration));
		sequence.Append(token.DORotate(new Vector3(0f, 0f, 0f), 8f * UIProperties.FrameDuration));
		sequence.Join(token.GetComponent<Image>().DOFade(1f, 8f * UIProperties.FrameDuration));
		sequence.Join(token.DOScale(Vector3.one, 8f * UIProperties.FrameDuration));
		sequence.Join(token.DOLocalMoveY(token.localPosition.y, 8f * UIProperties.FrameDuration));
		sequence.Join(glow.DOLocalMoveY(glow.localPosition.y, 8f * UIProperties.FrameDuration));
		sequence.Join(glow.DORotate(new Vector3(0f, 0f, 0f), 8f * UIProperties.FrameDuration));
		sequence.Join(glow.GetComponent<Image>().DOFade(0f, 6f * UIProperties.FrameDuration));
		sequence.Join(glow.DOScale(Vector3.one, 8f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(Vector3.one * 1.18f, 6f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(Vector3.one, 3f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(Vector3.one * 1.05f, 3f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(Vector3.one, 2f * UIProperties.FrameDuration));
		sequence.Append(realToken.DOFade(1f, 0f)).Join(resource.DOFade(1f, 0f));
		sequence.OnKill(delegate
		{
			realToken = chosenSlot.Find("IndustryToken").GetComponent<Image>();
			realToken.sprite = Resources.Load<Sprite>("Board/Industries/" + color2 + building);
			chosenSlot.GetComponent<BuildingSlotView>().UpdateResources(slot.GetCurrentImageName(), industry.GetResource());
			realToken.color = new Color(realToken.color.r, realToken.color.g, realToken.color.b, 1f);
			resource.color = new Color(resource.color.r, resource.color.g, resource.color.b, 1f);
			UnityEngine.Object.Destroy(token.gameObject);
			UnityEngine.Object.Destroy(glow.gameObject);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateResourceSoldAnimation(string slotUniqueId, int amount, string type, BuildingSlot buildingSlot)
	{
		Sequence sequence = DOTween.Sequence();
		Transform slot = _buildingSlotsContainter.Find((Transform t) => t.name.Equals(slotUniqueId));
		GameObject resourceGO = UnityEngine.Object.Instantiate(resourceSoldPrefab, slot.position, Quaternion.identity, base.transform);
		resourceGO.SetActive(value: false);
		sequence.AppendCallback(delegate
		{
			resourceGO.SetActive(value: true);
			resourceGO.GetComponent<SoldResourceView>().UpdateSoldResource(amount, type);
			BaseIndustry buildedIndustry = buildingSlot.BuildedIndustry;
			if (buildedIndustry != null)
			{
				if (!(buildedIndustry is CoalMine))
				{
					if (!(buildedIndustry is IronWorks))
					{
						if (buildedIndustry is Brewery)
						{
							_audioSource.PlayOneShot(BeerConsumedClip);
						}
					}
					else
					{
						_audioSource.PlayOneShot(IronConsumedClip);
					}
				}
				else
				{
					_audioSource.PlayOneShot(CoalConsumedClip);
				}
			}
		});
		sequence.AppendInterval(0.2f);
		sequence.Append(resourceGO.transform.DOScale(2f, 15f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.AppendCallback(delegate
		{
			BuildingSlotView component = slot.GetComponent<BuildingSlotView>();
			BaseIndustry buildedIndustry = buildingSlot.BuildedIndustry;
			if (buildedIndustry != null)
			{
				if (!(buildedIndustry is CoalMine coalMine))
				{
					if (!(buildedIndustry is IronWorks ironWorks))
					{
						if (buildedIndustry is Brewery brewery)
						{
							Brewery brewery2 = brewery;
							component.UpdateResources(buildingSlot.GetCurrentImageName(), brewery2.AvailableBeer - amount);
						}
					}
					else
					{
						IronWorks ironWorks2 = ironWorks;
						component.UpdateResources(buildingSlot.GetCurrentImageName(), ironWorks2.AvailableIron - amount);
					}
				}
				else
				{
					CoalMine coalMine2 = coalMine;
					component.UpdateResources(buildingSlot.GetCurrentImageName(), coalMine2.AvailableCoal - amount);
				}
			}
		});
		sequence.AppendInterval(0.5f);
		sequence.Append(resourceGO.transform.DOScale(5f, 10f * UIProperties.FrameDuration)).Join(resourceGO.GetComponent<CanvasGroup>().DOFade(0f, 10f * UIProperties.FrameDuration));
		sequence.OnKill(delegate
		{
			UnityEngine.Object.Destroy(resourceGO);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateBeerSuppliedFromMerchantAnimation(string merchantUniqueId, BorderRegion borderRegion)
	{
		Sequence sequence = DOTween.Sequence();
		BorderRegionView borderRegionView = borderRegionGameObjects[borderRegion.Name];
		MerchantSlotView merchantSlotView = borderRegionView.GetMerchantSlot(merchantUniqueId);
		GameObject resourceGO = UnityEngine.Object.Instantiate(resourceSoldPrefab, merchantSlotView.GetBeerPosition(), Quaternion.identity, base.transform);
		resourceGO.SetActive(value: false);
		sequence.AppendCallback(delegate
		{
			_audioSource.PlayOneShot(BeerConsumedClip);
			resourceGO.SetActive(value: true);
			resourceGO.GetComponent<SoldResourceView>().UpdateSoldResource(1, "Beer");
		});
		sequence.AppendInterval(0.2f);
		sequence.Append(resourceGO.transform.DOScale(2f, 15f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.AppendCallback(delegate
		{
			merchantSlotView.HideBeer();
		});
		sequence.AppendInterval(0.5f);
		sequence.Append(resourceGO.transform.DOScale(5f, 10f * UIProperties.FrameDuration)).Join(resourceGO.GetComponent<CanvasGroup>().DOFade(0f, 10f * UIProperties.FrameDuration));
		sequence.OnKill(delegate
		{
			UnityEngine.Object.Destroy(resourceGO);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateIncomeMarkerPositionChangedAnimation(int amount, string uniqueId)
	{
		Sequence sequence = DOTween.Sequence();
		GameObject incomeArrow = UnityEngine.Object.Instantiate(position: _buildingSlotsContainter.Find((Transform t) => t.name.Equals(uniqueId)).position + new Vector3(0f, 20f, 0f), original: incomeMarkerPrefab, rotation: Quaternion.identity, parent: base.transform);
		incomeArrow.SetActive(value: false);
		sequence.AppendCallback(delegate
		{
			incomeArrow.SetActive(value: true);
			incomeArrow.GetComponent<CanvasGroup>().alpha = 0f;
			incomeArrow.transform.Find("Image/Amount").GetComponent<TextMeshProUGUI>().text = amount.ToString();
		});
		sequence.Append(incomeArrow.transform.DOBlendableMoveBy(new Vector3(0f, 30f, 0f), 6f * UIProperties.FrameDuration)).Join(incomeArrow.GetComponent<CanvasGroup>().DOFade(1f, 6f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.AppendInterval(15f * UIProperties.FrameDuration);
		sequence.Append(incomeArrow.transform.DOScaleY(1.2f, 10f * UIProperties.FrameDuration)).Join(incomeArrow.transform.DOBlendableMoveBy(new Vector3(0f, 60f, 0f), 9f * UIProperties.FrameDuration)).Join(incomeArrow.GetComponent<CanvasGroup>().DOFade(0f, 6f * UIProperties.FrameDuration))
			.SetEase(Ease.InQuart);
		sequence.OnKill(delegate
		{
			UnityEngine.Object.Destroy(incomeArrow);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateIndustryFlippedAnimation(BuildingSlot buildingSlot)
	{
		Sequence sequence = DOTween.Sequence();
		Transform slot = _buildingSlotsContainter.Find((Transform t) => t.name.Equals(buildingSlot.UniqueId));
		GameObject glow = slot.Find("Glow").gameObject;
		Image glowImage = glow.GetComponent<Image>();
		Transform token = slot.Find("IndustryToken");
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(delegate
		{
			glow.SetActive(value: true);
			token.GetComponent<Canvas>().overrideSorting = true;
			token.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			slot.GetComponent<BuildingSlotView>().PlayIndustryFlippedSound();
		});
		sequence.Append(glowImage.DOFade(1f, 4f * UIProperties.FrameDuration));
		sequence.Append(glowImage.DOFade(0f, 6f * UIProperties.FrameDuration));
		sequence.Append(token.DOScale(new Vector3(2f, 2.5f, 1f), 8f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.8f, 1.8f, 1f), 12f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.6f, 1.8f, 1f), 10f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(0.3f, 2.5f, 1f), 1f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.AppendCallback(delegate
		{
			token.GetComponent<Image>().sprite = Resources.Load<Sprite>("Board/Industries/" + buildingSlot.GetCurrentImageName() + "Flipped");
		});
		sequence.Append(token.DOScale(new Vector3(2f, 1.9f, 1f), 3f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.8f, 2f, 1f), 7f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.6f, 1.6f, 1f), 5f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.9f, 1.9f, 1f), 7f * UIProperties.FrameDuration)).Join(token.DORotate(new Vector3(0f, 0f, 15f), 7f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Append(token.DOScale(new Vector3(1.2f, 1.2f, 1f), 9f * UIProperties.FrameDuration)).Join(token.DORotate(new Vector3(0f, 0f, 0f), 9f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.AppendCallback(delegate
		{
			glowImage.color = new Color(1f, 1f, 1f, 1f);
		});
		float num = sequence.Duration();
		sequence.Append(glow.transform.DOScale(5f, 15f * UIProperties.FrameDuration)).Join(glowImage.DOFade(0f, 15f * UIProperties.FrameDuration));
		sequence.Insert(num + 3f * UIProperties.FrameDuration, token.DOScale(new Vector3(1.2f, 1.4f, 1f), 3f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.Insert(num + 6f * UIProperties.FrameDuration, token.DOScale(new Vector3(1f, 1f, 1f), 3f * UIProperties.FrameDuration)).SetEase(Ease.OutCubic);
		sequence.OnKill(delegate
		{
			token.GetComponent<Canvas>().overrideSorting = false;
			token.localScale = Vector3.one;
			glow.SetActive(value: false);
			glow.transform.localScale = 1.4f * Vector3.one;
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreatePointPlayerFlippedIndustriesAnimation(PlayerColor player)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(1f);
		for (int i = 0; i < _buildingSlotsContainter.Count; i++)
		{
			BuildingSlotView buildingSlot = _buildingSlotsContainter[i].GetComponent<BuildingSlotView>();
			if (buildingSlot.GetIndustryColor() == player && buildingSlot.IsBuilt() && buildingSlot.IsFlipped())
			{
				sequence.AppendCallback(delegate
				{
					buildingSlot.StartShowPointsAnimation();
				});
			}
		}
		sequence.AppendInterval(0.5f);
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreatePointPlayerFlippedLvlTwoPlusIndustriesAnimation(PlayerColor player)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(1f);
		for (int i = 0; i < _buildingSlotsContainter.Count; i++)
		{
			BuildingSlotView buildingSlot = _buildingSlotsContainter[i].GetComponent<BuildingSlotView>();
			if (buildingSlot.GetIndustryColor() == player && buildingSlot.IsBuilt() && buildingSlot.IsFlipped() && !buildingSlot.IsLevelOne())
			{
				sequence.AppendCallback(delegate
				{
					buildingSlot.StartShowPointsAnimation();
				});
			}
		}
		sequence.AppendInterval(0.5f);
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateRemovePlayerLvlOneIndustriesAnimation(PlayerColor player)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_audioSource.PlayOneShot(ObsoleteIndustriesRemovedClip);
		});
		for (int num = 0; num < _buildingSlotsContainter.Count; num++)
		{
			BuildingSlotView buildingSlot = _buildingSlotsContainter[num].GetComponent<BuildingSlotView>();
			if (buildingSlot.GetIndustryColor() == player && buildingSlot.IsBuilt() && buildingSlot.IsLevelOne())
			{
				sequence.AppendCallback(delegate
				{
					buildingSlot.StartRemoveIndustryAnimation();
				});
			}
		}
		sequence.AppendInterval(0.5f);
		return new DOTweenAnimationSequence(sequence);
	}

	public void ScrollToRegion(RegionName region)
	{
		ScrollBoardToGivenLocation(regionGameObjects[region].transform.GetComponent<RectTransform>());
	}

	protected Sequence ScrollBoardToGivenLocation(RectTransform target)
	{
		Vector2 target2 = (Vector2)_zoomScrollRect.transform.InverseTransformPoint(_zoomScrollRect.content.position) - (Vector2)_zoomScrollRect.transform.InverseTransformPoint(target.position);
		return ScrollBoardToGivenLocation(target2);
	}

	protected Sequence ScrollBoardToGivenLocation(Vector2 target)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(Canvas.ForceUpdateCanvases);
		RectTransform component = _zoomScrollRect.content.GetComponent<RectTransform>();
		Vector2 vector = default(Vector2);
		_zoomScrollRect.UpdateBounds();
		Bounds futureViewBounds = new Bounds(target, _zoomScrollRect.viewport.rect.size);
		vector = _zoomScrollRect.CalculateFutureOffset(futureViewBounds, Vector2.zero);
		if (vector.x != 0f)
		{
			target.x -= ZoomScrollRect.RubberDelta(vector.x, _zoomScrollRect.viewport.rect.size.x);
		}
		if (vector.y != 0f)
		{
			target.y -= ZoomScrollRect.RubberDelta(vector.y, _zoomScrollRect.viewport.rect.size.y);
		}
		target.x *= 12.5f * component.localScale.x - 0.35f;
		target.y *= Math.Min(32f * component.localScale.y - 0.9f, 0.95f);
		sequence.Join(component.DOAnchorPos(target, UIProperties.AnimationDuration).SetEase(Ease.OutQuart));
		return sequence;
	}

	private void UpdateRegions(IDictionary<RegionName, RegionViewModel> regions, bool highlightUpdate = false)
	{
		foreach (KeyValuePair<RegionName, RegionView> regionGameObject in regionGameObjects)
		{
			RegionView value = regionGameObject.Value;
			RegionViewModel regionViewModel = regions[value.Name];
			foreach (BuildingSlotViewModel buildingSlotsViewModel in regionViewModel.BuildingSlotsViewModels)
			{
				buildingSlotsViewModel.ActionOnHover = ShowTooltipIndustry;
				buildingSlotsViewModel.ActionOnHoverExit = HideTooltip;
			}
			value.UpdateRegion(regionViewModel, highlightUpdate);
		}
	}

	private void UpdateBorderRegions(IDictionary<RegionName, BorderRegionViewModel> borderRegions)
	{
		foreach (KeyValuePair<RegionName, BorderRegionView> borderRegionGameObject in borderRegionGameObjects)
		{
			BorderRegionView value = borderRegionGameObject.Value;
			BorderRegionViewModel borderRegionViewModel = borderRegions[value.Name];
			borderRegionViewModel.OnHover = ShowTooltipBorderRegion;
			borderRegionViewModel.OnHoverExit = HideTooltip;
			value.UpdateRegion(borderRegionViewModel);
		}
	}

	private void UpdateLinks(IDictionary<string, LinkViewModel> links)
	{
		for (int i = 0; i < _linksContainer.childCount; i++)
		{
			Transform child = _linksContainer.GetChild(i);
			links[child.name].OnHover = ShowTooltipLink;
			links[child.name].OnHoverExit = HideTooltip;
			child.GetComponent<LinkView>().UpdateLink(links[child.name], null);
		}
	}

	private void UpdateLinks(IDictionary<string, LinkViewModel> links, ILinkSelectedDelegate linkSelectedDelegate)
	{
		for (int i = 0; i < _linksContainer.childCount; i++)
		{
			Transform child = _linksContainer.GetChild(i);
			links[child.name].OnHover = ShowTooltipLink;
			links[child.name].OnHoverExit = HideTooltip;
			child.GetComponent<LinkView>().UpdateLink(links[child.name], linkSelectedDelegate);
		}
	}

	private void OnRegionInBuildActionClicked(RegionName regionName)
	{
		_controller.OnRegionClicked(regionName);
	}

	public override void HandleKey(KeyEnum keyEnum)
	{
		base.HandleKey(keyEnum);
		if (keyEnum == KeyEnum.W || keyEnum == KeyEnum.UpArrow)
		{
			_zoomScrollRect.content.localPosition -= Time.deltaTime * scrollSpeed * Vector3.up;
		}
		if (keyEnum == KeyEnum.S || keyEnum == KeyEnum.DownArrow)
		{
			_zoomScrollRect.content.localPosition += Time.deltaTime * scrollSpeed * Vector3.up;
		}
		if (keyEnum == KeyEnum.A || keyEnum == KeyEnum.LeftArrow)
		{
			_zoomScrollRect.content.localPosition += Time.deltaTime * scrollSpeed * Vector3.right;
		}
		if (keyEnum == KeyEnum.D || keyEnum == KeyEnum.RightArrow)
		{
			_zoomScrollRect.content.localPosition -= Time.deltaTime * scrollSpeed * Vector3.right;
		}
		if (keyEnum == KeyEnum.PageUp)
		{
			_zoomScrollRect.content.localScale -= zoomSpeed * Time.deltaTime * Vector3.one;
		}
		if (keyEnum == KeyEnum.PageDown)
		{
			_zoomScrollRect.content.localScale += zoomSpeed * Time.deltaTime * Vector3.one;
		}
	}

	public void AllowRegionInteractions(bool isRaycastTarget)
	{
		for (int i = 0; i < _regionsContainer.childCount; i++)
		{
			_regionsContainer.GetChild(i).GetComponent<NonDrawingGraphic>().raycastTarget = isRaycastTarget;
		}
		for (int j = 0; j < _borderRegionsContainer.childCount; j++)
		{
			_borderRegionsContainer.GetChild(j).GetComponent<Image>().raycastTarget = isRaycastTarget;
		}
	}

	public void ShowTooltipIndustry(string slotUniqueId, float x, float y)
	{
		_controller.ShowTooltipIndustry(slotUniqueId, x, y, Screen.width, Screen.height);
	}

	public void ShowTooltipLink(string linkUniqueId, float x, float y)
	{
		_controller.ShowTooltipLink(linkUniqueId, x, y, Screen.width, Screen.height);
	}

	public void ShowTooltipBorderRegion(RegionName regionName, float x, float y)
	{
		_controller.ShowTooltipBorderRegion(regionName, x, y, Screen.width, Screen.height);
	}

	public void HideTooltip()
	{
		_controller.HideTooltip();
	}

	public virtual void ZoomOutToWholeMap(GameEra? gameEra = null)
	{
		Sequence s = DOTween.Sequence();
		s.AppendCallback(Canvas.ForceUpdateCanvases);
		RectTransform rectTransform = (RectTransform)base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content");
		s.Append(rectTransform.DOScale(0.028f, UIProperties.AnimationDuration));
		s.Insert(0f, rectTransform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, UIProperties.AnimationDuration).SetEase(Ease.OutQuart));
	}

	private void SetMapDayNight()
	{
		if (_controller.GetIsNightMap())
		{
			_background.GetComponent<Image>().sprite = _nightBoardElements[0];
			for (int i = 0; i < _background.childCount; i++)
			{
				_background.GetChild(i).GetComponent<Image>().sprite = _nightBoardElements[i + 1];
			}
		}
		else
		{
			_background.GetComponent<Image>().sprite = _dayBoardElements[0];
			for (int j = 0; j < _background.childCount; j++)
			{
				_background.GetChild(j).GetComponent<Image>().sprite = _dayBoardElements[j + 1];
			}
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_background = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/Background");
		_regionsContainer = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/Regions");
		_borderRegionsContainer = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/BorderRegions");
		_linksContainer = base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content/Links");
		_zoomScrollRect = base.transform.Find("SafeArea/Popup/Scroll View").GetComponent<ZoomScrollRect>();
		_audioSource = base.transform.GetComponent<AudioSource>();
		for (int i = 0; i < _regionsContainer.childCount; i++)
		{
			RegionView component = _regionsContainer.GetChild(i).GetComponent<RegionView>();
			regionGameObjects.Add(component.Name, component);
		}
		for (int j = 0; j < _borderRegionsContainer.childCount; j++)
		{
			BorderRegionView component2 = _borderRegionsContainer.GetChild(j).GetComponent<BorderRegionView>();
			borderRegionGameObjects.Add(component2.Name, component2);
		}
		foreach (Transform item in _regionsContainer)
		{
			Transform transform2 = item.Find("BuildingSlots/Row1");
			for (int k = 0; k < transform2.childCount; k++)
			{
				Transform component3 = transform2.GetChild(k).GetComponent<Transform>();
				if (component3.gameObject.activeSelf)
				{
					_buildingSlotsContainter.Add(component3);
				}
			}
			transform2 = item.Find("BuildingSlots/Row2");
			for (int l = 0; l < transform2.childCount; l++)
			{
				Transform component4 = transform2.GetChild(l).GetComponent<Transform>();
				if (component4.gameObject.activeSelf)
				{
					_buildingSlotsContainter.Add(component4);
				}
			}
		}
	}
}
