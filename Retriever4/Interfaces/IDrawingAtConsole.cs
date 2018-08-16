using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Retriever4.Interfaces
{
    public interface IDrawingAtConsole
    {
        int X { get; }
        int Y { get; }
        int MaxX { get; }
        int MaxY { get; }

        int PrintInitializationStatus(int Yposition, string status, Color color);
        int PrintInitializationComment(int Yposition, string comment, Color color);
        int PrintInitializationDescription(int Yposition, string title);
        int PrintInitializationBar(int startY, string bar);
        void CursorY(int Yposition);
        void CursorX(int Xposition);
        void RestoreCursorX();
        void RestoreCursorY();
        void PrintRowSelection(int Y);
        void ClearRowSelection(int Y);
        void ClearLine(int y);
        int PrintMainHeaders(int startY);
        int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting,
            Color color);
        int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting,
            Color leftColumnColor, Color rightColumnColor);
        int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting,
            Color leftColumnColor, Color rightColumnColor, Color descriptionColumnColor);
        int PrintHorizontalLine(int startY);
        void PrintModelTable(int startY, List<Location> locations);
        ConsoleKeyInfo Wait();
    }
}
