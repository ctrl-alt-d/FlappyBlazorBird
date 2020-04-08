using System;
using System.Collections.Generic;
using System.Linq;

namespace FlappyBlazorBird.Client.Data
{
    public class Printable : GameElement
    {
        public Printable() {}
        public Printable(int x, int y, string image=null, string name=null, int? r = null, double? opacity = null, Guid? guidKey = null )
        {
            this.X = x;
            this.Y = y;
            this.Image = image;
            this.R = r;
            this.Name = name;
            this.Opacity = opacity;
            if (guidKey != null && guidKey != Guid.Empty)
            {
                this.GuidKey = guidKey.Value;
            }
            
        }
        public Printable(double x, double y, string image=null, string name=null, int? r = null, double? opacity = null, Guid? guidKey = null ) :
          this(Convert.ToInt32(x), Convert.ToInt32(y), image, name, r, opacity, guidKey) 
        {
        }

        public Printable(double x, double y, string image=null,  int? r = null, double? opacity = null, Guid? guidKey = null ) :
          this(Convert.ToInt32(x), Convert.ToInt32(y), image, null, r, opacity, guidKey) 
        {
        }

        public static Printable Clone(Printable p)
        {
            return new Printable(Convert.ToInt32(p.X),Convert.ToInt32(p.Y), p.Image, p.Name, Convert.ToInt32(p.R), p.Opacity, p.GuidKey );
        }

        public string CssClass => $"{this.Image} unselectable";

    }
}
