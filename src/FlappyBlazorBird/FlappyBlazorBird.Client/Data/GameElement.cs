using System;

namespace FlappyBlazorBird.Client.Data
{
    public class GameElement
    {
        protected static readonly Random getrandom = new Random();
        private Guid key = Guid.NewGuid();
        public string Key => key.ToString();
        public string Name {get; set;} = "";
        public double X {set; get; }
        public double Y {set; get; }
        public double? R {set; get; }
        public virtual int Width {set; get; }
        public virtual int Height {set; get; }
        public long CssX =>Convert.ToInt32(X);
        public long CssY =>Convert.ToInt32(Y);
        public string CssClass => this.GetType().Name.ToLower();
        public virtual string Image { get; set; }


        public string RotateTransform => this.R.HasValue?$"transform: rotate({Convert.ToInt32(R).ToString()}deg);":"";
        public virtual string CssStyle => $@"
            position: absolute;
            top: {CssY.ToString()}px;
            left: {CssX.ToString()}px;
            z-index: 0;
            {RotateTransform}";
    }

}