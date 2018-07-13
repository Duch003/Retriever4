using Retriever4.Utilities;
using System;
using System.Linq;

// Console.Top = Oś Y
// Console.Left = Oś X
// A - szerokość kolumny = _consoleOneFifth
// 
//   +-------------------------------------------------------------------------------------> Oś X (+)
//   |   |<=====Opis=====>|<===========Kolumna lewa========>|<========Kolumna prawa=========>|
//   |   ____________________________________________________________________________________
//   |   |0 1 2 3 4 5 6 7 8 9              .                .                .               |
//   |   |1               .                .                .                .               |
//   |   |2               .                .                .                .               |
//   |   |3               .                .<-szerok. kol.->.                .               |
//   |   |4               .                .                .                .               |
//   |   |5               .                .                .                .               |
//   |   |6               .                .                .                .               |
//   |   |7               .                .                .                .               |
//   |   |8               .                .                .                .               |
//   |   |9               .                .                .                .               |
//   |   |--------------->. A              .                .                .               |
//   v   |-------------------------------->. 2A             .                .               |
//  Oś Y |------------------------------------------------->. 3A             .               |
//  (+)  |------------------------------------------------------------------>. 4A            |



namespace Retriever4
{
    public class DrawingAtConsole
    {
        //Pozycja kursora
        public int CursorCoordinateX {
            get {
                return Console.CursorLeft;
            }
        }

        public int CursorCoordinateY {
            get {
                return Console.CursorTop;
            }
        }

        //Piąta część szerokości konsoli
        private readonly int _consleOneFifth;
        //Maksymalna szerokość columny
        private readonly int _maxColumnWidth;
        //Pozioma linia / separator
        private readonly string _horizontalLine;

        //Krawędzie kolumny z opisami
        private readonly int _descriptionColumn_LeftEdge;
        private readonly int _descriptionColumn_RightEdge;

        //Krawędzie kolumny lewej
        private readonly int _leftColumn_LeftEdge;
        private readonly int _leftColumn_RightEdge;

        //Krawędzie kolumny prawej
        private readonly int _rightColumn_LeftEdge;
        private readonly int _rightColumn_RightEdge;

        //Stałe nazwy
        private string successLine { get; set; } = "Zrobione";
        private string failedLine { get; set; } = "Błąd";
        private string _leftMainHeader { get; set; } = "RZECZYWISTE";
        private string _rightMainHeader { get; set; } = "BAZA DANYCH";
        private string _descriptionMainHeader { get; set; } = "OPIS";

        //Konstruktor pobierający aktualne dane z konsoli i ustalający odpległości
        public DrawingAtConsole()
        {
            //Tworzenie dwóch poziomych linii, każda o długości 1/5 buforu konsoli
            string line = "";
            string lineWithCross = "";
            int oneFifth = (Console.BufferWidth - 1) / 5;
            for (int i = 0; i < oneFifth; i++)
            {
                if(i == oneFifth - 1)
                {
                    line += "-";
                    lineWithCross += "+";
                }
                else
                {
                    line += "-";
                    lineWithCross += "-";
                }
            }
            //Ustawienie stałych
            _horizontalLine = lineWithCross + line + lineWithCross + line + line;
            _maxColumnWidth = oneFifth - 2; //2 -> z każdej krawędzi po jednostce
            _consleOneFifth = oneFifth;
            _descriptionColumn_LeftEdge = 1;
            _descriptionColumn_RightEdge = oneFifth - 1;
            _leftColumn_LeftEdge = oneFifth + 1;
            _leftColumn_RightEdge = (3 * oneFifth) - 1;
            _rightColumn_LeftEdge = (3 * oneFifth) + 1;
            _rightColumn_RightEdge = (5 * oneFifth) - 1;
            
        }

        public void PrintMainHeaders()
        {
            //Legenda:
            //$ - pozycja kursora na konsoli
            //_ - spacja, dodatkowo oddzielona kropką by było łatwiej widoczne
            //Dlatego że ustawiam tutaj background, ten string trzeba potraktować inaczej, tak aby backgorund wypełnił całą linię
            //Odbywa się tutaj proces przygotowania stringa o długości szerokości bufora konsoli
            PrintHorizontalLine();
            MainHeaderColor();

            //#1. Bazowy pusty string. Zaczynamy od prawej krawędzi konsoli
            string line = "";
            RestoreCursorX();

            //#2. Policz ilośc białych znaków jakie można wydrukować zanim napotka się pierwszą literę nagłówka lewego
            //$                   |<tu się zaczyna nagłówek>
            int i = _horizontalLineQuarter - (_leftMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";
            //_._._._._._._._$|<tu się zaczyna nagłówek>

            //#3. Dopisz do powstałego stringa nazwę lewego nagłówka
            line += _leftMainHeader;
            //_._._._._._._._|<nagłówekLewy>$

            //#4. Oblicz ilość białych znaków potrzebnych do wydrukowania aby wypełnić odległóść między nagłówkiem lewym a nagłówkiem prawym
            //_._._._._._._._|<nagłówekLewy>|$                  |<tu się zaczyna nagłówek prawy>
            i = ((3 * _horizontalLineQuarter) - (_rightMainHeader.Length / 2)) - (_horizontalLineQuarter + _leftMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";
            //_._._._._._._._|<nagłówekLewy>|_._._._._._._._$|<tu się zaczyna nagłówek prawy>

            //#5. Dopisz prawy nagłówek do stringa
            line += _rightMainHeader;
            //_._._._._._._._|<nagłówekLewy>|_._._._._._._._|<nagłówekPrawy>|$

            //#6. Policz ilość białych znakó potrzebnych by osiągnąć prawy kraniec konsoli
            //_._._._._._._._|<nagłówekLewy>|_._._._._._._._|<nagłówekPrawy>|$             |Prawy kraniec konsoli
            i = Console.BufferWidth - 1 - ((3 * _horizontalLineQuarter) + (_rightMainHeader.Length / 2));
            for (; i > 0; i--)
                line += " ";
            //_._._._._._._._|<nagłówekLewy>|_._._._._._._._|<nagłówekPrawy>|_._._._._._._._$|Prawy kraniec konsoli

            //#7. Drukowanie linii
            Console.WriteLine(line);
            Console.WriteLine(_horizontalLine);
            RestoreColors();
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
                //Zapisanie aktualnej pozycji Y kursora
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
