using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.Serialization;

public class RijndaelSerializer : ISerialize
{
	private const int KeyLength = 16;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(RijndaelSerializer));

	private readonly byte[] _encryptionKey;

	private readonly ISerialize _inner;

	public RijndaelSerializer(ISerialize inner, byte[] encryptionKey)
	{
		if (!KeyIsValid(encryptionKey, 16))
		{
			throw new ArgumentException("The encryption key must be exactly 16 bytes.", "encryptionKey");
		}
		_encryptionKey = encryptionKey;
		_inner = inner;
	}

	public virtual void Serialize<T>(Stream output, T graph)
	{
		Logger.Verbose("Serializing type: {0}.", typeof(T));
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = _encryptionKey;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.GenerateIV();
		using ICryptoTransform transform = rijndaelManaged.CreateEncryptor();
		using IndisposableStream indisposableStream = new IndisposableStream(output);
		using CryptoStream cryptoStream = new CryptoStream(indisposableStream, transform, CryptoStreamMode.Write);
		indisposableStream.Write(rijndaelManaged.IV, 0, rijndaelManaged.IV.Length);
		_inner.Serialize(cryptoStream, graph);
		cryptoStream.Flush();
		cryptoStream.FlushFinalBlock();
	}

	public virtual T Deserialize<T>(Stream input)
	{
		Logger.Verbose("Deserializing to type: {0}.", typeof(T));
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = _encryptionKey;
		rijndaelManaged.IV = GetInitVectorFromStream(input, rijndaelManaged.IV.Length);
		rijndaelManaged.Mode = CipherMode.CBC;
		using ICryptoTransform transform = rijndaelManaged.CreateDecryptor();
		using CryptoStream input2 = new CryptoStream(input, transform, CryptoStreamMode.Read);
		return _inner.Deserialize<T>(input2);
	}

	private static bool KeyIsValid(ICollection key, int length)
	{
		if (key != null)
		{
			return key.Count == length;
		}
		return false;
	}

	private static byte[] GetInitVectorFromStream(Stream encrypted, int initVectorSizeInBytes)
	{
		byte[] array = new byte[initVectorSizeInBytes];
		encrypted.Read(array, 0, array.Length);
		return array;
	}
}
