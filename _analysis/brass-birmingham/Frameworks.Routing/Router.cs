using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Utils.Logging;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace Frameworks.Routing;

public class Router : IRouter, IViewManager, IInputRegistration
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(Router));

	private readonly List<IBaseView> _viewStack = new List<IBaseView>();

	private readonly Dictionary<IBaseView, IList<KeyEnum>> _registeredKeys = new Dictionary<IBaseView, IList<KeyEnum>>();

	private readonly Dictionary<Type, IBaseView> _viewCache = new Dictionary<Type, IBaseView>();

	private readonly Dictionary<Type, IAnimation> _viewAnimations = new Dictionary<Type, IAnimation>();

	private readonly IViewFactory _viewFactory;

	private readonly IInputSource _inputSource;

	private bool handle = true;

	public IUseCaseBuilder UseCaseBuilder { get; set; }

	public Router(IViewFactory viewFactory, IInputSource inputSource)
	{
		_viewFactory = viewFactory;
		_inputSource = inputSource;
	}

	public void RegisterKey(KeyEnum keyEnum, IBaseView baseView)
	{
		if (_registeredKeys.ContainsKey(baseView))
		{
			_registeredKeys[baseView].Add(keyEnum);
			return;
		}
		_registeredKeys.Add(baseView, new List<KeyEnum> { keyEnum });
	}

	public void UnregisterKey(KeyEnum keyEnum, IBaseView baseView)
	{
		if (_registeredKeys.ContainsKey(baseView))
		{
			_registeredKeys[baseView].Remove(keyEnum);
		}
	}

	public void UnregisterAllKeys(IBaseView baseView)
	{
		if (_registeredKeys.ContainsKey(baseView))
		{
			_registeredKeys.Remove(baseView);
		}
	}

	public void ProcessInput()
	{
		handle = true;
		_viewStack.ToList().ForEach(delegate(IBaseView view)
		{
			if (_registeredKeys.ContainsKey(view))
			{
				_registeredKeys[view].ToList().ForEach(delegate(KeyEnum key)
				{
					if (handle && _inputSource.GetKeyDown(key))
					{
						view.HandleKeyDown(key);
						handle = false;
					}
					if (_inputSource.GetKey(key))
					{
						view.HandleKey(key);
						handle = false;
					}
					if (handle && _inputSource.GetKeyUp(key))
					{
						view.HandleKeyUp(key);
						handle = false;
					}
				});
			}
		});
	}

	public IAnimation ShowView<T>() where T : class, IBaseView
	{
		T viewToShow = GetView<T>();
		if (!IsViewVisible<T>())
		{
			Logger.Debug("ShowView - " + viewToShow);
			PushViewToStack(viewToShow);
		}
		if (_viewAnimations.ContainsKey(typeof(T)))
		{
			_viewAnimations[typeof(T)].Kill();
		}
		IAnimation animation = viewToShow.CreateShowViewAnimation();
		animation.AppendCallback(delegate
		{
			ShowViewCompleted(viewToShow);
		});
		animation.Play();
		_viewAnimations[typeof(T)] = animation;
		return animation;
	}

	public IAnimation HideView<T>() where T : class, IBaseView
	{
		T viewToHide = GetView<T>();
		Logger.Debug("HideView - " + viewToHide);
		PopViewFromStack(viewToHide);
		if (_viewAnimations.ContainsKey(typeof(T)))
		{
			_viewAnimations[typeof(T)].Kill();
		}
		IAnimation animation = viewToHide.CreateHideViewAnimation();
		animation.AppendCallback(delegate
		{
			HideViewCompleted(viewToHide);
		});
		animation.Play();
		_viewAnimations[typeof(T)] = animation;
		return animation;
	}

	public void MoveOnTop<T>() where T : class, IBaseView
	{
		T view = GetView<T>();
		_viewStack.Remove(view);
		view.MoveToTop();
		_viewStack.Insert(0, view);
	}

	public bool IsViewVisible<T>() where T : class, IBaseView
	{
		return _viewStack.Any((IBaseView v) => typeof(T).IsInstanceOfType(v));
	}

	public bool IsViewCached<T>() where T : class, IBaseView
	{
		return _viewCache.Any((KeyValuePair<Type, IBaseView> v) => typeof(T).IsInstanceOfType(v.Value));
	}

	public T GetView<T>() where T : class, IBaseView
	{
		if (UseCaseBuilder == null)
		{
			throw new InvalidOperationException("UseCaseBuilder is not set");
		}
		Type type = LookForInheritedViewType(typeof(T));
		if (_viewCache.ContainsKey(type))
		{
			return (T)_viewCache[type];
		}
		T val = _viewFactory.CreateView<T>();
		_viewCache[typeof(T)] = val;
		IController controller = UseCaseBuilder.GetController(type);
		if (controller != null)
		{
			val.Controller = controller;
			val.Controller.OnViewCreated();
		}
		return val;
	}

	private void PushViewToStack(IBaseView viewToShow)
	{
		viewToShow.MoveToTop();
		_viewStack.Insert(0, viewToShow);
	}

	private void ShowViewCompleted(IBaseView viewToShow)
	{
		Logger.Debug("ShowViewCompleted - " + viewToShow);
		_viewAnimations.Remove(viewToShow.GetType());
		if (_viewStack.Count <= 0 || !viewToShow.DisableMenusUnderneath)
		{
			return;
		}
		foreach (IBaseView item in _viewStack)
		{
			if (item != viewToShow)
			{
				item.DisableView();
				if (item.DisableMenusUnderneath)
				{
					break;
				}
			}
		}
	}

	private void PopViewFromStack(IBaseView viewToHide)
	{
		if (_viewStack.Count == 0)
		{
			throw new InvalidOperationException("Can't hide view. View stack is empty.");
		}
		HideTopView(viewToHide);
	}

	private void HideTopView(IBaseView viewToHide)
	{
		_viewStack.Remove(viewToHide);
		foreach (IBaseView item in _viewStack)
		{
			item.EnableView();
			if (item.DisableMenusUnderneath)
			{
				break;
			}
		}
	}

	private void HideViewCompleted(IBaseView viewToHide)
	{
		Logger.Debug("HideViewCompleted - " + viewToHide);
		_viewAnimations.Remove(viewToHide.GetType());
		if (viewToHide.DestroyWhenClosed)
		{
			viewToHide.Controller?.OnViewDestroyed();
			_viewFactory.DestroyView(viewToHide);
			KeyValuePair<Type, IBaseView> keyValuePair = _viewCache.First((KeyValuePair<Type, IBaseView> kvp) => kvp.Value == viewToHide);
			_viewCache.Remove(keyValuePair.Key);
		}
		else
		{
			viewToHide.DisableView();
		}
	}

	public Type LookForInheritedViewType(Type t)
	{
		for (int i = 0; i < _viewCache.Keys.Count; i++)
		{
			Type type = _viewCache.Keys.ElementAt(i);
			if (t.IsAssignableFrom(type))
			{
				return type;
			}
		}
		return t;
	}
}
