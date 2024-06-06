
// ##############################################################################
//
//   This is the main file responsible for viewing the program
//
// ###############################################################################


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cairo;
using Gdk;
using Gtk;
using Point = Gdk.Point;


namespace paintClone
{

  //class responsible for viewing gtk window methods
  class MyWindow : Gtk.Window {

    //getting widgets objects set
    MenuItem undoItem, redoItem;
    ColorButton colorButton;
    ComboBox lineWeight;
    Toolbar toolbar = new Toolbar();
    Entry canvasHeight = new Entry();
    Entry canvasWidth = new Entry();

    //getting the canvas widget set
    Area area = new Area();
    bool toggling;

    //Set up the program window
    public MyWindow() : base("Painting Program") {
        DeleteEvent += delegate { Application.Quit(); };

        // Set initial window size
        SetDefaultSize(800, 600); // Set an initial size

        // Create a menu bar
        var menuBar = new MenuBar();

        // Create the menu bar option
        // Create the file button with its sub menu
        Menu fileMenu = new Menu();
        fileMenu.Append(item("Open...", on_open));
        fileMenu.Append(item("Exit", on_exit));

        Menu editMenu = new Menu();
        undoItem = item("Undo", on_undo);
        editMenu.Append(undoItem);
        redoItem = item("Redo", on_redo);
        editMenu.Append(redoItem);

        Menu helpMenu = new Menu();
        helpMenu.Append(item("About", on_about));

        MenuBar bar = new MenuBar();
        bar.Append(submenu("File", fileMenu));
        bar.Append(submenu("Edit", editMenu, on_open_transform_menu));
        bar.Append(submenu("Help", helpMenu));

        // Create a toolbar
        // Set the toolbar as icon only
        toolbar.Style = ToolbarStyle.Icons;
        // Set the pen to be toggled first
        ToggleToolButton pen_button = toggleToolButton(penIcon, Tool.Pen, "Pencil");
        pen_button.Active = true;
        toolbar.Add(pen_button);
        // Adding the next toggles/buttons
        toolbar.Add(toggleToolButton(eraserIcon, Tool.Eraser, "eraser"));
        toolbar.Add(toggleToolButton(bucketIcon, Tool.Bucket, "bucket fill"));
        toolbar.Add(new SeparatorToolItem());
        toolbar.Add(toggleToolButton(line_icon(), Tool.Line, "Line"));
        toolbar.Add(toggleToolButton(square_icon(), Tool.Square, "Square"));
        toolbar.Add(toggleToolButton(circle_icon(), Tool.Circle, "Circle"));
        toolbar.Add(toggleToolButton(triangle_icon(), Tool.Triangle, "Triangle"));

        toolbar.Add(new SeparatorToolItem());
        colorButton = new ColorButton();
        colorButton.Rgba = new RGBA() {
            Red = 0,
            Green = 0,
            Blue = 0,
            Alpha = 1
        };
        colorButton.UseAlpha = false;
        colorButton.ColorSet += OnColorSet;

        ToolItem colorButtonContainer = new ToolItem();
        colorButtonContainer.Add(colorButton);

        // Add the container to the toolbar
        toolbar.Add(colorButtonContainer);
        toolbar.Add(new SeparatorToolItem());

        //Set line wieght
        lineWeight = new ComboBox(
            new string[] {
                "3 px",
                "5 px",
                "7 px",
                "9 px"
            }
        );
        lineWeight.Changed += OnComboChange;
        lineWeight.Active = 0;

        ToolItem comboBoxContainer = new ToolItem();
        comboBoxContainer.Add(lineWeight);

        toolbar.Add(comboBoxContainer);

        toolbar.Add(new SeparatorToolItem());
        ToolButton b = new ToolButton(new Image(Gtk.Stock.Clear, IconSize.SmallToolbar), "clear");
        b.Clicked += on_clear;
        b.TooltipText = "Clear";
        toolbar.Add(b);

        // Create a container for the main application area
        ScrolledWindow scrolled = new ScrolledWindow();
        area.Halign = Align.Center; // Horizontal alignment to center
        area.Valign = Align.Center; // Vertical alignment to center
        scrolled.AddWithViewport(area);

        // Center the scrolled window
        Box canvasBox = new Box(Orientation.Horizontal, 0);
        canvasBox.PackStart(scrolled, true, true, 0);

        Box statusBox = new Box(Orientation.Horizontal, 0);
        statusBox.PackStart(new Label("Canvas Size:"), false, false, 5);
        statusBox.PackStart(canvasWidth, false, false, 3);
        statusBox.PackStart(new Label("x"), false, false, 5);
        statusBox.PackStart(canvasHeight, false, false, 3);
        statusBox.PackStart(new Label("px"), false, false, 5);
        Button resizebtn = new Button("Resize Canvas");
        resizebtn.Clicked += OnSetCanvasSize;
        resizebtn.TooltipText = "Resize Canvas";
        statusBox.PackStart(resizebtn, false, false, 0);
        statusBox.PackStart(new SeparatorToolItem(), false, false, 10);
        statusBox.PackStart(new Label("Zoom:"), false, false, 0);
        Adjustment zoomRange = new Adjustment(100.0, 0.0, 101.0, 0.1, 1.0, 1.0);
        var hScale = new HScale(zoomRange);
        hScale.SetSizeRequest(200, -1);
        hScale.ValueChanged += onZoomRange;
        statusBox.PackStart(hScale, false, false, 0);

        // Create a vertical box container for organizing widgets
        Box vbox = new Box(Orientation.Vertical, 0);
        vbox.Add(bar);
        vbox.Add(toolbar);
        vbox.PackStart(canvasBox, true, true, 0);
        vbox.Add(statusBox);

        // Add the vertical box container to the window
        Add(vbox);
    }

    //The following methods are responsible to communicate with the program.cs and the canvas methods

    static MenuItem item(string name, EventHandler handler) {
        MenuItem i = new MenuItem(name);
        i.Activated += handler;
        return i;
    }

    static MenuItem submenu(string name, Menu menu, EventHandler? handler = null) {
        MenuItem i = new MenuItem(name);
        i.Submenu = menu;
        if (handler != null)
            i.Activated += handler;
        return i;
    }

    void on_open(object? sender, EventArgs args) {
        using (FileChooserDialog d = new FileChooserDialog("Open Image...", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Ok)) {
            d.Filter = new FileFilter();
            d.Filter.AddPattern("*.png");
            d.Filter.Name = "PNG files";
            if (d.Run() == (int)ResponseType.Ok) {
                string filename = d.Filename;
                area.openImage(filename);
            }
        }
    }

    void on_open_transform_menu(object? sender, EventArgs args) {
        //uppercase_item.Sensitive = lowercase_item.Sensitive = text_view.Buffer.HasSelection;
    }

    void on_undo(object? sender, EventArgs args) {
        area.Undo();
    }

    void on_redo(object? sender, EventArgs args) {
        area.Redo();
    }

    void on_about(object? sender, EventArgs args) {
         OpenUrl("https://github.com/faridm2001/sumsemester2024renamed/blob/main/README.md");
    }

    void OpenUrl(string url) {
            try {
                // Check the platform
                if (Environment.OSVersion.Platform == PlatformID.Unix) {
                    // Linux or macOS
                    if (System.IO.File.Exists("/usr/bin/xdg-open")) {
                        // Use xdg-open for Linux
                        System.Diagnostics.Process.Start("/usr/bin/xdg-open", url);
                    } else if (System.IO.File.Exists("/usr/bin/open")) {
                        // Use open for macOS
                        System.Diagnostics.Process.Start("/usr/bin/open", url);
                    } else {
                        throw new InvalidOperationException("Cannot locate a method to open URLs on this OS.");
                    }
                } else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                    // Windows
                    System.Diagnostics.Process.Start("cmd", $"/c start {url}");
                }
            } catch (Exception ex) {
                // Log or handle the exception as needed
                Console.WriteLine("Failed to open URL: " + ex.Message);
            }
        }

    void on_exit(object? sender, EventArgs args) {
        Application.Quit();
    }


    Image penIcon = new("pencil.png");
    Image eraserIcon = new("icon/eraser.png");
    Image bucketIcon = new("bucket.png");

    Image icon(Action<Context> draw) {
        ImageSurface s = new ImageSurface(Format.Argb32, 16, 16);
        using (Context c = new Context(s)) {
            c.SetSourceRGB(0, 0, 0);    // black
            c.LineWidth = 4;
            draw(c);
            c.StrokePreserve();
            c.SetSourceRGB(1, 1, 1);    // white
            c.Fill();
        }
        return new Image(s);
    }

    void diagonalLine(Context c) {
        c.MoveTo(0, 0);
        c.LineTo(16, 16);
    }

    Image line_icon() => icon(diagonalLine);

    Image square_icon() => icon(c => c.Rectangle(2, 2, 12, 12));

    Image circle_icon() => icon(c => c.Arc(xc: 8, yc: 8, radius: 6, angle1: 0, angle2: 2 * Math.PI));

    void draw_triangle_icon(Context c) {
        c.MoveTo(8, 4);
        c.LineTo(14, 13);
        c.LineTo(2, 13);
        c.ClosePath();
    }

    Image triangle_icon() => icon(draw_triangle_icon);

    void toggle(object? sender, Tool tool) {
        if (toggling)
            return;     // prevent recursive invocations

        toggling = true;
        area.tool = tool;
        area.isMousePenPressed = false;
        area.isMouseEraserPressed = false;

        foreach (ToolItem b in toolbar)
            if (b != sender && b is ToggleToolButton ttb)
                ttb.Active = false;  // will fire Clicked event
        toggling = false;
    }

    ToggleToolButton toggleToolButton(Image icon, Tool tool, string name) {
        ToggleToolButton b = new ToggleToolButton();
        b.IconWidget = icon;
        b.Clicked += (obj, args) => toggle(obj, tool);
        b.TooltipText = name;
        return b;
    }

    void OnColorSet(object sender, EventArgs args) {
        Gdk.RGBA color = colorButton.Rgba;
        area.SetSourceColor(color.Red, color.Green, color.Blue, color.Alpha);
    }

    void OnComboChange(object sender, EventArgs args) {
        int selectedIndex = lineWeight.Active;
        switch (selectedIndex) {
            case 0:
                area.SetLineWeight(3);
                break;
            case 1:
                area.SetLineWeight(5);
                break;
            case 2:
                area.SetLineWeight(7);
                break;
            case 3:
                area.SetLineWeight(9);
                break;
            default:
                area.SetLineWeight(3); // Default to 3px if selectedIndex is out of range
                break;
        }
    }

    void OnSetCanvasSize(object? o, EventArgs args) {
        string heightText = canvasHeight.Text;
        string widthText = canvasWidth.Text;

        if (int.TryParse(heightText, out int height) && int.TryParse(widthText, out int width))
        {
            area.setCanvasSize(width, height);
        }
        else
        {
            // Handle the error if input is not a valid integer
            MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Please enter valid integers for width and height.");
            md.Run();
            md.Destroy();
        }
    }

    void onZoomRange(object sender, EventArgs args) {
        HScale scale = (HScale)sender; // Cast the sender to HScale
        double zoomValue = scale.Value / 100.0; // Convert the slider value to a zoom factor
        area.SetZoom(zoomValue); // Set the zoom level in the Area class
    }

    void on_clear(object? o, EventArgs args) {
        area.clear();
    }

    protected override bool OnDeleteEvent(Event e) {
        Application.Quit();
        return true;
    }
}  
}