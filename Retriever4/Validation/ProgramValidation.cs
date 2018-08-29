using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Retriever4.Classes;
using Retriever4.FileManagement;
using Retriever4.Interfaces;

namespace Retriever4.Validation
{
    public class ProgramValidation
    {
        //TODO Zrobić tak aby pusty plik Model.xml był usuwany/ignorowany
        private static IConfigFileManager configMgmt;
        private static IModelListManager listMgmt;
        private static ISHA1FileManager shaMgmt;

        public static bool Initialization(ref IDrawingAtConsole engine, ref IDatabaseManager dbMgmt, ref Configuration config, ref List<Location> modelList, ref IWmiReader gatherer, 
            Color pass, Color fail, Color warning, Color majorInfo, Color minorInfo)
        {
            //Preparation
            gatherer = new Retriever();
            
            configMgmt = new ConfigFileManagement();
            listMgmt = new ModelFile();
            shaMgmt = new SHA1FileManagement();
            engine = new DrawingAtConsole(Color.Black, Color.White, Color.White, Color.Blue, Color.Goldenrod);
            //#1. Check configuration existance


            if (!configMgmt.DoesConfigFileExists)
            {
                engine.PrintInitializationComment(0, "Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku Config.xml. ", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    Console.ReadKey();
                return false;
            }

            //#2. Deserialize configuration
            try
            {
                config = configMgmt.ReadConfiguration();
            }
            catch (Exception e)
            {
                engine.PrintInitializationComment(0, $"Odczyt pliku Config.xml nie powiódł się. Prawdopodbnie jest źle wypełniony.\nTresć błędu: {e.Message}\n" +
                                                         $"Błąd wewnątrzny: {e.InnerException?.Message}" +
                                                         $"Aplikacja nie zostanie uruchomiona.", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }

            //#3. Configuration null-checking
            if (!config.MakeDataStatic())
            {
                engine.PrintInitializationComment(0, "Program nie może zostać uruchomiony, ponieważ plik Config.xml jest nieprawidłowo wypełniony. " +
                                                         "Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }

            engine = new DrawingAtConsole(Configuration.DefaultBackgroundColor, Configuration.DefaultForegroundColor, Configuration.HeaderForegroundColor,
                Configuration.HeaderBackgroundColor, Configuration.SeparatorColor);
            dbMgmt = new DatabaseFileManagement();

            //#2. Existance of database
            if (!dbMgmt.DoesDatabaseFileExists)
            {
                engine.PrintInitializationComment(0, $"Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku {config.filename}.\n", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                Console.ReadKey();
                return false;
            }

           
            //An attempt to dected device model
            Dictionary<string, dynamic>[] model = null;
            Location result = null;
            var state = 0;
            try
            {
                model = gatherer.GetModelString();
            }
            catch (Exception)
            {
            }
            if(model != null)
                foreach (var z in model)
                {
                    foreach (var x in z.Values)
                    {
                        if (x != null)
                            state = DetectDeviceModel.FindModel(x.ToString(), modelList, out result);
                    }
                }
            return true;
        }
    }

    
}
