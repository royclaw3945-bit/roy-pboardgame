using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class IndustriesDetailsView : BaseView, IIndustriesDetailsView, IBaseView
{
	private IndustriesDetailsController _controller;

	private GameObject _backButton;

	private Transform _coalMinesContainer;

	private Transform _ironWorksContainer;

	private Transform _potteriesContainer;

	private Transform _cottonMillsContainer;

	private Transform _manufacturersContainer;

	private Transform _breweriesContainer;

	private Transform _playersContainer;

	private AudioSource _audioSource;

	public AudioClip DevelopClip;

	private TextMeshProUGUI _linkTilesCountText;

	private Image _linkTilesCountTokenImage;

	private bool bMerchantBonus;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is IndustriesDetailsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateIndustriesDetails(IndustriesDetailsViewModel industriesDetailsViewModel, LinkCountViewModel linkCountViewModel)
	{
		_backButton.SetActive(value: true);
		bMerchantBonus = false;
		IterateGivenIndustryType(industriesDetailsViewModel.CoalMineList, _coalMinesContainer, BuildingType.CoalMine);
		IterateGivenIndustryType(industriesDetailsViewModel.IronWorksList, _ironWorksContainer, BuildingType.IronWorks);
		IterateGivenIndustryType(industriesDetailsViewModel.PotteryList, _potteriesContainer, BuildingType.Pottery);
		IterateGivenIndustryType(industriesDetailsViewModel.CottonMillList, _cottonMillsContainer, BuildingType.CottonMill);
		IterateGivenIndustryType(industriesDetailsViewModel.ManufactureList, _manufacturersContainer, BuildingType.Manufacturer);
		IterateGivenIndustryType(industriesDetailsViewModel.BreweryList, _breweriesContainer, BuildingType.Brewery);
		_linkTilesCountTokenImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Links/Token", linkCountViewModel.GameEra, linkCountViewModel.Color));
		_linkTilesCountText.text = string.Concat(linkCountViewModel.LinkCount);
	}

	private void IterateGivenIndustryType(List<IndustryViewModel> industryList, Transform industryContainer, BuildingType industryType)
	{
		int num = industryList.Count - 1;
		for (int i = 0; i < industryContainer.childCount; i++)
		{
			IndustryView component = industryContainer.GetChild(i).GetComponent<IndustryView>();
			if (num >= 0)
			{
				component.SetIndustry(industryList[num - i]);
				continue;
			}
			IndustryViewModel industry = new IndustryViewModel
			{
				BuildingType = industryType,
				IndustryAvailable = false,
				Level = i
			};
			component.SetIndustry(industry);
		}
	}

	public void HighlightIndustriesToDevelop(IndustriesDetailsViewModel industriesDetailsViewModel)
	{
		_backButton.SetActive(value: false);
		bMerchantBonus = false;
		_playersContainer.gameObject.SetActive(value: false);
		for (int i = 0; i < _coalMinesContainer.childCount; i++)
		{
			if (industriesDetailsViewModel.CoalMineHighestLevelIndex == _coalMinesContainer.childCount - 1 - i)
			{
				_coalMinesContainer.GetChild(i).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
		}
		for (int j = 0; j < _ironWorksContainer.childCount; j++)
		{
			if (industriesDetailsViewModel.IronWorksHighestLevelIndex == _ironWorksContainer.childCount - 1 - j)
			{
				_ironWorksContainer.GetChild(j).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
		}
		for (int k = 0; k < _potteriesContainer.childCount; k++)
		{
			if (industriesDetailsViewModel.PotteryHighestLevelIndex == _potteriesContainer.childCount - 1 - k)
			{
				_potteriesContainer.GetChild(k).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
			else if (k == 2 || k == 4)
			{
				_potteriesContainer.GetChild(k).GetComponent<IndustryView>().SetCantDevelopTooltips(OnCantDevelopHoverEnter, OnCantDevelopHoverExit);
			}
		}
		for (int l = 0; l < _cottonMillsContainer.childCount; l++)
		{
			if (industriesDetailsViewModel.CottonMillHighestLevelIndex == _cottonMillsContainer.childCount - 1 - l)
			{
				_cottonMillsContainer.GetChild(l).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
		}
		for (int m = 0; m < _manufacturersContainer.childCount; m++)
		{
			if (industriesDetailsViewModel.ManufactureHighestLevelIndex == _manufacturersContainer.childCount - 1 - m)
			{
				_manufacturersContainer.GetChild(m).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
		}
		for (int n = 0; n < _breweriesContainer.childCount; n++)
		{
			if (industriesDetailsViewModel.BreweryHighestLevelIndex == _breweriesContainer.childCount - 1 - n)
			{
				_breweriesContainer.GetChild(n).GetComponent<IndustryView>().HighlightIndustry(OnIndustryClicked);
			}
		}
	}

	public void SetAsMerchantBonus()
	{
		bMerchantBonus = true;
	}

	public IAnimation CreateIndustryDevelopedAnimation(BuildingType buildingType, float delay = 0f)
	{
		GameObject developedIndustryGameObject = GetDevelopedIndustryGameObject(buildingType);
		IndustryView industryView = developedIndustryGameObject.GetComponent<IndustryView>();
		Sequence sequence = DOTween.Sequence();
		Image image = industryView.Tokens[industryView.Tokens.Count - 1];
		CanvasGroup component = image.transform.Find("IconsContainer").GetComponent<CanvasGroup>();
		sequence.AppendInterval(delay);
		sequence.AppendCallback(delegate
		{
			industryView.OutlineImage.gameObject.SetActive(value: false);
			_audioSource.PlayOneShot(DevelopClip);
		});
		sequence.Append(image.transform.DOBlendableRotateBy(new Vector3(0f, 0f, 30f), 11f * UIProperties.FrameDuration).SetEase(Ease.OutExpo));
		sequence.Append(image.transform.DOScale(Vector3.one * 5f, 20f * UIProperties.FrameDuration).SetEase(Ease.OutExpo));
		sequence.Join(image.DOFade(0f, 20f * UIProperties.FrameDuration).SetEase(Ease.OutExpo));
		sequence.Join(component.DOFade(0f, 20f * UIProperties.FrameDuration).SetEase(Ease.OutExpo));
		sequence.AppendInterval(delay / 2f);
		return new DOTweenAnimationSequence(sequence);
	}

	private GameObject GetDevelopedIndustryGameObject(BuildingType buildingType)
	{
		return buildingType switch
		{
			BuildingType.Brewery => GetLastActiveObject(_breweriesContainer), 
			BuildingType.CoalMine => GetLastActiveObject(_coalMinesContainer), 
			BuildingType.CottonMill => GetLastActiveObject(_cottonMillsContainer), 
			BuildingType.IronWorks => GetLastActiveObject(_ironWorksContainer), 
			BuildingType.Manufacturer => GetLastActiveObject(_manufacturersContainer), 
			BuildingType.Pottery => GetLastActiveObject(_potteriesContainer), 
			_ => throw new ArgumentOutOfRangeException("buildingType", buildingType, null), 
		};
	}

	private GameObject GetLastActiveObject(Transform container)
	{
		for (int num = container.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = container.GetChild(num).gameObject;
			if (gameObject.GetComponent<IndustryView>().TokenImage.IsActive())
			{
				return gameObject;
			}
		}
		throw new InvalidOperationException("No active child in given container");
	}

	private void OnIndustryClicked(BuildingType buildingType)
	{
		_controller.OnIndustryClicked(buildingType);
	}

	private void OnCantDevelopHoverEnter(float x, float y)
	{
		_controller.OnCantDevelopHoverEnter(x, y);
	}

	private void OnCantDevelopHoverExit()
	{
		_controller.OnCantDevelopHoverExit();
	}

	public void OnBackClicked()
	{
		_controller.OnBackClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape && !bMerchantBonus)
		{
			OnBackClicked();
		}
	}

	public void OnPlayerPortraitClicker(int buttonNumber)
	{
		PlayerColor playerColor = _playersContainer.GetChild(buttonNumber).GetChild(0).GetComponent<PlayerView>()
			.PlayerColor;
		_controller.OnPlayerPortraitClicked(playerColor);
	}

	public void SetPlayerSummary(List<PlayerViewModel> playerViewModels)
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			if (playerViewModels.Count > i)
			{
				_playersContainer.GetChild(i).GetChild(0).GetComponent<PlayerView>()
					.SetPlayer(playerViewModels[i]);
				if (playerViewModels[i].IsCurrent)
				{
					_playersContainer.GetChild(i).GetChild(0).GetComponent<RectTransform>()
						.anchoredPosition += new Vector2(35f, 0f);
				}
			}
			else
			{
				_playersContainer.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
			}
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_backButton = base.transform.Find("SafeArea/Popup/Panel/WindowFrame/ButtonClose").gameObject;
		_coalMinesContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/CoalMines");
		_ironWorksContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/IronWorks");
		_potteriesContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/Potteries");
		_cottonMillsContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/CottonMills");
		_manufacturersContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/Manufacturers");
		_breweriesContainer = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/Breweries");
		_playersContainer = base.transform.Find("PlayersContainer");
		_audioSource = GetComponent<AudioSource>();
		_linkTilesCountText = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/IndustryLinksIndicator/BlackBackground/BackgroundLinksCount/LinksCountText").GetComponent<TextMeshProUGUI>();
		_linkTilesCountTokenImage = base.transform.Find("SafeArea/Popup/Panel/PlayerMat/Background/IndustryLinksIndicator/BlackBackground/Token").GetComponent<Image>();
	}
}
