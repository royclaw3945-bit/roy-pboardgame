using System;
using System.Diagnostics;
using System.Reflection;

namespace Unity.Cloud;

public class SerializableStackFrame
{
	public string DeclaringType { get; set; }

	public int FileColumn { get; set; }

	public int FileLine { get; set; }

	public string FileName { get; set; }

	public string Method { get; set; }

	public string MethodName { get; set; }

	public SerializableStackFrame()
	{
	}

	public SerializableStackFrame(StackFrame stackFrame)
	{
		MethodBase method = stackFrame.GetMethod();
		Type declaringType = method.DeclaringType;
		DeclaringType = ((declaringType != null) ? declaringType.FullName : null);
		Method = method.ToString();
		MethodName = method.Name;
		FileName = stackFrame.GetFileName();
		FileLine = stackFrame.GetFileLineNumber();
		FileColumn = stackFrame.GetFileColumnNumber();
	}
}
