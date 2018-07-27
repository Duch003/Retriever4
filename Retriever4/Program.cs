using Retriever4.Classes;
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
using Retriever4.Interfaces;

namespace Retriever4
{
    public static class Program
    {
        private static Location _model;
        private static List<Location> ModelList;
        private static IDrawingAtConsole _engine;
        public static Configuration Config;
        private static IWmiReader reader;

        private static readonly string _success = "Zrobione";
        private static readonly string _failed = "Niepowodzenie";
        private static readonly ConsoleColor _fail = ConsoleColor.DarkRed;
        private static readonly ConsoleColor _pass = ConsoleColor.Green;
        private static readonly ConsoleColor _warning = ConsoleColor.Yellow;

        private static void Main(string[] args)
        {
            //TODO Komenda -Config tworzy plik schematu do wypełnienia
            if (!Initialize())
                return;

            if(args != null || args?.Length != 0)
            {
                foreach (var z in args)
                {
                    switch (z)
                    {
                        case "-SzymonMode":
                            break;
                        case "-Config":
                            break;
                    }
                }
            }

            if (!Menu() || _model == null)
                FindModel();
            

            Console.ReadLine();

            return;
        }

        private static bool Initialize()
        {
            try
            {
                ProgramValidation.Initialization(ref _engine, ref Config, ref ModelList, ref reader, _pass, _fail, _warning);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Naciśnięcie dowolnegop rzycisku spowoduje zamknięcie programu.");
                Console.ReadKey();
                return false;
            }
            return true;
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
            List<Location> ans = null;
            //First loop, which print basic informations
            do
            {
                //Clearing console and restoring cursor position
                Console.Clear();
                _engine.RestoreCursorY();
                _engine.RestoreCursorX();
                //Mechanism description
                const string message =
                    "Zacznij wpisywać model (min. 3 znaki), a modele pasujące do wzorca zostaną wyświetlone poniżej. " +
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
                    ans = new List<Location>(ModelList.Where(x =>
                        x.Model.Contains(pattern1) || x.PeaqModel.Contains(pattern1)));
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

        private static void PrintSpecification()
        {
            Console.Clear();
            _engine.RestoreCursorX();
            _engine.RestoreCursorY();
            int line = _engine.Y;

            _engine.PrintMainHeaders
        }

    }


}
