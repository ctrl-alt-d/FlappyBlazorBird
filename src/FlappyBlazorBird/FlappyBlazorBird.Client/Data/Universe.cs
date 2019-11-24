using System;
using System.Collections.Generic;
using System.Linq;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Data
{

    

    public class Universe: Printable
    {
        public List<Printable> ToRender = new List<Printable>();
        public string DivStyle => 
            $@"max-width: {SCREENWIDTH}px; 
            min-width: {SCREENWIDTH}px; 
            height: {SCREENHEIGHT}px; 
            max-height: {SCREENHEIGHT}px; 
            border: 0; 
            padding: 0; 
            margin: 0; 
            position: relative;";
            
        public const int FPS = 30;
        public int FPS_DELAY => Convert.ToInt32( 1000.0 / FPS );

        public const int SCREENWIDTH  = 288;
        public const int SCREENHEIGHT = 512;
        public override int Width => SCREENHEIGHT;
        public override int Height => SCREENHEIGHT;

        public const int PIPEGAPSIZE  = 100; // gap between upper and lower part of pipe
        public static double BASEY => SCREENHEIGHT * 0.79;

        //# image, sound and hitmask  dicts
        //IMAGES, SOUNDS, HITMASKS = {}, {}, {}

        public string CurrentBackgroundImage {get; set; } = BACKGROUNDS_LIST[0];
        public override string Image => CurrentBackgroundImage;
        public int GetPlayerHeight => 24;

        public int GetBaseWidth => 336;

        public int GetBackgroundWidth => 288;

        public int GetPipeHeight => 320;

        public int GetPlayerWidth => 34;

        public int GetPipeWidth => 52;

        public static Dictionary<string, string[]> IMAGESS =new Dictionary<string, string[]>() 
        {
            ["numbers"] = new [] {
                "assets/sprites/0.png",
                "assets/sprites/1.png",
                "assets/sprites/2.png",
                "assets/sprites/3.png",
                "assets/sprites/4.png",
                "assets/sprites/5.png",
                "assets/sprites/6.png",
                "assets/sprites/7.png",
                "assets/sprites/8.png",
                "assets/sprites/9.png",
            },            
        };

        public static Dictionary<string, string> IMAGES = new Dictionary<string, string>() 
        {
            ["gameover"] ="assets/sprites/gameover.png",
            ["message"] ="assets/sprites/message.png",
            ["base"] ="assets/sprites/base.png",
            ["pressptoplayagain"] = "assets/sprites/pressptoplayagain.png",
        };

        public static Dictionary<string, string> SOUNDS = new Dictionary<string, string>() 
        {
            ["die"] ="assets/audio/die.ogg",
            ["hit"] ="assets/audio/hit.ogg",
            ["point"] ="assets/audio/point.ogg",
            ["swoosh"] ="assets/audio/swoosh.ogg",
            ["swoosh"] ="assets/audio/wing.ogg",            
        };

        //list of all possible players (tuple of 3 positions of flap)
        public static string[][] PLAYERS_LIST = new []
        {
            // red bird
            new []
            {
                "assets/sprites/redbird-upflap.png",
                "assets/sprites/redbird-midflap.png",
                "assets/sprites/redbird-downflap.png",
            },

            // blue bird
            new []
            {
                "assets/sprites/bluebird-upflap.png",
                "assets/sprites/bluebird-midflap.png",
                "assets/sprites/bluebird-downflap.png",
            },

            // yellow bird
            new []
            {
                "assets/sprites/yellowbird-upflap.png",
                "assets/sprites/yellowbird-midflap.png",
                "assets/sprites/yellowbird-downflap.png",
            },

        };


        // list of backgrounds
        public static string[] BACKGROUNDS_LIST = new []
        {
            "assets/sprites/background-day.png",
            "assets/sprites/background-night.png",
        };

        // list of pipes
        public static string[] PIPES_LIST = new []
        {
            "assets/sprites/pipe-green.png",
            "assets/sprites/pipe-red.png",
        };

        public int GetDigitWidth(int digit)
        {
            return digit == 1 ? 16 : 24;
        }
    }
}