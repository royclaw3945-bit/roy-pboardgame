using System;
using BoardGameRules.Entities.Players;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Frameworks.View.Game;

public class PlayerTokenView : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Action<PlayerColor, Vector3> _onPointerEnter;

	public Action _onPointerExit;

	public PlayerColor playerColor;

	public void OnPointerEnter(PointerEventData eventData)
	{
		_onPointerEnter(playerColor, base.transform.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_onPointerExit();
	}
}
