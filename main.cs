
// ##############################################################################
//
//   This is the main file responsible for running the program
//
// ###############################################################################

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cairo;
using Gdk;
using Gtk;
using Point = Gdk.Point;

//namespace is used here to partition the file into multiple files
namespace paintClone {
    class Run {
        static void Main() {
            //the gtk run methods 
            Application.Init();
            MyWindow w = new MyWindow();
            w.ShowAll();
            Application.Run();
        }
    }
}