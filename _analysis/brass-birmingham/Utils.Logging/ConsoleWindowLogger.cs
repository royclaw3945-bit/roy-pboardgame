using System;
using BoardGameRules.Utils.Logging;

namespace Utils.Logging;

public class ConsoleWindowLogger : ILog
{
	private static readonly object Sync = new object();

	private readonly ConsoleColor _originalColor = Console.ForegroundColor;

	private readonly Type _typeToLog;

	public ConsoleWindowLogger(Type typeToLog)
	{
		Console.ForegroundColor = ConsoleColor.DarkGray;
		_typeToLog = typeToLog;
	}

	public virtual void Verbose(string message, params object[] values)
	{
	}

	public virtual void Debug(string message, params object[] values)
	{
		Log(ConsoleColor.Green, message, values);
	}

	public virtual void Info(string message, params object[] values)
	{
		Log(ConsoleColor.DarkGray, message, values);
	}

	public virtual void Warn(string message, params object[] values)
	{
		Log(ConsoleColor.Yellow, message, values);
	}

	public virtual void Error(string message, params object[] values)
	{
		Log(ConsoleColor.DarkRed, message, values);
	}

	public virtual void Fatal(string message, params object[] values)
	{
		Log(ConsoleColor.Red, message, values);
	}

	private void Log(ConsoleColor color, string message, params object[] values)
	{
		lock (Sync)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message.FormatMessage(_typeToLog, values));
			Console.ForegroundColor = _originalColor;
		}
	}
}
