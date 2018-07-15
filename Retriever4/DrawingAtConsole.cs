using Retriever4.Utilities;
using System;
using System.Linq;

// Console.Top = Oś Y
// Console.Left = Oś X
// A - szerokość kolumny = _consoleOneFifth
// 
//   +------------------------------------------------------------------------------> Oś X (+)
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
        public int X{
            get {
                return Console.CursorLeft;
            }
        }

        public int Y {
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
        private readonly int _descriptionColumn_Begin;
        private readonly int _descriptionColumn_End;
        private readonly int _descriptionColumn_SpaceToWriting;
        private readonly int _descriptionColumn_Middle;

        //Krawędzie kolumny lewej
        private readonly int _leftColumn_Begin;
        private readonly int _leftColumn_End;
        private readonly int _leftColumn_SpaceToWriting;
        private readonly int _leftColumn_Middle;

        //Krawędzie kolumny prawej
        private readonly int _rightColumn_Begin;
        private readonly int _rightColumn_End;
        private readonly int _rightColumn_SpaceToWriting;
        private readonly int _rightColumn_Middle;

        //Stałe nazwy
        private string _leftMainHeader { get; set; } = "RZECZYWISTE";
        private string _rightMainHeader { get; set; } = "BAZA DANYCH";
        private string _descriptionMainHeader { get; set; } = "PARAMETR";


        //Konstruktor pobierający aktualne dane z konsoli i ustalający odpległości
        public DrawingAtConsole()
        {
            if (Console.BufferWidth < 80)
                return;

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

            _descriptionColumn_Begin = 1;
            _descriptionColumn_End = oneFifth - 1;
            _descriptionColumn_SpaceToWriting = _descriptionColumn_End - _descriptionColumn_Begin;
            _descriptionColumn_Middle = _descriptionColumn_Begin + (_descriptionColumn_SpaceToWriting / 2);

            _leftColumn_Begin = oneFifth + 1;
            _leftColumn_End = (3 * oneFifth) - 1;
            _leftColumn_SpaceToWriting = _leftColumn_End - _leftColumn_Begin;
            _leftColumn_Middle = _leftColumn_Begin + (_leftColumn_SpaceToWriting / 2);

            _rightColumn_Begin = (3 * oneFifth) + 1;
            _rightColumn_End = (5 * oneFifth) - 1;
            _rightColumn_SpaceToWriting = _rightColumn_End - _rightColumn_Begin;
            _rightColumn_Middle = _rightColumn_Begin + (_rightColumn_SpaceToWriting / 2);
            
        }

        public int PrintMainHeaders(int startY)
        {
            if (startY < 0)
            {
                string message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy zero. Metoda: {nameof(PrintMainHeaders)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }
            CursorY(startY);
            RestoreCursorX();
            //Legenda:
            //$ - pozycja kursora na konsoli
            //_ - spacja, dodatkowo oddzielona kropką by było łatwiej widoczne
            //Dlatego że ustawiam tutaj background, ten string trzeba potraktować inaczej, tak aby backgorund wypełnił całą linię
            //Odbywa się tutaj proces przygotowania stringa o długości szerokości bufora konsoli
            MainHeaderColor();

            string line = "";
            //Od krawędzi do pierwszego nagłówka
            int i = (_consleOneFifth / 2) - (_descriptionMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Pierwszy nagłówek
            line += _descriptionMainHeader;
            
            //Od pierwszego nagłówka do końca pierwszej kolumny
            i = _consleOneFifth - (_descriptionMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Od pierszej kolumny do drugiego nagłówka
            i = (_consleOneFifth / 2) - (_leftMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Drugi nagłówek
            line += _leftMainHeader;

            //Od drugiego nagłówka do końca drugiej kolumny
            i = _consleOneFifth - (_leftMainHeader.Length / 2) - 1;
            for (; i > 0; i--)
                line += " ";

            //Od drugiej kolumny do trzeciego nagłówka
            i = _consleOneFifth - (_rightMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Trzeci nagłówek
            line += _rightMainHeader;

            //Od trzeciego nagłówka do końca trzeciej kolumny
            i = _consleOneFifth - (_rightMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            Console.WriteLine(line);
            RestoreColors();
            return 1;
        }

        public int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, ConsoleColor color)
        {
            #region Walidacja argumentów wejściowych
            if(description.Any(z => z == null))
            {
                string message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(description), message);
            }

            if (leftColumnWriting.Any(z => z == null))
            {
                string message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(leftColumnWriting), message);
            }

            if (rightColumnWriting.Any(z => z == null))
            {
                string message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(rightColumnWriting), message);
            }

            if(startY < 0)
            {
                string message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(startY), message);
            }
            #endregion

            int lines = 0;

            var tempLines = PrintColumn(startY, description, _descriptionColumn_Middle, _descriptionColumn_SpaceToWriting, ConsoleColor.White);
            lines = tempLines;

            tempLines = PrintColumn(startY, leftColumnWriting, _leftColumn_Middle, _leftColumn_SpaceToWriting, color);
            lines = lines > tempLines ? lines : tempLines;

            tempLines = PrintColumn(startY, rightColumnWriting, _rightColumn_Middle, _rightColumn_SpaceToWriting, color);
            lines = lines > tempLines ? lines : tempLines;

            PrintVerticalLines(startY, lines);

            return lines;
        }

        private void PrintVerticalLines(int startY, int lines)
        {
            #region Walidacja danych wejściowych
            if(startY < 0)
            {
                string message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Parametr musi być równy lub większy zero. Metoda: {nameof(PrintVerticalLines)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }

            if (lines < 0)
            {
                string message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na ilość linii jest ujemny: {lines}. Parametr musi być równy lub większy zero. Metoda: {nameof(PrintVerticalLines)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(lines), message);
            }
            #endregion
            var tempB = Console.BackgroundColor;
            var tempF = Console.ForegroundColor;
            LineColor();
            int i = 0;
            for(; i < lines; i++)
            {
                CursorY(startY + i);
                CursorX((_consleOneFifth)-1);
                Console.Write("|");
                CursorX((_consleOneFifth * 3)-1);
                Console.Write("|");
            }
            Console.ForegroundColor = tempF;
            Console.BackgroundColor = tempB;
        }

        private int PrintColumn(int startY, string[] writing, int middlePosition, int spaceToWriting, ConsoleColor color)
        {
            #region Walidacja argumentów wejściowych
            if (writing.Any(z => z == null))
            {
                string message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintColumn)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(writing), message);
            }

            if (startY < 0)
            {
                string message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Parametr musi być równy lub większy zero. Metoda: {nameof(PrintColumn)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }

            if (middlePosition < 0)
            {
                string message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący środek kolumny jest ujemny: {middlePosition}. Parametr musi być równy lub większy zero. Metoda: {nameof(PrintColumn)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(middlePosition), message);
            }

            if (spaceToWriting < 0)
            {
                string message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący długość przestrzeni do drukowania jest ujemny: {spaceToWriting}. Parametr musi być równy lub większy zero. Metoda: {nameof(PrintColumn)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(spaceToWriting), message);
            }
            #endregion

            //Sprawdzenie czy długośc każdego ze stringów mieści się w przestrzeni do pisania
            if (writing.Any(z => z.Length > spaceToWriting))
                writing = writing.JoinArray().SplitInParts(spaceToWriting).ToArray();

            var consoleF = Console.ForegroundColor;
            var consoleB = Console.BackgroundColor;
            SetConsoleForeground(color);
            CursorY(startY);
            int lines = 0;
            for (; lines < writing.Length; lines++)
            {
                CursorY(startY + lines);
                CursorX(middlePosition - (writing[lines].Length / 2));
                Console.Write(writing[lines]);
            }
            CursorY(startY);
            Console.ForegroundColor = consoleF;
            Console.BackgroundColor = consoleB;

            return lines;
        }

        public void PrintHorizontalLine(int startY)
        {
            if (startY < 0)
            {
                string message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy zero. Metoda: {nameof(PrintHorizontalLine)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }
            CursorY(startY);
            var fcolor = Console.ForegroundColor;
            var bcolor = Console.BackgroundColor;
            LineColor();
            RestoreCursorX();
            Console.WriteLine(_horizontalLine);
            Console.ForegroundColor = fcolor;
            Console.BackgroundColor = bcolor;
        }

        public void PrintInitializationBar(int startY, string bar)
        {
            if(bar == null)
            {
                string message = $"Nie można wydrukować statusu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(bar), message);
            }

            if (startY < 0)
            {
                string message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy zero. Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }

            CursorY(startY);
            MainHeaderColor();
            Console.WriteLine(bar.PadBoth(Console.BufferWidth));
            RestoreColors();
        }

        public void PrintInitializationDescription(int Yposition, string title) => PrintInitializationComment(Yposition, title, ConsoleColor.White);

        public void PrintInitializationComment(int Yposition, string comment, ConsoleColor color)
        {
            #region Walidacja danych wejściowych
            if (Yposition < 0)
            {
                string message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazujący na numer linii jest mniejszy od zera: {Yposition}. " +
                    $"Parametr musi być równy lub większy od zera. Metoda: {nameof(PrintInitializationDescription)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(Yposition), message);
            }

            if (comment == null)
            {
                string message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationDescription)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(comment), message);
            }
            #endregion

            SetConsoleForeground(color);
            CursorY(Yposition);
            RestoreCursorX();
            Console.Write(comment);
            RestoreColors();
        }

        public void PrintInitializationStatus(int Yposition, string status, ConsoleColor color)
        {
            #region Walidacja danych wejściowych
            if (Yposition < 0)
            {
                string message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazujący na numer linii jest mniejszy od zera: {Yposition}. " +
                    $"Parametr musi być równy lub większy od zera. Metoda: {nameof(PrintInitializationStatus)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(Yposition), message);
            }

            if (status == null)
            {
                string message = $"Nie można wydrukować statusu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationStatus)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(status), message);
            }
            #endregion

            SetConsoleForeground(color);
            CursorY(Yposition);
            CursorX(Console.BufferWidth - status.Length - 1);
            Console.Write(status);
            RestoreColors();
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
        #endregion

        #region Colors
        private void SetConsoleBackground(ConsoleColor color)
        {
            Console.BackgroundColor = color;
        }

        private void SetConsoleForeground(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private void DescriptionColor()
        {
            RestoreColors();
        }

        private void RestoreColors()
        {
            SetConsoleForeground(ConsoleColor.White);
            SetConsoleBackground(ConsoleColor.Black);
        }

        private void MainHeaderColor()
        {
            SetConsoleForeground(ConsoleColor.White);
            SetConsoleBackground(ConsoleColor.DarkBlue);
        }

        private void LineColor()
        {
            SetConsoleForeground(ConsoleColor.DarkYellow);
        }
        #endregion
    }
}
