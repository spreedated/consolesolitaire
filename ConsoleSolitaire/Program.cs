using ConsoleSolitaire.Classes;
using ConsoleSolitaire.Models;
using NAudio.Utils;
using NAudio.Wave;
using neXn.Lib.Playingcards.Classes;
using neXn.Lib.Playingcards.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ConsoleSolitaire.Classes.PreloadedResources;

namespace ConsoleSolitaire
{
    internal class Program
    {
        internal static Stopwatch roundTimer = new();

        internal static Deck Talon = null;

        internal static TableuPile Pile1 = new();
        internal static TableuPile Pile2 = new();
        internal static TableuPile Pile3 = new();
        internal static TableuPile Pile4 = new();
        internal static TableuPile Pile5 = new();
        internal static TableuPile Pile6 = new();
        internal static TableuPile Pile7 = new();

        internal static BuildingPile BuildingPile1 = new();
        internal static BuildingPile BuildingPile2 = new();
        internal static BuildingPile BuildingPile3 = new();
        internal static BuildingPile BuildingPile4 = new();

        internal readonly static Stack<Card> TalonOpen = new();

        internal static TableuPile[] piles = { Pile1, Pile2, Pile3, Pile4, Pile5, Pile6, Pile7 };
        internal static BuildingPile[] buildingPiles = { BuildingPile1, BuildingPile2, BuildingPile3, BuildingPile4 };

        private static void SetWindowsConsoleSize()
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WindowWidth = 144;
                Console.WindowHeight = 80;
            }
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();

            Preload();

            SetWindowsConsoleSize();

            Log.Information("[Program] Start");

            NewRound();

            Console.ReadKey();
        }

        public static void NewRound()
        {
            Console.Clear();

            StartNewRound();

            RenderTalon();
            RenderBuildingPiles();
            DisplayTableu();

            UserInput();
        }

        private static void DrawTableuRow(int pilecount)
        {
            for (int ii = 0; ii < pilecount; ii++)
            {
                Card c = Talon.DrawCard();
                if (ii == pilecount - 1)
                {
                    c.Hidden = false;
                }
                piles[ii].PushCard(c);
            }
        }

        private static void StartNewRound()
        {
            Sound.Play(SND_LOADED);

            TalonOpen.Clear();

            Talon = new();
            Talon.Create();
            Talon.Shuffle(3);

            Log.Information("[NewRound] Talon created");

            piles.ToList().ForEach(x => x.Clear());

            for (int i = 0; i < piles.Length; i++)
            {
                DrawTableuRow(piles.Length - i);
            }

            for (int i = 0; i < buildingPiles.Length; i++)
            {
                buildingPiles[i].Clear();
            }

            Log.Information("[NewRound] Piles filled");

            roundTimer.Restart();
        }

        public static void RenderBuildingPiles()
        {
            StringBuilder output = new();

            Console.ForegroundColor = ConsoleColor.White;

            string b1 = BuildingPile1.RenderPile();
            string b2 = BuildingPile2.RenderPile();
            string b3 = BuildingPile3.RenderPile();
            string b4 = BuildingPile4.RenderPile();

            for (int i = 0; i < b1.Split('\n').Length; i++)
            {
                output.Append(new String(' ', 30));
                output.Append(b1.Split('\n')[i]);
                output.Append(new String(' ', 5));
                output.Append(b2.Split('\n')[i]);
                output.Append(new String(' ', 5));
                output.Append(b3.Split('\n')[i]);
                output.Append(new String(' ', 5));
                output.Append(b4.Split('\n')[i]);
                output.Append(new String(' ', 5));
                output.Append('\n');
            }

            bool nextColor = false;
            foreach (char c in output.ToString())
            {
                if (c == '@')
                {
                    nextColor = true;
                    continue;
                }
                if (nextColor)
                {
                    if (c == 'W')
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (c == 'R')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    nextColor = false;
                    continue;
                }
                Console.Write(c);
            }
        }

        public static void RenderTalon()
        {
            Console.WriteLine($"{new string(' ', Console.WindowWidth - 30)}{roundTimer.Elapsed:hh\\:mm\\:ss\\:ffffff}");

            if (!TalonOpen.Any() && Talon.Carddeck.Any())
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(HIDDENFULLCARD);

                Log.Information($"Talon not open, {Talon.Carddeck.Count} cards on talon.");

                return;
            }

            if (!Talon.Carddeck.Any() && !TalonOpen.Any())
            {
                for (int i = 0; i < 9; i++)
                {
                    Console.WriteLine();
                }

                Log.Information($"Talon is empty.");

                return;
            }

            string card = FULLCARD.Replace("{{VALUE}}", TalonOpen.Peek().Value.Length >= 2 ? TalonOpen.Peek().Value : TalonOpen.Peek().Value + " ").Replace("{{SUIT}}", Encoding.UTF8.GetString(TalonOpen.Peek().UtfSymbol).ToString()).Replace("{{VALUE2}}", TalonOpen.Peek().Value.Length >= 2 ? TalonOpen.Peek().Value : " " + TalonOpen.Peek().Value);

            ConsoleColor ccolor = ConsoleColor.White;

            if (TalonOpen.Peek().Color != Card.Colors.Black)
            {
                ccolor = ConsoleColor.Red;
            }

            int linecount = 0;
            foreach (string line in HIDDENFULLCARD.Split('\n'))
            {
                Console.ForegroundColor = ConsoleColor.White;
                if (Talon.Carddeck.Any())
                {
                    Console.Write(line.Substring(0, 3));
                }
                Console.ForegroundColor = ccolor;
                Console.WriteLine(card.Split('\n')[linecount]);
                linecount++;
            }

            Log.Information($"Talon open with {TalonOpen.Count} cards, {Talon.Carddeck.Count} cards left on talon.");

            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void UserInput()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.Write("Select [#,#]: ");
            string r = Console.ReadLine();

            if ((r.StartsWith("l") || r.StartsWith("load")) && r.Split(' ').Length == 2)
            {
                LoadGameResponse resp = SaveGame.LoadState(r.Split(' ')[1]);
                if (!resp.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"\nFile could not be loaded,{(!resp.DoesExist ? $" file \"{resp.Filename}\" not found " : " ")} - please retry.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"\nFile \"{resp.Filename}\" sucessfully loaded!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
                Sound.Play(SND_LONGBEEP);
            }

            if ((r.StartsWith("s") || r.StartsWith("save")) && r.Split(' ').Length <= 2)
            {
                SaveStateResponse response = null;

                if (r.Split(' ').Length == 2 && SaveGame.SaveExists(r.Split(' ')[1]))
                {
                    Console.WriteLine($"\nA save with named \"{r.Split(' ')[1]}\" already exists, overwrite? [y/N]: ");
                    string userinput = Console.ReadLine();
                    if (userinput.ToLower().StartsWith("y"))
                    {
                        response = SaveGame.SaveState(r.Split(' ').Length == 2 ? r.Split(' ')[1] : null);
                    }
                }
                else
                {
                    response = SaveGame.SaveState(r.Split(' ').Length == 2 ? r.Split(' ')[1] : null);
                }
                if (response != null)
                {
                    Sound.Play(SND_LONGBEEP);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"Game has been saved \"");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{response.Filename}");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"\" with a filesize of ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{response.FileInfo.Length / (double)1024:#.00} KiB");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(".\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
            }

            if (r == "n")
            {
                Console.Write("\n Are you sure to restart? [y\\N]: ");
                r = Console.ReadLine();
                if (r == "y" || r == "Y")
                {
                    NewRound();
                    Sound.Play(SND_LONGBEEP);
                    return;
                }
            }

            if (r.EndsWith(",-") && !r.StartsWith('+'))
            {
                Card c = piles[Convert.ToInt32(r.Substring(0, r.IndexOf(','))) - 1].Peek();
                if (c != null)
                {
                    if (c.Value == "A")
                    {
                        buildingPiles.First(x => x.IsEmpty).PushCard(c);
                        piles[Convert.ToInt32(r.Substring(0, r.IndexOf(','))) - 1].PopCard();
                        Sound.Play(SND_BEEP);
                    }
                    else if (buildingPiles.Any(x => x.PileSuit == c.Suit) && buildingPiles.First(x => x.PileSuit == c.Suit).Peek().Numbervalue + 1 == c.Numbervalue)
                    {
                        buildingPiles.First(x => x.PileSuit == c.Suit).PushCard(c);
                        piles[Convert.ToInt32(r.Substring(0, r.IndexOf(','))) - 1].PopCard();
                        Sound.Play(SND_BEEP);
                    }
                }
            }

            if (r.IndexOf(',') == 1 && !r.Contains('b') && !r.Contains('t') && !r.Contains('+') && !r.Contains('-'))
            {
                MoveToAnotherPile(piles[Convert.ToInt32(r.Substring(0, r.IndexOf(','))) - 1], piles[Convert.ToInt32(r.Substring(r.IndexOf(',') + 1)) - 1]);
                Sound.Play(SND_BEEP);
            }

            if (r == "a" && !Talon.Carddeck.Any() && !TalonOpen.Any() && piles.All(x => x.pile.All(y => !y.Hidden)))
            {
                while (piles.Any(x => x.IsEmpty))
                {
                    var s = buildingPiles.OrderBy(x => x.Peek().Numbervalue);
                    var ss = piles.First(x => x.Peek() != null && x.Peek().Numbervalue == s.First().Peek().Numbervalue + 1 && x.Peek().Suit == s.First().Peek().Suit);
                    s.First().PushCard(ss.PopCard());

                    Console.Clear();
                    RenderTalon();
                    RenderBuildingPiles();
                    DisplayTableu();
                    WinCondition();

                    Thread.Sleep(10);
                }
            }

            if (r == "t" || r == "+")
            {
                if (Talon.Carddeck.Any())
                {
                    TalonOpen.Push(Talon.DrawCard());
                }
                else
                {
                    foreach (Card c in TalonOpen)
                    {
                        Talon.PushCard(c);
                    }
                    TalonOpen.Clear();
                }
            }

            if ((r.StartsWith("t,") || r.StartsWith("+,")) && !r.EndsWith('-') && IsAllowed(TalonOpen.Peek(), piles[Convert.ToInt32(r.Substring(r.IndexOf(',') + 1)) - 1].Peek()))
            {
                Card c = TalonOpen.Pop();
                c.Hidden = false;
                piles[Convert.ToInt32(r.Substring(r.IndexOf(',') + 1)) - 1].PushCard(c);
                Sound.Play(SND_BEEP);
            }

            if (r == "+,-")
            {
                if (TalonOpen.Peek().Value == "A")
                {
                    buildingPiles.First(x => x.IsEmpty)?.PushCard(TalonOpen.Pop());
                    Sound.Play(SND_BEEP);
                }
                else if (buildingPiles.Any(x => x.PileSuit == TalonOpen.Peek().Suit))
                {
                    buildingPiles.First(x => x.PileSuit == TalonOpen.Peek().Suit).PushCard(TalonOpen.Pop());
                    Sound.Play(SND_BEEP);
                }
            }

            Console.Clear();
            RenderTalon();
            RenderBuildingPiles();
            DisplayTableu();
            WinCondition();

            UserInput();
        }

        private static void WinCondition()
        {
            if (buildingPiles.All(x => x.IsFull))
            {
                roundTimer.Stop();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("You've won!");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"Round time: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{roundTimer.Elapsed:hh\\:mm\\:ss\\:ffffff}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                NewRound();
            }
        }

        private static void DisplayTableu()
        {
            bool nextColor = false;
            foreach (char c in RenderAllStacks())
            {
                if (nextColor)
                {
                    if (c == 'W')
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    nextColor = false;
                    continue;
                }
                if (c == '@')
                {
                    nextColor = true;
                    continue;
                }
                Console.Write(c);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static string RenderAllStacks()
        {
            string pile1 = Pile1.RenderPile();
            string pile2 = Pile2.RenderPile();
            string pile3 = Pile3.RenderPile();
            string pile4 = Pile4.RenderPile();
            string pile5 = Pile5.RenderPile();
            string pile6 = Pile6.RenderPile();
            string pile7 = Pile7.RenderPile();

            StringBuilder displayString = new();

            string[][] pileStrings = new string[][]
            {
                pile1.Split('\n'),
                pile2.Split('\n'),
                pile3.Split('\n'),
                pile4.Split('\n'),
                pile5.Split('\n'),
                pile6.Split('\n'),
                pile7.Split('\n')
            };

            for (int i = 0; i < pileStrings.Max(x => x.Length); i++)
            {
                string pileLine1 = pileStrings[0].Length > i ? pileStrings[0][i].Trim('\r').Trim('\n') : "";
                string pileLine2 = pileStrings[1].Length > i ? pileStrings[1][i].Trim('\r').Trim('\n') : "";
                string pileLine3 = pileStrings[2].Length > i ? pileStrings[2][i].Trim('\r').Trim('\n') : "";
                string pileLine4 = pileStrings[3].Length > i ? pileStrings[3][i].Trim('\r').Trim('\n') : "";
                string pileLine5 = pileStrings[4].Length > i ? pileStrings[4][i].Trim('\r').Trim('\n') : "";
                string pileLine6 = pileStrings[5].Length > i ? pileStrings[5][i].Trim('\r').Trim('\n') : "";
                string pileLine7 = pileStrings[6].Length > i ? pileStrings[6][i].Trim('\r').Trim('\n') : "";
                displayString.Append(String.Format("{0,11}  {1,11}  {2,11}  {3,11}  {4,11}  {5,11}  {6,11}\n", pileLine1, pileLine2, pileLine3, pileLine4, pileLine5, pileLine6, pileLine7));
            }


            return displayString.ToString();
        }

        private static void MoveToAnotherPile(TableuPile from, TableuPile to)
        {
            if (!IsAllowed(from.PeekHighStack(), to.Peek()))
            {
                return;
            }
            foreach (Card c in from.PopCards().Reverse())
            {
                to.PushCard(c);
            }
        }

        private static bool IsAllowed(Card from, Card to)
        {
            if (from == null)
            {
                return false;
            }
            if (to == null && from.Value == "K")
            {
                return true;
            }
            if (to != null && from.Numbervalue == to.Numbervalue - 1 && from.Color != to.Color)
            {
                return true;
            }
            return false;
        }
    }
}