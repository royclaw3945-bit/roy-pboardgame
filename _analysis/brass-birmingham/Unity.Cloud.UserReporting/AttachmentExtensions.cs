using System.Collections.Generic;
using System.Text;

namespace Unity.Cloud.UserReporting;

public static class AttachmentExtensions
{
	public static void AddJson(this List<UserReportAttachment> instance, string name, string fileName, string contents)
	{
		instance?.Add(new UserReportAttachment(name, fileName, "application/json", Encoding.UTF8.GetBytes(contents)));
	}

	public static void AddText(this List<UserReportAttachment> instance, string name, string fileName, string contents)
	{
		instance?.Add(new UserReportAttachment(name, fileName, "text/plain", Encoding.UTF8.GetBytes(contents)));
	}
}
