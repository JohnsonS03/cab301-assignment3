//CAB301 - Assignment 3A - Menu class
// Maolin Tang and Connor McLaughlin 
// 2025

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace assignment3A
{
    internal class Menu
    {

        //Display menu
        public void DisplayMenu()
        {

            Console.WriteLine("\nTransportation Network Planning");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("1. Load Transportation Network Information from File");
            Console.WriteLine("2. Save Transportation Network Information to File");
            Console.WriteLine("3. Add a Road to Transportation Network");
            Console.WriteLine("4. Remove a Road from Transportation Network");
            Console.WriteLine("5. Check Connectivity of Transportation Network");
            Console.WriteLine("6. Find Shortest Path and Distance between Two Suburbs");
            Console.WriteLine("7. Identify Minimum Backbone Transportation Network");
            Console.WriteLine("0. Exit");
            Console.WriteLine("--------------------------------------------------");
            Console.Write("Enter your choice (1-7) or press 0 to exit: ");
        }

        //Get the user's choice
        public int GetUserChoice()
        {
            int choice;
            while (true)
            {
                try
                {
                    if (int.TryParse(Console.ReadLine(), out choice))
                    {
                        if (choice == 0)
                        {
                            Console.WriteLine("Thanks for using this system!");
                            Environment.Exit(0);
                        }
                        else if (choice >= 1 || choice <= 4)
                        {
                            return choice;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Invalid input. Please enter a valid choice (0-5).");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid input. Please enter a valid choice (0-5).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        private bool CheckDataLoaded(TransportationNetwork g)
        {
            if (g.Suburbs == null)
            {
                Console.WriteLine("Transportation network plan data have not been read into the system!");
                Console.WriteLine("Please read transportation plan data into the system first.");
                return false;
            }

            return true;
        }

        private static int SelectInt(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            while (true)
            {
                Console.Write("{0}: ", prompt);

                string? line = Console.ReadLine();
                int value;
                if (line == null || !int.TryParse(line, out value) || value < minValue || value > maxValue)
                {
                    Console.WriteLine("Invalid input. The value must be an integer in the range of {0} to {1}.", minValue, maxValue);
                    continue;
                }

                return value;
            }
        }

        private (string, string) SelectSuburbPair(TransportationNetwork g)
        {
            while (true)
            {
                Console.Write("Enter a pair of suburbs in the format of \"A B\" (options: {0}): ", string.Join(", ", g.Suburbs!));
                string? suburbPair = Console.ReadLine();
                string[] suburbs = suburbPair!.Split(" ");
                if (suburbs.Length != 2 || (suburbs[0] = suburbs[0].Trim()).Length == 0 || (suburbs[1] = suburbs[1].Trim()).Length == 0)
                {
                    Console.WriteLine("Invalid input. You must specify a pair of suburbs.");
                    continue;
                }

                if (suburbs[0] == suburbs[1])
                {
                    Console.WriteLine("The two suburbs must be different.");
                    continue;
                }

                // Iterate through the array to check if the string is present
                bool containsValidSuburbs = true;
                foreach (string suburb in suburbs)
                {
                    if (Array.IndexOf(g.Suburbs!, suburb) < 0)
                    {
                        Console.WriteLine("{0} is not a valid suburb.", suburb);
                        containsValidSuburbs = false;
                    }
                }

                if (containsValidSuburbs)
                    return (suburbs[0], suburbs[1]);
            }
        }

        private static void DisplayNetwork(TransportationNetwork g)
        {
            if (g.Suburbs == null)
                return;

            Console.WriteLine("Roads in network:");
            for (int i = 0; i < g.Suburbs.Length; i++)
            {
                for (int j = i + 1; j < g.Suburbs.Length; j++)
                {
                    if (g.Distances![i, j] == TransportationNetwork.NO_ROAD)
                        continue;

                    Console.WriteLine("  {0} <=> {1} with distance {2}", g.Suburbs[i], g.Suburbs[j], g.Distances[i, j]);
                }
            }
            Console.WriteLine();
        }

        //Perform an action selected by the user
        public void PerformUserAction(int choice, ref TransportationNetwork g)
        {
            try
            {
                switch (choice)
                {
                    case 1: //1. Load Transportation Network Information from File
                        {
                            string currentDirectory = Environment.CurrentDirectory;
                            //Console.WriteLine("Current Directory: " + currentDirectory);
                            Console.WriteLine("Please input a file name from which the system reads information about a transportation network.");
                            Console.WriteLine("Make sure the transportation network file is in a sub-directory, namely Tests, under the current directory - the directory where the exe file is located.");
                            string? fileName = Console.ReadLine();
                            //string? filePath = currentDirectory + "\\" + "Tests" + "\\"+fileName;
                            string filePath = Path.Combine(currentDirectory, "Tests", fileName!);
                            if (g.LoadTransportationNetwork(filePath))
                            {
                                Console.WriteLine("\nFile read successfully.");
                            }
                            else
                            {
                                Console.WriteLine("\nFile did not read successfully.");
                            }

                            DisplayNetwork(g);
                        }
                        break;

                    case 2: //2. Save Transportation Network Information to File
                        {
                            if (CheckDataLoaded(g))
                            {
                                Console.WriteLine("Please input a file name to which the system saves the information about the current transportation network in the system.");
                                Console.WriteLine("The file will be created in a sub-directory, namely Tests, under the current directory - the directory where the exe file is located.");
                                string? fileName = Console.ReadLine();
                                string filePath = Path.Combine(Environment.CurrentDirectory, "Tests", fileName!);
                                if (g.SaveTransportationNetwork(filePath))
                                    Console.WriteLine("\nFile saved successfully.");
                                else
                                    Console.WriteLine("\nFile did not save successfully.");
                            }
                        }
                        break;

                    case 3: //3. Add a Road to Transportation Network
                        {
                            if (CheckDataLoaded(g))
                            {
                                var (suburb1, suburb2) = SelectSuburbPair(g);
                                int distance = SelectInt("Enter distance between roads", 1, (int)(TransportationNetwork.NO_ROAD - 1));
                                if (g.AddRoad(suburb1, suburb2, (uint)distance))
                                    Console.WriteLine("Road between {0} and {1} successfully added with distance of {2}.", suburb1, suburb2, distance);
                                else
                                    Console.WriteLine("Failed to add road between {0} and {1}. Does a road already exist?", suburb1, suburb2);

                                Console.WriteLine();
                                DisplayNetwork(g);
                            }
                        }
                        break;

                    case 4: //4. Remove a Road from Transportation Network
                        {
                            if (CheckDataLoaded(g))
                            {
                                var (suburb1, suburb2) = SelectSuburbPair(g);
                                if (g.RemoveRoad(suburb1, suburb2))
                                    Console.WriteLine("Road between {0} and {1} successfully removed.", suburb1, suburb2);
                                else
                                    Console.WriteLine("Failed to remove road between {0} and {1}. Does it exist?", suburb1, suburb2);

                                Console.WriteLine();
                                DisplayNetwork(g);
                            }
                        }
                        break;

                    case 5:  // 5.Check Connectivity of Transportation Network
                        {
                            if (CheckDataLoaded(g))
                            {
                                Console.WriteLine();

                                DisplayNetwork(g);

                                if (g.IsConnected())
                                    Console.WriteLine("The transportation network is connected.");
                                else
                                    Console.WriteLine("The transportation network is not connected.");

                                Console.WriteLine();
                            }
                        }
                        break;

                    case 6: //shortest path and distance
                        {
                            if (CheckDataLoaded(g))
                            {
                                var (suburb1, suburb2) = SelectSuburbPair(g);
                                Console.WriteLine();

                                if (suburb1 == suburb2)
                                {
                                    Console.WriteLine("The two Suburbs that you entered are the same.");
                                }
                                else
                                {
                                    uint distance;
                                    List<string>? path;
                                    if (g.FindShortestPath(suburb1!, suburb2!, out path, out distance))
                                    {
                                        Console.WriteLine("Shortest path between {0} and {1}:", suburb1, suburb2);
                                        Console.WriteLine("  {0}", string.Join(", ", path!));
                                        Console.WriteLine("With a distance of {0}", distance);
                                    }
                                    else
                                    {
                                        Console.WriteLine("No path found between {0} and {1}.", suburb1, suburb2);
                                    }

                                    Console.WriteLine();
                                }
                            }
                        }
                        break;

                    case 7: //7. Identify Minimum Backbone Transportation Network
                        {
                            if (CheckDataLoaded(g) && g.IsConnected())
                            {
                                List<Road>? mst = g.IdentifyMinimumBackboneInfrastructure();

                                Console.WriteLine("\nMinimum backbone transportation network:");
                                foreach (Road road in mst!)
                                    Console.WriteLine("{0} <=> {1} with distance {2}", road.Suburb1, road.Suburb2, road.Distance);
                                Console.WriteLine();
                            }
                            else
                                Console.WriteLine("\nThe transportation network has not been loaded or the transportation network is not connected!");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }

        }
    }
}
