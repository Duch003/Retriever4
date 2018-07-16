using System;
using System.Windows.Forms;

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
                int line = 0;
                line += _engine.PrintInitializationBar(line, "Ładowanie programu");
                line++;
                line += _engine.PrintHorizontalLine(line);
                line++;
                line += _engine.PrintInitializationDescription(line, "Te munere corpora usu. Sed in virtute vulputate, est eu repudiandae dissentiunt. Et ius agam doctus, sit id omnis phaedrum. Ei mel dicta propriae, his oporteat imperdiet definitionem ut, sit hinc copiosae constituto ne.");
                line += _engine.PrintInitializationStatus(line, "Zrobione", ConsoleColor.Green);
                line++;
                line += _engine.PrintInitializationComment(line, "Komentarz", ConsoleColor.Yellow);
                line++;
                line += _engine.PrintHorizontalLine(line);
                line++;
                line += _engine.PrintMainHeaders(line);
                line++;
                line += _engine.PrintHorizontalLine(line);
                line++;
                line += _engine.PrintSection(line++, new string[] { "Omnes partem vituperatoribus ea vis. An mea novum inermis. Te possit molestie quo. Ei eros efficiendi mei. Detracto ocurreret adipiscing ea per. Et intellegebat definitionem sea." },
                    new string[] { "Lorem ipsum dolor sit amet, ne consulatu evertitur vel, appareat mediocritatem mei ex, omnium bonorum commune an vel. Eam te prima nihil vivendum. Eleifend mandamus petentium at ius, inani sapientem an nec. Ad per viris facete, an ignota docendi salutandi vis, ne illud scriptorem interpretaris cum." },
                    new string[] { "Id vix unum antiopam. Per ipsum dignissim rationibus et. Ne ius modo facilis, menandri definitionem at sed, fabellas tractatos repudiare eos ea. Ei vix mutat veniam philosophia, nec nonumy dissentias ad. Omnis partiendo incorrupte ius id." }, ConsoleColor.Red);
                line++;
                line += _engine.PrintHorizontalLine(line);
                line++;
                Console.WriteLine($"\nSzerokość okna/bufora = {width + 1}[znaków]");
                Console.WriteLine($"Największa możliwa wartość: {Console.LargestWindowWidth}[znaków]");
                Console.WriteLine($"Rozdzielczość: {Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}[px]");
                Console.ReadLine();
            }
        }
    }
}
