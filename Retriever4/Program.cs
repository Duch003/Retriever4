using Retriever4.Classes;
using Retriever4.Enums;
using Retriever4.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Retriever4
{
    class Program
    {
        private static Configuration Config;
        private static string _done = "Zrobione.";
        private static int _pad = 66;
        private static Location _model = null;
        private static ObservableCollection<Location> _modelList;
        private static ObservableCollection<Container> _containersList;
        private static string _horizontalLine = "";
        private static int _horizontalLineAxis;
        private static int _staticPosition = (int)Math.Round((float)(_horizontalLine.Length / 4), 0);

        static void Main(string[] args)
        {
            Console.SetWindowSize(84, Console.LargestWindowHeight - 10);
            Console.SetBufferSize(84, 200);

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
            _staticPosition = tempLine.Length / 4;

            Initialization();
            if (!Menu())
                FindModel();
            if (_model == null)
                return;
            PrintDetails();

        }

        private static bool Menu()
        {
            bool breakLoop1 = false;
            bool isGood = false;
            Console.Clear();
            if (_model != null)
            {
                ConsoleKeyInfo z;
                do
                {
                    Console.WriteLine(_model.Model);
                    Console.WriteLine("Czy model urządzenia jest poprawny? (Y/N): ");
                    z = Console.ReadKey();
                    switch (z.Key)
                    {
                        case ConsoleKey.Y:
                            breakLoop1 = true;
                            isGood = true;
                            break;
                        case ConsoleKey.N:
                            breakLoop1 = true;
                            isGood = false;
                            break;
                        default:
                            break;
                    }
                } while (!breakLoop1);
            }
            return isGood;
        }

        private static void FindModel()
        {
            bool breakLoop1 = false, breakLoop2 = false;
            string pattern = "";
            int index = 6, patternLength = pattern.Length; ; //0
            Location[] ans = null;
            //Pierwsza pętla. Osiągana za każdym razem gdy zmieni się pattern
            while (!breakLoop1)
            {
                breakLoop2 = false;
                //Wyczyszczenie konsoli.
                Console.Clear();
                //Ustawienie kursora na początek
                Console.SetCursorPosition(0, 0);
                //Wypisanie informacji na ekran
                Console.Write("Zacznij wpisywać model (min. 3 znaki), a modele pasujące do wzorca zostaną wyświetlone poniżej.");
                Console.WriteLine("Strzałkami w górę i w dół przesuwasz się między modelami. Kiedy znajdziesz potrzebny - kliknij ENTER.");
                Console.Write(pattern);

                //Druga pętla
                while (!breakLoop2)
                {
                    if (pattern.Length < 3)
                        patternLength = pattern.Length;
                    //Wypisanie tabeli ze znalezionymi modelami
                    if (pattern.Length >= 3 && patternLength != pattern.Length)
                    {
                        patternLength = pattern.Length;
                        breakLoop1 = false;
                        breakLoop2 = false;
                        Console.SetCursorPosition(0, 4); //Indeks od którego zaczynam = 0, indeks nr linii = 4

                        var temp = from x in _modelList
                                   where x.Model == null ? false : x.Model.Contains(pattern)
                                   //|| x.MSN == null ? false : x.MSN.Contains(pattern)
                                   //|| x.OldMSN == null ? false : x.OldMSN.Contains(pattern)
                                   select x;
                        ans = temp.ToArray();
                        ConsoleHeaderColors();
                        //                 0       9|11                            41|43                          72|74       
                        Console.WriteLine("  MODEL  |               MSN              |          Stary MSN           |SELEKCJA");
                        ConsoleRestoreColors();
                        Console.WriteLine("---------+--------------------------------+------------------------------+--------");
                        for (int i = 0; i < ans.Count(); i++)
                        {
                            ConsoleGreenMode();
                            Console.Write(ans[i].Model);
                            ConsoleRestoreColors();
                            Console.SetCursorPosition(9, Console.CursorTop);
                            Console.Write("|" + ans[i].MSN);
                            Console.SetCursorPosition(42, Console.CursorTop);
                            Console.Write("|" + ans[i].OldMSN);
                            Console.SetCursorPosition(73, Console.CursorTop);
                            if (Console.CursorTop == index)
                                Console.WriteLine("|  [X]");
                            else
                                Console.WriteLine("|  [ ]");
                            Console.WriteLine("---------+--------------------------------+------------------------------+--------");
                        }
                        Console.SetCursorPosition(77, 6);
                    }

                    //Ustawienie kursora w linię w której wpisauje się szukany model

                    ConsoleKeyInfo z = Console.ReadKey();

                    switch (z.Key)
                    {
                        case ConsoleKey.Backspace:
                            if (pattern.Length > 0)
                            {
                                pattern = pattern.Remove(pattern.Length - 1);
                                //patternLength = pattern.Length;
                                breakLoop2 = true;
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (ans == null)
                                break;
                            if (ans.Length > 0)
                            {
                                _model = ans[(index - 6) / 2];
                                breakLoop1 = breakLoop2 = true;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (index >= ans.Length * 2 + 4)
                            {
                                Console.SetCursorPosition(77, index);
                                Console.Write("X]");
                                break;
                            }
                            else
                            {
                                Console.SetCursorPosition(77, index);
                                Console.Write(" ]");
                                index += 2; ;
                                Console.SetCursorPosition(77, index);
                                Console.Write("X]");
                                break;
                            }
                        case ConsoleKey.UpArrow:
                            if (index <= 6)
                            {
                                Console.SetCursorPosition(79, index);
                                break;
                            }
                            else
                            {
                                Console.SetCursorPosition(77, index);
                                Console.Write(" ]");
                                index -= 2; ;
                                Console.SetCursorPosition(77, index);
                                Console.Write("X]");
                                break;
                            }
                        default:
                            if (char.IsLetterOrDigit(z.KeyChar))
                            {
                                pattern = pattern + z.KeyChar;
                                Console.SetCursorPosition(pattern.Length, 2);
                                index = 6;
                                breakLoop2 = true;
                            }
                            else
                            {
                                Console.SetCursorPosition(77, index);
                                Console.Write("X]");
                            }
                            break;
                    }
                }
            }
        }

        private static void PrintHeaders()
        {
            ConsoleSeparator();
            Console.WriteLine(_horizontalLine);
            ConsoleHeaderColors();
            int position = (int)Math.Round((float)("RZECZYWISTE".Length / 2), 0);
            Console.Write("RZECZYWISTE".PadLeft(position + 20));
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

        private static void PrintSection(string header, string realValue, string dbValue, ConsoleColor mode, bool justData)
        {
            int position;
            if (!justData)
            {
                ConsoleSeparator();
                position = (int)Math.Round((float)((header.Length + 1) / 2), 0);
                Console.WriteLine(header.PadLeft(_horizontalLineAxis + position));
                PrintHorizontalLine();
            }

            Console.ForegroundColor = mode;
            position = (int)Math.Round((float)(realValue?.Length / 2), 0);
            Console.Write(realValue.PadLeft(position + _staticPosition));
            PrintVerticalLine();
            Console.ForegroundColor = mode;
            position = (int)Math.Round((float)(dbValue?.Length / 2), 0);
            Console.WriteLine(dbValue.PadLeft(position + _staticPosition));
        }

        private static void PrintInternalData(string header, string leftValue, string[] rightValues, ConsoleColor mode, bool justData, bool leftAlign)
        {
            int position;
            if (!justData)
            {
                ConsoleSeparator();
                position = (int)Math.Round((float)((header.Length + 1) / 2), 0);
                Console.WriteLine(header.PadLeft(_horizontalLineAxis + position));
                PrintHorizontalLine();
            }

            ConsoleRestoreColors();

            string[] tempLeftValue;
            if (leftValue.Length > _horizontalLineAxis)
            {
                tempLeftValue = leftValue.SplitInParts(_horizontalLineAxis - 1).ToArray();
                int cursorTop = Console.CursorTop;
                for (int i = 0; i < tempLeftValue.Length; i++)
                {
                    position = (int)Math.Round((float)(tempLeftValue[i].Length / 2), 0);
                    ConsoleRestoreColors();
                    Console.Write(tempLeftValue[i].PadLeft(_staticPosition + position));
                    PrintVerticalLine();
                    Console.SetCursorPosition(0, cursorTop + i + 1);
                }
                Console.SetCursorPosition(Console.CursorLeft, cursorTop);
            }
            else
            {
                position = (int)Math.Round((float)(leftValue.Length / 2), 0);
                Console.Write(leftValue.PadLeft(position + _staticPosition));
            }

            for (int i = 0; i < rightValues.Length; i++)
            {
                PrintVerticalLine();
                Console.ForegroundColor = mode;
                if (!leftAlign)
                    position = (int)Math.Round((float)(rightValues[i].Length / 2), 0) + _staticPosition;
                else
                    position = 0;
                Console.WriteLine(rightValues[i].PadLeft(position));
            }
        }

        private static void PrintHorizontalLine(bool newline = false)
        {
            ConsoleSeparator();
            if (!newline)
                Console.WriteLine(_horizontalLine);
            else
                Console.WriteLine("\n" + _horizontalLine);
        }

        private static void PrintVerticalLine()
        {
            Console.SetCursorPosition(_horizontalLineAxis, Console.CursorTop);
            ConsoleSeparator();
            Console.Write("|");
        }

        private static void PrintDetails()
        {
            Console.Clear();

            #region KOLUMNY
            PrintHeaders();
            #endregion

            #region MODEL
            object dbValue = null;
            try
            {
                dbValue = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, Config.DatabaseTableName, _model.DBRow, (int)Config.MD_ModelColumn);
            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji MODEL. Prawdopobnie plik bazy danych jest uruchomiony przez inny program.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }

            if (dbValue.ToString() != _model.Model)
                PrintSection("MODEL", _model.Model, dbValue.ToString(), ConsoleColor.Red, false);
            else
                PrintSection("MODEL", _model.Model, dbValue.ToString(), ConsoleColor.Green, false);
            #endregion

            #region OS
            var task = _containersList.Where(z => z.Section == ProgramSection.OS).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                try
                {
                    task.WMIResult = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
                    if ((bool)!task.UseConstant)
                        task.DatabaseResult = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                    else
                        task.DatabaseResult = task.Constant;
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji OS.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                //Określanie systemu operacyjnegp
                OS osDb = OS.None, osReal = OS.None;
                if (task.DatabaseResult.ToString().Contains("10"))
                    osDb = OS.Windows10;
                else if (task.DatabaseResult.ToString().Contains("8"))
                    osDb = OS.Windows8;
                else
                    osDb = OS.None;

                if (!(osDb == OS.None))
                {
                    if (task.WMIResult.ToString().Contains("10"))
                        osReal = OS.Windows10;
                    else
                        osReal = OS.Windows8;
                }

                if (osDb == osReal)
                    PrintSection("OS", osReal.ToString(), osDb.ToString(), ConsoleColor.Green, false);
                else
                    PrintSection("OS", osReal.ToString(), osDb.ToString(), ConsoleColor.Red, false);
            }
            #endregion

            #region SWM
            task = _containersList.Where(z => z.Section == ProgramSection.SWM).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                //Wydobycie danych
                try
                {
                    task.HiddenConstant = Retriever.GetSwm();
                    task.DatabaseResult = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji SWM.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                //Walidacja
                bool switch0 = true;
                string[] message = new string[0];
                if (task.HiddenConstant == null)
                {
                    message.Expand();
                    message[message.Length - 1] = "\n         Nie znaleziono plików swconf.dat na urządzeniu; ";
                    switch0 = false;
                }
                if (task.DatabaseResult == null || task.DatabaseResult?.ToString().Length == 0)
                {
                    message.Expand();
                    message[message.Length - 1] = "\n         Brak SWM w bazie danych";
                    switch0 = false;
                }

                //Drukowanie
                if (switch0)
                {
                    switch0 = false;
                    for (int i = 0; i < (task.HiddenConstant as string[]).Length; i++)
                    {
                        var computer = (task.HiddenConstant as string[])[i].ToString();
                        string[] database = task.DatabaseResult.ToString().Replace(" ", "").Replace(@"\", @"/").Split('/');
                        bool isPrinted = false;
                        for (int j = 0; j < database.Length; j++)
                        {
                            if (computer.Contains(database[j]))
                            {
                                PrintSection("SWM", computer.Replace(";", " "), task.DatabaseResult.ToString(), ConsoleColor.Green, switch0);
                                switch0 = true;
                                isPrinted = true;
                            }
                            else if (database.Length - 1 == j && !isPrinted)
                            {
                                PrintSection("SWM", computer.Replace(";", " "), task.DatabaseResult.ToString(), ConsoleColor.Red, switch0);
                                switch0 = true;
                                isPrinted = true;
                            }
                            if (isPrinted)
                                continue;
                        }
                    }
                }
                else
                {
                    ConsoleRestoreColors();
                    Console.Write("Nie można porównać numerów SWM: ");
                    ConsoleWarningMode();
                    for (int i = 0; i < message.Length; i++)
                        Console.Write(message[i]);
                    Console.WriteLine();
                }
            }
            #endregion

            #region WearLevel
            task = _containersList.Where(z => z.Section == ProgramSection.WearLevel).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                int tag = 0;
                uint designedCapacity = 0;
                uint[] fullChargeCapacity = new uint[0];
                try
                {
                    //TODO zmienić w pracy tak aby wypluwał warning zamiast error
                    //Najpierw pobrać tag
                    string query = "SELECT Tag FROM BatteryStaticData";
                    string property = "Tag";
                    string scope = @"ROOT\wmi";
                    var tempTag = Retriever.ReadDetailsFromComputer(query, property, scope);
                    if (!int.TryParse(tempTag.ToString(), out tag))
                    {
                        string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(int)}: {(string)tempTag}";
                        throw new Exception(message);
                    }

                    //Pobrać dodatkowo DesignedCapacity (będzie potrzebne potem)
                    query = "SELECT DesignedCapacity FROM BatteryStaticData";
                    property = "DesignedCapacity";
                    var tempDC = Retriever.ReadDetailsFromComputer(query, property, scope);
                    if (!uint.TryParse(tempDC.ToString(), out designedCapacity))
                    {
                        string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(uint)}: {(string)tempDC}";
                        throw new Exception(message);
                    }

                    //Pobrać fullChargeCapacity
                    query = $"SELECT FullChargedCapacity FROM BatteryFullChargedCapacity WHERE Tag = {tag}";
                    property = "FullChargedCapacity";
                    var tempCC_IEnumerable = Retriever.ReadArrayFromComputer(query, property, scope);
                    if (tempCC_IEnumerable.Count() == 0)
                    {
                        string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Wynikowa tablica jest pusta (wymagany przynajmniej jeden element).";
                        throw new Exception(message);
                    }
                    else
                    {
                        var tempCC = tempCC_IEnumerable.ToArray();
                        for (int i = 0; i < tempCC.Count(); i++)
                        {
                            fullChargeCapacity.Expand();
                            if (tempCC[i] == null)
                            {
                                fullChargeCapacity[i] = 0;
                                continue;
                            }

                            if (!uint.TryParse(tempCC[i].ToString(), out fullChargeCapacity[i]))
                            {
                                string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(uint)}: {(string)tempCC[i]}.";
                                throw new Exception(message);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji WearLevel.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < fullChargeCapacity.Count(); i++)
                {
                    bool switch0 = false;
                    if (fullChargeCapacity[i] == 0)
                        continue;
                    double temp = Math.Round(1 - (fullChargeCapacity[i] / (double)designedCapacity), 4) * 100;
                    string wearLevel = temp.ToString() + "%";
                    string maxWearLevel = task.Constant[0].Value + "%";
                    if (temp > Convert.ToDouble(task.Constant[0].Value))
                    {
                        PrintSection("Wear level", wearLevel, maxWearLevel, ConsoleColor.Red, switch0);
                        switch0 = true;
                    }
                    else
                    {
                        PrintSection("Wear level", wearLevel, maxWearLevel, ConsoleColor.Green, switch0);
                        switch0 = true;
                    }
                }

            }
            #endregion

            #region Płyta główna
            task = _containersList.Where(z => z.Section == ProgramSection.Mainboard).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                //Pobranie danych
                object currentMaiboard = null;
                object dbMaiboard = null;
                try
                {
                    currentMaiboard = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
                    dbMaiboard = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Płyta główna.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                //Sprawdzenie dowolnego znaku (x - dowolny znak w ciągu)
                if (dbMaiboard.ToString().ToLower().Contains("x"))
                {
                    //Walidaacja
                    string[] tempDbMaibaord = dbMaiboard.ToString().ToLower().Split('x');
                    bool isGood = true;
                    for (int i = 0; i < tempDbMaibaord.Length; i++)
                    {
                        if (currentMaiboard.ToString().ToLower().Contains(tempDbMaibaord[i]))
                            continue;
                        else
                            isGood = false;
                        if (!isGood)
                            break;
                    }
                    //Drukowanie
                    if (isGood)
                        PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Green, false);
                    else
                        PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Red, false);
                }
                else
                {
                    //Walidacja i drukowanie
                    string tempDbMainboard = dbMaiboard.ToString().ToLower();
                    if (currentMaiboard.ToString().Contains(tempDbMainboard.ToString()) || dbMaiboard.ToString().Contains(currentMaiboard.ToString()))
                        PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Green, false);
                    else
                        PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Red, false);
                }
            }
            #endregion

            #region CPU
            task = _containersList.Where(z => z.Section == ProgramSection.CPU).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                object currentCPU = null;
                object dbCPU = null;
                PrintHorizontalLine();
                try
                {
                    currentCPU = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
                    dbCPU = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji CPU.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
                if (currentCPU.ToString().ToLower().Contains(dbCPU.ToString().ToLower()))
                    PrintSection("CPU", currentCPU.ToString().Replace("Intel ", "").Replace("Core ", "").Replace("(TM) ", "").Replace("(R) ", "").Replace("CPU ", "").Replace("  ", ""), dbCPU.ToString(), ConsoleColor.Green, false);
                else
                    PrintSection("CPU", currentCPU.ToString().Replace("Intel", "").Replace("Core", "").Replace("(TM)", "").Replace("(R)", "").Replace("CPU", "").Replace("  ", ""), dbCPU.ToString(), ConsoleColor.Red, false);
            }
            #endregion

            #region Bios
            //TODO Dopisać wyświetlanie modelu plyty glownej (D17K itd)

            task = _containersList.Where(z => z.Section == ProgramSection.Bios).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                object currentBios = null;
                object dbBios = null;
                try
                {
                    currentBios = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
                    dbBios = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.BiosRow, (int)task.Column);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji CPU.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                if (_model.BiosRow == 0)
                {
                    dbBios = currentBios;
                }

                ConsoleColor color;
                if (currentBios.ToString().Contains(dbBios.ToString()) || dbBios.ToString().Contains(currentBios.ToString()))
                    color = ConsoleColor.Green;
                else
                    color = ConsoleColor.Red;

                bool isPrinted = false;
                var tempBios = dbBios.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < tempBios.Length; i++)
                {
                    if (!isPrinted)
                    {
                        PrintSection("Bios", currentBios.ToString(), tempBios[i].ToString(), color, isPrinted);
                        isPrinted = true;
                    }
                        
                    else
                    {
                        PrintSection("Bios", "", tempBios[i].ToString(), color, isPrinted);
                        isPrinted = true;
                    }
                }
            }
            #endregion

            #region MaiboardModel
            task = _containersList.Where(z => z.Section == ProgramSection.MainboardModel).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                object currentModel = null;
                object dbModel = null;
                try
                {
                    currentModel = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
                    dbModel = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Bios.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                if (_model.BiosRow == 0)
                    dbModel = currentModel + @" / brak modelu w bazie";


                if (currentModel == null)
                {
                    currentModel = "Nie wykryto modelu płyty głównej";
                    PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
                }

                if (dbModel == null || (string)dbModel == "")
                {
                    dbModel = "Brak modelu płyty głównej w bazie.";
                    PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
                }

                string[] tempDbModel = dbModel.ToString().ToLower().Split('x');

                if (tempDbModel.Length > 1)
                {
                    bool match = true;
                    for (int i = 0; i < tempDbModel.Length; i++)
                        match = currentModel.ToString().Contains(tempDbModel[i]) && match;

                    if (match)
                        PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Green, true);
                    else
                        PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);

                }
                else
                {
                    if (currentModel.ToString().Contains(dbModel.ToString()) || dbModel.ToString().Contains(currentModel.ToString()))
                        PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Green, true);
                    else
                        PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
                }
            }
            #endregion

            #region RAM
            task = _containersList.Where(z => z.Section == ProgramSection.RAM).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                PrintHorizontalLine();
                ulong[] realSize = new ulong[0];
                object dbSize = null;
                try
                {
                    var tempRealSize = Retriever.ReadArrayFromComputer(task.Query, task.Property, task.Scope).ToArray();
                    dbSize = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                    for (int i = 0; i < tempRealSize.Length; i++)
                    {
                        realSize.Expand();
                        if (!ulong.TryParse(tempRealSize[i].ToString(), out realSize[i]))
                        {
                            string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość inną niż {typeof(ulong)}: {tempRealSize.ToString()}";
                            throw new Exception(message);
                        }
                    }

                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji RAM.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                int fullRealSize = 0;
                for (int i = 0; i < realSize.Length; i++)
                {
                    fullRealSize += (int)Math.Round((decimal)(realSize[i] / 1000000000), 0);
                }

                if (dbSize.ToString().Replace(" ", "").ToLower().Equals((fullRealSize.ToString() + "GB").ToLower()))
                    PrintSection("RAM", fullRealSize.ToString() + " GB", dbSize.ToString(), ConsoleColor.Green, false);
                else
                    PrintSection("RAM", fullRealSize.ToString() + " GB", dbSize.ToString(), ConsoleColor.Red, false);
            }
            #endregion

            #region Storage
            task = _containersList.Where(z => z.Section == ProgramSection.Storage).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                //Pobranie danych
                PrintHorizontalLine();
                ulong[] realSize = new ulong[0];
                double[] dbSize = new double[0];
                try
                {
                    //Konwersja rzeczywistej wielkości pamięci na ulong
                    var tempRealSize = Retriever.ReadArrayFromComputer(task.Query, task.Property, task.Scope).ToArray();
                    for (int i = 0; i < tempRealSize.Length; i++)
                    {
                        realSize.Expand();
                        if (!ulong.TryParse(tempRealSize[i].ToString(), out realSize[i]))
                        {
                            string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość {tempRealSize[i].ToString()}";
                            throw new Exception(message);
                        }
                    }

                    //Rozbicie informacji z bazy na poszczególne dyski
                    var tempDbSize = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
                    var splitedDbSize = tempDbSize.ToString().Split(';');
                    for (int i = 0; i < splitedDbSize.Length; i++)
                    {

                        bool gbUnit = splitedDbSize[i].ToLower().Contains("gb");
                        var tempDbSizeValue = splitedDbSize[i].RemoveLetters().Replace(',', '.').RemoveWhiteSpaces();
                        if (string.IsNullOrEmpty(tempDbSizeValue))
                            continue;
                        dbSize.Expand();
                        if (!double.TryParse(tempDbSizeValue, out dbSize[i]))
                        {
                            string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość {tempDbSizeValue.ToString()}, {splitedDbSize[i]}.";
                            throw new Exception(message);
                        }
                        else
                        {
                            if (!gbUnit)
                                dbSize[i] *= 1000;
                        }
                    }
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Storage.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
                bool switch0 = false;
                double gigabyte = 1000000000;//1073741824;//[B]

                //Drukowanie wartosci prawidlowych
                //Podstawa: wartosc z bazy danych przeksztalcam z B na GB
                //Nastepnie wyliczam margines bledu (15%)
                //Potem sprawdzam czy w drugiej petli czy ktoras z wartosci w tablicy miesci sie w marginesie bledu
                //Jezeli tak to drukuje na ekran, usuwam wydrukowane indexy i skracam tablice
                for (int i = realSize.Length; i != 0; i--)
                {
                    double convertedValue = Math.Round((realSize[i - 1] / gigabyte), 0);
                    double lowerLimit = convertedValue * 0.85;
                    double upperLimit = convertedValue * 1.15;
                    for (int j = dbSize.Length; j > 0; j--)
                    {
                        if (dbSize[j - 1] >= lowerLimit && dbSize[j - 1] <= upperLimit)
                        {
                            PrintSection("Dyski twarde", convertedValue + " GB", dbSize[j - 1] + " GB", ConsoleColor.Green, switch0);
                            switch0 = true;
                            dbSize.RemoveIndexAndShrink(j - 1);
                            realSize.RemoveIndexAndShrink(i - 1);
                            break;
                        }
                    }
                }

                //Drukowanie wartosci nieprawidlowych - leci tak jak jest zapisane w tablicach
                int maxLength = dbSize.Length > realSize.Length ? dbSize.Length : realSize.Length;
                for (int j = 0; j < maxLength; j++)
                {
                    string leftValue, rightValue;
                    if (j >= realSize.Length)
                        leftValue = "Nie znaleziono dysku";
                    else
                        leftValue = Math.Round((realSize[j] / gigabyte), 0) + " GB";
                    if (j >= dbSize.Length)
                        rightValue = "Nie znaleziono dysku";
                    else
                        rightValue = dbSize[j] + " GB";
                    PrintSection("Dyski twarde", leftValue, rightValue, ConsoleColor.Red, switch0);
                    switch0 = true;
                }
            }
            #endregion

            #region Statusy
            PrintHorizontalLine();
            task = task = _containersList.Where(z => z.Section == ProgramSection.Shipping).Select(z => z).First();
            if ((bool)task.ShowSection)
            {
                object tempShippingMode;
                try
                {
                    tempShippingMode = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column).ToString();
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji RAM.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }

                if (tempShippingMode.ToString() == "Nie")
                    PrintInternalData("Shipping mode", "Shipping mode:", new string[] { tempShippingMode.ToString() }, ConsoleColor.Yellow, false, false);
                else
                    PrintInternalData("Shipping mode", "Shipping mode:", new string[] { tempShippingMode.ToString() }, ConsoleColor.Green, false, false);
            }


            PrintHorizontalLine();
            string windowsKey;
            WindowsActivationStatus windowsStatus;
            SecureBootStatus secureBoot;
            Dictionary<string, DeviceManagerErrorCode> deviceManager;
            bool availableNetworks;
            Dictionary<string, string> mac;

            try
            {
                windowsKey = Retriever.GetWindowsProductKey();
                windowsStatus = Retriever.CheckWindowsActivationStatus();
                secureBoot = (SecureBootStatus)((ushort)Retriever.CheckSecureBootStatus());
                deviceManager = Retriever.CheckDeviceManager();
                availableNetworks = Retriever.CheckWirelessConnection();
                mac = Retriever.CheckEthernetInterfaceMAC();

            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Statusy - Windows.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            if (string.IsNullOrEmpty(windowsKey))
            {
                windowsKey = "Nie znaleziono";
                PrintInternalData("Statusy", "Klucz Windows:", new string[] { windowsKey }, ConsoleColor.Red, false, false);
            }
            else
                PrintInternalData("Statusy", "Klucz Windows:", new string[] { windowsKey }, ConsoleColor.Green, false, false);

            #region Statusy - Windows
            if (windowsStatus != WindowsActivationStatus.Licensed)
                PrintInternalData(null, "Status aktywacji:", new string[] { windowsStatus.ToString() }, ConsoleColor.Red, true, false);
            else
                PrintInternalData(null, "Status aktywacji:", new string[] { windowsStatus.ToString() }, ConsoleColor.Green, true, false);

            if (secureBoot == SecureBootStatus.Disabled)
                PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Red, true, false);
            else if (secureBoot == SecureBootStatus.NotSupported)
                PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Yellow, true, false);
            else
                PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Green, true, false);
            #endregion

            PrintHorizontalLine();
            if (availableNetworks)
                PrintInternalData("Sieć bezprzewodowa", "Wykrywanie sieci WiFi:", new string[] { "Wykryto" }, ConsoleColor.Green, false, false);
            else
                PrintInternalData("Sieć bezprzewodowa", "Wykrywanie sieci WiFi:", new string[] { "Nie wykryto" }, ConsoleColor.Red, false, false);

            PrintHorizontalLine();
            if (mac.Count() == 0)
                PrintInternalData("Adresy MAC", "Brak interfejsu Ethernetowego", new string[] { "Brak adresu MAC" }, ConsoleColor.Red, false, false);
            else
            {
                bool switch0 = false;
                for (int i = 0; i < mac.Count(); i++)
                {
                    PrintInternalData("Adresy MAC", mac.First().Key, new string[] { mac.First().Value }, ConsoleColor.Green, switch0, false);
                    switch0 = true;
                    mac.Remove(mac.First().Key);
                }
            }

            PrintHorizontalLine();
            if (deviceManager.Count() == 0)
                PrintInternalData("Menedżer urządzeń", "Status urządzeń:", new string[] { "Wszystkie urządzenia działają poprawnie" }, ConsoleColor.Green, false, false);
            else
            {
                bool switch0 = false;
                for (int i = 0; i < deviceManager.Count(); i++)
                {
                    string[] info = ConfigManagerErrorDescription.ReturnDescription(deviceManager.First().Value).SplitInParts(35).ToArray();
                    PrintInternalData("Menedżer urządzeń", deviceManager.First().Key, info, ConsoleColor.Red, switch0, false);
                    switch0 = true;
                    deviceManager.Remove(deviceManager.First().Key);
                }
            }
            #endregion
            ConsoleSeparator();
            PrintHorizontalLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            return;
        }

        private static void Initialization(string model = "")
        {
            //Rozpoczęcie procesu weryfikującego czy aplikację można uruchomić
            ConsoleHeaderColors();
            Console.Write("===================PRZYGOTOWANIE APLIKACJI DO DZIAŁANIA===================");
            ConsoleRestoreColors();
            Console.WriteLine();

            #region Sprawdzenie czy istnieje plik Config.xml
            Console.Write("Sprawdzenie czy istnieje plik Config.xml");
            if (!Retriever.DoesConfigFileExists)
            {
                ConsoleErrorMode();
                Console.WriteLine($"\nNie znaleziono pliku kofiguracyjnego Config.xml w ścieżce {Environment.CurrentDirectory}. Aplikacja nie może zostać uruchomiona");
                Console.WriteLine("Wciśnij ENTER aby kontynuować.");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Deserializacja pliku Config.xml
            //Próba odczytu pliku konfiguracyjnego
            Console.Write("Deserializacja pliku Config.xml.");
            try
            {
                Config = Retriever.ReadConfiguration();
            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd podczas próby odczytania pliku konfiguracyjnego.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie poprawności wypełnienia Config.xml
            //Sprawdzenie czy niezbędne pola pliku konfiguracyjnego zostały wypełnione
            Console.Write("Sprawdzenie poprawności wypełnienia Config.xml.");
            if (Config.CheckFiledsForNulls())
            {
                ConsoleErrorMode();
                string message = $"\nAplikacja nie może zostać uruchomiona. W pliku konfiguracyjnym występują wartości null.";
                Console.WriteLine(message);
                Console.WriteLine("Wciśnij ENTER aby kontynuować.");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie istanienia plików Schema.xml
            //Sprawdzenie czy istnieją pliki: baza danych i schemat odczytu
            Console.Write($"Sprawdzenie istnienia plików Schema.xml i {Config.Filename.Replace(@"\", "")}");
            if (!Retriever.DoesSchemaFileExists || !Retriever.DoesDatabaseFileExists(Config.Filepath, Config.Filename))
            {
                string message = "";
                if (!Retriever.DoesSchemaFileExists && Retriever.DoesDatabaseFileExists(Config.Filepath, Config.Filename))
                {
                    message = $"\nNie znaleziono pliku Schema.xml w ścieżce {Environment.CurrentDirectory}. Aplikacja nie może zostać uruchomiona";
                }
                else if (!Retriever.DoesDatabaseFileExists(Config.Filepath, Config.Filename) && Retriever.DoesSchemaFileExists)
                {
                    message = $"\nNie znaleziono pliku {Config.Filename} w ścieżce {Environment.CurrentDirectory}. Aplikacja nie może zostać uruchomiona";
                }
                else
                {
                    message = $"\nNie znaleziono plików Schema.xml oraz {Config.Filename} w ścieżce {Environment.CurrentDirectory}. Aplikacja nie może zostać uruchomiona";
                }
                ConsoleErrorMode();
                Console.WriteLine(message);
                Console.WriteLine("Wciśnij ENTER aby kontynuować.");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie czy istnieje plik SHA1.txt
            //Sprawdzenie czy istnieje plik SHA1.txt
            Console.Write("Sprawdzenie czy istnieje plik SHA1.txt");
            if (!Retriever.DoesHashFileExists)
            {
                ConsoleWarningMode();
                Console.Write("\nUtworzenie pliku SHA1.txt");
                ConsoleRestoreColors();
                try
                {
                    var hash = Retriever.ComputeSHA1(Config.Filepath, Config.Filename);
                    Retriever.WriteHash(hash);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd podczas tworzenia nowego pliku SHA1.txt.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie czy istnieje plik Model.xml
            //Określenie czy istnieje plik Model.xml, jeżeli nie, to utworzenie go
            Console.Write("Sprawdzenie czy istnieje plik Model.xml");
            if (!Retriever.DoestModelListFileExists)
            {
                ConsoleWarningMode();
                Console.Write("\nUtworzenie pliku Model.xml");
                ConsoleRestoreColors();
                try
                {
                    Retriever.SerializeModelList(Config);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd podczas tworzenia nowego pliku Model.xml.", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Pobieranie haszy
            //Pobranie haszy
            Console.Write("Pobranie haszy");
            string currentHash;
            string savedHash;
            try
            {
                currentHash = Retriever.ComputeSHA1(Config.Filepath, Config.Filename);
                savedHash = Retriever.ReadHash();
                if (string.IsNullOrEmpty(savedHash))
                {
                    ConsoleWarningMode();
                    Console.Write("\nPlik SHA1.txt jest pusty. Tworzenie nowego hasza.");
                    ConsoleRestoreColors();
                    var temp = Retriever.ComputeSHA1(Config.Filepath, Config.Filename);
                    Retriever.WriteHash(temp);
                    savedHash = temp;
                }
            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd podczas pobierania haszy.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Porównanie haszy
            //Porównanie haszy i ewentualna aktualizacja pliku Model.xml
            Console.Write("Porównanie haszy");
            if (!currentHash.Equals(savedHash))
            {
                ConsoleWarningMode();
                Console.Write("\nHasze są różne. Aktualizacja listy modeli.");
                ConsoleRestoreColors();
                try
                {
                    Retriever.WriteHash(currentHash);
                    Retriever.SerializeModelList(Config);
                }
                catch (Exception e)
                {
                    ConsoleErrorMode();
                    Log.PrintErrorMessage("\nBłąd w trakcie zapisywania nowego hasza lub podczas serializacji listy modeli", e, "Program.cs");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Deserializacja pliku Model.xml
            //Deserializacja pliku Model.xml
            Console.Write("Deserializacja pliku Model.xml.");
            try
            {
                _modelList = Retriever.DeserializeModelList();
            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd w trakcie deserializacji listy modeli.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Deserializacja pliku Schema.xml
            //Próba odczytu pliku Schema.xml
            Console.Write("Deserializacja pliku Schema.xml");
            try
            {
                _containersList = Retriever.ReadSchema();
            }
            catch (Exception e)
            {
                ConsoleErrorMode();
                Log.PrintErrorMessage("\nBłąd podczas odczytu pliku Schema.xml.", e, "Program.cs");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie ilości komend w pliku Schema.xml
            //Sprawdzenie czy są komendy do wykonania
            Console.Write("Sprawdzenie ilości komend z pliku Schema.xml");
            if (_containersList.Count == 0)
            {
                ConsoleErrorMode();
                Console.WriteLine("\nNie można odpalić programu. Plik Schema.xml jest pusty.\nAby odpalić program w pliku Schema.xml musi być przynajmniej jedno zadanie do wykonania.");
                Console.WriteLine("Wciśnij ENTER aby kontynuować.");
                ConsoleRestoreColors();
                Console.ReadLine();
                return;
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Sprawdzenie poprawności zapisu komend w pliku Schema.xml
            //Sprawdzenie czy są komendy do wykonania
            Console.Write("Sprawdzenie poprawności zapisu komend w pliku Schema.xml");
            foreach (var z in _containersList)
            {
                if (z.CheckRequiredFields())
                {
                    ConsoleErrorMode();
                    string section = z.Section?.ToString();
                    section = (bool)section?.Equals("New") ? null : section;
                    section = section == null ? z.Header?.ToString() : section;
                    section = section == null ? "Nieznana sekcja" : section;
                    Console.WriteLine("\nNie można odpalić programu. Plik Schema.xml jest źle wypełniony." +
                        "\nAby odpalić program w pliku Schema.xml musi być przynajmniej jedno zadanie do wykonania." +
                        $"\nAktualnie sprawdzana sekcja: {section}");
                    Console.WriteLine("Wciśnij ENTER aby kontynuować.");
                    ConsoleRestoreColors();
                    Console.ReadLine();
                    return;
                }
            }
            MoveCursor();
            ConsoleGreenMode();
            Console.WriteLine(_done);
            ConsoleRestoreColors();
            #endregion

            #region Próba wykrycia modelu urządzenia
            //Próba wykrycia modelu urządzenia
            Console.Write("Wykrycie modelu urządzenia");
            if (model.Length != 5)
                model = Retriever.AnalyzeForModel();
            if (string.IsNullOrEmpty(model))
            {
                MoveCursor();
                ConsoleWarningMode();
                Console.WriteLine("Nie wykryto.");
                ConsoleRestoreColors();
            }
            else
            {

                var ansArr = _modelList?.Where(z => z.Model.Contains(model));
                    
                if (ansArr.Count() == 0)
                {
                    MoveCursor();
                    ConsoleWarningMode();
                    Console.WriteLine("Nie wykryto.");
                    ConsoleRestoreColors();
                }
                else
                {
                    MoveCursor();
                    ConsoleGreenMode();
                    _model = ansArr.First();
                    Console.WriteLine(_model.Model);
                    ConsoleRestoreColors();
                }
            }

            ConsoleGreenMode();
            Console.WriteLine("\n\nAplikacja gotowa do użycia!");
            ConsoleRestoreColors();
            Console.ReadLine();
            #endregion
        }

        #region ConsoleChangeMethods
        private static void MoveCursor()
        {
            Console.SetCursorPosition(_pad, Console.CursorTop);
        }

        private static void SetConsoleBackground(ConsoleColor color)
        {
            Console.BackgroundColor = color;
        }

        private static void SetConsoleForeground(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private static void ConsoleWarningMode()
        {
            SetConsoleForeground(ConsoleColor.Yellow);
        }

        private static void ConsoleErrorMode()
        {
            SetConsoleForeground(ConsoleColor.Red);
        }

        private static void ConsoleGreenMode()
        {
            SetConsoleForeground(ConsoleColor.Green);
        }

        private static void ConsoleRestoreColors()
        {
            SetConsoleForeground(ConsoleColor.White);
            SetConsoleBackground(ConsoleColor.Black);
        }

        private static void ConsoleHeaderColors()
        {
            SetConsoleBackground(ConsoleColor.DarkBlue);
        }

        private static void ConsoleSeparator()
        {
            SetConsoleForeground(ConsoleColor.DarkYellow);
        }
        #endregion
    }

    
}
