using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using Boo.Lang;
using Frameworks.Utils;
using InterfaceAdapters.Game.IndustriesSummary;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IndustryView : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private readonly Vector3 TOKEN_STACK_DISPLACEMENT = new Vector3(-5.7f, 5.7f, 0f);

	protected BuildingType BuildingType;

	private Image _iconImage;

	private TextMeshProUGUI _levelText;

	private TextMeshProUGUI _moneyCostText;

	private TextMeshProUGUI _vpText;

	private TextMeshProUGUI _incomeText;

	private Image _vpLink;

	private Transform _coalCostContainer;

	private Transform _ironCostContainer;

	private Image _beerCostContainer;

	private Image _coalSourceContainer;

	private Image _ironSourceContainer;

	private Image _beerSourceContainer;

	public GameObject _leftPanel;

	public GameObject _rightPanel;

	private GameObject _cantDevelopContainer;

	private Button _button;

	private Action<BuildingType> _onIndustryClicked = delegate
	{
	};

	private Action<float, float> _onCantDevelopHoverEnter = delegate
	{
	};

	private Action _onCantDevelopHoverExit = delegate
	{
	};

	private GameObject _canalEra;

	private GameObject _railEra;

	private GameObject _iconsContainer;

	public List<Image> Tokens { get; private set; }

	public Image TokenImage { get; private set; }

	public Image OutlineImage { get; private set; }

	public new virtual void Awake()
	{
		PrepareReferences();
	}

	public virtual void SetIndustry(IndustryViewModel industryViewModel)
	{
		BuildingType = industryViewModel.BuildingType;
		_iconImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Industries/Board", industryViewModel.BuildingType, "Type"));
		TokenImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Industries/", industryViewModel.Color, industryViewModel.BuildingType));
		_levelText.text = UIProperties.GetRomanNumber(industryViewModel.Level);
		TokenImage.gameObject.SetActive(industryViewModel.IndustryAvailable);
		_iconsContainer.SetActive(industryViewModel.IndustryAvailable);
		if (Tokens != null)
		{
			for (int num = Tokens.Count - 1; num > 0; num--)
			{
				UnityEngine.Object.Destroy(Tokens[num].gameObject);
				Tokens.RemoveAt(num);
			}
		}
		_moneyCostText.text = industryViewModel.MoneyCost.ToString();
		_vpText.text = industryViewModel.VPForBuilding.ToString();
		_incomeText.text = industryViewModel.IncomeMarkerIncrease.ToString();
		_vpLink.transform.parent.gameObject.SetActive(industryViewModel.VpPerLink > 0);
		_vpLink.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconVpLink" + industryViewModel.VpPerLink);
		_coalCostContainer.GetChild(0).gameObject.SetActive(industryViewModel.CoalCost > 0);
		_coalCostContainer.GetChild(1).gameObject.SetActive(industryViewModel.CoalCost > 1);
		_ironCostContainer.GetChild(0).gameObject.SetActive(industryViewModel.IronCost > 0);
		_ironCostContainer.GetChild(1).gameObject.SetActive(industryViewModel.IronCost > 1);
		_railEra.SetActive(industryViewModel.OnlyRailEra);
		_canalEra.SetActive(industryViewModel.OnlyCanalEra);
		_onIndustryClicked = delegate
		{
		};
		if (!industryViewModel.IndustryAvailable)
		{
			return;
		}
		_cantDevelopContainer.SetActive(!industryViewModel.CanBeDeveloped);
		_coalSourceContainer.transform.parent.gameObject.SetActive(industryViewModel.HasCoal);
		_coalSourceContainer.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconCoal" + industryViewModel.CoalSource);
		_ironSourceContainer.transform.parent.gameObject.SetActive(industryViewModel.HasIron);
		_ironSourceContainer.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconIron" + industryViewModel.IronSource);
		_beerSourceContainer.transform.parent.gameObject.SetActive(industryViewModel.HasBeer);
		_beerSourceContainer.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconBeer" + industryViewModel.BeerSource);
		_beerCostContainer.transform.parent.gameObject.SetActive(industryViewModel.BeerCost > 0);
		_beerCostContainer.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconBeer" + industryViewModel.BeerCost);
		OutlineImage.gameObject.SetActive(industryViewModel.Active);
		Tokens = new List<Image>(industryViewModel.SameLevelCount);
		if (industryViewModel.SameLevelCount > 0)
		{
			Tokens.Add(TokenImage);
			for (int num2 = 1; num2 < industryViewModel.SameLevelCount; num2++)
			{
				Tokens.Add(UnityEngine.Object.Instantiate(TokenImage.gameObject, TokenImage.transform.position + Vector3.Scale(TOKEN_STACK_DISPLACEMENT, TokenImage.transform.lossyScale) * num2, Quaternion.identity, TokenImage.transform.parent).GetComponent<Image>());
			}
			OutlineImage.transform.position = Tokens[Tokens.Count - 1].transform.position;
		}
		OutlineImage.transform.SetAsLastSibling();
	}

	public void SetIndustry(BaseIndustry industry)
	{
		BuildingType = industry.BuildingType;
		_iconImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Industries/Board", industry.BuildingType, "Type"));
		TokenImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Industries/", industry.Color, industry.BuildingType));
		_levelText.text = UIProperties.GetRomanNumber(industry.Level);
		TokenImage.gameObject.SetActive(value: true);
		_iconsContainer.SetActive(value: true);
		if (Tokens != null)
		{
			for (int num = Tokens.Count - 1; num > 0; num--)
			{
				UnityEngine.Object.Destroy(Tokens[num].gameObject);
				Tokens.RemoveAt(num);
			}
		}
		_moneyCostText.text = industry.MoneyCost.ToString();
		_vpText.text = industry.VPForBuilding.ToString();
		_onIndustryClicked = delegate
		{
		};
		_leftPanel.SetActive(value: false);
		_rightPanel.SetActive(value: false);
		_coalSourceContainer.transform.parent.gameObject.SetActive(value: false);
		_ironSourceContainer.transform.parent.gameObject.SetActive(value: false);
		_beerSourceContainer.transform.parent.gameObject.SetActive(value: false);
		_beerCostContainer.transform.parent.gameObject.SetActive(value: false);
	}

	public void HighlightIndustry(Action<BuildingType> onIndustryClicked)
	{
		OutlineImage.gameObject.SetActive(value: true);
		_onIndustryClicked = onIndustryClicked;
	}

	public void SetCantDevelopTooltips(Action<float, float> onCantDevelopHoverEnter, Action onCantDevelopHoverExit)
	{
		_onCantDevelopHoverEnter = onCantDevelopHoverEnter;
		_onCantDevelopHoverExit = onCantDevelopHoverExit;
	}

	private void OnIndustryClicked()
	{
		_onIndustryClicked(BuildingType);
	}

	protected virtual void PrepareReferences()
	{
		_rightPanel = base.transform.Find("RightPanel").gameObject;
		_vpText = base.transform.Find("RightPanel/VP/VPText").GetComponent<TextMeshProUGUI>();
		_incomeText = base.transform.Find("RightPanel/Income/IncomeText").GetComponent<TextMeshProUGUI>();
		_vpLink = base.transform.Find("RightPanel/VPForLinks/Link").GetComponent<Image>();
		_vpLink = base.transform.Find("RightPanel/VPForLinks/Link").GetComponent<Image>();
		_leftPanel = base.transform.Find("LeftPanel").gameObject;
		_moneyCostText = base.transform.Find("LeftPanel/CostMoney/CostMoneyText").GetComponent<TextMeshProUGUI>();
		_coalCostContainer = base.transform.Find("LeftPanel/ResourcesCost/Coal");
		_ironCostContainer = base.transform.Find("LeftPanel/ResourcesCost/Iron");
		_beerCostContainer = base.transform.Find("BlackBackground/Token/IconsContainer/BeerCostContainer/BreweryCost").GetComponent<Image>();
		_levelText = base.transform.Find("BlackBackground/BackgroundLevel/LevelIndustry").GetComponent<TextMeshProUGUI>();
		_coalSourceContainer = base.transform.Find("BlackBackground/Token/IconsContainer/CoalContainer/SellCoalStock").GetComponent<Image>();
		_ironSourceContainer = base.transform.Find("BlackBackground/Token/IconsContainer/IronContainer/SellIronStock").GetComponent<Image>();
		_beerSourceContainer = base.transform.Find("BlackBackground/Token/IconsContainer/BeerSourceContainer/BrewerySource").GetComponent<Image>();
		_cantDevelopContainer = base.transform.Find("BlackBackground/Token/IconsContainer/CantDevelopContainer").gameObject;
		_iconImage = base.transform.Find("BlackBackground").GetComponent<Image>();
		_railEra = base.transform.Find("LeftPanel/EraRail").gameObject;
		_canalEra = base.transform.Find("LeftPanel/EraCanal").gameObject;
		_button = base.transform.GetComponent<Button>();
		TokenImage = base.transform.Find("BlackBackground/Token").GetComponent<Image>();
		OutlineImage = base.transform.Find("BlackBackground/Outline").GetComponent<Image>();
		_iconsContainer = base.transform.Find("BlackBackground/Token/IconsContainer").gameObject;
		_button.onClick.AddListener(OnIndustryClicked);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (_cantDevelopContainer.activeSelf)
		{
			_onCantDevelopHoverEnter(base.transform.position.x, base.transform.position.y);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_onCantDevelopHoverExit();
	}
}
