//CAB301 Assignment 3A - TransportationNetwok class
// Maolin Tang and Connor McLaughlin 
// 2025

using System;
using System.Collections.Generic;

namespace assignment3A
{
    public partial class TransportationNetwork
    {
        // Sentinel value representing no edge between two vertices.
        public const uint NO_ROAD = Int32.MaxValue;

        private string[]? suburbs; //an array storing the names of those suburbs in the city
        private uint[,]? distances; //an adjacency matrix storing distances between the suburbs. If there is a road linking two suburbs, the distance is Int32.MaxValue.

        public string[]? Suburbs
        {
            get { return suburbs; }
        }

        public uint[,]? Distances
        {
            get { return distances; }
        }

        //Load information about a transportation network from a text file, filePath, and put into the information into this transportation network.
        //Preconditions: the given file exists at the given path and the file is not empty.
        //Postconditions: return true, if the information about the transportation network has been loaded successfully (the suburbs are stored in the class field, suburbs, in ascending order by suburb name and the road distances between the suburbs are stored in the class fields, distances;
        //                otherwise, return false and both suburbs and distances are null.
        public bool LoadTransportationNetwork(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            List<char> tempSub = [];
            List<string> networks = new List<string>();

            string? line = sr.ReadLine();
            do
            {
                line = line.Replace(" ", "");
                line = line.Replace(",", "");
                char[] charArray = line.ToCharArray(); //Parse current line into a char array
                networks.Add(charArrToString(charArray)); //Add the charArray parsed string to the list of networks
                Console.WriteLine(line);

                for (int i = 0; i < 2; i++)
                {
                    if (!tempSub.Contains(charArray[i]))
                    {
                        tempSub.Add(charArray[i]);
                    }
                }
                line = sr.ReadLine();
            }
            while (line != null);

            tempSub.Sort();
            suburbs = new string[tempSub.Count];
            for(int i = 0; i < tempSub.Count; i++)
            {
                suburbs[i] = tempSub[i].ToString();
            }
            sr.Close();

            distances = new uint[tempSub.Count, tempSub.Count];
            foreach(string network in networks)
            {
                char[] charArray = network.ToCharArray();
                int[] index = ntwArrayToDistanceMatrix(charArray);
                distances[index[0], index[1]] = Convert.ToUInt32(charArray[2].ToString());
                distances[index[1], index[0]] = Convert.ToUInt32(charArray[2].ToString());
            }

            polishDistanceMatrix(); //Polish the distance matrix to remove any 0s that are not roads
            return true;
        }

        /// <summary>
        /// Returns a string of the parsed input charArray of a specific line. (e.g. {A, B, 5} -> "AB5")
        /// </summary>
        /// <param name="charArray"></param>
        /// <returns></returns>
        private string charArrToString(char[] charArray)
        {
            string line = string.Empty;
            for (int i = 0; i < charArray.Length; i++)
            {
                line += charArray[i].ToString();
            }
            return line;
        }

        /// <summary>
        /// Returns the index of the suburbs in the distances matrix.
        /// </summary>
        /// <param name="charArray"></param>
        /// <returns></returns>
        private int[] ntwArrayToDistanceMatrix(char[] charArray)
        {
            int row = 0;
            int col = 0;
            row = Array.IndexOf(suburbs, charArray[0].ToString()); 
            col = Array.IndexOf(suburbs, charArray[1].ToString());

            return new int[] { row, col };
        }

        /// <summary>
        /// To polish the distance matrix to remove any 0s that are not roads.
        /// </summary>
        private void polishDistanceMatrix()
        {
            for (int i = 0; i < suburbs.Length; i++)
            {
                for (int j = 0; j < suburbs.Length; j++)
                {
                    if (distances[i, j] == 0)
                    {
                        distances[i, j] = NO_ROAD;
                    }
                }
            }
        }

        //Save information about the transportation network to a text file, filePath  
        //Preconditions: a transportation network has been loaded and is valid.
        //Postconditions: return true, if the information about this transportation network has been successfully saved to the given text file;
        //                otherwise, return false. This transportation network remains unchanged.    
        public bool SaveTransportationNetwork(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                string[] allNetworks = distanceMatrixToNtwArray();
                foreach (string network in allNetworks)
                {
                    sw.WriteLine(network);
                }
                sw.Close();
            }
            return true;
        }

        /// <summary>
        /// Returns the distance matrix to a string array of networks.
        /// </summary>
        /// <returns></returns>
        private string[] distanceMatrixToNtwArray()
        {
            List<string> ntwArray = new List<string>();
            for (int i = 0; i < suburbs.Length; i++) //i for row
            {
                for (int j = i; j < suburbs.Length; j++) //j for column
                {
                    if (suburbs[i] == suburbs[j] || distances[i, j] == 0)
                        continue; //Skip if the suburbs are the same
                    if (distances[i, j] != NO_ROAD)
                    {
                        string lineString = $"{suburbs[i]},{suburbs[j]},{distances[i, j]}";
                        ntwArray.Add(lineString); //Add the line to the list of networks
                    }
                }
            }
            return ntwArray.ToArray(); //Convert the list of networks to an array of strings
        }

        //Add a new road between suburb 1 and suburb 2 to the transportation network   
        //Preconditions: there is no road between suburb1 and suburb2, both suburb1 and suburb2 are valid (in suburbs), distance > 0, suburb1 != suburb2, and currently there is no road between suburb1 and suburb2. 
        //Postconditions: return true, if the new road has been added into this transportation network; otherwise, 
        //                return false, if the road is not new or suburb1 or suburb2 is not in the transportation network, and this transportation network remains unchanged. 
        public bool AddRoad(string suburb1, string suburb2, uint distance)
        {
            int row = 0;
            int col = 0;
            //To check if the suburbs are valid and distance is greater than 0
            if (suburb1[0] == suburb2[0] || !char.IsLetter(suburb1[0]) || !char.IsLetter(suburb2[0]) || distance <= 0)
            {
                return false;
            }
            else if (findRoad(suburb1[0], suburb2[0], ref row, ref col))
            {
                return false; //If the road already exists, return false
            }
            else
            {
                distances[row, col] = distance; //Add the new road to the distance matrix
                distances[col, row] = distance; //Add the new road to the distance matrix (for both directions)
                return true; //If the road does not exist(then road will be add in above elseif), return true
            }
        }

        private bool findRoad(char suburb1, char suburb2, ref int row, ref int col)
        {
            row = Array.IndexOf(suburbs, suburb1.ToString());
            col = Array.IndexOf(suburbs, suburb2.ToString());
            if (distances[row, col] != NO_ROAD) //If the distance is not equal to the sentinel value, the road exists
            {
                return true;
            }
            else
            {
                return false; //If the distance is equal to the sentinel value, the road does not exist
            }
        }

        //Remove an existing road from the transportation network   
        //Preconditions: both suburb1 and suburb2 are connected by at least two roads
        //Postconditions: return true, the road (suburb1, suburb2) has been removed from the transportation network if the road exists; 
        //                otherwise, return false, the transportation network remains unchanged.    
        public bool RemoveRoad(string suburb1, string suburb2)
        {
            int row = 0;
            int col = 0;
            if (!findRoad(suburb1[0], suburb2[0], ref row, ref col))
            {
                return false; //If the road does not exist, return false
            }
            else
            {
                distances[row, col] = NO_ROAD; //Add the new road to the distance matrix
                distances[col, row] = NO_ROAD; //Add the new road to the distance matrix (for both directions)
                return true; //If the road exists, remove the road and return true
            }
        }

        //Check if this transportation network is connected. A transportation network is connected, if there is a path from any suburb to all other suburbs in this transportation network. 
        //Preconditions: transportation network information has been loaded into the system.
        //Postconditions: return true, if this transportation network is connected; otherwise, return false. This transportation network remains unchanged.
        public bool IsConnected()
        {
            //To be completed by students
            throw new NotImplementedException("IsConnected is not implemented");
        }

        // Searches for the shortest path between startSuburb and endSuburb. The total distance between these two suburbs is saved in the "distance" output variable.
        // Preconditions: startSuburb and endSuburb are in the suburbs list, and startSuburb != endSuburb.
        // Postconditions: return true and a shortest path via the out parametre *path*, and the shortest distance via the out parametre *distance* if there exists a path between startSuburb and endSuburb; otherwise, return false, path = null, and distance = Int32.MaxValue.
        public bool FindShortestPath(string startSuburb, string endSuburb, out List<string>? path, out uint distance)
        {
            //To be completed by students
            throw new NotImplementedException("FindShortestPath is not implemented");
        }

        //Find the minimum backbone transportation network (the minimum spanning tree)
        //Preconditions: transportation network information has been loaded into this transportation network and the transportation network is connected.
        //Postconditions: return an array of roads, forming a minimum spanning tree of the transportation network, if the pre-conditions are true; otherwise, return null.
        public List<Road>? IdentifyMinimumBackboneInfrastructure()
        {
            //To be completed by students
            throw new NotImplementedException("IdentifyMinimumBackboneInfrastructure is not implemented");
        }
    }
}