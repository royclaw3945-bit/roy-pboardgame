using System.Linq;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using UnityEngine;

namespace Frameworks.Routing.Unity;

public class UnityViewFactory : MonoBehaviour, IViewFactory
{
	public Transform ParentTransform;

	public BaseViewList ViewList;

	public T CreateView<T>() where T : class, IBaseView
	{
		BaseView baseView = Object.Instantiate(GetPrefab<T>(), ParentTransform);
		baseView.name = typeof(T).Name;
		baseView.DisableView();
		return baseView as T;
	}

	public void DestroyView(IBaseView view)
	{
		BaseView baseView = (BaseView)view;
		if (!(baseView == null))
		{
			Object.Destroy(baseView.gameObject);
		}
	}

	private BaseView GetPrefab<T>() where T : class, IBaseView
	{
		Debug.Log("Trying to get prefab for " + typeof(T));
		BaseView baseView = ViewList.Views.First((IBaseView view) => view is T) as BaseView;
		if (baseView != null)
		{
			return baseView;
		}
		throw new MissingReferenceException("Prefab not found for type " + typeof(T));
	}
}
