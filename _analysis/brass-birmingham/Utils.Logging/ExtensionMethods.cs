using System;
using System.Globalization;
using System.Threading;

namespace Utils.Logging;

internal static class ExtensionMethods
{
	private const string MessageFormat = "{0:yyyy/MM/dd HH:mm:ss.ff} - {1} - {2} - {3}";

	private const string MessageJSONFormat = "{{\"timestamp\":\"{0:yyyy-MM-ddTHH:mm:ss.ffffffZ}\",\"thread\": \"{1}\",\"class\": \"{2}\",\"message\": \"{3}\"}}";

	public static string FormatMessage(this string message, Type typeToLog, params object[] values)
	{
		return string.Format(CultureInfo.InvariantCulture, "{0:yyyy/MM/dd HH:mm:ss.ff} - {1} - {2} - {3}", DateTime.Now, Thread.CurrentThread.GetName(), typeToLog.GetFriendlyName(), string.Format(CultureInfo.InvariantCulture, message, values));
	}

	public static string FormatJSONMessage(this string message, Type typeToLog, params object[] values)
	{
		return string.Format(CultureInfo.InvariantCulture, "{{\"timestamp\":\"{0:yyyy-MM-ddTHH:mm:ss.ffffffZ}\",\"thread\": \"{1}\",\"class\": \"{2}\",\"message\": \"{3}\"}}", DateTime.UtcNow, Thread.CurrentThread.GetName(), typeToLog.GetFriendlyName(), string.Format(CultureInfo.InvariantCulture, message, values));
	}

	private static string GetFriendlyName(this Type type, bool fullName = true)
	{
		if (type == typeof(int))
		{
			return "int";
		}
		if (type == typeof(short))
		{
			return "short";
		}
		if (type == typeof(byte))
		{
			return "byte";
		}
		if (type == typeof(bool))
		{
			return "bool";
		}
		if (type == typeof(long))
		{
			return "long";
		}
		if (type == typeof(float))
		{
			return "float";
		}
		if (type == typeof(double))
		{
			return "double";
		}
		if (type == typeof(decimal))
		{
			return "decimal";
		}
		if (type == typeof(string))
		{
			return "string";
		}
		if (type.IsGenericType)
		{
			string text = (fullName ? type.FullName : type.Name);
			text = text.Split('`')[0] + "<";
			bool flag = true;
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					text += ",";
				}
				text += type2.GetFriendlyName(fullName: false);
			}
			return text + ">";
		}
		if (!fullName)
		{
			return type.Name;
		}
		return type.FullName;
	}

	private static string GetName(this Thread thread)
	{
		if (string.IsNullOrEmpty(thread.Name))
		{
			return thread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
		}
		return thread.Name;
	}
}
