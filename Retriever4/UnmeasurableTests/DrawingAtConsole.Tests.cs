using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Retriever4.UnmeasurableTests
{
    public static class DrawingAtConsoleTests
    {
        public static void DrawingAtConsole_DrawingTestsWithVariousConsoleWidths()
        {
            for(var width = 80; width < Console.LargestWindowWidth; width++)
            {
                Console.Clear();
                Console.WindowWidth = width + 1;
                Console.BufferWidth = width + 1;
                var engine = new DrawingAtConsole(Color.Black);
                var line = 0;
                line += engine.PrintInitializationBar(line, "Ładowanie programu");
                line++;
                line += engine.PrintHorizontalLine(line);
                line++;
                line += engine.PrintInitializationDescription(line, "Te munere corpora usu. Sed in virtute vulputate, est eu repudiandae dissentiunt. Et ius agam doctus, sit id omnis phaedrum. Ei mel dicta propriae, his oporteat imperdiet definitionem ut, sit hinc copiosae constituto ne.");
                line += engine.PrintInitializationStatus(line, "Zrobione", Color.Green);
                line++;
                line += engine.PrintInitializationComment(line, "Komentarz", Color.Yellow);
                line++;
                line += engine.PrintHorizontalLine(line);
                line++;
                line += engine.PrintMainHeaders(line);
                line++;
                line += engine.PrintHorizontalLine(line);
                line++;
                line += engine.PrintSection(line++, new[] { "Omnes partem vituperatoribus ea vis. An mea novum inermis. Te possit molestie quo. Ei eros efficiendi mei. Detracto ocurreret adipiscing ea per. Et intellegebat definitionem sea." },
                    new[] { "Lorem ipsum dolor sit amet, ne consulatu evertitur vel, appareat mediocritatem mei ex, omnium bonorum commune an vel. Eam te prima nihil vivendum. Eleifend mandamus petentium at ius, inani sapientem an nec. Ad per viris facete, an ignota docendi salutandi vis, ne illud scriptorem interpretaris cum." },
                    new[] { "Id vix unum antiopam. Per ipsum dignissim rationibus et. Ne ius modo facilis, menandri definitionem at sed, fabellas tractatos repudiare eos ea. Ei vix mutat veniam philosophia, nec nonumy dissentias ad. Omnis partiendo incorrupte ius id." }, Color.Red);
                line++;
                engine.PrintHorizontalLine(line);
                Console.WriteLine($"\nSzerokość okna/bufora = {width + 1}[znaków]");
                Console.WriteLine($"Największa możliwa wartość: {Console.LargestWindowWidth}[znaków]");
                Console.WriteLine($"Rozdzielczość: {Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}[px]");
                Console.ReadLine();
            }
        }

        public static void DrawingAtConsole_TableCursorMechanismTest()
        {
            
            for (var width = 80; width < Console.LargestWindowWidth; width++)
            {
                var break0 = false;
                Console.Clear();
                Console.WindowWidth = width + 1;
                Console.BufferWidth = width + 1;
                var engine = new DrawingAtConsole(Color.Black);
                var modelList = new List<Location>
                {
                    new Location("99850", null, 0, 0),
                    new Location("12345", "Test", 0, 0),
                    new Location("99790", null, 0, 0),
                    new Location("60150", "Long long long long test", 0, 0),
                    new Location("60011", null, 0, 0),
                    new Location("99850m", "Test", 0, 0)
                };
                engine.PrintModelTable(0, modelList);
                engine.CursorY(14);
                engine.RestoreCursorX();
                Console.WriteLine("Wciśnij ESC aby powiększyć okno.");
                Console.WriteLine($"\nSzerokość okna/bufora = {width + 1}[znaków]");
                Console.WriteLine($"Największa możliwa wartość: {Console.LargestWindowWidth}[znaków]");
                Console.WriteLine($"Rozdzielczość: {Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}[px]");
                var line = 2;
                engine.PrintRowSelection(line);
                while (!break0)
                {
                    var key = Console.ReadKey();
                    engine.RestoreCursorX();
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (line == 2)
                                continue;
                            else
                            {
                                engine.ClearRowSelection(line);
                                line -= 2;
                                engine.PrintRowSelection(line);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (line == modelList.Count * 2)
                                continue;
                            else
                            {
                                engine.ClearRowSelection(line);
                                line += 2;
                                engine.PrintRowSelection(line);
                            }
                            break;
                        case ConsoleKey.Escape:
                            break0 = true;
                            break;
                    }
                }
            }
        }
    }
}
