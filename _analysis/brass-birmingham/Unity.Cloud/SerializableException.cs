using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Cloud;

public class SerializableException
{
	public string DetailedProblemIdentifier { get; set; }

	public string FullText { get; set; }

	public SerializableException InnerException { get; set; }

	public string Message { get; set; }

	public string ProblemIdentifier { get; set; }

	public List<SerializableStackFrame> StackTrace { get; set; }

	public string Type { get; set; }

	public SerializableException()
	{
	}

	public SerializableException(Exception exception)
	{
		Message = exception.Message;
		FullText = exception.ToString();
		Type type = exception.GetType();
		Type = type.FullName;
		StackTrace = new List<SerializableStackFrame>();
		StackFrame[] frames = new StackTrace(exception, fNeedFileInfo: true).GetFrames();
		foreach (StackFrame stackFrame in frames)
		{
			StackTrace.Add(new SerializableStackFrame(stackFrame));
		}
		if (StackTrace.Count > 0)
		{
			SerializableStackFrame serializableStackFrame = StackTrace[0];
			ProblemIdentifier = $"{Type} at {serializableStackFrame.DeclaringType}.{serializableStackFrame.MethodName}";
		}
		else
		{
			ProblemIdentifier = Type;
		}
		if (StackTrace.Count > 1)
		{
			SerializableStackFrame serializableStackFrame2 = StackTrace[0];
			SerializableStackFrame serializableStackFrame3 = StackTrace[1];
			DetailedProblemIdentifier = $"{Type} at {serializableStackFrame2.DeclaringType}.{serializableStackFrame2.MethodName} from {serializableStackFrame3.DeclaringType}.{serializableStackFrame3.MethodName}";
		}
		if (exception.InnerException != null)
		{
			InnerException = new SerializableException(exception.InnerException);
		}
	}
}
