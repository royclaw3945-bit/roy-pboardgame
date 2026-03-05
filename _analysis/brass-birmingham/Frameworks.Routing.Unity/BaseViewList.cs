using System.Collections.Generic;
using System.Linq;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using UnityEngine;

namespace Frameworks.Routing.Unity;

public class BaseViewList : MonoBehaviour
{
	[SerializeField]
	private BaseView[] GameViews;

	public IList<IBaseView> Views => GameViews.Cast<IBaseView>().ToList();
}
