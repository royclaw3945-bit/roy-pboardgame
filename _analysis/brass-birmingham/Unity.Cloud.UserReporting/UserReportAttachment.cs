using System;

namespace Unity.Cloud.UserReporting;

public struct UserReportAttachment
{
	public string ContentType { get; set; }

	public string DataBase64 { get; set; }

	public string DataIdentifier { get; set; }

	public string FileName { get; set; }

	public string Name { get; set; }

	public UserReportAttachment(string name, string fileName, string contentType, byte[] data)
	{
		Name = name;
		FileName = fileName;
		ContentType = contentType;
		DataBase64 = Convert.ToBase64String(data);
		DataIdentifier = null;
	}
}
