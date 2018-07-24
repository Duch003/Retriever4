﻿using Retriever4.Classes;
using Retriever4.Enums;
using Retriever4.Tests.UnmeasurableTests;
using Retriever4.Utilities;
using Retriever4.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using Retriever4.FileManagement;
using Retriever4.UnmeasurableTests;

namespace Retriever4
{
    class Program
    {
        private static Location _model;
        public static ObservableCollection<Location> ModelList { get; private set; }
        private static DrawingAtConsole _engine;
        public static Configuration Config;

        private static readonly string _success = "Zrobione";
        private static readonly string _failed = "Niepowodzenie";
        private static readonly ConsoleColor _failColor = ConsoleColor.DarkRed;
        private static readonly ConsoleColor _passColor = ConsoleColor.Green;
        private static readonly ConsoleColor _warningColor = ConsoleColor.Yellow;

        private static void Main(string[] args)
        {
            _engine = new DrawingAtConsole();
            Initialization();
            //TODO Komenda -Config tworzy plik schematu do wypełnienia
            //var config = new Configuration
            //{
            //    filepath = Environment.CurrentDirectory,
            //    filename = "TestBase.xlsx",
            //    databaseTableName = "Test",
            //    biosTableName = "BIOS",
            //    db_Model = 0,
            //    db_PeaqModel = -5,
            //    db_CaseModel = 17,
            //    db_Cpu = -7,
            //    db_MainboardVendor = 6,
            //    db_OS = 4,
            //    db_Ram = 5,
            //    db_ShippingMode = -3,
            //    db_Storage = 9,
            //    db_SWM = 8,
            //    wearLevel = 12,
            //    bios_Bios = 1,
            //    bios_BuildDate = 2,
            //    bios_CaseModel = 3,
            //    bios_EC = 4,
            //    bios_MainboardModel = 5
            //};
            
            //_engine = new DrawingAtConsole();
            //Config = ConfigFileManagement.ReadConfiguration();
            //Config.MakeDataStatic();
            //ModelFile.SerializeModelList();
            //ModelList = ModelFile.DeserializeModelList();
            //FindModel();
            //while (true)
            //{
                
            //}
            Console.ReadLine();
            ////Initialization();
            //if (!Menu())
            //if (_model == null)
            //    return;
            return;
        }

        private static bool Menu()
        {
            Console.Clear();
            if (_model == null) return false;
            do
            {
                Console.WriteLine(_model.Model);
                Console.WriteLine("Czy model urządzenia jest poprawny? (Y/N): ");
                var z = Console.ReadKey();
                switch (z.Key)
                {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                        return false;
                }
            } while (true);
        }

        /// <summary>
        /// Menu section: manage model choice
        /// </summary>
        private static void FindModel()
        {
            //Container for pattern writed by user
            var pattern = "";
            //Container for strained out list of models
            ObservableCollection<Location> ans = null;
            //First loop, which print basic informations
            do
            {
                //Clearing console and restoring cursor position
                Console.Clear();
                _engine.RestoreCursorY();
                _engine.RestoreCursorX();
                //Mechanism description
                const string message = "Zacznij wpisywać model (min. 3 znaki), a modele pasujące do wzorca zostaną wyświetlone poniżej. " +
                                       "Strzałkami w górę i w dół przesuwasz się między modelami. Kiedy znajdziesz potrzebny - kliknij ENTER.";
                //Counting how many lines will cover message. Value round with celinig function
                var lines = (int) Math.Ceiling((double) message.Length / _engine.MaxX);
                //Printing informations with actual pattern
                Console.WriteLine(message);
                Console.Write(pattern);
                //Restore X position and make distance between pattern and table
                _engine.RestoreCursorX();
                lines += 2;
                _engine.CursorY(lines);
                //If pattern is enough long, print matched models
                if (pattern.Length >= 3)
                {
                    var pattern1 = pattern;
                    ans = new ObservableCollection<Location>(ModelList.Where(x => x.Model.Contains(pattern1) || x.PeaqModel.Contains(pattern1)));
                    _engine.PrintModelTable(lines, ans);
                    lines += 2;
                    _engine.PrintRowSelection(lines);
                }

                //Second loop for managing model selection
                bool break1;
                do
                {
                    //Restore (close loop)
                    break1 = false;
                    //Read user key
                    var z = Console.ReadKey();
                    switch (z.Key)
                    {
                        //Remove letter from pattern and reprint whole window
                        case ConsoleKey.Backspace:
                            if (pattern.Length > 0)
                            {
                                pattern = pattern.Remove(pattern.Length - 1);
                                break1 = true;
                            }
                            break;
                        //Choose model
                        case ConsoleKey.Enter:
                            if (ans == null)
                                break;
                            _model = ans[(_engine.Y - lines) / 2];
                            return;
                        //Navigating table - down
                        case ConsoleKey.DownArrow:
                            //If actualy selected model is last one, dont move any further
                            if (_engine.Y >= ans?.Count * 2 + lines - 2)
                                _engine.RestoreCursorX();
                            //Else remove actual arrows and print new ones
                            else
                            {
                                _engine.ClearRowSelection(_engine.Y);
                                _engine.CursorY(_engine.Y + 2);
                                _engine.PrintRowSelection(_engine.Y);
                            }
                            break;
                        //Navigating table - up
                        case ConsoleKey.UpArrow:
                            //If selected model is first one, dont go any further
                            if (_engine.Y <= lines)
                                _engine.RestoreCursorX();
                            //Remove actual arrows ant print new ones
                            else
                            {
                                _engine.ClearRowSelection(_engine.Y);
                                _engine.CursorY(_engine.Y - 2);
                                _engine.PrintRowSelection(_engine.Y);

                            }
                            break;
                        //Managing pattern
                        default:
                            //Add new letter - accept only letters, numbers and dash
                            if (char.IsLetterOrDigit(z.KeyChar) || z.KeyChar == '-')
                                pattern = pattern + z.KeyChar;
                            //Do nothing
                            else
                                _engine.RestoreCursorX();
                            //Cheking pattern length. If have 3 or more - brak loop and print matched models
                            break1 = pattern.Length >= 3;
                            break;
                    }
                } while (!break1);

            } while (true);


        }



        //private static void PrintDetails()
        //{
        //    Console.Clear();

        //    #region KOLUMNY
        //    PrintHeaders();
        //    #endregion

        //    #region MODEL
        //    object dbValue = null;
        //    try
        //    {
        //        dbValue = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, Config.DatabaseTableName, _model.DBRow, (int)Config.MD_ModelColumn);
        //    }
        //    catch (Exception e)
        //    {
        //        ConsoleErrorMode();
        //        Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji MODEL. Prawdopobnie plik bazy danych jest uruchomiony przez inny program.", e, "Program.cs");
        //        ConsoleRestoreColors();
        //        Console.ReadLine();
        //        return;
        //    }

        //    if (dbValue.ToString() != _model.Model)
        //        PrintSection("MODEL", _model.Model, dbValue.ToString(), ConsoleColor.Red, false);
        //    else
        //        PrintSection("MODEL", _model.Model, dbValue.ToString(), ConsoleColor.Green, false);
        //    #endregion

        //    #region OS
        //    var task = _containersList.Where(z => z.Section == ProgramSection.OS).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        try
        //        {
        //            task.WMIResult = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
        //            if ((bool)!task.UseConstant)
        //                task.DatabaseResult = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //            else
        //                task.DatabaseResult = task.Constant;
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji OS.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        //Określanie systemu operacyjnegp
        //        OS osDb = OS.None, osReal = OS.None;
        //        if (task.DatabaseResult.ToString().Contains("10"))
        //            osDb = OS.Windows10;
        //        else if (task.DatabaseResult.ToString().Contains("8"))
        //            osDb = OS.Windows8;
        //        else
        //            osDb = OS.None;

        //        if (!(osDb == OS.None))
        //        {
        //            if (task.WMIResult.ToString().Contains("10"))
        //                osReal = OS.Windows10;
        //            else
        //                osReal = OS.Windows8;
        //        }

        //        if (osDb == osReal)
        //            PrintSection("OS", osReal.ToString(), osDb.ToString(), ConsoleColor.Green, false);
        //        else
        //            PrintSection("OS", osReal.ToString(), osDb.ToString(), ConsoleColor.Red, false);
        //    }
        //    #endregion

        //    #region SWM
        //    task = _containersList.Where(z => z.Section == ProgramSection.SWM).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        //Wydobycie danych
        //        try
        //        {
        //            task.HiddenConstant = Retriever.GetSwm();
        //            task.DatabaseResult = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji SWM.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        //Walidacja
        //        bool switch0 = true;
        //        string[] message = new string[0];
        //        if (task.HiddenConstant == null)
        //        {
        //            message.Expand();
        //            message[message.Length - 1] = "\n         Nie znaleziono plików swconf.dat na urządzeniu; ";
        //            switch0 = false;
        //        }
        //        if (task.DatabaseResult == null || task.DatabaseResult?.ToString().Length == 0)
        //        {
        //            message.Expand();
        //            message[message.Length - 1] = "\n         Brak SWM w bazie danych";
        //            switch0 = false;
        //        }

        //        //Drukowanie
        //        if (switch0)
        //        {
        //            switch0 = false;
        //            for (int i = 0; i < (task.HiddenConstant as string[]).Length; i++)
        //            {
        //                var computer = (task.HiddenConstant as string[])[i].ToString();
        //                string[] database = task.DatabaseResult.ToString().Replace(" ", "").Replace(@"\", @"/").Split('/');
        //                bool isPrinted = false;
        //                for (int j = 0; j < database.Length; j++)
        //                {
        //                    if (computer.Contains(database[j]))
        //                    {
        //                        PrintSection("SWM", computer.Replace(";", " "), task.DatabaseResult.ToString(), ConsoleColor.Green, switch0);
        //                        switch0 = true;
        //                        isPrinted = true;
        //                    }
        //                    else if (database.Length - 1 == j && !isPrinted)
        //                    {
        //                        PrintSection("SWM", computer.Replace(";", " "), task.DatabaseResult.ToString(), ConsoleColor.Red, switch0);
        //                        switch0 = true;
        //                        isPrinted = true;
        //                    }
        //                    if (isPrinted)
        //                        continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ConsoleRestoreColors();
        //            Console.Write("Nie można porównać numerów SWM: ");
        //            ConsoleWarningMode();
        //            for (int i = 0; i < message.Length; i++)
        //                Console.Write(message[i]);
        //            Console.WriteLine();
        //        }
        //    }
        //    #endregion

        //    #region WearLevel
        //    task = _containersList.Where(z => z.Section == ProgramSection.WearLevel).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        int tag = 0;
        //        uint designedCapacity = 0;
        //        uint[] fullChargeCapacity = new uint[0];
        //        try
        //        {
        //            //TODO zmienić w pracy tak aby wypluwał warning zamiast error
        //            //Najpierw pobrać tag
        //            string query = "SELECT Tag FROM BatteryStaticData";
        //            string property = "Tag";
        //            string scope = @"ROOT\wmi";
        //            var tempTag = Retriever.ReadDetailsFromComputer(query, property, scope);
        //            if (!int.TryParse(tempTag.ToString(), out tag))
        //            {
        //                string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(int)}: {(string)tempTag}";
        //                throw new Exception(message);
        //            }

        //            //Pobrać dodatkowo DesignedCapacity (będzie potrzebne potem)
        //            query = "SELECT DesignedCapacity FROM BatteryStaticData";
        //            property = "DesignedCapacity";
        //            var tempDC = Retriever.ReadDetailsFromComputer(query, property, scope);
        //            if (!uint.TryParse(tempDC.ToString(), out designedCapacity))
        //            {
        //                string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(uint)}: {(string)tempDC}";
        //                throw new Exception(message);
        //            }

        //            //Pobrać fullChargeCapacity
        //            query = $"SELECT FullChargedCapacity FROM BatteryFullChargedCapacity WHERE Tag = {tag}";
        //            property = "FullChargedCapacity";
        //            var tempCC_IEnumerable = Retriever.ReadArrayFromComputer(query, property, scope);
        //            if (tempCC_IEnumerable.Count() == 0)
        //            {
        //                string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Wynikowa tablica jest pusta (wymagany przynajmniej jeden element).";
        //                throw new Exception(message);
        //            }
        //            else
        //            {
        //                var tempCC = tempCC_IEnumerable.ToArray();
        //                for (int i = 0; i < tempCC.Count(); i++)
        //                {
        //                    fullChargeCapacity.Expand();
        //                    if (tempCC[i] == null)
        //                    {
        //                        fullChargeCapacity[i] = 0;
        //                        continue;
        //                    }

        //                    if (!uint.TryParse(tempCC[i].ToString(), out fullChargeCapacity[i]))
        //                    {
        //                        string message = $"Błąd po wykonaniu zapytania WMI: \"{query}\". Właściwość {property} zwróciła wartość inną niż {typeof(uint)}: {(string)tempCC[i]}.";
        //                        throw new Exception(message);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji WearLevel.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        for (int i = 0; i < fullChargeCapacity.Count(); i++)
        //        {
        //            bool switch0 = false;
        //            if (fullChargeCapacity[i] == 0)
        //                continue;
        //            double temp = Math.Round(1 - (fullChargeCapacity[i] / (double)designedCapacity), 4) * 100;
        //            string wearLevel = temp.ToString() + "%";
        //            string maxWearLevel = task.Constant[0].Value + "%";
        //            if (temp > Convert.ToDouble(task.Constant[0].Value))
        //            {
        //                PrintSection("Wear level", wearLevel, maxWearLevel, ConsoleColor.Red, switch0);
        //                switch0 = true;
        //            }
        //            else
        //            {
        //                PrintSection("Wear level", wearLevel, maxWearLevel, ConsoleColor.Green, switch0);
        //                switch0 = true;
        //            }
        //        }

        //    }
        //    #endregion

        //    #region Płyta główna
        //    task = _containersList.Where(z => z.Section == ProgramSection.Mainboard).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        //Pobranie danych
        //        object currentMaiboard = null;
        //        object dbMaiboard = null;
        //        try
        //        {
        //            currentMaiboard = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
        //            dbMaiboard = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Płyta główna.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        //Sprawdzenie dowolnego znaku (x - dowolny znak w ciągu)
        //        if (dbMaiboard.ToString().ToLower().Contains("x"))
        //        {
        //            //Walidaacja
        //            string[] tempDbMaibaord = dbMaiboard.ToString().ToLower().Split('x');
        //            bool isGood = true;
        //            for (int i = 0; i < tempDbMaibaord.Length; i++)
        //            {
        //                if (currentMaiboard.ToString().ToLower().Contains(tempDbMaibaord[i]))
        //                    continue;
        //                else
        //                    isGood = false;
        //                if (!isGood)
        //                    break;
        //            }
        //            //Drukowanie
        //            if (isGood)
        //                PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Green, false);
        //            else
        //                PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Red, false);
        //        }
        //        else
        //        {
        //            //Walidacja i drukowanie
        //            string tempDbMainboard = dbMaiboard.ToString().ToLower();
        //            if (currentMaiboard.ToString().Contains(tempDbMainboard.ToString()) || dbMaiboard.ToString().Contains(currentMaiboard.ToString()))
        //                PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Green, false);
        //            else
        //                PrintSection("Płyta główna", currentMaiboard.ToString(), dbMaiboard.ToString(), ConsoleColor.Red, false);
        //        }
        //    }
        //    #endregion

        //    #region CPU
        //    task = _containersList.Where(z => z.Section == ProgramSection.CPU).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        object currentCPU = null;
        //        object dbCPU = null;
        //        PrintHorizontalLine();
        //        try
        //        {
        //            currentCPU = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
        //            dbCPU = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji CPU.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }
        //        if (currentCPU.ToString().ToLower().Contains(dbCPU.ToString().ToLower()))
        //            PrintSection("CPU", currentCPU.ToString().Replace("Intel ", "").Replace("Core ", "").Replace("(TM) ", "").Replace("(R) ", "").Replace("CPU ", "").Replace("  ", ""), dbCPU.ToString(), ConsoleColor.Green, false);
        //        else
        //            PrintSection("CPU", currentCPU.ToString().Replace("Intel", "").Replace("Core", "").Replace("(TM)", "").Replace("(R)", "").Replace("CPU", "").Replace("  ", ""), dbCPU.ToString(), ConsoleColor.Red, false);
        //    }
        //    #endregion

        //    #region Bios
        //    //TODO Dopisać wyświetlanie modelu plyty glownej (D17K itd)

        //    task = _containersList.Where(z => z.Section == ProgramSection.Bios).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        object currentBios = null;
        //        object dbBios = null;
        //        try
        //        {
        //            currentBios = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
        //            dbBios = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.BiosRow, (int)task.Column);
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji CPU.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        if (_model.BiosRow == 0)
        //        {
        //            dbBios = currentBios;
        //        }

        //        ConsoleColor color;
        //        if (currentBios.ToString().Contains(dbBios.ToString()) || dbBios.ToString().Contains(currentBios.ToString()))
        //            color = ConsoleColor.Green;
        //        else
        //            color = ConsoleColor.Red;

        //        bool isPrinted = false;
        //        var tempBios = dbBios.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        //        for (int i = 0; i < tempBios.Length; i++)
        //        {
        //            if (!isPrinted)
        //            {
        //                PrintSection("Bios", currentBios.ToString(), tempBios[i].ToString(), color, isPrinted);
        //                isPrinted = true;
        //            }

        //            else
        //            {
        //                PrintSection("Bios", "", tempBios[i].ToString(), color, isPrinted);
        //                isPrinted = true;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region MaiboardModel
        //    task = _containersList.Where(z => z.Section == ProgramSection.MainboardModel).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        object currentModel = null;
        //        object dbModel = null;
        //        try
        //        {
        //            currentModel = Retriever.ReadDetailsFromComputer(task.Query, task.Property, task.Scope);
        //            dbModel = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Bios.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        if (_model.BiosRow == 0)
        //            dbModel = currentModel + @" / brak modelu w bazie";


        //        if (currentModel == null)
        //        {
        //            currentModel = "Nie wykryto modelu płyty głównej";
        //            PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
        //        }

        //        if (dbModel == null || (string)dbModel == "")
        //        {
        //            dbModel = "Brak modelu płyty głównej w bazie.";
        //            PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
        //        }

        //        string[] tempDbModel = dbModel.ToString().ToLower().Split('x');

        //        if (tempDbModel.Length > 1)
        //        {
        //            bool match = true;
        //            for (int i = 0; i < tempDbModel.Length; i++)
        //                match = currentModel.ToString().Contains(tempDbModel[i]) && match;

        //            if (match)
        //                PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Green, true);
        //            else
        //                PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);

        //        }
        //        else
        //        {
        //            if (currentModel.ToString().Contains(dbModel.ToString()) || dbModel.ToString().Contains(currentModel.ToString()))
        //                PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Green, true);
        //            else
        //                PrintSection("Bios", currentModel.ToString(), dbModel.ToString(), ConsoleColor.Red, true);
        //        }
        //    }
        //    #endregion

        //    #region RAM
        //    task = _containersList.Where(z => z.Section == ProgramSection.RAM).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        PrintHorizontalLine();
        //        ulong[] realSize = new ulong[0];
        //        object dbSize = null;
        //        try
        //        {
        //            var tempRealSize = Retriever.ReadArrayFromComputer(task.Query, task.Property, task.Scope).ToArray();
        //            dbSize = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //            for (int i = 0; i < tempRealSize.Length; i++)
        //            {
        //                realSize.Expand();
        //                if (!ulong.TryParse(tempRealSize[i].ToString(), out realSize[i]))
        //                {
        //                    string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość inną niż {typeof(ulong)}: {tempRealSize.ToString()}";
        //                    throw new Exception(message);
        //                }
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji RAM.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        int fullRealSize = 0;
        //        for (int i = 0; i < realSize.Length; i++)
        //        {
        //            fullRealSize += (int)Math.Round((decimal)(realSize[i] / 1000000000), 0);
        //        }

        //        if (dbSize.ToString().Replace(" ", "").ToLower().Equals((fullRealSize.ToString() + "GB").ToLower()))
        //            PrintSection("RAM", fullRealSize.ToString() + " GB", dbSize.ToString(), ConsoleColor.Green, false);
        //        else
        //            PrintSection("RAM", fullRealSize.ToString() + " GB", dbSize.ToString(), ConsoleColor.Red, false);
        //    }
        //    #endregion

        //    #region Storage
        //    task = _containersList.Where(z => z.Section == ProgramSection.Storage).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        //Pobranie danych
        //        PrintHorizontalLine();
        //        ulong[] realSize = new ulong[0];
        //        double[] dbSize = new double[0];
        //        try
        //        {
        //            //Konwersja rzeczywistej wielkości pamięci na ulong
        //            var tempRealSize = Retriever.ReadArrayFromComputer(task.Query, task.Property, task.Scope).ToArray();
        //            for (int i = 0; i < tempRealSize.Length; i++)
        //            {
        //                realSize.Expand();
        //                if (!ulong.TryParse(tempRealSize[i].ToString(), out realSize[i]))
        //                {
        //                    string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość {tempRealSize[i].ToString()}";
        //                    throw new Exception(message);
        //                }
        //            }

        //            //Rozbicie informacji z bazy na poszczególne dyski
        //            var tempDbSize = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column);
        //            var splitedDbSize = tempDbSize.ToString().Split(';');
        //            for (int i = 0; i < splitedDbSize.Length; i++)
        //            {

        //                bool gbUnit = splitedDbSize[i].ToLower().Contains("gb");
        //                var tempDbSizeValue = splitedDbSize[i].RemoveLetters().Replace(',', '.').RemoveWhiteSpaces();
        //                if (string.IsNullOrEmpty(tempDbSizeValue))
        //                    continue;
        //                dbSize.Expand();
        //                if (!double.TryParse(tempDbSizeValue, out dbSize[i]))
        //                {
        //                    string message = $"Błąd po wykonaniu zapytania WMI: \"{task.Query}\". Właściwość {task.Property} zwróciła wartość {tempDbSizeValue.ToString()}, {splitedDbSize[i]}.";
        //                    throw new Exception(message);
        //                }
        //                else
        //                {
        //                    if (!gbUnit)
        //                        dbSize[i] *= 1000;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Storage.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }
        //        bool switch0 = false;
        //        double gigabyte = 1000000000;//1073741824;//[B]

        //        //Drukowanie wartosci prawidlowych
        //        //Podstawa: wartosc z bazy danych przeksztalcam z B na GB
        //        //Nastepnie wyliczam margines bledu (15%)
        //        //Potem sprawdzam czy w drugiej petli czy ktoras z wartosci w tablicy miesci sie w marginesie bledu
        //        //Jezeli tak to drukuje na ekran, usuwam wydrukowane indexy i skracam tablice
        //        for (int i = realSize.Length; i != 0; i--)
        //        {
        //            double convertedValue = Math.Round((realSize[i - 1] / gigabyte), 0);
        //            double lowerLimit = convertedValue * 0.85;
        //            double upperLimit = convertedValue * 1.15;
        //            for (int j = dbSize.Length; j > 0; j--)
        //            {
        //                if (dbSize[j - 1] >= lowerLimit && dbSize[j - 1] <= upperLimit)
        //                {
        //                    PrintSection("Dyski twarde", convertedValue + " GB", dbSize[j - 1] + " GB", ConsoleColor.Green, switch0);
        //                    switch0 = true;
        //                    dbSize.RemoveIndexAndShrink(j - 1);
        //                    realSize.RemoveIndexAndShrink(i - 1);
        //                    break;
        //                }
        //            }
        //        }

        //        //Drukowanie wartosci nieprawidlowych - leci tak jak jest zapisane w tablicach
        //        int maxLength = dbSize.Length > realSize.Length ? dbSize.Length : realSize.Length;
        //        for (int j = 0; j < maxLength; j++)
        //        {
        //            string leftValue, rightValue;
        //            if (j >= realSize.Length)
        //                leftValue = "Nie znaleziono dysku";
        //            else
        //                leftValue = Math.Round((realSize[j] / gigabyte), 0) + " GB";
        //            if (j >= dbSize.Length)
        //                rightValue = "Nie znaleziono dysku";
        //            else
        //                rightValue = dbSize[j] + " GB";
        //            PrintSection("Dyski twarde", leftValue, rightValue, ConsoleColor.Red, switch0);
        //            switch0 = true;
        //        }
        //    }
        //    #endregion

        //    #region Statusy
        //    PrintHorizontalLine();
        //    task = task = _containersList.Where(z => z.Section == ProgramSection.Shipping).Select(z => z).First();
        //    if ((bool)task.ShowSection)
        //    {
        //        object tempShippingMode;
        //        try
        //        {
        //            tempShippingMode = Retriever.ReadDetailsFromDatabase(Config.Filepath, Config.Filename, task.Table, _model.DBRow, (int)task.Column).ToString();
        //        }
        //        catch (Exception e)
        //        {
        //            ConsoleErrorMode();
        //            Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji RAM.", e, "Program.cs");
        //            ConsoleRestoreColors();
        //            Console.ReadLine();
        //            return;
        //        }

        //        if (tempShippingMode.ToString() == "Nie")
        //            PrintInternalData("Shipping mode", "Shipping mode:", new string[] { tempShippingMode.ToString() }, ConsoleColor.Yellow, false, false);
        //        else
        //            PrintInternalData("Shipping mode", "Shipping mode:", new string[] { tempShippingMode.ToString() }, ConsoleColor.Green, false, false);
        //    }


        //    PrintHorizontalLine();
        //    string windowsKey;
        //    WindowsActivationStatus windowsStatus;
        //    SecureBootStatus secureBoot;
        //    Dictionary<string, DeviceManagerErrorCode> deviceManager;
        //    bool availableNetworks;
        //    Dictionary<string, string> mac;

        //    try
        //    {
        //        windowsKey = Retriever.GetWindowsProductKey();
        //        windowsStatus = Retriever.CheckWindowsActivationStatus();
        //        secureBoot = (SecureBootStatus)((ushort)Retriever.CheckSecureBootStatus());
        //        deviceManager = Retriever.CheckDeviceManager();
        //        availableNetworks = Retriever.CheckWirelessConnection();
        //        mac = Retriever.CheckEthernetInterfaceMAC();

        //    }
        //    catch (Exception e)
        //    {
        //        ConsoleErrorMode();
        //        Log.PrintErrorMessage("\nBłąd w trakcie drukowania sekcji Statusy - Windows.", e, "Program.cs");
        //        ConsoleRestoreColors();
        //        Console.ReadLine();
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(windowsKey))
        //    {
        //        windowsKey = "Nie znaleziono";
        //        PrintInternalData("Statusy", "Klucz Windows:", new string[] { windowsKey }, ConsoleColor.Red, false, false);
        //    }
        //    else
        //        PrintInternalData("Statusy", "Klucz Windows:", new string[] { windowsKey }, ConsoleColor.Green, false, false);

        //    #region Statusy - Windows
        //    if (windowsStatus != WindowsActivationStatus.Licensed)
        //        PrintInternalData(null, "Status aktywacji:", new string[] { windowsStatus.ToString() }, ConsoleColor.Red, true, false);
        //    else
        //        PrintInternalData(null, "Status aktywacji:", new string[] { windowsStatus.ToString() }, ConsoleColor.Green, true, false);

        //    if (secureBoot == SecureBootStatus.Disabled)
        //        PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Red, true, false);
        //    else if (secureBoot == SecureBootStatus.NotSupported)
        //        PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Yellow, true, false);
        //    else
        //        PrintInternalData(null, "Secure Boot:", new string[] { secureBoot.ToString() }, ConsoleColor.Green, true, false);
        //    #endregion

        //    PrintHorizontalLine();
        //    if (availableNetworks)
        //        PrintInternalData("Sieć bezprzewodowa", "Wykrywanie sieci WiFi:", new string[] { "Wykryto" }, ConsoleColor.Green, false, false);
        //    else
        //        PrintInternalData("Sieć bezprzewodowa", "Wykrywanie sieci WiFi:", new string[] { "Nie wykryto" }, ConsoleColor.Red, false, false);

        //    PrintHorizontalLine();
        //    if (mac.Count() == 0)
        //        PrintInternalData("Adresy MAC", "Brak interfejsu Ethernetowego", new string[] { "Brak adresu MAC" }, ConsoleColor.Red, false, false);
        //    else
        //    {
        //        bool switch0 = false;
        //        for (int i = 0; i < mac.Count(); i++)
        //        {
        //            PrintInternalData("Adresy MAC", mac.First().Key, new string[] { mac.First().Value }, ConsoleColor.Green, switch0, false);
        //            switch0 = true;
        //            mac.Remove(mac.First().Key);
        //        }
        //    }

        //    PrintHorizontalLine();
        //    if (deviceManager.Count() == 0)
        //        PrintInternalData("Menedżer urządzeń", "Status urządzeń:", new string[] { "Wszystkie urządzenia działają poprawnie" }, ConsoleColor.Green, false, false);
        //    else
        //    {
        //        bool switch0 = false;
        //        for (int i = 0; i < deviceManager.Count(); i++)
        //        {
        //            string[] info = ConfigManagerErrorDescription.ReturnDescription(deviceManager.First().Value).SplitInParts(35).ToArray();
        //            PrintInternalData("Menedżer urządzeń", deviceManager.First().Key, info, ConsoleColor.Red, switch0, false);
        //            switch0 = true;
        //            deviceManager.Remove(deviceManager.First().Key);
        //        }
        //    }
        //    #endregion
        //    ConsoleSeparator();
        //    PrintHorizontalLine();
        //    Console.ReadLine();
        //    Console.ReadLine();
        //    Console.ReadLine();
        //    return;
        //}

        private static bool Initialization(string model = "")
        {
            Console.Clear();
            _engine.RestoreCursorX();
            _engine.RestoreCursorY();
            var lines = _engine.Y; //0
            //#1. Check configuration existance
            lines += _engine.PrintInitializationBar(lines, "INICJALIZACJA PROGRAMU");
            lines++;
            _engine.PrintInitializationDescription(lines, "Sprawdzanie istnienia pliku Config.xml.");
            if (!ConfigFileManagement.DoesConfigFileExists)
            {
                _engine.PrintInitializationStatus(lines, "Nie znaleziono pliku", _failColor);
                lines++;
                _engine.PrintInitializationComment(lines, "Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku Config.xml. " +
                    "Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", ConsoleColor.Gray);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);

            //#2. Deserialize configuration
            lines++;
            _engine.PrintInitializationDescription(lines, "Deserializacja pliku Config.xml.");
            try
            {
                Config = ConfigFileManagement.ReadConfiguration();
            }
            catch(Exception e)
            {
                _engine.PrintInitializationStatus(lines, "Niepowodzenie!", _failColor);
                lines++;
                _engine.PrintInitializationComment(lines, $"Odczyt pliku Config.xml nie powiódł się. Prawdopodbnie jest źle wypełniony.\nTresć błędu: {e.Message}\n" +
                    $"Błąd wewnątrzny: {e.InnerException.Message}" +
                    $"Aplikacja nie zostanie uruchomiona. Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", ConsoleColor.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);

            //#3. Configuration null-checking
            lines++;
            _engine.PrintInitializationDescription(lines, "Sprawdzanie poprawności wypełnienia pliku Config.xml.");
            if (!Config.MakeDataStatic())
            {
                _engine.PrintInitializationStatus(lines, "Niepowodzenie!", _failColor);
                lines++;
                _engine.PrintInitializationComment(lines, "Program nie może zostać uruchomiony, ponieważ plik Config.xml jest nieprawidłowo wypełniony. " +
                    "Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", ConsoleColor.Gray);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);

            //#2. Existance of database
            lines++;
            if (DatabaseFileManagement.DoesDatabaseFileExists())
            {
                _engine.PrintInitializationStatus(lines, "Niepowodzenie!", _failColor);
                lines++;
                _engine.PrintInitializationComment(lines, $"Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku {Config.filename} w ścieżce {Config.filepath}", ConsoleColor.Gray);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);

            //#4. Reading SHA1.txt
            lines++;
            _engine.PrintInitializationDescription(lines, "Odczyt hasza z pliku SHA1.txt.");
            var lastHash = "";
            //If file doesnt exists, just create new one.
            if (!SHA1FileManagement.DoesHashFileExists)
            {
                lines++;
                _engine.PrintInitializationComment(lines, "Nie znaleziono pliku SHA1.txt. Tworzenie nowego pliku...", _warningColor);
                var hash = "";
                try
                {
                    hash = SHA1FileManagement.ComputeSHA1();
                    SHA1FileManagement.WriteHash(hash);
                }
                catch(Exception e)
                {
                    lines++;
                    _engine.PrintInitializationComment(lines, $"Błąd podczas tworzenia nowego pliku SHA1.txt.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException.Message}" +
                        $"Program nie moż zostać uruchomiony.", _failColor);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    Console.ReadKey();
                    return false;
                }
                lastHash = hash;
            }
            //If does exists, just read hashcode
            else
            {
                try
                {
                    lastHash = SHA1FileManagement.ReadHash();
                    _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);
                }
                catch(Exception e)
                {
                    lines++;
                    _engine.PrintInitializationComment(lines, $"Błąd podczas odczytywania pliku SHA1.txt.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException.Message}" +
                        $"Program nie moż zostać uruchomiony.", _failColor);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    Console.ReadKey();
                    return false;
                }
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);
            
            //$6 Computing current hash
            lines++;
            _engine.PrintInitializationDescription(lines, $"Odczyt aktualnego hasza pliku {Config.filename.Replace(@"/", "")}.");
            var currentHash = "";
            try
            {
                currentHash = SHA1FileManagement.ComputeSHA1();
            }
            catch(Exception e)
            {
                lines++;
                _engine.PrintInitializationComment(lines, $"Błąd podczas odczytywania odczytywania hasza pliku {Config.filename.Replace(@"/", "")}.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException.Message}" +
                    $"Program nie moż zostać uruchomiony.", _failColor);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);

            //$7. Hash copmarison
            lines++;
            _engine.PrintInitializationDescription(lines, $"Sprawdzenie czy lista modeli jest aktualna i odczyt listy.");
            //If hashes are diffrent, then refresh list and deserialize.
            if (currentHash != lastHash || !ModelFile.DoestModelListFileExists)
            {
                lines++;
                _engine.PrintInitializationComment(lines, "Hasze są różne. Aktualizacja listy modeli...", _warningColor);
                try
                {
                    ModelFile.SerializeModelList();
                    SHA1FileManagement.WriteHash(currentHash);
                    ModelList = ModelFile.DeserializeModelList();
                }
                catch (Exception e)
                {
                    lines++;
                    _engine.PrintInitializationComment(lines, $"Błąd podczas aktualizacji listy modeli w pliku Model.xml lub odczytu listy..\nTreść błędu: " +
                        $"{e.Message}\nWewnętrzny wyjątek: {e.InnerException.Message}" +
                        $"Program nie moż zostać uruchomiony.", _failColor);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    Console.ReadKey();
                    return false;
                }
            }
            //Else just deserialize
            else
            {
                try
                {
                    ModelList = ModelFile.DeserializeModelList();
                }
                catch(Exception e)
                {
                    lines++;
                    _engine.PrintInitializationComment(lines, $"Błąd podczas odczytu listy modeli w pliku Model.xml.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException.Message}" +
                        $"Program nie moż zostać uruchomiony.", _failColor);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    Console.ReadKey();
                    return false;
                }
            }
            _engine.PrintInitializationStatus(lines, "Zrobione", _passColor);
            lines += 3;
            _engine.PrintInitializationComment(lines, "Aplikacja została poprawnie zainicjalizowana. Kliknij ENTER aby kontynuować...", ConsoleColor.White);
            Console.ReadKey();
            return true;
        }
    }

    
}
