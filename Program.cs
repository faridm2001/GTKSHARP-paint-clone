
// ##############################################################################
//
//   This file is responsible for the control part of the code
//   Two main parts 
//      - Struct Cairocolor: Responsible for setting the colors
//
//      - Class Area: responsible for controlling the program with multiple tools
//          - canvas setup
//          - Tools setup and mechanism
//          - Canvas modification 
//
// ###############################################################################


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cairo;
using Gdk;
using Gtk;
using Point = Gdk.Point;

//for program partition
namespace paintClone
{
    //the tools currently present in the program   
    enum Tool { Pen, Eraser, Bucket, Line, Square, Circle, Triangle };

    //struct to modify colors
    struct CairoColor {
        public double Red { get; set; }
        public double Green { get; set; }
        public double Blue { get; set; }
        public double Alpha { get; set; }

        public CairoColor(double red, double green, double blue, double alpha = 1) {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public override bool Equals(object obj) {
            if (obj is CairoColor other) {
                return Math.Abs(Red - other.Red) < 0.01 &&
                    Math.Abs(Green - other.Green) < 0.01 &&
                    Math.Abs(Blue - other.Blue) < 0.01 &&
                    Math.Abs(Alpha - other.Alpha) < 0.01;
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Red, Green, Blue, Alpha);
        }
    }

    //The most important class where all the program gets its commands from
    class Area : DrawingArea {

        //The following variables  are used to set up the canvas 
        private double zoomLevel = 1.0; // Default zoom level
        ImageSurface canvas = new ImageSurface(Format.Argb32, 1152, 648);

        public bool isMousePenPressed = false;
        public bool isMouseEraserPressed = false;

        PointD? prevPoint = null;
        int lineWeight = 3;
        double red_, green_, blue_, alpha_;
        public Tool tool = Tool.Pen;

        //the following method sets the canvas with a white background and a default canvas size
        public Area() {
            AddEvents((int)EventMask.ButtonPressMask);
            AddEvents((int)EventMask.PointerMotionMask);
            AddEvents((int)EventMask.PointerMotionHintMask);
            SetSizeRequest(1152, 648);
            red_ = 0;
            green_ = 0;
            blue_ = 0;
            alpha_ = 1;
            using (Context ctx = new Context(canvas)) {
                ctx.SetSourceRGB(1, 1, 1); // White color
                ctx.Paint(); // Fill the entire surface with white
            }
        }

        //this method changes the canvas size when called
        public void setCanvasSize(int w, int h ) {
            ImageSurface newCanvas = new ImageSurface(Format.Argb32, w, h);
            using (Context cr = new Context(newCanvas))
            {
                cr.SetSourceSurface(canvas, 0, 0);
                cr.Paint();  // This paints the old surface onto the new surface
            }
            canvas = newCanvas;
            SetSizeRequest(w, h);
            QueueDraw();
        }

        //change canvas view size (zoom) when called
        public void SetZoom(double zoom) {
            zoomLevel = zoom;
            QueueDraw(); // Redraw the area with the new zoom level
        }

        //Deletes everything on the canvas
        public void clear() {
            canvas = new ImageSurface(Format.Argb32, 1152, 648);
            using (Cairo.Context ctx = new Cairo.Context(canvas)) {
                ctx.SetSourceRGB(1, 1, 1); // White color
                ctx.Paint(); // Fill the entire surface with white
            }
            QueueDraw();
        }

        //Opens an image on the canvas
        public void openImage(string filename) {
            using (ImageSurface newSurface = new ImageSurface(filename)) {
                using (Context ctx = new Context(canvas)) {
                    ctx.SetSourceSurface(newSurface, 0, 0);
                    ctx.Paint();
                }
            }
            QueueDraw();
        }

        //Under construction
        public void Undo() {
            //coming soon..
        }

        public void Redo() {
            //coming soon..
        }

        //the following methods are responsible for the tools Functions 

        public void SetSourceColor(double red = 0, double green = 0, double blue = 0, double alpha = 1) {
            red_ = red;
            green_ = green;
            blue_ = blue;
            alpha_ = alpha;
        }

        public void SetLineWeight(int l) {
            lineWeight = l;
        }


        void DrawBrush(double x, double y) {
            using (Cairo.Context ctx = new Cairo.Context(canvas)) {
                ctx.SetSourceRGB(red_, green_, blue_);
                ctx.Rectangle((int)x - 3, (int)y - 3, lineWeight, lineWeight);
                ctx.Fill();
            }

            QueueDrawArea((int)x - 3, (int)y - 3, lineWeight, lineWeight);
        }

        void EraserBrush(double x, double y) {
            using (Cairo.Context ctx = new Cairo.Context(canvas)) {
                ctx.SetSourceRGB(1, 1, 1);
                ctx.Rectangle((int)x - 3, (int)y - 3, lineWeight, lineWeight);
                ctx.Fill();
            }

            QueueDrawArea((int)x - 3, (int)y - 3, lineWeight, lineWeight);
        }


        //The method responsible for switiching between tools and calling the tools functionality

        protected override bool OnButtonPressEvent(EventButton e) {
            using (Context c = new Context(canvas)) {
                c.SetSourceRGB(red_, green_, blue_);    // set the pen color to black
                c.LineWidth = lineWeight;               //set the line width
                switch (tool) {
                    case Tool.Pen:
                        isMousePenPressed = true;
                        DrawBrush(e.X, e.Y);
                        break;
                    case Tool.Eraser:
                        isMouseEraserPressed = true;
                        EraserBrush(e.X, e.Y);
                        break;
                    case Tool.Bucket:
                        BucketFill((int)e.X, (int)e.Y, new CairoColor(red_, green_, blue_, alpha_));
                        break;
                    case Tool.Line:
                        if (prevPoint == null) {
                            prevPoint = new PointD(e.X, e.Y);
                        } else {
                            c.MoveTo(prevPoint.Value.X, prevPoint.Value.Y);
                            c.LineTo(e.X, e.Y);
                            c.Stroke();
                            prevPoint = null;
                        }
                        break;
                    case Tool.Square:
                        if (prevPoint == null) {
                            prevPoint = new PointD(e.X, e.Y);
                        } else {
                            double x1 = Math.Min(prevPoint.Value.X, e.X);
                            double y1 = Math.Min(prevPoint.Value.Y, e.Y);
                            double x2 = Math.Max(prevPoint.Value.X, e.X);
                            double y2 = Math.Max(prevPoint.Value.Y, e.Y);
                            c.Rectangle(x1, y1, x2 - x1, y2 - y1);
                            c.Stroke();
                            prevPoint = null;
                        }
                        break;
                    case Tool.Circle:
                        if (prevPoint == null) {
                            prevPoint = new PointD(e.X, e.Y);
                        } else {
                            double radius = Math.Sqrt(Math.Pow(prevPoint.Value.X - e.X, 2) + Math.Pow(prevPoint.Value.Y - e.Y, 2));
                            c.Arc(prevPoint.Value.X, prevPoint.Value.Y, radius, 0, 2 * Math.PI);
                            c.Stroke();
                            prevPoint = null;
                        }
                        break;
                    case Tool.Triangle:
                        c.MoveTo(e.X, e.Y - 60);
                        c.LineTo(e.X + 60, e.Y + 30);
                        c.LineTo(e.X - 60, e.Y + 30);
                        c.ClosePath();
                        break;
                }

                c.StrokePreserve();
            }

            QueueDraw();
            return true;
        }

        protected override bool OnDrawn(Context c) {
            c.Scale(zoomLevel, zoomLevel);
            c.SetSourceSurface(canvas, 0, 0);
            c.Paint();
            return true;
        }

        protected override bool OnMotionNotifyEvent(EventMotion ev) {
            if (isMousePenPressed) {
                int x, y;
                Gdk.ModifierType state;
                ev.Window.GetPointer(out x, out y, out state);
                if ((state & Gdk.ModifierType.Button1Mask) != 0)
                    DrawBrush(x, y);
                return true;
            } else if (isMouseEraserPressed) {
                int x, y;
                Gdk.ModifierType state;
                ev.Window.GetPointer(out x, out y, out state);
                if ((state & Gdk.ModifierType.Button1Mask) != 0)
                    EraserBrush(x, y);
                return true;
            }
            return false;
        }


        private void BucketFill(int x, int y, CairoColor fillColor) {
            var surface = canvas;
            var data = surface.DataPtr;
            int width = surface.Width;
            int height = surface.Height;
            int stride = surface.Stride;

            // Get the color to replace
            var initialColor = GetPixel(data, x, y, stride);

            // Do not fill if the fill color is the same as the initial color
            if (initialColor.Equals(fillColor))
                return;

            Queue<Point> points = new Queue<Point>();
            points.Enqueue(new Point(x, y));

            while (points.Count > 0) {
                var point = points.Dequeue();
                int px = point.X;
                int py = point.Y;

                // Check boundaries
                if (px < 0 || px >= width || py < 0 || py >= height)
                    continue;

                // Check the current pixel color
                var currentColor = GetPixel(data, px, py, stride);
                if (!currentColor.Equals(initialColor))
                    continue;

                // Set the pixel to the fill color
                SetPixel(data, px, py, stride, fillColor);

                // Add neighboring points
                points.Enqueue(new Gdk.Point(px + 1, py));
                points.Enqueue(new Gdk.Point(px - 1, py));
                points.Enqueue(new Gdk.Point(px, py + 1));
                points.Enqueue(new Gdk.Point(px, py - 1));
            }

            surface.MarkDirty();
            QueueDraw();
        }

        private CairoColor GetPixel(IntPtr data, int x, int y, int stride) {
            int offset = y * stride + x * 4;
            byte b = Marshal.ReadByte(data, offset);
            byte g = Marshal.ReadByte(data, offset + 1);
            byte r = Marshal.ReadByte(data, offset + 2);
            byte a = Marshal.ReadByte(data, offset + 3);
            return new CairoColor(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
        }

        private void SetPixel(IntPtr data, int x, int y, int stride, CairoColor color) {
            int offset = y * stride + x * 4;
            Marshal.WriteByte(data, offset, (byte)(color.Blue * 255));
            Marshal.WriteByte(data, offset + 1, (byte)(color.Green * 255));
            Marshal.WriteByte(data, offset + 2, (byte)(color.Red * 255));
            Marshal.WriteByte(data, offset + 3, (byte)(color.Alpha * 255));
        }
    }
}

