using System;
using UnityEngine.EventSystems;

namespace Frameworks.View.Game;

public class IncomeTrackMoneyIncome : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Action ActionOnHover = delegate
	{
	};

	public Action ActionOnHoverExit = delegate
	{
	};

	public void OnPointerEnter(PointerEventData eventData)
	{
		ActionOnHover?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ActionOnHoverExit?.Invoke();
	}
}
