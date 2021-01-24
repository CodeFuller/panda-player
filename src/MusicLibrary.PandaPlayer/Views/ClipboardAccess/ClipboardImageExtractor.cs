using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	// https://www.thomaslevesque.com/2009/02/05/wpf-paste-an-image-from-the-clipboard/
	internal class ClipboardImageExtractor : IClipboardImageExtractor
	{
		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private struct BITMAPFILEHEADER
		{
			public const short BM = 0x4d42;
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
			public short bfType;
			public int bfSize;
			public short bfReserved1;
			public short bfReserved2;
			public int bfOffBits;
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct BITMAPINFOHEADER
		{
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
		}

		public BitmapFrame GetImageFromClipboard()
		{
			if (!Clipboard.ContainsImage())
			{
				return null;
			}

			MemoryStream ms = Clipboard.GetData("DeviceIndependentBitmap") as MemoryStream;
			if (ms == null)
			{
				return null;
			}

			byte[] dibBuffer = new byte[ms.Length];
			ms.Read(dibBuffer, 0, dibBuffer.Length);

			BITMAPINFOHEADER infoHeader = BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

			int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
			int infoHeaderSize = infoHeader.biSize;
			int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

			BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER
			{
				bfType = BITMAPFILEHEADER.BM,
				bfSize = fileSize,
				bfReserved1 = 0,
				bfReserved2 = 0,
				bfOffBits = fileHeaderSize + infoHeaderSize + (infoHeader.biClrUsed * 4),
			};

			byte[] fileHeaderBytes = BinaryStructConverter.ToByteArray<BITMAPFILEHEADER>(fileHeader);

			using (MemoryStream msBitmap = new MemoryStream())
			{
				msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
				msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
				msBitmap.Seek(0, SeekOrigin.Begin);
				return BitmapFrame.Create(msBitmap, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
			}
		}
	}
}
