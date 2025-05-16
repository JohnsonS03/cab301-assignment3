//CAB301 - Assignment 3A - Main program
// Maolin Tang and Connor McLaughlin 
// 2025

using System;
using System.Collections.Generic;

namespace assignment3A
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            TransportationNetwork graph_expected = new TransportationNetwork();

            while (true)
            {
                menu.DisplayMenu();
                int choice = menu.GetUserChoice();
                
                menu.PerformUserAction(choice, ref graph_expected);
            }
            
        }
    }
}

