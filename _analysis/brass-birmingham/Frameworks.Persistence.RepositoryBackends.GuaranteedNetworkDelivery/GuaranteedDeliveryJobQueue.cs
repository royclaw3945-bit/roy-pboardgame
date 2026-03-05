using System;
using System.IO;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class GuaranteedDeliveryJobQueue : IGuaranteedDeliveryJobQueue
{
	private readonly string _filesystemLocation;

	private readonly ISerialize _serializer;

	private const string JobFilePrefix = "job_";

	private const int NumberOfDigitsForIndex = 4;

	private readonly object _lock = new object();

	public GuaranteedDeliveryJobQueue(string filesystemLocation, ISerialize serializer)
	{
		if (string.IsNullOrEmpty(filesystemLocation))
		{
			throw new ArgumentException("filesystemLocation can't be null or empty");
		}
		if (serializer == null)
		{
			throw new ArgumentException("serializer can't be null or empty");
		}
		_filesystemLocation = filesystemLocation;
		_serializer = serializer;
		CreateFilesystemLocation();
	}

	private void CreateFilesystemLocation()
	{
		if (!Directory.Exists(_filesystemLocation))
		{
			Directory.CreateDirectory(_filesystemLocation);
		}
	}

	public int Count()
	{
		lock (_lock)
		{
			return Directory.GetFiles(_filesystemLocation, "job_*").Length;
		}
	}

	public void Enqueue(IJob job)
	{
		lock (_lock)
		{
			string newJobFilePath = GetNewJobFilePath();
			string directoryName = Path.GetDirectoryName(newJobFilePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using FileStream output = new FileStream(newJobFilePath, FileMode.Create);
			_serializer.Serialize(output, job);
		}
	}

	public IJob Dequeue()
	{
		lock (_lock)
		{
			IJob result = null;
			string firstJobFilePath = GetFirstJobFilePath();
			if (!File.Exists(firstJobFilePath))
			{
				return result;
			}
			using (FileStream input = new FileStream(firstJobFilePath, FileMode.Open))
			{
				result = _serializer.Deserialize<IJob>(input);
			}
			File.Delete(firstJobFilePath);
			return result;
		}
	}

	public IJob Peek()
	{
		lock (_lock)
		{
			IJob result = null;
			string firstJobFilePath = GetFirstJobFilePath();
			if (!File.Exists(firstJobFilePath))
			{
				return result;
			}
			using (FileStream input = new FileStream(firstJobFilePath, FileMode.Open))
			{
				result = _serializer.Deserialize<IJob>(input);
			}
			return result;
		}
	}

	public void Clear()
	{
		lock (_lock)
		{
			string[] files = Directory.GetFiles(_filesystemLocation, "job_*");
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
		}
	}

	private string GetNewJobFilePath()
	{
		string[] files = Directory.GetFiles(_filesystemLocation, "job_*");
		int num = 0;
		string[] array = files;
		foreach (string obj in array)
		{
			int num2 = int.Parse(obj.Substring(obj.Length - 4));
			if (num2 > num)
			{
				num = num2;
			}
		}
		return Path.Combine(_filesystemLocation, "job_" + (num + 1).ToString("D" + 4));
	}

	private string GetFirstJobFilePath()
	{
		string[] files = Directory.GetFiles(_filesystemLocation, "job_*");
		int num = int.MaxValue;
		string[] array = files;
		foreach (string obj in array)
		{
			int num2 = int.Parse(obj.Substring(obj.Length - 4));
			if (num2 < num)
			{
				num = num2;
			}
		}
		return Path.Combine(_filesystemLocation, "job_" + num.ToString("D" + 4));
	}
}
