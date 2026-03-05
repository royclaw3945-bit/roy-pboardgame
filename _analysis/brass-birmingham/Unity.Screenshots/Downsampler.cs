using System;

namespace Unity.Screenshots;

public static class Downsampler
{
	public static byte[] Downsample(byte[] dataRgba, int stride, int maximumWidth, int maximumHeight, out int downsampledStride)
	{
		if (stride == 0)
		{
			throw new ArgumentException("The stride must be greater than 0.");
		}
		if (stride % 4 != 0)
		{
			throw new ArgumentException("The stride must be evenly divisible by 4.");
		}
		if (dataRgba == null)
		{
			throw new ArgumentNullException("dataRgba");
		}
		if (dataRgba.Length == 0)
		{
			throw new ArgumentException("The data length must be greater than 0.");
		}
		if (dataRgba.Length % 4 != 0)
		{
			throw new ArgumentException("The data must be evenly divisible by 4.");
		}
		if (dataRgba.Length % stride != 0)
		{
			throw new ArgumentException("The data must be evenly divisible by the stride.");
		}
		int num = stride / 4;
		int num2 = dataRgba.Length / stride;
		float val = (float)maximumWidth / (float)num;
		float val2 = (float)maximumHeight / (float)num2;
		float num3 = Math.Min(val, val2);
		if (num3 < 1f)
		{
			int num4 = (int)Math.Round((float)num * num3);
			int num5 = (int)Math.Round((float)num2 * num3);
			float[] array = new float[num4 * num5 * 4];
			float num6 = (float)num / (float)num4;
			float num7 = (float)num2 / (float)num5;
			int num8 = (int)Math.Floor(num6);
			int num9 = (int)Math.Floor(num7);
			int num10 = num8 * num9;
			for (int i = 0; i < num5; i++)
			{
				for (int j = 0; j < num4; j++)
				{
					int num11 = i * num4 * 4 + j * 4;
					int num12 = (int)Math.Floor((float)j * num6);
					int num13 = (int)Math.Floor((float)i * num7);
					int num14 = num12 + num8;
					int num15 = num13 + num9;
					for (int k = num13; k < num15; k++)
					{
						if (k >= num2)
						{
							continue;
						}
						for (int l = num12; l < num14; l++)
						{
							if (l < num)
							{
								int num16 = k * num * 4 + l * 4;
								array[num11] += (int)dataRgba[num16];
								array[num11 + 1] += (int)dataRgba[num16 + 1];
								array[num11 + 2] += (int)dataRgba[num16 + 2];
								array[num11 + 3] += (int)dataRgba[num16 + 3];
							}
						}
					}
					array[num11] /= num10;
					array[num11 + 1] /= num10;
					array[num11 + 2] /= num10;
					array[num11 + 3] /= num10;
				}
			}
			byte[] array2 = new byte[num4 * num5 * 4];
			for (int m = 0; m < num5; m++)
			{
				for (int n = 0; n < num4; n++)
				{
					int num17 = (num5 - 1 - m) * num4 * 4 + n * 4;
					int num18 = m * num4 * 4 + n * 4;
					array2[num18] += (byte)array[num17];
					array2[num18 + 1] += (byte)array[num17 + 1];
					array2[num18 + 2] += (byte)array[num17 + 2];
					array2[num18 + 3] += (byte)array[num17 + 3];
				}
			}
			downsampledStride = num4 * 4;
			return array2;
		}
		byte[] array3 = new byte[dataRgba.Length];
		for (int num19 = 0; num19 < num2; num19++)
		{
			for (int num20 = 0; num20 < num; num20++)
			{
				int num21 = (num2 - 1 - num19) * num * 4 + num20 * 4;
				int num22 = num19 * num * 4 + num20 * 4;
				array3[num22] += dataRgba[num21];
				array3[num22 + 1] += dataRgba[num21 + 1];
				array3[num22 + 2] += dataRgba[num21 + 2];
				array3[num22 + 3] += dataRgba[num21 + 3];
			}
		}
		downsampledStride = num * 4;
		return array3;
	}
}
