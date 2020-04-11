using System;
using System.Runtime.InteropServices;

namespace CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	public static class BinaryStructConverter
	{
		public static T FromByteArray<T>(byte[] data)
			where T : struct
		{
			IntPtr ptr = IntPtr.Zero;
			try
			{
				int size = Marshal.SizeOf(typeof(T));
				ptr = Marshal.AllocHGlobal(size);
				Marshal.Copy(data, 0, ptr, size);
				object obj = Marshal.PtrToStructure(ptr, typeof(T));
				return (T)obj;
			}
			finally
			{
				if (ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(ptr);
				}
			}
		}

		public static byte[] ToByteArray<T>(T data)
			where T : struct
		{
			IntPtr ptr = IntPtr.Zero;
			try
			{
				int size = Marshal.SizeOf(typeof(T));
				ptr = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(data, ptr, true);
				byte[] bytes = new byte[size];
				Marshal.Copy(ptr, bytes, 0, size);
				return bytes;
			}
			finally
			{
				if (ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(ptr);
				}
			}
		}
	}
}
