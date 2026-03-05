using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Tutorials.Board;
using UnityEngine;

public class TutorialBoardView : BoardView, ITutorialBoardView, IBoardView, IBaseView
{
	public void ScrollToPosition(float x, float y, float scale)
	{
		Sequence s = DOTween.Sequence();
		s.AppendCallback(Canvas.ForceUpdateCanvases);
		RectTransform rectTransform = (RectTransform)base.transform.Find("SafeArea/Popup/Scroll View/Viewport/Content");
		s.Append(rectTransform.DOScale(scale, UIProperties.AnimationDuration));
		s.Insert(0f, rectTransform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(x, y), UIProperties.AnimationDuration).SetEase(Ease.OutQuart));
	}

	public override void ZoomOutToWholeMap(GameEra? gameEra = null)
	{
		if (!gameEra.HasValue || gameEra == GameEra.RailEra)
		{
			base.ZoomOutToWholeMap(gameEra);
		}
		else
		{
			ScrollToPosition(4.96f, -412f, 0.054f);
		}
	}

	public void EnableScrolling(bool isEnabled)
	{
		if (isEnabled)
		{
			scrollSpeed = 1000f;
			_zoomScrollRect.SetBlocked(blocked: false);
		}
		else
		{
			scrollSpeed = 0f;
			_zoomScrollRect.SetBlocked(blocked: true);
		}
	}

	public void HideAllMerchantRegions()
	{
		for (int i = 0; i < _borderRegionsContainer.childCount; i++)
		{
			string value = _borderRegionsContainer.GetChild(i).GetComponent<BorderRegionView>().Name.ToString();
			for (int j = 0; j < _linksContainer.childCount; j++)
			{
				if (_linksContainer.GetChild(j).name.Contains(value))
				{
					_linksContainer.GetChild(j).GetComponent<CanvasGroup>().DOFade(0f, 1f);
				}
			}
			_borderRegionsContainer.GetChild(i).GetComponent<CanvasGroup>().DOFade(0f, 1f);
		}
	}

	public void ShowRegion(RegionName regionName, float delay = 0f)
	{
		Sequence s = DOTween.Sequence();
		s.AppendCallback(Canvas.ForceUpdateCanvases);
		s.AppendInterval(delay);
		for (int i = 0; i < _linksContainer.childCount; i++)
		{
			if (_linksContainer.GetChild(i).name.Contains(regionName.ToString()))
			{
				s.Insert(delay, _linksContainer.GetChild(i).GetComponent<CanvasGroup>().DOFade(1f, 2f));
			}
		}
		for (int j = 0; j < _borderRegionsContainer.childCount; j++)
		{
			if (_borderRegionsContainer.GetChild(j).GetComponent<BorderRegionView>().Name == regionName)
			{
				s.Insert(delay, _borderRegionsContainer.GetChild(j).GetComponent<CanvasGroup>().DOFade(1f, 2f));
			}
		}
	}

	public (float x, float y) GetBuildingSlotPosition(string buildingSlotUniqueId, RegionName regionName)
	{
		Vector3 position = ((RegionView)GetRegionViewForGivenRegionName(regionName))._buildingSlots.Find((GameObject slot) => slot.GetComponent<BuildingSlotView>().name == buildingSlotUniqueId).transform.position;
		return (x: position.x, y: position.y);
	}
}
