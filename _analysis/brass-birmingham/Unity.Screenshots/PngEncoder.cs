using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Unity.Screenshots;

public static class PngEncoder
{
	public class Crc32
	{
		private static uint generator = 3988292384u;

		private readonly uint[] checksumTable;

		public Crc32()
		{
			checksumTable = Enumerable.Range(0, 256).Select(delegate(int i)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 0) ? (generator ^ (num >> 1)) : (num >> 1));
				}
				return num;
			}).ToArray();
		}

		public uint Calculate<T>(IEnumerable<T> byteStream)
		{
			return ~byteStream.Aggregate(uint.MaxValue, (uint checksumRegister, T currentByte) => checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8));
		}
	}

	private static Crc32 crc32;

	static PngEncoder()
	{
		crc32 = new Crc32();
	}

	private static uint Adler32(byte[] bytes)
	{
		uint num = 1u;
		uint num2 = 0u;
		foreach (byte b in bytes)
		{
			num = (num + b) % 65521;
			num2 = (num2 + num) % 65521;
		}
		return (num2 << 16) | num;
	}

	private static void AppendByte(this byte[] data, ref int position, byte value)
	{
		data[position] = value;
		position++;
	}

	private static void AppendBytes(this byte[] data, ref int position, byte[] value)
	{
		foreach (byte value2 in value)
		{
			data.AppendByte(ref position, value2);
		}
	}

	private static void AppendChunk(this byte[] data, ref int position, string chunkType, byte[] chunkData)
	{
		byte[] chunkTypeBytes = GetChunkTypeBytes(chunkType);
		if (chunkTypeBytes != null)
		{
			data.AppendInt(ref position, chunkData.Length);
			data.AppendBytes(ref position, chunkTypeBytes);
			data.AppendBytes(ref position, chunkData);
			data.AppendUInt(ref position, GetChunkCrc(chunkTypeBytes, chunkData));
		}
	}

	private static void AppendInt(this byte[] data, ref int position, int value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		data.AppendBytes(ref position, bytes);
	}

	private static void AppendUInt(this byte[] data, ref int position, uint value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		data.AppendBytes(ref position, bytes);
	}

	private static byte[] Compress(byte[] bytes)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream stream = new DeflateStream(memoryStream, CompressionMode.Compress))
		{
			using MemoryStream memoryStream2 = new MemoryStream(bytes);
			memoryStream2.WriteTo(stream);
		}
		return memoryStream.ToArray();
	}

	public static byte[] Encode(byte[] dataRgba, int stride)
	{
		if (dataRgba == null)
		{
			throw new ArgumentNullException("dataRgba");
		}
		if (dataRgba.Length == 0)
		{
			throw new ArgumentException("The data length must be greater than 0.");
		}
		if (stride == 0)
		{
			throw new ArgumentException("The stride must be greater than 0.");
		}
		if (stride % 4 != 0)
		{
			throw new ArgumentException("The stride must be evenly divisible by 4.");
		}
		if (dataRgba.Length % 4 != 0)
		{
			throw new ArgumentException("The data must be evenly divisible by 4.");
		}
		if (dataRgba.Length % stride != 0)
		{
			throw new ArgumentException("The data must be evenly divisible by the stride.");
		}
		int num = dataRgba.Length / 4;
		int num2 = stride / 4;
		int num3 = num / num2;
		byte[] array = new byte[13];
		int position = 0;
		array.AppendInt(ref position, num2);
		array.AppendInt(ref position, num3);
		array.AppendByte(ref position, 8);
		array.AppendByte(ref position, 6);
		array.AppendByte(ref position, 0);
		array.AppendByte(ref position, 0);
		array.AppendByte(ref position, 0);
		byte[] array2 = new byte[dataRgba.Length + num3];
		int position2 = 0;
		int num4 = 0;
		for (int i = 0; i < dataRgba.Length; i++)
		{
			if (num4 >= stride)
			{
				num4 = 0;
			}
			if (num4 == 0)
			{
				array2.AppendByte(ref position2, 0);
			}
			array2.AppendByte(ref position2, dataRgba[i]);
			num4++;
		}
		byte[] array3 = Compress(array2);
		byte[] array4 = new byte[2 + array3.Length + 4];
		int position3 = 0;
		array4.AppendByte(ref position3, 120);
		array4.AppendByte(ref position3, 156);
		array4.AppendBytes(ref position3, array3);
		array4.AppendUInt(ref position3, Adler32(array2));
		byte[] array5 = new byte[8 + array.Length + 12 + array4.Length + 12 + 12];
		int position4 = 0;
		array5.AppendByte(ref position4, 137);
		array5.AppendByte(ref position4, 80);
		array5.AppendByte(ref position4, 78);
		array5.AppendByte(ref position4, 71);
		array5.AppendByte(ref position4, 13);
		array5.AppendByte(ref position4, 10);
		array5.AppendByte(ref position4, 26);
		array5.AppendByte(ref position4, 10);
		array5.AppendChunk(ref position4, "IHDR", array);
		array5.AppendChunk(ref position4, "IDAT", array4);
		array5.AppendChunk(ref position4, "IEND", new byte[0]);
		return array5;
	}

	public static void EncodeAsync(byte[] dataRgba, int stride, Action<Exception, byte[]> callback)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				byte[] arg = Encode(dataRgba, stride);
				callback(null, arg);
			}
			catch (Exception arg2)
			{
				callback(arg2, null);
				throw;
			}
		}, null);
	}

	private static uint GetChunkCrc(byte[] chunkTypeBytes, byte[] chunkData)
	{
		byte[] array = new byte[chunkTypeBytes.Length + chunkData.Length];
		Array.Copy(chunkTypeBytes, 0, array, 0, chunkTypeBytes.Length);
		Array.Copy(chunkData, 0, array, 4, chunkData.Length);
		return crc32.Calculate(array);
	}

	private static byte[] GetChunkTypeBytes(string value)
	{
		char[] array = value.ToCharArray();
		if (array.Length < 4)
		{
			return null;
		}
		byte[] array2 = new byte[4];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = (byte)array[i];
		}
		return array2;
	}
}
