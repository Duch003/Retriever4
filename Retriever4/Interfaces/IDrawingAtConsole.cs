using System;
using System.Collections.Generic;
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

        int PrintInitializationStatus(int Yposition, string status, ConsoleColor color);
        int PrintInitializationComment(int Yposition, string comment, ConsoleColor color);
        int PrintInitializationDescription(int Yposition, string title);
        int PrintInitializationBar(int startY, string bar);
        void CursorY(int Yposition);
        void CursorX(int Xposition);
        void RestoreCursorX();
        void RestoreCursorY();
        void PrintRowSelection(int Y);
        void ClearRowSelection(int Y);
        int PrintMainHeaders(int startY);
        int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, ConsoleColor color);
        int PrintHorizontalLine(int startY);
        void PrintModelTable(int startY, List<Location> locations);
        void Wait();
    }
}
