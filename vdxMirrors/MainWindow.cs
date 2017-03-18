using Gtk;
using System;
using System.Collections;
using vdxMirrors;

public partial class MainWindow : Gtk.Window
{

	public Mirror currentMirror { get; protected set; }
	public Cache currentCache { get; protected set; }

	protected Hashtable caches { get; }
	protected Hashtable mirrors { get; }

	public EventHandler OnCacheChanged { get; set; }
	public EventHandler OnMirrorChanged { get; set; }

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{		
		Build();
		caches = new Hashtable();
		mirrors = new Hashtable();
		cbxCache.Data.Clear();
		foreach (DictionaryEntry pair in caches){
			cbxCache.Data.Add(pair.Key, pair.Value);
		}
		cbxMirrors.Data.Clear();
		foreach (DictionaryEntry pair in mirrors)
		{
			cbxMirrors.Data.Add(pair.Key, pair.Value);
		}

		cbxCache.Changed += OncbxCacheChangedEvent;
	}

	protected void OncbxCacheChangedEvent(object sender, EventArgs a)
	{
		currentCache = (Cache) caches[cbxCache.Active];
		if(OnCacheChanged != null)
			OnCacheChanged(this, a);
	}

	protected void OncbxMirrorsChangedEvent(object sender, EventArgs a)
	{
		currentMirror = (Mirror) mirrors[cbxMirrors.Active];
		if(OnMirrorChanged != null)
			OnMirrorChanged(this, a);
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	public void AddMirror(Mirror m)
	{
		mirrors.Add(mirrors.Count, m);
		cbxMirrors.InsertText(mirrors.Count - 1, m.title);
	}

	public void AddCache(Cache c)
	{
		caches.Add(caches.Count, c);
		cbxCache.InsertText(caches.Count - 1, c.title);
	}


}