using System;

namespace Retriever4.UnmeasurableTests
{
    public static class DrawingAtConsoleTests
    {
        public static void DrawingAtConsole_DrawingTestsWithVariousConsoleWidths()
        {
            for(int width = 80; width < Console.LargestWindowWidth; width++)
            {
                Console.Clear();
                Console.WindowWidth = width + 1;
                Console.BufferWidth = width + 1;
                DrawingAtConsole _engine = new DrawingAtConsole();
                _engine.PrintInitializationBar(0, "Ładowanie programu");
                _engine.PrintHorizontalLine(1);
                _engine.PrintInitializationDescription(2, "Test opisu 1");
                _engine.PrintInitializationStatus(2, "Zrobione", ConsoleColor.Green);
                _engine.PrintInitializationComment(3, "Komentarz", ConsoleColor.Yellow);
                _engine.PrintHorizontalLine(4);
                _engine.PrintMainHeaders(5);
                _engine.PrintHorizontalLine(6);
                int lines = _engine.PrintSection(7, new string[] { "Omnes partem vituperatoribus ea vis. An mea novum inermis. Te possit molestie quo. Ei eros efficiendi mei. Detracto ocurreret adipiscing ea per. Et intellegebat definitionem sea." },
                    new string[] { "Lorem ipsum dolor sit amet, ne consulatu evertitur vel, appareat mediocritatem mei ex, omnium bonorum commune an vel. Eam te prima nihil vivendum. Eleifend mandamus petentium at ius, inani sapientem an nec. Ad per viris facete, an ignota docendi salutandi vis, ne illud scriptorem interpretaris cum." },
                    new string[] { "Id vix unum antiopam. Per ipsum dignissim rationibus et. Ne ius modo facilis, menandri definitionem at sed, fabellas tractatos repudiare eos ea. Ei vix mutat veniam philosophia, nec nonumy dissentias ad. Omnis partiendo incorrupte ius id." }, ConsoleColor.Red);
                _engine.PrintHorizontalLine(lines + 7);
                Console.WriteLine($"\nSzerokość okna/bufora = {width + 1}[znaków]");
                Console.WriteLine($"Największa możliwa wartość: {Console.LargestWindowWidth}[znaków]");
                Console.ReadLine();
            }
        }
    }
}
