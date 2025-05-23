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
            try 
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
                for (int i = 0; i < tempSub.Count; i++)
                {
                    suburbs[i] = tempSub[i].ToString();
                }
                sr.Close();
                distances = new uint[tempSub.Count, tempSub.Count];
                foreach (string network in networks)
                {
                    char[] charArray = network.ToCharArray();
                    int[] index = ntwArrayToDistanceMatrix(charArray);
                    string distance = new string(charArray);
                    distances[index[0], index[1]] = uint.Parse(distance.Substring(2));
                    distances[index[1], index[0]] = uint.Parse(distance.Substring(2));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("File did not read successfully");
                return false;
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
            else if (findRoad(suburb1[0], suburb2[0], out row, out col))
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

        /// <summary>
        /// Returns true if the road exists between suburb1 and suburb2, false if the road does not exist.
        /// </summary>
        /// <param name="suburb1"></param>
        /// <param name="suburb2"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool findRoad(char suburb1, char suburb2, out int row, out int col)
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

        /// <summary>
        /// Check if there is other adjacent road to each suburb other than the connecting road between them,
        /// return true if there is each suburb is connected with other suburbs, false if the connecting road is the only path bwtween both suburbs.
        /// </summary>
        /// <param name="suburb1"></param>
        /// <param name="suburb2"></param>
        /// <returns></returns>
        private bool moreThanOneRoad(char suburb1, char suburb2)
        {
            int sub1AdjacentRoad = 0;
            int sub2AdjacentRoad = 0;
            for (int i = 0; i < suburbs.Length; i++)
            {
                if (findRoad(suburb1, suburbs[i][0], out _, out _))
                {
                    sub1AdjacentRoad++;
                }
                if (findRoad(suburb2, suburbs[i][0], out _, out _))
                {
                    sub2AdjacentRoad++;
                }
            }
            if (sub1AdjacentRoad > 1 && sub2AdjacentRoad > 1) return true;
            else return false;
        }

        //Remove an existing road from the transportation network   
        //Preconditions: both suburb1 and suburb2 are connected by at least two roads
        //Postconditions: return true, the road (suburb1, suburb2) has been removed from the transportation network if the road exists; 
        //                otherwise, return false, the transportation network remains unchanged.    
        public bool RemoveRoad(string suburb1, string suburb2)
        {
            int row = 0;
            int col = 0;
            if (!findRoad(suburb1[0], suburb2[0], out row, out col))
            {
                return false; //If the road does not exist, return false
            }
            else
            {
                if (moreThanOneRoad(suburb1[0], suburb2[0]))
                {
                    distances[row, col] = NO_ROAD; //Add the new road to the distance matrix
                    distances[col, row] = NO_ROAD; //Add the new road to the distance matrix (for both directions)
                    return true; //If the road exists, remove the road and return true
                }
                else { return false; }
            }
        }

        //Check if this transportation network is connected. A transportation network is connected, if there is a path from any suburb to all other suburbs in this transportation network. 
        //Preconditions: transportation network information has been loaded into the system.
        //Postconditions: return true, if this transportation network is connected; otherwise, return false. This transportation network remains unchanged.
        public bool IsConnected()
        {
            List<string> allRoads = distanceMatrixToNtwArray().ToList(); //Get the list of all roads in the transportation network
            List<char> visited = new List<char>(); //List to store visited suburbs
            for (int i = 0; i < suburbs.Length; i++)
            {
                for (int j = 0; j < suburbs.Length; j++)
                {
                    string road = $"{suburbs[i]},{suburbs[j]},{distances[i, j]}"; //Get the road between the two suburbs
                    string roadReverse = $"{suburbs[j]},{suburbs[i]},{distances[j, i]}"; //Get the road between the two suburbs in reverse order
                    if (allRoads.Contains(road) || allRoads.Contains(roadReverse)) //If the road exists in either direction
                    {
                        if (!visited.Contains(suburbs[i][0])) //If the suburb has not been visited
                        {
                            visited.Add(suburbs[i][0]); //Add the suburb to the list of visited suburbs
                        }
                        if (!visited.Contains(suburbs[j][0])) //If the suburb has not been visited
                        {
                            visited.Add(suburbs[j][0]); //Add the suburb to the list of visited suburbs
                        }
                        allRoads.Remove(road); //Remove the road from the list of all roads
                        allRoads.Remove(roadReverse); //Remove the road from the list of all roads in reverse order
                    }

                }
            }
            if (visited.Count == suburbs.Length) //If all suburbs have been visited
            {
                return true; //Return true, the transportation network is connected
            }
            else
            {
                return false; //Return false, the transportation network is not connected
            }
        }

        // Searches for the shortest path between startSuburb and endSuburb. The total distance between these two suburbs is saved in the "distance" output variable.
        // Preconditions: startSuburb and endSuburb are in the suburbs list, and startSuburb != endSuburb.
        // Postconditions: return true and a shortest path via the out parametre *path*, and the shortest distance via the out parametre *distance* if there exists a path between startSuburb and endSuburb; otherwise, return false, path = null, and distance = Int32.MaxValue.
        public bool FindShortestPath(string startSuburb, string endSuburb, out List<string>? path, out uint distance)
        {
            int n = distances.GetLength(0); //Get the number of suburbs
            uint[] dist = new uint[n]; //Create an array to store the distance from the start suburb to each suburb
            int[] prev = new int[n]; //Create an array to store the previous suburb for each suburb
            bool[] visited = new bool[n]; //Create a boolean array to store visited suburbs
            const uint INF = uint.MaxValue; //Set the sentinel value to the maximum value of an unsigned integer

            for (int i = 0; i < n; i++)
            {
                dist[i] = INF; //Set the distance to the sentinel value
                prev[i] = -1; //Set the previous suburb to -1
                visited[i] = false; //Set the suburb to not visited
            }
            dist[Array.IndexOf(suburbs, startSuburb)] = 0;

            for (int count = 0; count < n - 1; count++)
            {
                int u = -1; //Set the suburb to -1
                uint min = INF; //Set the minimum distance to the sentinel value
                for (int v = 0; v < n; v++) //Loop through the suburbs
                {
                    if (!visited[v] && dist[v] < min) //If the suburb has not been visited and the distance is less than the current minimum distance
                    {
                        min = dist[v]; //Set the minimum distance to the current distance
                        u = v; //Set the suburb to the current suburb
                    }
                }

                if (u == -1) //If the suburb is -1, there are no more suburbs to visit
                {
                    break; //Break the loop
                }

                visited[u] = true; //Set the suburb to visited

                for (int v = 0; v < n; v++) //Loop through the suburbs
                {
                    if (distances[u, v] > 0 && !visited[v])
                    {
                        uint alt = dist[u] + distances[u, v]; //Calculate the alternative distance
                        if (alt < dist[v])
                        {
                            dist[v] = alt;
                            prev[v] = u; //Set the previous suburb to the current suburb
                        }
                    }
                }
            }

            List<int> thePath = new List<int>(); //Create a list to store the path
            for (int at = Array.IndexOf(suburbs, endSuburb); at != -1; at = prev[at]) //Loop through the suburbs
            {
                thePath.Insert(0, at); //Insert the suburb at the beginning of the list
            }

            if (dist[Array.IndexOf(suburbs, endSuburb)] == INF) //If the distance is equal to the sentinel value, there is no path
            {
                path = null; //Set the path to null
                distance = INF; //Set the distance to the sentinel value
                return false; //Return false, there is no path
            }
            else
            {
                for (int i = 0; i < thePath.Count; i++) //Loop through the path
                {
                    thePath[i] = Array.IndexOf(suburbs, suburbs[thePath[i]]); //Get the index of the suburb in the suburbs array
                }
                path = thePath.ConvertAll(x => suburbs[x]); //Convert the path to a list of suburb names
                distance = dist[Array.IndexOf(suburbs, endSuburb)]; //Get the distance from the start suburb to the end suburb
                return true; //Return true, the path exists
            }
        }

        //Find the minimum backbone transportation network (the minimum spanning tree)
        //Preconditions: transportation network information has been loaded into this transportation network and the transportation network is connected.
        //Postconditions: return an array of roads, forming a minimum spanning tree of the transportation network, if the pre-conditions are true; otherwise, return null.
        public List<Road>? IdentifyMinimumBackboneInfrastructure()
        {
            int n = distances.GetLength(0); //Get the number of suburbs
            bool[] visited = new bool[n]; //Create a boolean array to store visited suburbs
            uint[] minEdge = new uint[n]; //Create an array to store the minimum edge for each suburb
            int[] parent = new int[n]; //Create an array to store the parent of each suburb

            const uint INF = uint.MaxValue; //Set the sentinel value to the maximum value of an unsigned integer
            for (int i = 0; i < n; i++)
            {
                minEdge[i] = INF; //Set the minimum edge to the sentinel value
                parent[i] = -1; //Set the parent to -1
                visited[i] = false; //Set the suburb to not visited
            }

            minEdge[0] = 0; //Set the first suburb to be visited

            for (int count = 0; count < n - 1; count++) //Loop through the suburbs
            {
                uint min = INF; //Set the minimum edge to the sentinel value
                int u = -1; //Set the suburb to -1
                for (int v = 0; v < n; v++) //Loop through the suburbs
                {
                    if (!visited[v] && minEdge[v] < min) //If the suburb has not been visited and the minimum edge is less than the current minimum edge
                    {
                        min = minEdge[v]; //Set the minimum edge to the current minimum edge
                        u = v; //Set the suburb to the current suburb
                    }
                }
                if (u == -1) //If the suburb is -1, there are no more suburbs to visit
                {
                    break; //Break the loop
                }
                visited[u] = true; //Set the suburb to visited
                for (int v = 0; v < n; v++) //Loop through the suburbs
                {
                    if (!visited[v] && distances[u, v] != NO_ROAD && distances[u, v] < minEdge[v]) //If the suburb has not been visited and the distance is not equal to the sentinel value and the distance is less than the current minimum edge
                    {
                        parent[v] = u; //Set the parent of the suburb to the current suburb
                        minEdge[v] = distances[u, v]; //Set the minimum edge to the distance
                    }
                }
            }

            List<Road> minBackbone = new List<Road>(); //Create a list to store the minimum backbone roads
            for (int i = 1; i < n; i++) //Loop through the suburbs
            {
                int from = parent[i]; //Get the parent of the suburb
                int to = i; //Get the suburb
                uint distance = distances[to, from]; //Get the distance between the two suburbs
                minBackbone.Add(new Road(suburbs[from], suburbs[to], distance)); //Add the road to the list of minimum backbone roads
            }

            return minBackbone;
        }
    }
}