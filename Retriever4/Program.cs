using Retriever4.Utilities;
using Retriever4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Retriever4.Interfaces;
using Retriever4.Classes;
using System.Drawing;
using Retriever4.FileManagement;
using Console = Colorful.Console;
using System.Threading;
using System.Threading.Tasks;

namespace Retriever4
{
    public static class Program
    {
        
        //W tej zmiennej trzymam aktualnie wybrany model
        public static Location _model;
        //Do tej listy wczytuje dane z pliku Model.xml - jest to lista modeli z bazy danych
        private static List<Location> ModelList;
        //Ten obiekt sluzy do pisania w konsooli - ma predefiniowane metody dla rozrysowywania tabel itd.
        private static IDrawingAtConsole _engine;
        //Tutaj jest przechowywana konfiguracja z pliku Config.xml
        public static Configuration Config;
        //To jest obiekt sluzacy do wydobywania danych z komputera
        private static IWmiReader gatherer;
        //To jest obiekt sluzacy do wydobywania danych z bazy danych + wyszukiwanie plikow
        private static IDatabaseManager reader;

        //Tutaj sa wczytywane kolory z konfiguracji
        private static Color _pass;
        private static Color _warning;
        private static Color _fail;
        private static Color _minorInfo;
        private static Color _majorInfo;

        private static void Main(string[] args)
        {
            Console.SetBufferSize(Console.BufferWidth, 120);
            Console.WriteLine("© 2018 Tomasz \"Duch003\" Mankin <tomaszmankin003@gmail.com>. All rights reserved.");

            //http://colorfulconsole.com/
            //Wywolywana jest moetoda Initialize, ktora zwroci obiekty do wszystkich zmiennych powyzej
            //Dodatkowo podczas inicjalizacji jest podejmowana proba wykrycia modelu urzadzenia
            //Jezeli nie uda sie zainicjalizowac poprawnie programu, zostanie zostanie on zakonczony (obsluga bledu w metodzie)
            if (!Initialize())
                return;
            //Po inicjalizacji sprawdzane jest czy model zostal wykryty
            //Jezeli tak to wyswietli sie menu z zapytaniem czy jest to model poprawny
            //Jezeli nie, automatycznie zostanie odpalona metoda przeszukiwania listy FindModel()
            if (_model == null || !Menu())
                FindModel();
            var refresh = false;
            do
            {
                PrintSpecification();
                Console.WriteLine("\nR - przeładowanie danych");
                Console.WriteLine("ENTER - wyjście z aplikacji");
                Console.WriteLine("BACKSPACE - powrót do wyboru modelu");

                var userKey = Console.ReadKey();
                switch (userKey.Key)
                {
                    case ConsoleKey.Enter:
                        refresh = false;
                        break;
                    default:
                    case ConsoleKey.R:
                        refresh = true;
                        break;
                    case ConsoleKey.Backspace:
                        FindModel();
                        refresh = true;
                        break;
                }
            } while (refresh);
        }

        public class ConsoleSpiner
        {
            int counter;
            public ConsoleSpiner()
            {
                counter = 0;
            }
            public void Turn()
            {
                counter++;
                switch (counter % 4)
                {
                    case 0: Console.Write("/"); break;
                    case 1: Console.Write("-"); break;
                    case 2: Console.Write("\\"); break;
                    case 3: Console.Write("|"); break;
                }
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }

        /// <summary>
        /// Initialization, fill all variables with objects if initialized correctly. If not - informs about error and close application
        /// </summary>
        /// <returns>False if failed</returns>
        private static bool Initialize()
        {
            var result = true;
            try
            {
                //Klasa znajduje sie w pliku ProgramValidation.cs. Jest to osobno dlatego ze chcialem probowac robic na tym testy jednostkowe
                //i ogolnie za duzo miejsca to tutaj zajmowalo. Niestety nieprzemyslalem tego wczesniej jak robic na tym testy, dlatego nie ma ich do tej metody

                //Jako argumenty metody podawane sa wszystkie niezbedne do dzialania zmienne (zadeklarowane na poczatku klasy)
                //Kolory tutaj podane sa tylko dlatego, ze to kolejna rzecz ktorej nie przemyslalem, ale o tym wiecej informacji znajdziesz w tej klasie
                ProgramValidation.Initialization(ref _engine, ref reader, ref Config, ref ModelList, ref gatherer, Color.Green, Color.Yellow, Color.Red, Color.LightGray, Color.White, out var detectedModel);
                _pass = Configuration.PassColor;
                _warning = Configuration.WarningColor;
                _fail = Configuration.FailColor;
                _majorInfo = Configuration.MajorInformationColor;
                _minorInfo = Configuration.MinorInformationColor;
                _model = detectedModel;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                Console.WriteLine("Naciśnięcie dowolnego przycisku spowoduje zamknięcie programu.");
                return false;
            }
            return result;
        }

        /// <summary>
        /// Ask user about detected model, in case it's wrong.
        /// </summary>
        /// <returns>True if selected model is correct, false if not.</returns>
        private static bool Menu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine(_model.Model);
                Console.WriteLine("Czy model urządzenia jest poprawny? (Y/n): ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                    case ConsoleKey.Enter:
                        return true;
                    case ConsoleKey.N:
                        return false;
                }
            } while (true);
        }

        /// <summary>
        /// Menu section: manage model choice. Prints list of models that matches pattern. Model must be chosen.
        /// </summary>
        private static void FindModel()
        {
            //Kontener na wzorzec wpisywany przez uzytkownika
            var pattern = "";
            //Kontener na liste modeli spelniajacych dany wzorzec
            //Zmienna ta przechowuje podzbiór modeli wyselekcjonowany za pomocą wzorca
            List<Location> ans = null;

            /* ZEWNĘTRZNA PĘTLA
             * Zadaniem zewnętrznej pętli jest drukowanie informacji - informacje o mechaniźmie działania,
             * aktualny wzorzec i tabela */
            #region Zewnętrzna pętla
            do
            {
                //Czyszczenie konsoli i przywracanie poczatkowej pocyzji kursora w konsoli
                Console.Clear();
                _engine.RestoreCursorY();
                _engine.RestoreCursorX();
                //Opis mechanizmu
                const string message =
                    "Zacznij wpisywać model (min. 3 znaki), a modele pasujące do wzorca zostaną wyświetlone poniżej. " +
                    "Strzałkami w górę i w dół przesuwasz się między modelami. Kiedy znajdziesz potrzebny - kliknij ENTER.";
                //Tutaj obliczam ile lini zajmuje informacja o zasadzie dzialania mechanizmu. Potrzebne jest to aby poprawnie przeliczac
                //pozycje kursora w konsoli. 
                //Zmienna LINES - sluzy ona do przechowywania informacji ile linii (w sensie wierszy w konsoli) zajal dany tekst.
                var lines = (int)Math.Ceiling((double)message.Length / _engine.MaxX);
                Console.WriteLine(message);
                Console.Write(pattern);

                //Przywrocenie poczatkowej pozycji kursora w osi X (na szerokosc)
                _engine.RestoreCursorX();
                //Po wypisaniu zasady dzialania mechanizmu przesuwam sie jeszcze o dwie linie w dol
                lines += 2;
                //Ustawiam pozycje kursora w osi Y (na wysokosc)
                _engine.CursorY(lines);
                //Sprawdzenie dlugosci wzorca, tak aby przeszukiwal liste tylko od 3 znakow
                if (pattern.Length >= 3)
                {
                    //Za pomoca LINQ przeszukuje liste modeli
                    //Zapytanie: SELECT * FROM ModelList WHERE Model.Contains(pattern) OR peaqModel.Contains(pattern)
                    ans = new List<Location>(ModelList
                        .Where(x => x.Model.Contains(pattern) || x.PeaqModel.Contains(pattern)));
                    _engine.PrintModelTable(lines, ans);
                    lines += 2;
                    _engine.PrintRowSelection(lines);
                }

                //Pętla wewnętrzna do zarządzania wyborem
                bool break1;

                /* WEWNĘTRZNA PĘTLA
                 * Służy do zarządzania wyborem użytkownika - "edytuje" już wypisany tekst, zapamiętuje wpisany wzorzec
                 * oraz wybór modelu użytkownika */
                #region Wewnętrzna pętla
                do
                {
                    //Zamknij pętlę
                    break1 = false;
                    //Odczytaj klawisz
                    var key = Console.ReadKey();
                    switch (key.Key)
                    {
                        //=====Usuwanie litery====
                        case ConsoleKey.Backspace:
                            //Usunięcie litery i wyjście z pętli by przedrukować całość
                            if (pattern.Length > 0)
                            {
                                pattern = pattern.Remove(pattern.Length - 1);
                                break1 = true;
                            }

                            break;
                        //=====Wybór modelu=====
                        case ConsoleKey.Enter:
                            //Warunek sprawdzający czy lista zawiera zaznaczony element (przypadek kiedy użytkownik wpisze randomowe znaki
                            //co zaowocuje wypisaniem pustej listy, a następnie kliknie ENTER
                            //Odnośnie LINQ - ToArray() zapobiega tzw. "leniwej egzekucji kodu LINQ". LINQ działa w ten sposób że zapytanie nie jest wykonane
                            //do póki nie wywoła go jakakolwiek metoda wymagająca rezultatu zapytania.
                            if (ModelList.Where(z => z.Model.Contains(pattern) || z.PeaqModel.Contains(pattern)).ToArray().Count() < 1 || ans == null)
                                break;
                            _model = ans[(_engine.Y - lines) / 2];
                            return;
                        //=====Nawigacja tabeli - na dół=====
                        case ConsoleKey.DownArrow:
                            //Jeżeli zaznaczony rekord jest ostatnim, nie rób nic
                            if (_engine.Y >= ans?.Count * 2 + lines - 2)
                                //Przywracam położenie w osi X ponieważ po kliknięciu kalwisza strzałki kursor przesuwa się o jedno miejsce w osi X w prawo
                                _engine.RestoreCursorX();
                            //W innym przypadku wymaż aktualne zaznaczenie i narysuj wyżej
                            else
                            {
                                _engine.ClearRowSelection(_engine.Y);
                                _engine.CursorY(_engine.Y + 2);
                                _engine.PrintRowSelection(_engine.Y);
                            }

                            break;
                        //=====Nawigacja tabeli - do góry=====
                        case ConsoleKey.UpArrow:
                            //Jeżeli zaznaczony rekord jest pierwszy, nie rób nic
                            if (_engine.Y <= lines)
                                //Przywracam położenie w osi X ponieważ po kliknięciu kalwisza strzałki kursor przesuwa się o jedno miejsce w osi X w prawo
                                _engine.RestoreCursorX();
                            //W innym przypadku usuń aktualne zaznaczenie i wypisz nowe poniżej
                            else
                            {
                                _engine.ClearRowSelection(_engine.Y);
                                _engine.CursorY(_engine.Y - 2);
                                _engine.PrintRowSelection(_engine.Y);
                            }

                            break;
                        //=====Zarządzanie wzorcen=====
                        default:
                            //Weryfikacja czy otrzymany znak jest alfanumeryczny lub myślnikiem
                            if (char.IsLetterOrDigit(key.KeyChar) || key.KeyChar == '-')
                                pattern = pattern + key.KeyChar;
                            else
                                _engine.RestoreCursorX();
                            //Jeżeli długość wzorca jest większa niż 2, wyjdź z pętli wewnętrznej by przedrukować tabelę
                            break1 = pattern.Length >= 3;
                            break;
                    }
                } while (!break1);
                #endregion

            } while (true);
            #endregion
        }

        /// <summary>
        /// Execute all printing methods.
        /// </summary>
        private static void PrintSpecification()
        {
            // OPIS                 RZECZYWISTE                     BAZA DANYCH
            #region Przygotowanie
            Console.Clear();
            _engine.RestoreCursorX();
            _engine.RestoreCursorY();
            var line = _engine.Y;
            #endregion

            #region Headers
            line += _engine.PrintMainHeaders(line);
            line++;
            line += _engine.PrintHorizontalLine(line);
            line++;
            #endregion

            //Tablica metod do wykonania. Będą wykonane w podanej kolejności.
            //Wszystkie metody znajdują się pod tą.
            Func<int, int>[] functions =
            {
                Model,
                OS,
                SWM,
                Batteries,
                Mainboard,
                Memory,
                Statuses,
                WirelessConnections,
                WiredConnections,
                DeviceManager,
                Tip
            };

            //Pętla wywołująca podane metody
            foreach (var z in functions)
            {
                //Try executing
                try
                {
                    line += z.Invoke(line);
                    line++;
                    line += _engine.PrintHorizontalLine(line);
                    line++;
                }
                //If failed, print error message
                catch (Exception e)
                {
                    line += _engine.PrintSection(line,
                        new[] { $"<<{z.Method.Name}>>" },
                        new[] { $"Wyjątek: {e.Message}" },
                        new[] { $"Wewnętrzny wyjątek: {e.InnerException?.Message}" },
                        Color.OrangeRed);
                    line++;
                    line += _engine.PrintHorizontalLine(line);
                    line++;
                }
            }
        }

        /* ==============================KONWENCJA NAZEWNICTWA ZMIENNYCH==============================
         * Raw - Jeżeli zmienna ma w sobie dopisane "Raw" znaczy że jest to świeżo pobrana informacja.
         *       Zmienne bez dopisku "Raw" są już odpowiednio przerobione (jeżeli takie przerobienie jest wymagane i następuje) LUB zmienna nie wymaga przeróbki
         * Prefix db -   oznacza że w tej zmiennej będę trzymać ifnormacje z bazy danych
         * Prefix real - oznacza że w tej zmiennej będę trzymać informacje z komputera (rzeczywiste)
         * Postfix -     oznacza co konkretnie przechowuje dana zmienna
         */

        /* Pobieranie danych z bazy danych odbywa się za pomocą metody ReadDetailsFromDatabae(nazwa tabeli, wiersz z któego pobrać dane, kolumna z której pobrać dane)
         * Wszystkie potrzebne dane znajdują się w zmiennych: _model - wiersz w którym znajduje się wybrany przez użytkownika model, oraz Configuration (klasa traktowana jak zmienna globalna)
         */

        /* Pobieranie rzeczywistych danych odbywa sięza pomocą metod zawartych w obiekcie gatherer (klasa Retriever.cs)
         * Każda metoda zwraca Dictionary<string, dynamic>[], gdzie:
         *      string jest jako nazwa właściwości;
         *      dynamic jest to dynamicznie przydzielany typ dla zmiennej
         * Przyjmuję że każda instancja w tablicy to jeden element danego typu, np.: tablica dwuelementowa dla baterii
         * zawiera informacje na temat dwóch baterii. Każda bateria ma swoje właściwości do odczytu. Wydaje mi się to łatwiejsze do odczytu
         * i do walki niż odczytywanie bezpośrednio z WMI. */

        /// <summary>
        /// Prints model data.
        /// </summary>
        /// <param name="line">Starting position on console (Y axis).</param>
        /// <returns>How many lines took printing.</returns>
        private static int Model(int line)
        {
            var color = Color.Red;
            //Pobranie informacji z bazy danych - model urządzenia
            var dbRawModel =
                reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_Model);
            //Walidacja i przetworzenie zmiennej na string
            var dbModel = string.IsNullOrEmpty(dbRawModel.ToString())
                ? "Brak w bazie danych"
                : dbRawModel.ToString();

            //Pobranie informacji z bazy danych - model PEAQ urządzenia
            var dbPeaqModel = 
                reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_PeaqModel);
            //Walidacja i przetworzenie zmiennej na string
            var peaqModel = 
                dbPeaqModel.ToString() == "-" ? "" : dbPeaqModel.ToString();

            //Pobranie informacji z urządzenia
            //Właściwość: Model, typ: string
            var realRawModel = gatherer.GetModelString();
            var realModel = "";
            //Walidacja czy zapytanie WMI zwróciło cokolwiek
            if (realRawModel == null || realRawModel.Length == 0)
            {
                realModel = "Brak informacji w komputerze.";
                color = _warning;
            }
            else
            {
                realModel = realRawModel[0]["Model"];
                //Sprawdzenie czy model z bazy danych zawiera się w rzeczywistym i na odwrót
                if(StringValidation.
                    CompareStrings(
                        realModel.RemoveSymbols().RemoveWhiteSpaces(), 
                        dbModel.RemoveSymbols().RemoveWhiteSpaces()) ||
                   (peaqModel != "" && StringValidation.
                    CompareStrings(
                        realModel.RemoveSymbols().RemoveWhiteSpaces(),
                        peaqModel.RemoveSymbols().RemoveWhiteSpaces())))
                    color = _pass;
                else
                    color = _warning;
            }
            //Drukuje sekcjęi zwraca ilosc zajętych linii
            return _engine.PrintSection(line, new[] {"Model urządzenia"}, new[] {realModel},
                new[] {dbModel, peaqModel}, color, color, _majorInfo);
        }

        /// <summary>
        /// Prints OS data.
        /// </summary>
        /// <param name="line">Starting position on console (Y axis).</param>
        /// <returns>How many lines took printing.</returns>
        private static int OS(int line)
        {
            var color = Color.Red;
            //Pobranie danych z bazy danych
            var dbRawOs =
                reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_OS);
            //Walidacja czy baza danych zwróciła cokolwiek
            var dbOs = 
                string.IsNullOrEmpty(dbRawOs.ToString().RemoveSymbols()) 
                ? "Brak OS / nie podano w bazie danych" 
                : dbRawOs.ToString();

            //Pobranie informacji z urządzenia
            //Właściwość: Caption, typ: string
            var realRawOs = gatherer.OS();
            var realOs = "";

            //Sprawdzenie czy WMI zwróciło cokolwiek
            if (realRawOs == null || realRawOs.Length == 0)
            {
                realOs = "Brak informacji w komputerze";
                color = _warning;
            }
            else
            {
                //Sprawdzenie czy OS rzeczywisty zgadza się z tym z bazy danych
                var isMatch = true;
                realOs = realRawOs[0]["Caption"];
                //Dzielę daną z bazy danych na mniejsze stringi na podstawie spacji (każda spacja oddziela kolejny nowy string)
                //Następnie sprawdzam czy wszystkie stringi zawierają się w rzeczywistej nazwie
                //Przykład: z bazy danych mam Win 10
                //Komputer zwrócił nazwę Windows 10 Home Premium
                //Jeżeli i "Win" i "10" zawiera się w tym co zwróćił komputer, wtedy się zgadza
                foreach (var z in dbOs.Split(' '))
                {
                    if (!realOs.Contains(z))
                        isMatch = false;
                }

                color = isMatch ? _pass : _fail;
            }
            //Wydrukowanie sekcji i zwrot ilości zutych linii
            return _engine.PrintSection(line, new[] {"OS"}, new[] {realOs}, new[] {dbOs}, color, color, _majorInfo);
        }

        /// <summary>
        /// Prints OS image data.
        /// </summary>
        /// <param name="line">Starting position on console (Y axis).</param>
        /// <returns>How many lines took printing.</returns>
        private static int SWM(int line)
        {
            var lines = _engine.Y;
            var sectionTitle = new[] {"SWM"};
            var dbSwm = new string[0];

            //Pobranie informacji z bazy danych
            var dbRawSwm =
                reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_SWM);
            //Sprawdzanie czy baza danych zwróciła cokolwiek
            if (string.IsNullOrEmpty(dbRawSwm.ToString()))
                dbSwm = new string[0];
            else
                //Jeżeli zwrócony ciag znaków zawiera znak '/', podziel go na tablicę po znaku '/'.
                dbSwm = dbRawSwm.ToString().Any(z => z == '/')
                    ? new[] {dbRawSwm.ToString()}
                    : dbRawSwm.ToString().RemoveWhiteSpaces().Split('/');

            //Pobranie informacji z urządzenia
            //Metoda zwraca tablicę stringów z swm z prefixem w postaci dysku na któym został znaleziony dany SWM
            var realSwm = gatherer.GetSwm();
            //Sprawdzenie czy metoda zwróciła cokolwiek
            if (realSwm == null)
            {
                line += _engine.PrintSection(line, sectionTitle, new[] {"Brak plików SWConf.dat na urządzeniu!"},
                    new string[] {dbRawSwm.ToString()}, _fail, _fail, _majorInfo);
                return line - lines - 1;
            }

            var checkedNumbers = "";
            //Printing matched values
            for (var real = 0; real < realSwm.Length; real++)
            {
                if (realSwm[real] == null)
                    continue;
                for (var db = 0; db < dbSwm.Length; db++)
                {
                    if (dbSwm[db] == null || !StringValidation.CompareStrings(realSwm[real], dbSwm[db]))
                        continue;
                    _engine.PrintSection(line, sectionTitle, new[] {realSwm[real].Replace(";", " ")}, new[] {dbSwm[db]},
                        _pass, _pass, _majorInfo);
                    line++;
                    sectionTitle = new string[0];
                    realSwm[real] = null;
                    checkedNumbers += dbSwm[db] + ";";
                    break;
                }
            }

            //Removing printed numbers
            var temp = checkedNumbers.Split(';');
            foreach (var z in temp)
            {
                if (string.IsNullOrEmpty(z))
                    continue;
                for(var j = 0; j < dbSwm.Length; j++)
                {
                    if(string.IsNullOrEmpty(dbSwm[j]))
                        continue;
                    if (dbSwm[j].Contains(z.RemoveSymbols().RemoveWhiteSpaces()))
                        dbSwm[j] = null;
                }
            }
            //Printing reaminings
            for (var real = 0; real < realSwm.Length; real++)
            {
                for (var db = 0; db < dbSwm.Length; db++)
                {
                    if (realSwm[real] == null && dbSwm[db] == null)
                        continue;

                    if (realSwm[real] == null && dbSwm[db] != null)
                        realSwm[real] = "Brak SWM!";
                    else if (realSwm[real] != null && dbSwm[db] == null)
                        dbSwm[db] = "Brak SWM!";

                    _engine.PrintSection(line, sectionTitle, new[] {realSwm[real].Replace(";", " ")}, new[] {dbSwm[db]},
                        _fail, _fail, _majorInfo);
                    line++;
                    sectionTitle = new string[0];
                    realSwm[real] = null;
                    dbSwm[db] = null;
                    break;
                }
            }

            return line - lines - 2;
        }

        private static int Batteries(int line)
        {
            //Additional lines -> how many lines take printing
            var lines = _engine.Y;

            //Get databate data
            var maxLevel = Configuration.WearLevel;

            //Get device data
            //Retrun instances of every battery
            //Property: Tag,                        type: int
            //Property: DesignedCapacity,           type: long
            //Property: FullChargedCapacity         type: long
            //Property: EstimatedChargeRemaining    type: int
            //Property: Wearlevel                   type: double
            //Expected: 0 if battery damaged/not plugged in, 1 or more
            var realRawBatteries = gatherer.BatteriesData();

            //Batteries simple checking
            if (realRawBatteries == null || realRawBatteries.Length == 0)
            {
                line += _engine.PrintSection(line, new[] {"Baterie"}, new[] {"Nie wykryto baterii!"},
                    new[] {maxLevel.ToString() + "%"}, _fail, _minorInfo, _majorInfo);
                return line - lines - 1;
            }
            else
            {
                var temp = new Dictionary<string, dynamic>();

                var batteryCounter = 1;
                foreach (var batteryInstance in realRawBatteries)
                {
                    if (batteryInstance == null)
                        line += _engine.PrintSection(line, new[] { $"Bateria [{batteryCounter}]" },
                            new[] { "Bateria uszkodzona!" }, new[] { maxLevel.ToString() + "%" }, _fail, _fail,
                            _majorInfo);
                    else
                    {
                        var color = Color.Red;
                        string message = "";
                        double wearLevel = 0;
                        try
                        {
                            wearLevel = (double)batteryInstance["Wearlevel"] * 100;
                        }
                        catch (Exception)
                        {
                            wearLevel = 999;
                        }
                        if(wearLevel == 999)
                            message = "Nie można określić poziomu wearlevel";
                        if (wearLevel < maxLevel - 1)
                            color = _pass;
                        else if (wearLevel >= maxLevel - 1 && wearLevel < maxLevel)
                            color = _warning;
                        else
                            color = _fail;

                        line += _engine.PrintSection(line, new[] { $"BATERIA [{batteryCounter}]" }, new string[0],
                            new string[0], color, color, _majorInfo);
                        line++;
                        line += _engine.PrintSection(line, new[] { "Wearlevel" },
                            new[] { $"{wearLevel}%", message }, new[] { $"{maxLevel}%" }, color, color,
                            _minorInfo);
                        line++;
                        line += _engine.PrintSection(line, new[] { "Poziom naładowania" },
                            new[] { $"{batteryInstance["EstimatedChargeRemaining"]}%" },
                            new[] { $"{batteryInstance["EstimatedChargeRemaining"]}%" }, _minorInfo, _minorInfo, _minorInfo);
                        line++;
                        line += _engine.PrintSection(line, new[] { "Status" }, new[] { $"{batteryInstance["Status"]}" },
                            new[] { $"{batteryInstance["Status"]}" }, _minorInfo, _minorInfo, _minorInfo);
                        line++;
                    }

                    batteryCounter++;
                }
            }
            return line - lines - 2;
        }

        //Mainboard name, CPU, bios
        private static int Mainboard(int line)
        {
            //TODO Model plyty glownej - usuwac wszystkie znaki po za gwiazdka
            var lines = _engine.Y;
            line += _engine.PrintSection(line, new[] {"PŁYTA GŁÓWNA"}, new string[0], new string[0], Color.Black,
                Color.Black, _majorInfo);
            line++;

            #region Mainboard name

            var dbRawMainboardName = "";
            //Get database data
            if (_model.BiosRow == 0)
                dbRawMainboardName = string.IsNullOrEmpty(
                    reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow,
                        Configuration.DB_MainboardModel).ToString())
                    ? "Brak w bazie danych"
                    : reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow,
                        Configuration.DB_MainboardModel).ToString();
            else
                dbRawMainboardName = string.IsNullOrEmpty(
                    reader.ReadDetailsFromDatabase(Configuration.BiosTableName, _model.BiosRow,
                        Configuration.Bios_MainboardModel).ToString())
                    ? "Brak w bazie danych"
                    : reader.ReadDetailsFromDatabase(Configuration.BiosTableName, _model.BiosRow,
                        Configuration.Bios_MainboardModel).ToString();

            //Get device data
            //Property: Product, Type: string
            //https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/win32-baseboard
            //Property: OEMStringArray, Type: string[]
            //https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/win32-computersystem
            var realRawMaiboardName = gatherer.MainboardModel();
            if (realRawMaiboardName == null || realRawMaiboardName.Length == 0)
                line += _engine.PrintSection(line, new[] {"Model"}, new[] {"Brak informacji w komputerze"},
                    new string[] {dbRawMainboardName}, _fail, _minorInfo, _minorInfo);
            else
            {
                //Product is string
                var realRawMainboardProdut = realRawMaiboardName[0]["Product"].ToString();
                //OEMStringArray is string[]
                var realRawMainboardOEMString = realRawMaiboardName[0]["OEMStringArray"];
                var isPrinted = false;
                if (StringValidation.CompareMainboardModel(dbRawMainboardName, realRawMainboardProdut))
                {
                    line += _engine.PrintSection(line, new[] {"Model"}, new string[] {realRawMainboardProdut},
                        new[] {dbRawMainboardName}, _pass, _minorInfo, _minorInfo);
                    isPrinted = true;
                }
                else
                {
                    foreach (var z in realRawMainboardOEMString)
                    {
                        if (!StringValidation.CompareMainboardModel(dbRawMainboardName, z)) continue;
                        line += _engine.PrintSection(line, new[] {"Model"}, new string[] {z},
                            new[] {dbRawMainboardName}, _pass);
                        isPrinted = true;
                        break;
                    }
                }

                if (!isPrinted)
                    line += _engine.PrintSection(line, new[] {"Model"}, new string[] {realRawMainboardProdut},
                        new[] {dbRawMainboardName}, _fail);
            }

            #endregion

            line++;

            #region CPU

            //Get database data
            var dbRawCpu =
                string.IsNullOrEmpty(
                    reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_Cpu)
                        .ToString())
                    ? "Brak w bazie danych"
                    : reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow,
                        Configuration.DB_Cpu).ToString();
            //Get device data
            //Property: Name, type: string
            //https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/win32-processor
            var realRawCpu = gatherer.ProcessorID();
            if (realRawCpu == null || realRawCpu.Length == 0)
            {
                line += _engine.PrintSection(line, new[] {"Procesor"}, new[] {"Brak informacji w komputerze"},
                    new[] {dbRawCpu}, _fail, _minorInfo, _minorInfo);
            }
            else
            {
                var realCpu = realRawCpu[0]["Name"].ToString();
                var color = StringValidation.CompareCpu(dbRawCpu, realCpu) ? _pass : _fail;
                line += _engine.PrintSection(line, new[] {"Procesor"}, new string[] {realCpu}, new[] {dbRawCpu}, color,
                    color, _minorInfo);
            }

            #endregion

            line++;

            #region Bios

            //Get database data
            var dbRawBios = "";
            var dbRawRelease = "";
            if (_model.BiosRow == 0)
                dbRawBios = "Brak informacji w bazie danych";
            else
            {
                dbRawBios = string.IsNullOrEmpty(
                    reader.ReadDetailsFromDatabase(Configuration.BiosTableName, _model.BiosRow, Configuration.Bios_Bios)
                        .ToString())
                    ? "Brak w bazie danych"
                    : reader.ReadDetailsFromDatabase(Configuration.BiosTableName, _model.BiosRow,
                        Configuration.Bios_Bios).ToString();
                dbRawRelease = reader.ReadDetailsFromDatabase(Configuration.BiosTableName, _model.BiosRow, Configuration.Bios_BuildDate).ToString();
            }

            //Get device data
            //Property: SMBIOSBIOSVersion, type: string
            //Property: ReleaseDate, type: datetime
            var realRawBios = gatherer.MainboardBiosData();
            if (realRawBios == null || realRawBios.Length == 0)
                line += _engine.PrintSection(line, new[] {"Wersja BIOS"}, new[] {"Brak informacji w komputerze"},
                    new[] {dbRawCpu}, _fail, _minorInfo, _minorInfo);
            else
            {
                var realRawVersion = realRawBios[0]["SMBIOSBIOSVersion"];
                var color = StringValidation.CompareStrings(realRawVersion, dbRawBios) ? _pass : _fail;
                line += _engine.PrintSection(line, new[] {"Wersja BIOS"}, new string[] {realRawVersion},
                    new[] {dbRawBios}, color, color, _minorInfo);
                line++;
                //string sample: 20160429000000.000000+000
                var realRawRelease = realRawBios[0]["ReleaseDate"].ToString();
                DateTime realRelease = Convert.ToDateTime(StringExtension.RetrieveDateTime(realRawRelease));
                color = dbRawRelease.RemoveWhiteSpaces().RemoveSymbols().Contains(realRelease.ToString().RemoveWhiteSpaces().RemoveSymbols()) ? _pass : _fail;
                
                line += _engine.PrintSection(line, new[] {"Wydanie BIOS"},
                    new string[] {realRelease.ToShortDateString()}, new[] {dbRawRelease.ToString()}, color, color,
                    _minorInfo);
            }

            #endregion

            return line - lines - 1;
        }

        private static int Memory(int line)
        {
            var lines = _engine.Y;
            const long gb = 1000000000;

            #region RAM

            line += _engine.PrintSection(line, new[] {"PAMIĘĆ"}, new[] {""}, new[] {""}, Color.Black, Color.Black,
                _majorInfo);
            line++;
            var dbRawRam =
                reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_Ram);
            //Propery: MemoryChip[i], type: int
            //Property: Total, type: int
            var realRawRam = gatherer.RamData();
            if (realRawRam == null || realRawRam.Length == 0)
                line += _engine.PrintSection(line, new[] {"Ram"}, new[] {"Nie wykryto kości RAM!"},
                    new[] {dbRawRam.ToString()}, _fail, _minorInfo, _minorInfo);
            else
            {
                var realRam = realRawRam[0]["Total"];
                var color = StringValidation.CompareStrings(realRam.ToString(), dbRawRam.ToString()) ? _pass : _fail;
                line += _engine.PrintSection(line, new[] {"Ram"}, new[] {$"{realRam} GB"}, new[] {dbRawRam.ToString()},
                    color, color, _minorInfo);
            }

            #endregion

            line++;

            #region Storage

            var dbRawStorages = reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow,
                Configuration.DB_Storage);
            //Property: Size, type: long
            var realRawStorages = gatherer.StorageData();
            if (realRawStorages == null || realRawStorages.Length == 0)
                line += _engine.PrintSection(line, new[] {"Dyski"}, new[] {"Nie wykryto dysków!"},
                    new[] {dbRawStorages.ToString()}, _fail, _minorInfo, _minorInfo);
            else
            {
                var dbStorages = dbRawStorages.ToString().Replace("+", ";").Replace(",", ".").RemoveSymbols(new[] {';', '.'}).RemoveWhiteSpaces().Split(';');
                //Printing matched values
                for (var real = 0; real < realRawStorages.Length; real++)
                {
                    if (realRawStorages[real] == null)
                        continue;
                    for (var db = 0; db < dbStorages.Length; db++)
                    {
                        if (string.IsNullOrEmpty(dbStorages[db]) || !NumbersValidation.CompareStorages(dbStorages[db],
                                (double) realRawStorages[real]["Size"], out var dbStorage, out var realStorage))
                            continue;
                        _engine.PrintSection(line, new[] {$"Dysk [{real+1}]"}, new[] {realStorage + " GB"},
                            new[] {dbStorage + " GB"}, _pass, _pass, _minorInfo);
                        line++;
                        realRawStorages[real] = null;
                        dbStorages[db] = null;
                        break;
                    }
                }

                //Printing reaminings
                for (var real = 0; real < realRawStorages.Length; real++)
                {
                    for (var db = 0; db < dbStorages.Length; db++)
                    {
                        if (realRawStorages[real] == null && string.IsNullOrEmpty(dbStorages[db]))
                            continue;
                        var realValue = "";
                        var dbValue = "";
                        if (realRawStorages[real] == null && !string.IsNullOrEmpty(dbStorages[db]))
                        {
                            realValue = "Nie wykryto dysku";
                            dbValue = dbStorages[db];
                        }
                        else if (realRawStorages[real] != null && string.IsNullOrEmpty(dbStorages[db]))
                        {
                            if (string.IsNullOrEmpty(realRawStorages[real]["Size"].ToString()))
                                continue;
                            realValue = Math.Round((double) realRawStorages[real]["Size"] / gb, 0) + " GB";
                            dbValue = "Nie wykryto dysku";
                        }
                        else
                        {
                            realValue = Math.Round((double) realRawStorages[real]["Size"] / gb, 0) + " GB";
                            dbValue = dbStorages[db];
                        }

                        _engine.PrintSection(line, new[] {"Nieznany dysk"}, new[] {realValue}, new[] {dbValue}, _fail,
                            _fail, _minorInfo);
                        line++;
                        realRawStorages[real] = null;
                        dbStorages[db] = null;
                        break;
                    }
                }

            }

            #endregion

            return line - lines - 2;
        }

        private static int Statuses(int line)
        {
            var lines = _engine.Y;
            line += _engine.PrintSection(line, new[] {"STATUSY"}, new[] {"STATUS"}, new[] {"DODATKOWE INFORMACJE"},
                _majorInfo, _majorInfo, _majorInfo);
            line++;

            #region Windows product key

            //OA3xOriginalProductKey, type: string
            var windowsKey = gatherer.GetWindowsProductKey();
            if (windowsKey == null || windowsKey.Length == 0)
                line += _engine.PrintSection(line, new[] {"Klucz Windows"}, new[] {"--Brak klucza--"},
                    new[] {"Nie wykryto! DPK"}, _fail, _minorInfo, _minorInfo);
            else
            {
                string productKey = windowsKey[0]["OA3xOriginalProductKey"].ToString();
                var color = string.IsNullOrEmpty(productKey) ? _fail : _pass;
                var info = string.IsNullOrEmpty(productKey)
                    ? "Brak klucza w SoftwareLicensingService"
                    : "Klucz z klasy SoftwareLicensingService";
                if (string.IsNullOrEmpty(productKey))
                    productKey = "DPK";
                line += _engine.PrintSection(line, new[] {"Klucz Windows"}, new[] {productKey}, new[] {info}, color,
                    _minorInfo, _minorInfo);
            }

            #endregion

            line++;

            #region Activation status

            //Property: LicenseStatus, type: WindowsActivationStatus
            var activationStatus = gatherer.CheckWindowsActivationStatus();
            if (activationStatus == null || activationStatus.Length == 0)
                line += _engine.PrintSection(line, new[] {"Status aktywacji"}, new[] {"--Brak danych--"},
                    new[] {"System nie ma informacji na temat statusu aktywacji"}, _fail, _minorInfo, _minorInfo);
            else
            {
                var status = (WindowsActivationStatus) activationStatus[0]["LicenseStatus"];
                var color = (int) status == 1 ? _pass : _warning;
                line += _engine.PrintSection(line, new[] {"Status aktywacji"}, new[] {status.ToString()},
                    new[] {$"Nr statusu: {(int) status}"}, color, _minorInfo, _minorInfo);
            }

            #endregion

            line++;

            #region SecureBoot

            var secureStatus = gatherer.CheckSecureBootStatus();
            switch ((int) secureStatus)
            {
                case 0:
                    line += _engine.PrintSection(line, new[] {"Secure Boot"}, new[] {secureStatus.ToString()},
                        new[] {$"Nr statusu: {(int) secureStatus}"}, _fail, _minorInfo, _minorInfo);
                    break;
                case 1:
                    line += _engine.PrintSection(line, new[] {"Secure Boot"}, new[] {secureStatus.ToString()},
                        new[] {$"Nr statusu: {(int) secureStatus}"}, _pass, _minorInfo, _minorInfo);
                    break;
                default:
                    line += _engine.PrintSection(line, new[] {"Secure Boot"}, new[] {"Nie wspierany"},
                        new[] {$"Nr statusu: {(int) secureStatus}"}, _pass, _minorInfo, _minorInfo);
                    break;
            }

            #endregion

            line++;

            #region Shipping

            var shippingStatus = reader
                .ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_ShippingMode)
                .ToString();
            if (shippingStatus.ToLower() == "tak")
                line += _engine.PrintSection(line, new[] {"Shipping Mode"}, new[] {"Należy włączyć"},
                    new[] {true.ToString()}, _warning, _minorInfo, _minorInfo);
            else
                line += _engine.PrintSection(line, new[] {"Shipping Mode"}, new[] {"Nie wspierany"},
                    new[] {false.ToString()}, _pass, _minorInfo, _minorInfo);

            #endregion

            return line - lines - 1;
        }

        private static int WirelessConnections(int line)
        {
            var lines = _engine.Y;

            //Property: Description
            //Property: OperationalStatus
            //Property: BytesSent
            //Property: BytesReceived
            var wirelessDevices = gatherer.CheckWirelessConnection();
            if (wirelessDevices == null || wirelessDevices.Length == 0)
                line += _engine.PrintSection(line, new string[0], new[] {"Brak"},
                    new[] {"Nie wykryto urządzeń bezprzewodowych"}, _fail);
            else
            {
                var i = 1;
                foreach (var z in wirelessDevices)
                {
                    var operationalStatus = (int) z["OperationalStatus"];
                    var status = "";
                    var color = Color.Black;
                    switch (operationalStatus)
                    {
                        //Up
                        case 1:
                            color = _pass;
                            status = "Włączona";
                            break;
                        //Down
                        case 2:
                            color = _fail;
                            status = "Wyłączona";
                            break;
                        //Testing
                        case 3:
                            color = _warning;
                            status = "Testowanie";
                            break;
                        //Unknown
                        case 4:
                            color = _fail;
                            status = "Nieznany";
                            break;
                        //Dormant
                        case 5:
                            color = _warning;
                            status = "Uśpiona";
                            break;
                        //NotPresent
                        case 6:
                            color = _fail;
                            status = "Nie wykryto";
                            break;
                        //LowerLayerDown
                        case 7:
                            color = _fail;
                            status = "Niższa warstwa wyłączona";
                            break;
                        default:
                            color = _fail;
                            status = "Nieznany";
                            break;
                    }

                    line += _engine.PrintSection(line,
                        new[] {$"KARTA BEZPRZEWODOWA [{i}]"},
                        new string[] {z["Description"]},
                        new[] {status},
                        _minorInfo, color, _majorInfo);
                    line++;
                    color = z["BytesSent"] > 0 && z["BytesReceived"] > 0 ? _pass : _warning;
                    var rawsSentBytes = z["BytesSent"];
                    var sentBytes = rawsSentBytes > 0 ? rawsSentBytes / 1024 : rawsSentBytes;
                    var rawsReceivedBytes = z["BytesReceived"];
                    var receivedBytes = rawsReceivedBytes > 0 ? rawsReceivedBytes / 1024 : rawsReceivedBytes;
                    line += _engine.PrintSection(line,
                        new[] {"Bilans pakietów"},
                        new[] {$"Wysłano bajtów: {sentBytes} kB"},
                        new[] {$"Odebrano bajtów: {receivedBytes} kB"},
                        color, color, _minorInfo);
                    line++;
                    line += _engine.PrintSection(line, new[] {""}, new[] {""}, new[] {""}, Color.Black);
                    line++;
                    i++;
                }
            }

            return line - lines - 2;
        }

        private static int WiredConnections(int line)
        {
            var lines = _engine.Y;
            line += _engine.PrintSection(line, new[] {"Karty ethernet"}, new[] {"NAZWA KARTY ETHERNET"},
                new[] {"ADRES MAC"}, _majorInfo, _majorInfo, _majorInfo);
            line++;
            var mac = gatherer.CheckEthernetInterfaceMAC();
            if (mac == null || mac.Count == 0)
                line += _engine.PrintSection(line, new string[0], new[] {"Nie wykryto karty Ethernet"}, new[] {"-"},
                    _warning, _minorInfo, _minorInfo);
            else
            {
                foreach (var z in mac)
                {
                    line += _engine.PrintSection(line, new string[0], new[] {z.Key}, new[] {z.Value}, _pass,
                        _minorInfo, _minorInfo);
                    line++;
                }
            }

            return line - lines - 2;
        }

        private static int DeviceManager(int line)
        {
            var lines = _engine.Y;
            var deviceManagerErrors = gatherer.CheckDeviceManager();
            if (deviceManagerErrors == null || deviceManagerErrors.Length == 0)
                line += _engine.PrintSection(line, new[] {"Menedżer urządzeń"}, new[] {"Nie wykryto problemów"},
                    new[] {"Wykryte komponenty działają poprawnie"}, _pass, _minorInfo, _majorInfo);
            else
            {
                line += _engine.PrintSection(line, new[] {"Menedżer urządzeń"}, new[] {"Nazwa urządzenia"},
                    new[] {"Opis"}, _majorInfo, _majorInfo, _majorInfo);
                line++;
                //Caption, ConfigManagerErrorCode
                foreach (var z in deviceManagerErrors)
                {
                    line += _engine.PrintSection(line, new string[0], new string[] {z["Caption"]},
                        new string[]
                        {
                            ConfigManagerErrorDescription.ReturnDescription(
                                (DeviceManagerErrorCode) z["ConfigManagerErrorCode"])
                        }, _fail, _minorInfo, _minorInfo);
                    line++;
                }

                line--;
            }

            return line - lines - 1;
        }

        private static int Tip(int line)
        {
            var lines = _engine.Y;
            var tip = "";
       
            tip = reader.ReadDetailsFromDatabase(Configuration.DatabaseTableName, _model.DBRow, Configuration.DB_Tip)?.ToString();
            var color = Color.Red;
            if (string.IsNullOrEmpty(tip))
            {
                tip = "Brak wskazówek dla tego urządzenia";
                color = _pass;
            }
            else
                color = _warning;
                
            line += _engine.PrintSection(line, new[] { "Wskazówki" }, new string[] { }, new[] { tip }, _minorInfo, color,_majorInfo);

            return line - lines - 1;
        }
    }
}
