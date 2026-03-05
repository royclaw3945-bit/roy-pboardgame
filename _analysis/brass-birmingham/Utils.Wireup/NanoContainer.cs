using System;
using System.Collections.Generic;
using BoardGameRules.Utils.Logging;
using Utils.Collections.Utils;

namespace Utils.Wireup;

public class NanoContainer
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NanoContainer));

	private readonly MultiMap<Type, ContainerRegistration> _registrations = new MultiMap<Type, ContainerRegistration>();

	public virtual ContainerRegistration Register<TService>(Func<NanoContainer, TService> resolve) where TService : class
	{
		Logger.Verbose("Registering wireup resolver for service of type '{0}'.", typeof(TService));
		ContainerRegistration containerRegistration = new ContainerRegistration((NanoContainer c) => resolve(c));
		_registrations.Add(typeof(TService), containerRegistration);
		return containerRegistration;
	}

	public virtual ContainerRegistration Register<TService>(TService instance)
	{
		if (object.Equals(instance, null))
		{
			throw new ArgumentNullException("instance", "The instance provided cannot be null.");
		}
		if (!typeof(TService).IsValueType && !typeof(TService).IsInterface)
		{
			throw new ArgumentException("The type provided must be registered as an interface rather than as a concrete type.", "instance");
		}
		Logger.Verbose("Registering wireup instance for service of type '{0}'.", typeof(TService));
		ContainerRegistration containerRegistration = new ContainerRegistration(instance);
		_registrations.Add(typeof(TService), containerRegistration);
		return containerRegistration;
	}

	public virtual TService Resolve<TService>()
	{
		Logger.Verbose("Attempting to resolve instance for service of type '{0}'.", typeof(TService));
		List<ContainerRegistration> list = _registrations[typeof(TService)];
		if (list.Count > 0)
		{
			return (TService)list[list.Count - 1].Resolve(this);
		}
		Logger.Warn("Unable to resolve requested instance of type '{0}'.", typeof(TService));
		return default(TService);
	}

	public virtual TService ResolveNamed<TService>(string name)
	{
		Logger.Verbose("Attempting to resolve instance for service of type '{0}' named {1}.", typeof(TService), name);
		foreach (ContainerRegistration item in _registrations[typeof(TService)])
		{
			if (name.Equals(item.Name))
			{
				return (TService)item.Resolve(this);
			}
		}
		Logger.Warn("Unable to resolve requested instance of type '{0}' and name {1}.", typeof(TService), name);
		return default(TService);
	}
}
