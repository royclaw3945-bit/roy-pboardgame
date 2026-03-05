namespace BrassApplication.Utils;

public interface IVersionReader
{
	string PublicVersion { get; }

	string BuildVersion { get; }
}
