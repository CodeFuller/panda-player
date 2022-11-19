using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PandaPlayer.Views
{
	internal static class NativeMethods
	{
		/// <summary>
		/// The WM_DRAWCLIPBOARD message notifies a clipboard viewer window that
		/// the content of the clipboard has changed.
		/// </summary>
		internal const int WM_DRAWCLIPBOARD = 0x0308;

		/// <summary>
		/// A clipboard viewer window receives the WM_CHANGECBCHAIN message when
		/// another window is removing itself from the clipboard viewer chain.
		/// </summary>
		internal const int WM_CHANGECBCHAIN = 0x030D;

		public enum MapType : uint
		{
			MAPVK_VK_TO_VSC = 0x0,
			MAPVK_VSC_TO_VK = 0x1,
			MAPVK_VK_TO_CHAR = 0x2,
			MAPVK_VSC_TO_VK_EX = 0x3,
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		public static extern int ToUnicode(
			uint wVirtKey,
			uint wScanCode,
			byte[] lpKeyState,
#pragma warning disable CA1838 // Avoid StringBuilder parameters for P/Invokes
			[Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
#pragma warning restore CA1838 // Avoid StringBuilder parameters for P/Invokes
			int cchBuff,
			uint wFlags);

		[DllImport("user32.dll")]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll")]
		[DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
		public static extern uint MapVirtualKey(uint uCode, MapType uMapType);
	}
}
