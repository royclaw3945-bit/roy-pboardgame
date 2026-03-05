using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InterfaceAdapters.Utils;

public static class DoubleDispatch
{
	public static void Dispatch(object instance, string methodName, object parameter)
	{
		Dispatch<object>(instance, methodName, typeof(void), parameter);
	}

	public static T Dispatch<T>(object instance, string methodName, Type returnType, object parameter)
	{
		if (parameter == null)
		{
			throw new ArgumentException("Event is null");
		}
		IEnumerable<MethodInfo> source = from m in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where m.Name == methodName && m.GetParameters().Length == 1 && !m.IsVirtual && GetType(m.GetParameters()[0].ParameterType.FullName).IsAssignableFrom(parameter.GetType()) && m.ReturnType == returnType
			select m;
		if (!source.Any())
		{
			throw new ArgumentException(string.Concat("Can't find method: ", typeof(T), " ", methodName, "(", parameter.GetType(), ")."));
		}
		return (T)source.Single().Invoke(instance, new object[1] { parameter });
	}

	private static Type GetType(string typeName)
	{
		Type type = Type.GetType(typeName);
		if (type != null)
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType(typeName);
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}
}
