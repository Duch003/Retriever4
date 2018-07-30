using Retriever4.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Retriever4.Interfaces;

// Console.Top = Axis Y
// Console.Left = Axis X
// A - kolumn width = _consoleOneFifth
// 
//   +------------------------------------------------------------------------------> Axis X (+)
//   |   |<=Description==>|<===========Left column=========>|<========Right column==========>|
//   |   ____________________________________________________________________________________
//   |   |0 1 2 3 4 5 6 7 8 9              .                .                .               |
//   |   |1               .                .                .                .               |
//   |   |2               .                .                .                .               |
//   |   |3               .                .<-column width->.                .               |
//   |   |4               .                .                .                .               |
//   |   |5               .                .                .                .               |
//   |   |6               .                .                .                .               |
//   |   |7               .                .                .                .               |
//   |   |8               .                .                .                .               |
//   |   |9               .                .                .                .               |
//   |   |--------------->. A              .                .                .               |
//   v   |-------------------------------->. 2A             .                .               |
//Axis Y |------------------------------------------------->. 3A             .               |
//  (+)  |------------------------------------------------------------------>. 4A            |



namespace Retriever4
{
    public class DrawingAtConsole : IDrawingAtConsole
    {
        //Current cursor position
        public int X => Console.CursorLeft;

        public int Y => Console.CursorTop;

        public int MaxX => Console.LargestWindowWidth;

        public int MaxY => Console.LargestWindowHeight;

        //One fifth part of connsole width
        private readonly int _consleOneFifth;
        //Horizontal separator
        private readonly string _horizontalLine;

        //Description column area
        private readonly int _descriptionColumn_Begin;
        private readonly int _descriptionColumn_End;
        private readonly int _descriptionColumn_SpaceToWriting;
        private readonly int _descriptionColumn_Middle;

        //Left column area
        private readonly int _leftColumn_Begin;
        private readonly int _leftColumn_End;
        private readonly int _leftColumn_SpaceToWriting;
        private readonly int _leftColumn_Middle;

        //Right column area
        private readonly int _rightColumn_Begin;
        private readonly int _rightColumn_End;
        private readonly int _rightColumn_SpaceToWriting;
        private readonly int _rightColumn_Middle;

        //Model table 
        private readonly int _consoleOneTenth;
        private readonly string _tableHorizontalSeparator;
        //First selection column
        private readonly int _firstSelectTable_ColumnBegin;
        private readonly int _firstSelectTable_Middle;
        private readonly int _firstSelectTable_SpaceToWriting;
        private readonly int _firstSelectTable_ColumnEnd;
        //Standard model column
        private readonly int _modelColumn_ColumnBegin;
        private readonly int _modelColumn_Middle;
        private readonly int _modelColumn_SpaceToWriting;
        private readonly int _modelColumn_ColumnEnd;
        //Peaq model column
        private readonly int _peaqColumn_ColumnBegin;
        private readonly int _peaqColumn_Middle;
        private readonly int _peaqColumn_SpaceToWriting;
        private readonly int _peaqColumn_ColumnEnd;
        //Selection column
        private readonly int _secondSelectTable_ColumnBegin;
        private readonly int _secondSelectTable_Middle;
        private readonly int _secondSelectTable_SpaceToWriting;
        private readonly int _secondSelectTable_ColumnEnd;

        //Arrows
        private readonly string _leftSideArrow;
        private readonly string _rightSideArrow;
        private readonly string _arrowCleaner;

        //Constant titles
        private string _leftMainHeader { get; set; } = "RZECZYWISTE";
        private string _rightMainHeader { get; set; } = "BAZA DANYCH";
        private string _descriptionMainHeader { get; set; } = "PARAMETR";
        private string _modelTableModelHeader { get; set; } = "MODEL";
        private string _modelTablePeaqHeader { get; set; } = "PEAQ MODEL";

        private readonly string _clearLine;
        private Color _defaultBackground = Color.Black;


        public DrawingAtConsole(Color defaultBackground)
        {
            if (Console.BufferWidth < 80)
                return;

            //Creating separators - one with cross, one without
            var line = "";
            var lineWithCross = "";
            var oneFifth = (Console.BufferWidth - 1) / 5;
            for (var i = 0; i < oneFifth; i++)
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
            //Setting up constant values
            _horizontalLine = lineWithCross + line + lineWithCross + line + line;
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

            //Creating basic lines for model table
            line = "";
            lineWithCross = "";
            for (var i = 0; i < (oneFifth / 2); i++)
            {
                if (i == (oneFifth / 2) - 1)
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

            //Set up basic length for table
            _consoleOneTenth = oneFifth / 2;

            //Creating arrows
            var leftArrow = "";
            var rightArrow = "";
            var arrowCleaner = "";
            for(var i = 1; i < _consoleOneTenth - 2; i++)
            {
                if (i == 1)
                    rightArrow += "<";
                else
                    rightArrow += "=";
                if (i == _consoleOneTenth - 3)
                    leftArrow += ">";
                else
                    leftArrow += "=";
                arrowCleaner += " ";
            }
            //Setting up contant values
            _arrowCleaner = arrowCleaner;
            _leftSideArrow = leftArrow;
            _rightSideArrow = rightArrow;
            _tableHorizontalSeparator = lineWithCross + line  + lineWithCross + line + line + line + line + line + lineWithCross + lineWithCross;

            
            _firstSelectTable_ColumnBegin = 1;
            _firstSelectTable_ColumnEnd = _consoleOneTenth - 1;
            _firstSelectTable_SpaceToWriting = _firstSelectTable_ColumnEnd - _firstSelectTable_ColumnBegin;
            _firstSelectTable_Middle = _firstSelectTable_ColumnBegin + (_firstSelectTable_SpaceToWriting / 2);

            _modelColumn_ColumnBegin = _consoleOneTenth + 1;
            _modelColumn_ColumnEnd = (3 * _consoleOneTenth) - 1;
            _modelColumn_SpaceToWriting = _modelColumn_ColumnEnd - _modelColumn_ColumnBegin;
            _modelColumn_Middle = _modelColumn_ColumnBegin + (_modelColumn_SpaceToWriting / 2);

            _peaqColumn_ColumnBegin = (3 * _consoleOneTenth) + 1;
            _peaqColumn_ColumnEnd = (9 * _consoleOneTenth) - 1;
            _peaqColumn_SpaceToWriting = _peaqColumn_ColumnEnd - _peaqColumn_ColumnBegin;
            _peaqColumn_Middle = _peaqColumn_ColumnBegin + (_peaqColumn_SpaceToWriting / 2);

            _secondSelectTable_ColumnBegin = (9 * _consoleOneTenth) + 1;
            _secondSelectTable_ColumnEnd = (10 *_consoleOneTenth) - 1;
            _secondSelectTable_SpaceToWriting = _secondSelectTable_ColumnEnd - _secondSelectTable_ColumnBegin;
            _secondSelectTable_Middle = _secondSelectTable_ColumnBegin + (_secondSelectTable_SpaceToWriting / 2);

            for (var i = 0; i < Console.BufferWidth; i++)
                _clearLine += " ";
        }

        /// <summary>
        /// Prints list of models with table headers.
        /// </summary>
        /// <param name="startY">Begining of the table.</param>
        /// <param name="locations">List of locations to print</param>
        public void PrintModelTable(int startY, List<Location> locations)
        {
            //Printing header (line 0)
            var lines = 0;
            RestoreCursorX();
            CursorY(startY);
            SetConsoleForeground(Color.Yellow);
            CursorX(_modelColumn_Middle - (_modelTableModelHeader.Length / 2));
            Console.Write(_modelTableModelHeader);
            CursorX(_peaqColumn_Middle - (_modelTablePeaqHeader.Length / 2));
            Console.Write(_modelTablePeaqHeader);
            RestoreColors();

            //Printing horizontal separator (line 1)
            lines++;
            CursorY(startY + lines);
            RestoreCursorX();
            LineColor();
            Console.Write(_tableHorizontalSeparator);
            
            //From line 2 goes collection
            for (var i = 0; i < locations.Count(); i++)
            {
                //Printing data with vertical separators
                var tempPeaqModel = string.IsNullOrEmpty(locations[i].PeaqModel) ? "" : locations[i].PeaqModel;
                lines++;
                CursorY(startY + lines);
                RestoreColors();
                CursorX(_firstSelectTable_ColumnEnd);
                LineColor();
                Console.Write("|");
                RestoreColors();
                CursorX(_modelColumn_Middle - (locations[i].Model.Length / 2));
                Console.Write(locations[i].Model);
                CursorX(_peaqColumn_Middle - (tempPeaqModel.Length / 2));
                Console.Write(tempPeaqModel);
                CursorX(_peaqColumn_ColumnEnd);
                LineColor();
                Console.Write("|");
                RestoreColors();
                CursorX(_modelColumn_ColumnEnd);
                LineColor();
                Console.Write("|");
                RestoreColors();

                //Printing horizontal line
                lines++;
                CursorY(startY + lines);
                RestoreCursorX();
                LineColor();
                Console.Write(_tableHorizontalSeparator);
                RestoreColors();
            }
            CursorY(startY + 2);
        }

        /// <summary>
        /// Prints selection arrows in specific line.
        /// </summary>
        /// <param name="Y">Line in which print arrows.</param>
        public void PrintRowSelection(int Y)
        {
            RestoreCursorX();
            CursorY(Y);
            SetConsoleForeground(Color.Green);
            CursorX(_firstSelectTable_ColumnEnd - _leftSideArrow.Length - 1);
            Console.Write(_leftSideArrow);
            CursorX(_secondSelectTable_ColumnBegin);
            Console.Write(_rightSideArrow);
            RestoreCursorX();
            RestoreColors();
        }

        /// <summary>
        /// Prints string filled by whitespaces which length is same as selection arrow length.
        /// </summary>
        /// <param name="Y">Line in which print blank string.</param>
        public void ClearRowSelection(int Y)
        {
            RestoreCursorX();
            CursorY(Y);
            CursorX(_firstSelectTable_ColumnEnd - _leftSideArrow.Length - 1);
            Console.Write(_arrowCleaner);
            CursorX(_secondSelectTable_ColumnBegin);
            Console.Write(_arrowCleaner);
        }

        /// <summary>
        /// Print table main headers in specific line.
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <returns>How many additional lines (without first startY) it took.</returns>
        public int PrintMainHeaders(int startY)
        {
            //Validation
            if (startY < 0)
            {
                var message = $"Parametr wskazujący linię w której ma zostać wydrukowany tekst jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy od zera. Metoda: {nameof(PrintMainHeaders)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }
            
            //Set cursor in specific line
            CursorY(startY);
            //Save current position of cursur
            var tempY = Y;
            //Return carriage to the begining of the line
            RestoreCursorX();
            //Set colors
            MainHeaderColor();

            //Printing headers is a special case of printing due to backgorund change. 
            var line = "";
            //From left edge to first letter of first header
            var i = (_consleOneFifth / 2) - (_descriptionMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //First header
            line += _descriptionMainHeader;
            
            //From last letter of first header to right edge of first column
            i = _consleOneFifth - (_descriptionMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //From left edge of second column (right edge od first one) to first letter of second header
            i = (_consleOneFifth / 2) - (_leftMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Second header
            line += _leftMainHeader;

            //From last letter of second header to right edge od second column
            i = _consleOneFifth - (_leftMainHeader.Length / 2) - 1;
            for (; i > 0; i--)
                line += " ";

            //From left edge of third column (right edge of second ona) to third header
            i = _consleOneFifth - (_rightMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Third header
            line += _rightMainHeader;

            //From third header to end of line
            i = _consleOneFifth - (_rightMainHeader.Length / 2);
            for (; i > 0; i--)
                line += " ";

            //Printing
            Console.Write(line);
            //Restoring colors
            RestoreColors();
            //Check printing correctness
            //If it took more than one line (sometimes happen, when last char took last place in current line, it goes to the beginning of the next)
            //return carriage to prevoius line
            if ((Y - tempY) - 1 == 0)
                CursorY(Y - 1);
            //If not, just return
            else if(Y - tempY == 0)
            {
                return tempY - Y;
            }
            //In other case
            else
            {
                var message = $"Błąd podczas drukowania głównych nagłówków tabeli. String jest za długi, zajmuje więcej niż jedną linię. Metoda: {nameof(PrintMainHeaders)}, klasa: DrawingAtConsole.";
                throw new Exception(message);
            }
            return Y - tempY;
        }

        /// <summary>
        /// Prints data inside of columns. Automaticly count how many additional lines are took during printing.
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <param name="description">Put here all descriptions about data You want to print in columns.</param>
        /// <param name="leftColumnWriting">Put here values for first column.</param>
        /// <param name="rightColumnWriting">Put here values for second column.</param>
        /// <param name="color">Set color for left and right column.</param>
        /// <returns>How many additional lines took printing.</returns>
        public int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, Color color)
        {
            #region Data validation
            if(description.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(description), message);
            }

            if (leftColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(leftColumnWriting), message);
            }

            if (rightColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(rightColumnWriting), message);
            }

            if(startY < 0)
            {
                var message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(startY), message);
            }
            #endregion

            //Additional lines counter
            var lines = 0;

            //Printing description column
            var tempLines = PrintColumn(startY, description, _descriptionColumn_Middle, _descriptionColumn_SpaceToWriting, Color.White);
            lines = tempLines;

            //Printing left column
            tempLines = PrintColumn(startY, leftColumnWriting, _leftColumn_Middle, _leftColumn_SpaceToWriting, color);
            lines = lines > tempLines ? lines : tempLines;

            //Printing right column
            tempLines = PrintColumn(startY, rightColumnWriting, _rightColumn_Middle, _rightColumn_SpaceToWriting, color);
            lines = lines > tempLines ? lines : tempLines;

            //Automaticly fill table with vertical lines depends on largest amount of lines that took printing particular columns
            PrintVerticalLines(startY, lines);

            return lines;
        }

        public int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, 
            Color leftColumnColor, Color rightColumnColor)
        {
            #region Data validation
            if (description.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(description), message);
            }

            if (leftColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(leftColumnWriting), message);
            }

            if (rightColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(rightColumnWriting), message);
            }

            if (startY < 0)
            {
                var message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(startY), message);
            }
            #endregion

            //Additional lines counter
            var lines = 0;

            //Printing description column
            var tempLines = PrintColumn(startY, description, _descriptionColumn_Middle, _descriptionColumn_SpaceToWriting, Color.White);
            lines = tempLines;

            //Printing left column
            tempLines = PrintColumn(startY, leftColumnWriting, _leftColumn_Middle, _leftColumn_SpaceToWriting, leftColumnColor);
            lines = lines > tempLines ? lines : tempLines;

            //Printing right column
            tempLines = PrintColumn(startY, rightColumnWriting, _rightColumn_Middle, _rightColumn_SpaceToWriting, rightColumnColor);
            lines = lines > tempLines ? lines : tempLines;

            //Automaticly fill table with vertical lines depends on largest amount of lines that took printing particular columns
            PrintVerticalLines(startY, lines);

            return lines;
        }

        public int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, 
            Color leftColumnColor, Color rightColumnColor, Color descriptionColumnColor)
        {
            #region Data validation
            if (description.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(description), message);
            }

            if (leftColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(leftColumnWriting), message);
            }

            if (rightColumnWriting.Any(z => z == null))
            {
                var message = $"Nie można wydrukować nagłówka parametru. Tablica wejściowa zawiera w sobie string wskazujący na null. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(rightColumnWriting), message);
            }

            if (startY < 0)
            {
                var message = $"Nie można wydrukować nagłówka parametru. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. Metoda: {nameof(PrintSection)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentNullException(nameof(startY), message);
            }
            #endregion

            //Additional lines counter
            var lines = 0;

            //Printing description column
            var tempLines = PrintColumn(startY, description, _descriptionColumn_Middle, _descriptionColumn_SpaceToWriting, descriptionColumnColor);
            lines = tempLines;

            //Printing left column
            tempLines = PrintColumn(startY, leftColumnWriting, _leftColumn_Middle, _leftColumn_SpaceToWriting, leftColumnColor);
            lines = lines > tempLines ? lines : tempLines;

            //Printing right column
            tempLines = PrintColumn(startY, rightColumnWriting, _rightColumn_Middle, _rightColumn_SpaceToWriting, rightColumnColor);
            lines = lines > tempLines ? lines : tempLines;

            //Automaticly fill table with vertical lines depends on largest amount of lines that took printing particular columns
            PrintVerticalLines(startY, lines);

            return lines;
        }

        /// <summary>
        /// Printing vertical lines depends on constant values
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <param name="lines">Additional lines to print vertical separators.</param>
        private void PrintVerticalLines(int startY, int lines)
        {
            //Save current colors
            var tempB = Console.BackgroundColor;
            var tempF = Console.ForegroundColor;
            //Change color for lines
            LineColor();
            //Printing lines from startY (additional lines + first line)
            for(var i = 0; i < lines+1; i++)
            {
                CursorY(startY + i);
                CursorX((_consleOneFifth)-1);
                Console.Write("|");
                CursorX((_consleOneFifth * 3)-1);
                Console.Write("|");
            }
            //Bring back colors
            Console.ForegroundColor = tempF;
            Console.BackgroundColor = tempB;
        }

        /// <summary>
        /// Print data in specific column.
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <param name="writing">A writing to print.</param>
        /// <param name="middlePosition">Middle position of specific column.</param>
        /// <param name="spaceToWriting">Area to print in specific column</param>
        /// <param name="color">Text color (foreground).</param>
        /// <returns>How many additional lines took printing.</returns>
        private int PrintColumn(int startY, string[] writing, int middlePosition, int spaceToWriting, Color color)
        {
            //Check every string in array if fit to column bounds
            if (writing.Any(z => z.Length > spaceToWriting))
                //If not, convert array to one string, then split it on maximum allowed parts
                writing = writing.JoinArray().SplitInParts(spaceToWriting).ToArray();

            //Save colors
            var consoleF = Console.ForegroundColor;
            var consoleB = Console.BackgroundColor;
            //Set particular color
            SetConsoleForeground(color);
            //Set cursor position
            CursorY(startY);
            //Additional lines counter
            var lines = 0;
            //Printing
            for (; lines < writing.Length; lines++)
            {
                CursorY(startY + lines);
                CursorX(middlePosition - (writing[lines].Length / 2));
                Console.Write(writing[lines]);
            }
            //Return cursor starting position
            CursorY(startY);
            //Bringing back colors
            Console.ForegroundColor = consoleF;
            Console.BackgroundColor = consoleB;
            
            //Return additional lines minus first one (startY)
            return lines - 1;
        }

        /// <summary>
        /// Prints horizontal separator.
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <returns>How many lines took printing (should be 0).</returns>
        public int PrintHorizontalLine(int startY)
        {
            //Validation
            if (startY < 0)
            {
                var message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy zero. Metoda: {nameof(PrintHorizontalLine)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }
            
            //Set cursor position
            CursorY(startY);
            //Save current cursor position
            var tempY = Y;
            //Save colors
            var fcolor = Console.ForegroundColor;
            var bcolor = Console.BackgroundColor;
            //Change color for lines
            LineColor();
            //Return carriage to the begining of the line
            RestoreCursorX();
            //Print separator
            Console.Write(_horizontalLine);
            //Restore colors
            Console.ForegroundColor = fcolor;
            Console.BackgroundColor = bcolor;
            //Check printing correctness
            //If it took more than one line (sometimes happen, when last char took last place in current line, it goes to the beginning of the next)
            //return carriage to prevoius line
            if ((Y - tempY) - 1 == 0)
                CursorY(Y - 1);
            //If not, just return
            else if (Y - tempY == 0)
            {
                return Y - tempY;
            }
            //In other case
            else
            {
                var message = $"Błąd podczas drukowania poziomego separatora. String jest za długi, zajmuje więcej niż jedną linię. Metoda: {nameof(PrintHorizontalLine)}, klasa: DrawingAtConsole.";
                throw new Exception(message);
            }
            return Y - tempY;
        }

        /// <summary>
        /// Prints header for initialization screen
        /// </summary>
        /// <param name="startY">Console line number (from top to bottom).</param>
        /// <param name="bar">Title.</param>
        /// <returns>How many additional lines took printing (should by 0).</returns>
        public int PrintInitializationBar(int startY, string bar)
        {
            //Validation
            if(bar == null)
            {
                var message = $"Nie można wydrukować statusu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(bar), message);
            }

            if (startY < 0)
            {
                var message = $"Nie można wydrukować pionowego separatora. Parametr wejściowy wskazujący na pierwszą linię jest ujemny: {startY}. " +
                    $"Parametr musi być równy lub większy zero. Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.";
                throw new ArgumentOutOfRangeException(nameof(startY), message);
            }
            //Set cursor position
            CursorY(startY);
            //Save current position
            var tempY = Y;
            //Change color
            MainHeaderColor();
            //Print
            Console.Write(bar.PadBoth(Console.BufferWidth-1));
            //Restore colors
            RestoreColors();
            //Check printing correctness
            //If it took more than one line (sometimes happen, when last char took last place in current line, it goes to the beginning of the next)
            //return carriage to prevoius line
            if ((Y - tempY) - 1 == 0)
                CursorY(Y - 1);
            //If not, just return
            else if (Y - tempY == 0)
            {
                return Y - tempY;
            }
            //In other case
            else
            {
                var message = $"Błąd podczas drukowania nagłówka dla ekranu inicjalizacji. String jest za długi, zajmuje więcej niż jedną linię. Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.";
                throw new Exception(message);
            }
            return Y - tempY;
        }

        /// <summary>
        /// Print description for current action.
        /// </summary>
        /// <param name="Yposition">Console line number (from top to bottom).</param>
        /// <param name="title">Description.</param>
        /// <returns>How many additional lines took printing.</returns>
        public int PrintInitializationDescription(int Yposition, string title) => PrintInitializationComment(Yposition, title, Color.White);

        /// <summary>
        /// Print additional comment for current action.
        /// </summary>
        /// <param name="Yposition">Console line number (from top to bottom).</param>
        /// <param name="comment">Description.</param>
        /// <param name="color">Text foreground color.</param>
        /// <returns></returns>
        public int PrintInitializationComment(int Yposition, string comment, Color color)
        {
            #region Validation
            if (Yposition < 0)
            {
                var message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazujący na numer linii jest mniejszy od zera: {Yposition}. " +
                    $"Parametr musi być równy lub większy od zera. Metoda: {nameof(PrintInitializationDescription)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(Yposition), message);
            }

            if (comment == null)
            {
                var message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationDescription)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(comment), message);
            }
            #endregion
            //Additional lines counter
            var lines = 0; 
            //Check if comment is longer than allowed space
            //If is, split it in parts and print in multiple lines
            if (comment.Length > Console.BufferWidth - 15)
            {
                var splittedComment = comment.SplitInParts(Console.BufferWidth - 15).ToArray();
                SetConsoleForeground(color);
                for (var i = 0; i < splittedComment.Length; i++)
                {
                    CursorY(Yposition + lines);
                    RestoreCursorX();
                    Console.Write(splittedComment[i]);
                    lines++;
                }
                lines--; //Minus start line
                RestoreColors();
            }
            //If not, just print
            else
            {
                SetConsoleForeground(color);
                CursorY(Yposition);
                RestoreCursorX();
                Console.Write(comment);
                RestoreColors();
            }

            return lines;
        }

        /// <summary>
        /// Print status for initialization action. Yposition must points at last used line.
        /// </summary>
        /// <param name="Yposition">Console line number (from top to bottom). Put here last line used by InitializationPrinting methods.</param>
        /// <param name="status">Status for action.</param>
        /// <param name="color">Color of statuc writing.</param>
        /// <returns>How many lines took printing (sholud be 0).</returns>
        public int PrintInitializationStatus(int Yposition, string status, Color color)
        {
            #region Validation
            if (Yposition < 0)
            {
                var message = $"Nie można wydrukować opisu inicjalizacji. Argument wejściowy wskazujący na numer linii jest mniejszy od zera: {Yposition}. " +
                    $"Parametr musi być równy lub większy od zera. Metoda: {nameof(PrintInitializationStatus)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(Yposition), message);
            }

            if (status == null)
            {
                var message = $"Nie można wydrukować statusu inicjalizacji. Argument wejściowy wskazuje na null. " +
                    $"Metoda: {nameof(PrintInitializationStatus)}, klasa: DrawingAtConsole.cs.";
                throw new ArgumentOutOfRangeException(nameof(status), message);
            }
            #endregion
            
            //Change text color
            SetConsoleForeground(color);
            //Set cursor position
            CursorY(Yposition);
            //Save current Y position
            var tempY = Y;
            //Return carriage to the begining of the line
            CursorX(Console.BufferWidth - status.Length - 1);
            //Printing
            Console.Write(status);
            //Restore colors
            RestoreColors();
            //Check printing correctness
            //If it took more than one line (sometimes happen, when last char took last place in current line, it goes to the beginning of the next)
            //return carriage to prevoius line
            if ((Y - tempY) - 1 == 0)
                CursorY(Y - 1);
            //If not, just return
            else if (Y - tempY == 0)
            {
                return Y - tempY;
            }
            //In other case
            else
            {
                var message = $"Błąd podczas drukowania nagłówka dla ekranu inicjalizacji. String jest za długi, zajmuje więcej niż jedną linię. Metoda: {nameof(PrintInitializationBar)}, klasa: DrawingAtConsole.";
                throw new Exception(message);
            }
            return Y - tempY;
        }

        /// <summary>
        /// Useless method, but needed for unit tests.
        /// </summary>
        public void Wait() => Console.ReadKey();

        public void ClearLine(int Y)
        {
            CursorY(Y);
            Console.Write(_clearLine);
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
        private void SetConsoleBackground(Color color)
        {
            Colorful.Console.BackgroundColor = color;
        }

        private void SetConsoleForeground(Color color)
        {
            Colorful.Console.ForegroundColor = color;
        }

        private void DescriptionColor()
        {
            RestoreColors();
        }

        private void RestoreColors()
        {
            SetConsoleForeground(Color.White);
            SetConsoleBackground(_defaultBackground);
        }

        private void MainHeaderColor()
        {
            SetConsoleForeground(Color.White);
            SetConsoleBackground(Color.DarkBlue);
        }

        private void LineColor()
        {
            SetConsoleForeground(Color.DarkGoldenrod);
        }
        #endregion
    }
}
