using System;
using BoardGameRules.Utils.Logging;
using UnityEngine;

namespace Utils.Logging;

public class UnityLogger : ILog
{
	private readonly Type _typeToLog;

	public UnityLogger(Type typeToLog)
	{
		_typeToLog = typeToLog;
	}

	public virtual void Verbose(string message, params object[] values)
	{
	}

	public virtual void Debug(string message, params object[] values)
	{
	}

	public virtual void Info(string message, params object[] values)
	{
	}

	public virtual void Warn(string message, params object[] values)
	{
		UnityEngine.Debug.LogWarning(message.FormatMessage(_typeToLog, values));
	}

	public virtual void Error(string message, params object[] values)
	{
		UnityEngine.Debug.LogError(message.FormatMessage(_typeToLog, values));
	}

	public virtual void Fatal(string message, params object[] values)
	{
		UnityEngine.Debug.LogError(message.FormatMessage(_typeToLog, values));
	}
}
