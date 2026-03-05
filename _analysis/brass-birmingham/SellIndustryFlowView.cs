using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.SellIndustryFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellIndustryFlowView : BaseView, ISellIndustryFlowView, IBaseView
{
	[SerializeField]
	private IndustryView _industryPrefab;

	[SerializeField]
	private GameObject _vpPrefab;

	[SerializeField]
	private GameObject _rowPrefab;

	private Transform _industriesContainer;

	private Button _confirmButton;

	private Action _actionOnClickConfirmButton = delegate
	{
	};

	private SellIndustryFlowController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is SellIndustryFlowController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_confirmButton.onClick.AddListener(delegate
		{
			_actionOnClickConfirmButton();
		});
	}

	public void UpdateView(Dictionary<PlayerColor, List<BaseIndustry>> industries, Dictionary<PlayerColor, int> lostVPs, Action onConfirmAction)
	{
		_actionOnClickConfirmButton = onConfirmAction;
		RemoveAllIndustries();
		GenerateRows(industries, lostVPs);
	}

	private void RemoveAllIndustries()
	{
		for (int num = _industriesContainer.childCount - 2; num >= 1; num--)
		{
			UnityEngine.Object.Destroy(_industriesContainer.GetChild(num).gameObject);
		}
	}

	private void GenerateRows(Dictionary<PlayerColor, List<BaseIndustry>> industries, Dictionary<PlayerColor, int> lostVPs)
	{
		foreach (KeyValuePair<PlayerColor, List<BaseIndustry>> industry in industries)
		{
			if (industry.Value.Count <= 0)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_rowPrefab, _industriesContainer);
			gameObject.transform.SetSiblingIndex(_industriesContainer.childCount - 2);
			foreach (BaseIndustry item in industry.Value)
			{
				UnityEngine.Object.Instantiate(_industryPrefab, gameObject.transform).SetIndustry(item);
			}
			if (lostVPs.ContainsKey(industry.Key) && lostVPs[industry.Key] > 0)
			{
				int num = lostVPs[industry.Key];
				UnityEngine.Object.Instantiate(_vpPrefab, gameObject.transform).GetComponentInChildren<TextMeshProUGUI>().text = string.Concat(num);
			}
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_industriesContainer = base.transform.Find("SafeArea/Popup/Vertical");
		_confirmButton = base.transform.Find("SafeArea/Popup/Vertical/ConfirmButton").GetComponent<Button>();
	}
}
