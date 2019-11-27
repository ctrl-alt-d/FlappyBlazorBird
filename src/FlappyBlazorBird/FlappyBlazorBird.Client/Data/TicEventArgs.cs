using System;
using System.Collections.Generic;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Data
{
    public class TicEventArgs : EventArgs
    {
        public readonly List<Bird> Players;
        public List<Printable> PrintablePipes;

        public readonly Universe Universe;

        public TicEventArgs(List<Bird> players,  List<Printable> printablePipes, Universe universe)
        {
            Players = players;
            Universe = universe;
            PrintablePipes = printablePipes;
        }
    }
}