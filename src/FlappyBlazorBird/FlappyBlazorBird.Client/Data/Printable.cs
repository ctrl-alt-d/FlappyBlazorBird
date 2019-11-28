using System;
using System.Collections.Generic;
using System.Linq;

namespace FlappyBlazorBird.Client.Data
{
    public class Printable : GameElement
    {
        public Printable() {}
        public Printable(int x, int y, string image, int? r = null, double? opacity = null)
        {
            this.X = x;
            this.Y = y;
            this.Image = image;
            this.R = r;
            this.Opacity = opacity;
        }

        public static Printable Clone(Printable p)
        {
            return new Printable(Convert.ToInt32(p.X),Convert.ToInt32(p.Y),p.Image,Convert.ToInt32(p.R), p.Opacity );
        }

    }
}
