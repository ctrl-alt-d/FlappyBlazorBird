using System.Collections.Generic;
using System.Linq;

namespace FlappyBlazorBird.Client.Data
{
    public class Printable : GameElement
    {
        public Printable() {}
        public Printable(int x, int y, string image, int? r = null)
        {
            this.X = x;
            this.Y = y;
            this.Image = image;
            this.R = r;
        }

    }
}
