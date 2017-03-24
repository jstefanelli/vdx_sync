using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gtk;

namespace vdxMirrors
{
	class MainClass
	{

#if DEBUG
		public const string defaultCacheAddr = "http://localhost/vdx";
#else
		public const string defaultCacheAddr = "http://jstefanelli.com/vdx"; 
#endif
		[System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		static extern bool SetDllDirectory(string lpPathName);

		static bool DisplayWindowsOkCancelMessage(string message, string caption)
		{
			var name = typeof(int).Assembly.FullName.Replace("mscorlib", "System.Windows.Forms");
			var asm = Assembly.Load(name);
			var md = asm.GetType("System.Windows.Forms.MessageBox");
			var mbb = asm.GetType("System.Windows.Forms.MessageBoxButtons");
			var okCancel = Enum.ToObject(mbb, 1);
			var dr = asm.GetType("System.Windows.Forms.DialogResult");
			var ok = Enum.ToObject(dr, 1);

			const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
			return md.InvokeMember("Show", flags, null, null, new object[] { message, caption, okCancel }).Equals(ok);
		}

		static bool CheckWindowsGtk()
		{
			string location = null;
			Version version = null;
			Version minVersion = new Version(2, 12, 22);

			using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\InstallFolder"))
			{
				if (key != null)
					location = key.GetValue(null) as string;
			}
			using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\Version"))
			{
				if (key != null)
					Version.TryParse(key.GetValue(null) as string, out version);
			}

			//TODO: check build version of GTK# dlls in GAC
			if (version == null || version < minVersion || location == null || !File.Exists(Path.Combine(location, "bin", "libgtk-win32-2.0-0.dll")))
			{
				string url = "http://monodevelop.com/Download";
				string caption = "Fatal Error";
				string message =
					"{0} did not find the required version of GTK#. Please click OK to open the download page, where " +
					"you can download and install the latest version.";
				if (DisplayWindowsOkCancelMessage(
					string.Format(message, "vdxMirrors", url), caption)
				)
				{
					Process.Start(url);
				}
				return false;
			}

			Console.WriteLine("Found GTK# version " + version);

			var path = Path.Combine(location, @"bin");
			Console.WriteLine("SetDllDirectory(\"{0}\") ", path);
			try
			{
				if (SetDllDirectory(path))
				{
					return true;
				}
			}
			catch (EntryPointNotFoundException)
			{
				Console.WriteLine("Exception");
			}
			// this shouldn't happen unless something is weird in Windows
			Console.WriteLine("Error: Unable to set GTK+ dll directory");
			return false;
		}


		public static void Main(string[] args)
		{
			MainClass m = new MainClass();
			m.start();
		}

		public void start()
		{
			if(vdxNet.Net.nextFolderChar == '\\')
				CheckWindowsGtk();
			Application.Init();

			MainWindow win = new MainWindow();
			win.Show();
			Cache defaultCache = new Cache(defaultCacheAddr);
			defaultCache.title = "Default Cache";
			win.AddCache(defaultCache);
			win.SetActiveCache(0);
			win.OnCacheChanged += OnCacheChanged;
			win.OnMirrorChanged += OnMirrorChanged;
			Application.Run();
		}

		public void OnCacheChanged(object sender, EventArgs a)
		{

		}

		public void OnMirrorChanged(object sender, EventArgs a)
		{

		}
	}
}
