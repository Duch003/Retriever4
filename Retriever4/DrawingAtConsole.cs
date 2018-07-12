using Retriever4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4
{
    public class DrawingAtConsole
    {
        private int _horizontalLineAxis { get; set; }
        private int _horizontalLineQuarter { get; set; }
        private string _horizontalLine { get; set; }

        private string successLine { get; set; }
        private string failedLine { get; set; }
        private int _paddingLeft { get; set; }

        private string _leftMainHeader { get; set; } = "RZECZYWISTE";
        private string _rightMainHeader { get; set; } = "BAZA DANYCH";

        public DrawingAtConsole()
        {
            string tempLine = "";
            for (int i = 0; i < Console.BufferWidth - 1; i++)
            {
                if (i == (Console.BufferWidth - 1) / 2)
                    tempLine += "+";
                else
                    tempLine += "-";
            }
            _horizontalLine = tempLine;
            _horizontalLineAxis = tempLine.IndexOf('+');
            _horizontalLineQuarter = tempLine.Length / 4;
        }

        public void PrintMainHeaders()
        {
            //Dlatego że ustawiam tutaj background, ten string trzeba potraktować inaczej, tak aby backgorund wypełnił całą linię
            PrintHorizontalLine();
            MainHeaderColor();
            string line = "";
            int i = _horizontalLineAxis - (_leftMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";
            line += _leftMainHeader;
            SetLeftColumnAxisPositionForText(_leftMainHeader.Length);
            Console.Write("RZECZYWISTE");
            
            string text = "";
            for (int i = 40 - (position + 20); i > 0; i--)
                text = text + " ";
            Console.Write(text);
            ConsoleRestoreColors();
            ConsoleSeparator();
            Console.SetCursorPosition(40, Console.CursorTop);
            Console.Write("|");
            ConsoleHeaderColors();
            position = (int)Math.Round((float)("BAZA DANYCH".Length / 2), 0);
            Console.Write("BAZA DANYCH".PadLeft(position + 20));
            text = "";
            for (int i = 40 - (position + 20); i > 0; i--)
                text = text + " ";
            Console.Write(text);
            ConsoleRestoreColors();
            Console.WriteLine();
            ConsoleSeparator();
            Console.WriteLine(_horizontalLine);
        }

        public void PrintTableSection(string header, string leftString, string rightString, ConsoleColor mode, bool skipHeader)
        {
            //Walidacja parametrów wejściowych
            if (leftString == null || rightString == null || header == null)
            {
                string message = $"Parametr metody jest null.\nParametr header: {header};\nParametr leftValue: {leftString};\nParamter rightValues: {rightString}.\n" +
                    $"Metoda: PrintTableSection, Klasa: DrawingAtConsole.cs";
                throw new ArgumentNullException(message);
            }

            //Sprawdzenie czy leftString nie jest dłuższy niż szerokość kolumny
            string[] fragmentedLeftString = null;
            if (leftString.Length > _horizontalLineAxis - 1)
                fragmentedLeftString = leftString.SplitInParts(_horizontalLineAxis - 1).ToArray();

            //Sprawdzenie czy rightString nie jest dłuższy niż szerokość kolumny
            string[] fragmentedRightString = null;
            if (rightString.Length > _horizontalLineAxis - 1)
                fragmentedRightString = rightString.SplitInParts(_horizontalLineAxis - 1).ToArray();

            //Drukowanie nagłówka
            //Czy pominąć nagłówek?
            if (!skipHeader)
            //NIE
            {
                YellowColor();
                SetLeftColumnAxisPositionForText(header.Length);
                Console.WriteLine(header);
                //Nowa linia
                PrintHorizontalLine();
                //Nowa linia
            }

            //Drukowanie treści lewej kolumny
            SetConsoleForeground(mode);
            //Sprawdzenie czy tekst wejściowy został podzielony
            if (fragmentedLeftString != null)
            //TAK
            {
                var currentY = Console.CursorTop;
                for(int i = 0; i < fragmentedLeftString.Length; i++)
                {
                    SetLeftColumnAxisPositionForText(fragmentedLeftString[i].Length);
                    Console.Write(fragmentedLeftString[i]);
                    PrintVerticalLine();
                    CursorY(currentY + i + 1);
                    //Nowa linia
                }
                CursorY(currentY);
            }
            else
            //NIE
            {
                SetLeftColumnAxisPositionForText(leftString.Length);
                Console.Write(leftString);
                PrintVerticalLine();
            }

            //Drukowanie treści prawej kolumny
            //Sprawdzenie czy tekst wejściowy został podzielony
            if (fragmentedLeftString != null)
            //TAK
            {
                var currentY = Console.CursorTop;
                for (int i = 0; i < fragmentedRightString.Length; i++)
                {
                    SetRightColumnAxisPositionForText(fragmentedRightString[i].Length);
                    Console.Write(fragmentedRightString[i]);
                    PrintVerticalLine();
                    CursorY(currentY + i + 1);
                    //Nowa linia
                }
                CursorY(currentY);
            }
            else
            //NIE
            {
                SetLeftColumnAxisPositionForText(rightString.Length);
                Console.Write(rightString);
                PrintVerticalLine();
            }
        }

        public void PrintStatusSection(string header, string leftString, string[] rightStrings, ConsoleColor mode, bool skipHeader, bool leftAlignRightColumn)
        {
            //Walidacja parametrów wejściowych
            if(leftString == null || rightStrings == null || header == null)
            {
                string message = $"Parametr metody jest null.\nParametr header: {header};\nParametr leftValue: {leftString};\nParamter rightValues: {rightStrings}.\n" +
                    $"Metoda: PrintStatusSection, Klasa: DrawingAtConsole.cs";
                throw new ArgumentNullException(message);
            }

            //Sprawdzenie poprawności długości stringów w tablicy rightStrings
            if(rightStrings.Any(z => z.Length > _horizontalLineAxis - 1))
            {
                //Złączenie tablicy w string i ponowne rozdzielenie
                var mergedRightStrings = rightStrings.JoinArray();
                rightStrings = mergedRightStrings.SplitInParts(_horizontalLineAxis - 1).ToArray();
            }
                
            //Drukowanie nagłówka
            if (!skipHeader)
            {
                YellowColor();
                SetLeftColumnAxisPositionForText(header.Length);
                Console.WriteLine(header);
                LineColor();
                PrintHorizontalLine();
            }
            //Nowa linia

            //Drukowanie treści lewej kolumny
            //Sprawdzenie czy długość tesktu do wydrukowania nie jest większa niż szerokość kolumny
            if (leftString.Length > _horizontalLineAxis) 
            //TAK
            {
                //Podzielenie tekstu na części
                string[] fragmentedLeftString = leftString.SplitInParts(_horizontalLineAxis - 1).ToArray();
                //Zapisanie aktualnej pozycji kursora w osi Y
                int cursorTop = Console.CursorTop;
                //Drukowanie tablicy
                for (int i = 0; i < fragmentedLeftString.Length; i++)
                {
                    SetLeftColumnAxisPositionForText(fragmentedLeftString[i].Length);
                    RestoreColors();
                    Console.Write(fragmentedLeftString[i]);
                    CursorXAxis();
                    PrintVerticalLine();
                    CursorX(0);
                    CursorY(cursorTop + i + 1);
                    //Nowa linia
                }
                //Przywrócenie pozycji kursora - powrót do pierwszej linii danej sekcji
                CursorY(cursorTop);
                CursorX(0);
            }
            else 
            //NIE
            {
                SetLeftColumnAxisPositionForText(leftString.Length);
                RestoreColors();
                Console.Write(leftString);
                CursorXAxis();
                LineColor();
                PrintVerticalLine();
                //Nowa linia
            }

            //Drukowanie treści prawej kolumny
            for (int i = 0; i < rightStrings.Length; i++)
            {
                PrintVerticalLine();
                SetConsoleForeground(mode);
                if (leftAlignRightColumn)
                    CursorX(_horizontalLineAxis + 1);
                else
                    SetRightColumnAxisPositionForText(rightStrings[i].Length);
                Console.WriteLine(rightStrings[i]);
                //Nowa linia
            }
        }

        public void PrintHorizontalLine(bool newline = false)
        {
            var fcolor = Console.ForegroundColor;
            var bcolor = Console.BackgroundColor;
            LineColor();
            if (!newline)
                Console.WriteLine(_horizontalLine);
            else
                Console.WriteLine("\n" + _horizontalLine);
            RestoreCursorX();
            Console.ForegroundColor = fcolor;
            Console.BackgroundColor = bcolor;
        }

        public void PrintVerticalLine()
        {
            var fcolor = Console.ForegroundColor;
            var bcolor = Console.BackgroundColor;
            CursorXAxis();
            LineColor();
            Console.Write("|");
            RestoreCursorX();
            Console.ForegroundColor = fcolor;
            Console.BackgroundColor = bcolor;
        }

        #region Cursor position
        public void CursorY(int Yposition)
        {
            Console.SetCursorPosition(Console.CursorLeft, Yposition);
        }

        public void CursorX(int Xposition)
        {
            Console.SetCursorPosition(Xposition, Console.CursorTop);
        }

        public void RestoreCursorX()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public void RestoreCursorY()
        {
            Console.SetCursorPosition(Console.CursorLeft, 0);
        }

        public void CursorXAxis()
        {
            CursorX(_horizontalLineAxis);
        }

        public void SetLeftColumnAxisPositionForText(int stringLength)
        {
            int position = (int)Math.Round(((double)stringLength / 2), 0);
            CursorX(_horizontalLineQuarter - position);
        }

        public void SetRightColumnAxisPositionForText(int stringLength)
        {
            int position = (int)Math.Round(((double)stringLength / 2), 0);
            CursorX(_horizontalLineQuarter + _horizontalLineAxis - position);
        }
        #endregion

        #region Colors
        private static void SetConsoleBackground(ConsoleColor color)
        {
            Console.BackgroundColor = color;
        }

        private static void SetConsoleForeground(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private static void YellowColor()
        {
            SetConsoleForeground(ConsoleColor.Yellow);
        }

        private static void RedColor()
        {
            SetConsoleForeground(ConsoleColor.Red);
        }

        private static void GreenColor()
        {
            SetConsoleForeground(ConsoleColor.Green);
        }

        private static void RestoreColors()
        {
            SetConsoleForeground(ConsoleColor.White);
            SetConsoleBackground(ConsoleColor.Black);
        }

        private static void MainHeaderColor()
        {
            SetConsoleBackground(ConsoleColor.DarkBlue);
        }

        private static void LineColor()
        {
            SetConsoleForeground(ConsoleColor.DarkYellow);
        }
        #endregion
    }
}
