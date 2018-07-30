using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Retriever4.FileManagement;
using Retriever4.Interfaces;

namespace Retriever4.Validation
{
    public class ProgramValidation
    {
        private static IConfigFileManager configMgmt;
        private static IModelListManager listMgmt;
        private static ISHA1FileManager shaMgmt;

        //public bool PrepareObjects(ref IDrawingAtConsole engine)
        //{
        //    try
        //    {
                
        //    }
        //    catch(Exception e)
        //    {
        //        string message = $"Błąd podczas preinicjalizacji. Treść błędu: {e.Message}.\nWewnętrzny wyjątek: {e.InnerException?.Message}.\nMetoda: {nameof(PrepareObjects)}, klasa: ProgramValidation.cs.";
        //        throw new Exception(message);
        //    }
        //    return true;
        //}

        

        public static bool Initialization(ref IDrawingAtConsole engine, ref IDatabaseManager dbMgmt, ref Configuration config, ref List<Location> modelList, ref IWmiReader gatherer, 
            Color pass, Color fail, Color warning, Color defaultBackgroundColor)
        {
            //Preparation
            dbMgmt = new DatabaseFileManagement();
            configMgmt = new ConfigFileManagement();
            listMgmt = new ModelFile();
            shaMgmt = new SHA1FileManagement();
            engine = new DrawingAtConsole(defaultBackgroundColor);
            gatherer = new Retriever();

            //Console clearing
            Console.Clear();
            engine.RestoreCursorX();
            engine.RestoreCursorY();
            var lines = engine.Y;

            lines += engine.PrintInitializationBar(lines, "INICJALIZACJA PROGRAMU");
            //#1. Check configuration existance
            lines++;
            engine.PrintInitializationDescription(lines, "Sprawdzanie istnienia pliku Config.xml.");
            if (!configMgmt.DoesConfigFileExists)
            {
                engine.PrintInitializationStatus(lines, "Nie znaleziono pliku", fail);
                lines++;
                engine.PrintInitializationComment(lines, "Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku Config.xml. " +
                                                         "Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                engine.Wait();
                return false;
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //#2. Deserialize configuration
            lines++;
            engine.PrintInitializationDescription(lines, "Deserializacja pliku Config.xml.");
            try
            {
                config = configMgmt.ReadConfiguration();
            }
            catch (Exception e)
            {
                engine.PrintInitializationStatus(lines, "Niepowodzenie!", fail);
                lines++;
                engine.PrintInitializationComment(lines, $"Odczyt pliku Config.xml nie powiódł się. Prawdopodbnie jest źle wypełniony.\nTresć błędu: {e.Message}\n" +
                                                         $"Błąd wewnątrzny: {e.InnerException?.Message}" +
                                                         $"Aplikacja nie zostanie uruchomiona. Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                engine.Wait();
                return false;
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //#3. Configuration null-checking
            lines++;
            engine.PrintInitializationDescription(lines, "Sprawdzanie poprawności wypełnienia pliku Config.xml.");
            if (!config.MakeDataStatic())
            {
                engine.PrintInitializationStatus(lines, "Niepowodzenie!", fail);
                lines++;
                engine.PrintInitializationComment(lines, "Program nie może zostać uruchomiony, ponieważ plik Config.xml jest nieprawidłowo wypełniony. " +
                                                         "Aby wygenerować schemat Config.xml do wypełnienia, odpal Retriever4.exe z komendą -Config.", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                engine.Wait();
                return false;
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //#2. Existance of database
            lines++;
            engine.PrintInitializationDescription(lines, "Sprawdzenie istnienia bazy danych.");
            if (!dbMgmt.DoesDatabaseFileExists)
            {
                engine.PrintInitializationStatus(lines, "Niepowodzenie!", fail);
                lines++;
                engine.PrintInitializationComment(lines, $"Program nie może zostać uruchomiony, ponieważ nie znaleziono pliku {config.filename} w ścieżce {config.filepath}", Color.White);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                engine.Wait();
                return false;
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //#4. Reading SHA1.txt
            lines++;
            engine.PrintInitializationDescription(lines, "Odczyt hasza z pliku SHA1.txt.");
            string lastHash;
            //If file doesnt exists, just create new one.
            if (!shaMgmt.DoesHashFileExists)
            {
                lines++;
                engine.PrintInitializationComment(lines, "Nie znaleziono pliku SHA1.txt. Tworzenie nowego pliku...", warning);
                try
                {
                    lastHash = shaMgmt.ComputeSHA1();
                    shaMgmt.WriteHash(lastHash);
                }
                catch (Exception e)
                {
                    lines++;
                    engine.PrintInitializationComment(lines, $"Błąd podczas tworzenia nowego pliku SHA1.txt.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException?.Message}" +
                                                             $"Program nie moż zostać uruchomiony.", fail);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    engine.Wait();
                    return false;
                }
            }
            //If does exists, just read hashcode
            else
            {
                try
                {
                    lastHash = shaMgmt.ReadHash();
                    engine.PrintInitializationStatus(lines, "Zrobione", pass);
                }
                catch (Exception e)
                {
                    lines++;
                    engine.PrintInitializationComment(lines, $"Błąd podczas odczytywania pliku SHA1.txt.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException?.Message}" +
                                                             $"Program nie moż zostać uruchomiony.", fail);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    engine.Wait();
                    return false;
                }
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //$6 Computing current hash
            lines++;
            engine.PrintInitializationDescription(lines, $"Odczyt aktualnego hasza pliku {config.filename.Replace(@"/", "")}.");
            string currentHash;
            try
            {
                currentHash = shaMgmt.ComputeSHA1();
            }
            catch (Exception e)
            {
                lines++;
                engine.PrintInitializationComment(lines, "Błąd podczas odczytywania odczytywania hasza pliku " +
                                                         $"{config.filename.Replace(@"/", "")}.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException?.Message}" +
                                                         "Program nie moż zostać uruchomiony.", fail);
                Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                engine.Wait();
                return false;
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //$7. Hash copmarison
            lines++;
            engine.PrintInitializationDescription(lines, $"Sprawdzenie czy lista modeli jest aktualna i odczyt listy.");
            //If hashes are diffrent, then refresh list and deserialize.
            if (currentHash != lastHash || !listMgmt.DoestModelListFileExists)
            {
                lines++;
                engine.PrintInitializationComment(lines, "Hasze są różne. Aktualizacja listy modeli...", warning);
                try
                {
                    listMgmt.SerializeModelList();
                    shaMgmt.WriteHash(currentHash);
                    modelList = listMgmt.DeserializeModelList();
                }
                catch (Exception e)
                {
                    lines++;
                    engine.PrintInitializationComment(lines, $"Błąd podczas aktualizacji listy modeli w pliku Model.xml lub odczytu listy..\nTreść błędu: " +
                                                             $"{e.Message}\nWewnętrzny wyjątek: {e.InnerException?.Message}" +
                                                             $"Program nie może zostać uruchomiony.", fail);
                    Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                    engine.Wait();
                    return false;
                }
            }
            //Else just deserialize
            else
            {
                try
                {
                    modelList = listMgmt.DeserializeModelList();
                }
                catch (Exception e)
                {
                    lines++;
                    engine.PrintInitializationComment(lines, $"Uszkodzony plik Model.xml. Tworzenie nowego...", warning);
                    try
                    {
                        listMgmt.SerializeModelList();
                        shaMgmt.WriteHash(currentHash);
                    }
                    catch (Exception ex)
                    {
                        lines++;
                        engine.PrintInitializationComment(lines, $"Błąd podczas aktualizacji listy modeli w pliku Model.xml.\nTreść błędu: " +
                                                                 $"{ex.Message}\nWewnętrzny wyjątek: {ex.InnerException?.Message}" +
                                                                 $"Program nie może zostać uruchomiony.", fail);
                        Console.WriteLine("Naciśnięcie dowolnego przycisku zamknie aplikację.");
                        engine.Wait();
                        return false;
                    }
                }
            }
            engine.PrintInitializationStatus(lines, "Zrobione", pass);

            //An attempt to dected device model
            lines++;
            engine.PrintInitializationDescription(lines, $"Próba wykrycia modelu urządzenia: ");
            Dictionary<string, dynamic>[] model = null;
            Location result = null;
            int state = 0;
            try
            {
                model = gatherer.GetModelString();
            }
            catch (Exception e)
            {
                engine.PrintInitializationStatus(lines, "Nie można uzyskać danych.", fail);
            }
            if(model == null)
                engine.PrintInitializationStatus(lines, "Brak danych.", warning);
            else
            {
                foreach (var z in model)
                {
                    foreach (var x in z.Values)
                    {
                        if (x == null)
                            continue;
                        else
                        {
                            state = DetectDeviceModel.FindModel(x.ToString(), modelList, out result);
                        }
                    }
                }
            }

            switch (state)
            {
                //Not found in database
                case -1:
                    engine.PrintInitializationStatus(lines, "Brak modelu w bazie.", fail);
                    break;
                //Not found
                case 0:
                    engine.PrintInitializationStatus(lines, "Nie wykryto.", warning);
                    break;
                //Found
                case 1:
                    engine.PrintInitializationStatus(lines, $"{result?.Model}", pass);
                    break;
            }

            lines += 3;
            engine.PrintInitializationComment(lines, "Aplikacja została poprawnie zainicjalizowana. Kliknij ENTER aby kontynuować...", Color.White);
            engine.Wait();
            return true;
        }
    }

    
}
