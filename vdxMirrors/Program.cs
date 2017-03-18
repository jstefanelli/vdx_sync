using System;
using Gtk;

namespace vdxMirrors
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			MainClass m = new MainClass();
			m.start();
		}

		public void start()
		{
			Application.Init();
			MainWindow win = new MainWindow();
			win.Show();

			Mirror demoMirror = new Mirror("http://localhost/vdx_archive/", 1);
			demoMirror.title = "Demo Mirror";
			Cache demoCache = new Cache("http://localhost/vdx/");
			win.AddMirror(demoMirror);
			win.AddCache(demoCache);
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
