using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.NetworkAction;
using TMPro;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class NetworkActionView : BaseView, INetworkActionView, IBaseView
{
	private NetworkActionController _controller;

	private Button _anotherLinkButton;

	private TextMeshProUGUI _cantPlaceAnotherText;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is NetworkActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void OnEndButtonClicked()
	{
		_controller.OnEndButtonClicked();
	}

	public void OnLinkAgainButtonClicked()
	{
		_controller.OnLinkAgainButtonClicked();
	}

	public void EnableBuildingAnotherLink(bool enableBuilding)
	{
		_cantPlaceAnotherText.gameObject.SetActive(!enableBuilding);
		_anotherLinkButton.interactable = enableBuilding;
	}

	public void SetCantBuildMessage(string message)
	{
		_cantPlaceAnotherText.text = message;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_cantPlaceAnotherText = base.transform.Find("SafeArea/Popup/Panel/CantBuildText").GetComponent<TextMeshProUGUI>();
		_anotherLinkButton = base.transform.Find("SafeArea/Popup/Panel/PlaceAnotherLinkButton").GetComponent<Button>();
	}
}
