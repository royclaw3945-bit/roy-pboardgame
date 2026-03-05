using System;
using BoardGameRules.Utils.Logging;

namespace Utils.Wireup;

public class ContainerRegistration
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ContainerRegistration));

	private readonly Func<NanoContainer, object> _resolve;

	private object _instance;

	private bool _instancePerCall;

	private string _name;

	public string Name => _name;

	public ContainerRegistration(Func<NanoContainer, object> resolve)
	{
		Logger.Verbose("Adding wireup registration callback.");
		_resolve = resolve;
	}

	public ContainerRegistration(object instance)
	{
		Logger.Verbose("Adding wireup registration for an object instance of type '{0}'.", instance.GetType());
		_instance = instance;
	}

	public virtual ContainerRegistration InstancePerCall()
	{
		Logger.Verbose("Registration configured to resolve a new instance per call.");
		_instancePerCall = true;
		return this;
	}

	public virtual object Resolve(NanoContainer container)
	{
		Logger.Verbose("Resolving instance.");
		if (_instancePerCall)
		{
			Logger.Verbose("Building new instance.");
			return _resolve(container);
		}
		Logger.Verbose("Attempting to resolve existing instance.");
		if (_instance != null)
		{
			return _instance;
		}
		Logger.Verbose("Building (and storing) new instance for later calls.");
		return _instance = _resolve(container);
	}

	public virtual ContainerRegistration Named(string name)
	{
		Logger.Verbose("Registration configured with name: " + name);
		_name = name;
		return this;
	}
}
