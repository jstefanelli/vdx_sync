
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox1;

	private global::Gtk.HBox hbox1;

	private global::Gtk.Label lblCaches;

	private global::Gtk.ComboBox cbxCache;

	private global::Gtk.Label label2;

	private global::Gtk.ComboBox cbxMirrors;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox();
		this.hbox1.Name = "hbox1";
		this.hbox1.Homogeneous = true;
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.lblCaches = new global::Gtk.Label();
		this.lblCaches.Name = "lblCaches";
		this.lblCaches.LabelProp = "Caches";
		this.hbox1.Add(this.lblCaches);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.lblCaches]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.cbxCache = global::Gtk.ComboBox.NewText();
		this.cbxCache.Name = "cbxCache";
		this.hbox1.Add(this.cbxCache);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.cbxCache]));
		w2.Position = 1;
		// Container child hbox1.Gtk.Box+BoxChild
		this.label2 = new global::Gtk.Label();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("Mirrors");
		this.hbox1.Add(this.label2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.label2]));
		w3.Position = 2;
		w3.Expand = false;
		w3.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.cbxMirrors = global::Gtk.ComboBox.NewText();
		this.cbxMirrors.Name = "cbxMirrors";
		this.hbox1.Add(this.cbxMirrors);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.cbxMirrors]));
		w4.Position = 3;
		this.vbox1.Add(this.hbox1);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
		w5.Position = 0;
		w5.Expand = false;
		w5.Fill = false;
		this.Add(this.vbox1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 400;
		this.DefaultHeight = 300;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
	}
}