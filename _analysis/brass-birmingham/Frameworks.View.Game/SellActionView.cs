using System;
using System.Collections.Generic;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.SellAction;
using TMPro;
using UnityEngine;

namespace Frameworks.View.Game;

public class SellActionView : BaseView, ISellActionView, IBaseView
{
	private SellActionController _controller;

	private Transform _industriesGridLayout;

	private TextMeshProUGUI _numberOfSalesText;

	public IndustryToSellView IndustryPrefab;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is SellActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void SetSellActionView(List<string> industries)
	{
		foreach (string industry in industries)
		{
			UnityEngine.Object.Instantiate(IndustryPrefab, _industriesGridLayout).SetIndustry(industry);
		}
		_numberOfSalesText.text = industries.Count.ToString();
	}

	public void OnEndButtonClicked()
	{
		_controller.OnEndButtonClicked();
	}

	public void OnSellAgainButtonClicked()
	{
		_controller.OnSellAgainButtonClicked();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_industriesGridLayout = base.transform.Find("SafeArea/Popup/Panel/IndustriesPanel");
		_numberOfSalesText = base.transform.Find("SafeArea/Popup/Panel/PossibleSalesText/Number").GetComponent<TextMeshProUGUI>();
	}
}
