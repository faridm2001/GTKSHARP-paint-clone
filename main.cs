using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cairo;
using Gdk;
using Gtk;
using Point = Gdk.Point;

namespace paintClone {
        class Run {
        static void Main() {
            Application.Init();
            MyWindow w = new MyWindow();
            w.ShowAll();
            Application.Run();
        }
    }
}