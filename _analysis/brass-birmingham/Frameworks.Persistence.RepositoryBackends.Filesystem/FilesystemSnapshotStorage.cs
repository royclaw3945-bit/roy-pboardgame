using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BoardGameRules.Entities.EventSourcing.Snapshot;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.RepositoryBackends.Filesystem;

public class FilesystemSnapshotStorage : ISnapshotStorage
{
	private const string FileNamePrefix = "snapshot_";

	private const string FileNameExtension = ".storage";

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(FilesystemSnapshotStorage));

	private string RootPath { get; set; }

	private ISerialize Serializer { get; set; }

	public FilesystemSnapshotStorage(string rootPath, ISerialize serializer)
	{
		RootPath = rootPath;
		Serializer = serializer;
	}

	public ISnapshot LoadSnapshot(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		bool flag = false;
		string path = null;
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		for (int num = Math.Min(GetMaxRevisionInStore(gameId), maxRevisionNumber); num >= 0; num--)
		{
			path = BuildPath(gameId, num);
			if (File.Exists(path))
			{
				flag = true;
				Logger.Info("Found snapshot for {0}, revision {1}", gameId, num);
				break;
			}
		}
		if (!flag)
		{
			Logger.Info("There is no snapshot for given gameId and revision. Returning null.");
			return null;
		}
		using FileStream input = new FileStream(path, FileMode.Open);
		return Serializer.Deserialize<ISnapshot>(input);
	}

	public void SaveSnapshot(ISnapshot snapshot)
	{
		if (snapshot == null)
		{
			throw new ArgumentException("Snapshot is null");
		}
		if (snapshot.Metadata == null)
		{
			throw new ArgumentException("Snapshot.Metadata is null");
		}
		string path = BuildPath(snapshot.Metadata.GameId, snapshot.Metadata.Revision);
		if (File.Exists(path))
		{
			throw new ArgumentException("There is already snapshot for given gameId and revision");
		}
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		Logger.Info("Saving snapshot for {0}, revision {1}", snapshot.Metadata.GameId, snapshot.Metadata.Revision);
		using FileStream output = new FileStream(path, FileMode.Create);
		Serializer.Serialize(output, snapshot);
	}

	public void DeleteSnapshot(ISnapshot snapshot)
	{
		if (snapshot == null)
		{
			throw new ArgumentException("The snapshot can't be null");
		}
		string text = BuildPath(snapshot.Metadata.GameId, snapshot.Metadata.Revision);
		if (!File.Exists(text))
		{
			throw new ArgumentException("There is no such snapshot in the store");
		}
		new FileInfo(text).Delete();
	}

	public void DeleteSnapshot(string gameId, int revision)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string text = BuildPath(gameId, revision);
		if (!File.Exists(text))
		{
			throw new ArgumentException("There is no snapshot for given gameId and revision");
		}
		new FileInfo(text).Delete();
	}

	public void DeleteSnapshots(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string path = BuildPath(gameId);
		if (!Directory.Exists(path))
		{
			throw new ArgumentException("There are no snapshots for given gameId in the store");
		}
		new DirectoryInfo(path).Delete(recursive: true);
	}

	public void MigrateOldSnapshotsToNewDirectory(string gameId)
	{
		string destDirName = BuildPath(gameId);
		string text = BuildOldPath();
		if (Directory.Exists(text))
		{
			Directory.Move(text, destDirName);
		}
	}

	private string BuildPath(string gameId, int revision)
	{
		return Path.Combine(Path.Combine(Path.Combine(RootPath, gameId), "snapshots"), "snapshot_" + revision + ".storage");
	}

	private string BuildPath(string gameId)
	{
		return Path.Combine(Path.Combine(RootPath, gameId), "snapshots");
	}

	private string BuildOldPath()
	{
		return Path.Combine(RootPath, "snapshots");
	}

	private int GetVersion(string filePath)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
		int num = fileNameWithoutExtension.IndexOf("snapshot_");
		return int.Parse((num < 0) ? fileNameWithoutExtension : fileNameWithoutExtension.Remove(num, "snapshot_".Length));
	}

	private int GetMaxRevisionInStore(string gameId)
	{
		string path = BuildPath(gameId);
		if (!Directory.Exists(path))
		{
			return 0;
		}
		string[] files = Directory.GetFiles(path);
		if (files.Length == 0)
		{
			return 0;
		}
		Regex r = new Regex(".*?_(\\d+).storage");
		Array.Sort(files.Select((string el) => int.Parse(r.Match(el).Groups[1].Captures[0].Value)).ToArray(), files);
		string filePath = files[files.Length - 1];
		return GetVersion(filePath);
	}
}
