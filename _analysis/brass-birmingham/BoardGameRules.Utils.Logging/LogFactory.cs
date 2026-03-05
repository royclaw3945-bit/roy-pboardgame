using System;

namespace BoardGameRules.Utils.Logging;

public static class LogFactory
{
	public class NullLogger : ILog
	{
		public void Verbose(string message, params object[] values)
		{
		}

		public void Debug(string message, params object[] values)
		{
		}

		public void Info(string message, params object[] values)
		{
		}

		public void Warn(string message, params object[] values)
		{
		}

		public void Error(string message, params object[] values)
		{
		}

		public void Fatal(string message, params object[] values)
		{
		}
	}

	public static Func<Type, ILog> BuildLogger { get; set; }

	static LogFactory()
	{
		NullLogger logger = new NullLogger();
		BuildLogger = (Type type) => logger;
	}
}
