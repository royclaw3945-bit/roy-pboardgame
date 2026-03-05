namespace Unity.Cloud.UserReporting;

public struct UserReportScreenshot
{
	public string DataBase64 { get; set; }

	public string DataIdentifier { get; set; }

	public int FrameNumber { get; set; }

	public int Height => PngHelper.GetPngHeightFromBase64Data(DataBase64);

	public int Width => PngHelper.GetPngWidthFromBase64Data(DataBase64);
}
