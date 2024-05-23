# To Build

Download the whole repository
-main.cs is the program runner
-program.cs is the application
-view.cs is responsible for the GUI

# PaintClone Documentation

PaintClone is a simple paint application built using C#, GTK, and the Cairo graphics library. It provides basic drawing and painting functionalities, including the use of different tools like a pen, eraser, bucket fill, and shapes like lines, squares, circles, and triangles.

## Table of Contents

- [Installation](#installation)
- [Classes](#classes)
  - [MyWindow](#mywindow)
  - [Area](#area)
- [Enumerations](#enumerations)
  - [Tool](#tool)
- [Structs](#structs)
  - [CairoColor](#cairocolor)

## Installation

To use PaintClone, ensure you have the following dependencies installed:
- GTK+ 3
- Gdk 3
- Cairo graphics library

You can compile the application using a C# compiler that supports .NET Framework or .NET Core with GTK# bindings.

## Classes

### MyWindow

`MyWindow` is the main window class for the application. It handles user interactions and manages the UI components of the painting program.

#### Key Features:
- **Toolbar:** Contains tools like the pen, eraser, bucket, and shapes. Users can select these to draw on the canvas.
- **Color Selection:** A color picker allows users to choose the drawing color.
- **Line Weight:** Users can select the thickness of the lines they draw.
- **Menu Bar:** Offers options like opening an image file, undoing or redoing actions, and accessing help information.
- **Zoom and Canvas Size:** Users can adjust the zoom level and the canvas size.

#### Key Methods:
- `on_open`: Opens a dialog to select and open an image file.
- `on_undo` and `on_redo`: Manage undo and redo operations.
- `OnColorSet`: Updates the drawing color based on user selection from the color picker.
- `OnComboChange`: Changes the line weight based on user selection.
- `OnSetCanvasSize`: Adjusts the canvas size based on user input.

### Area

`Area` is a custom drawing area where all the drawing operations are rendered. It extends the `DrawingArea` class from GTK.

#### Key Features:
- **Tool Usage:** Depending on the selected tool (pen, eraser, bucket, etc.), it modifies the canvas.
- **Zooming:** Supports zooming in and out to adjust the view of the canvas.
- **Canvas Operations:** Supports operations like clearing the canvas and opening an image.

#### Key Methods:
- `SetZoom`: Adjusts the zoom level of the canvas.
- `clear`: Clears the canvas by resetting it to white.
- `openImage`: Loads an image onto the canvas from a specified file path.
- `Undo` and `Redo`: Placeholder methods for undoing and redoing actions.
- `SetSourceColor`: Sets the drawing color.
- `SetLineWeight`: Sets the thickness of the lines drawn.

## Enumerations

### Tool

Represents the tools available in the application.

- `Pen`
- `Eraser`
- `Bucket`
- `Line`
- `Square`
- `Circle`
- `Triangle`

## Structs

### CairoColor

A structure representing a color in the Cairo graphics library.

#### Properties:
- `Red`: Red component of the color.
- `Green`: Green component of the color.
- `Blue`: Blue component of the color.
- `Alpha`: Alpha (transparency) component of the color.
