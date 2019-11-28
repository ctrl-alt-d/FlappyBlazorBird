using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Data
{
    public class Universe: Printable
    {
        public Universe():base()
        {
            (upperPipes, lowerPipes) = GetNewPipes();
            MainLoop();
        }

        public int CurrentFps = 0;
        
        public async void MainLoop()
        {
            while (true)
            {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Stop();                
                stopWatch.Start();
                this.Recalcula();
                this.OnTic();                
                var d = this.FPS_DELAY - stopWatch.Elapsed.Milliseconds;
                if (d<=1) d = 1;
                await Task.Delay(d);                
                CurrentFps = Convert.ToInt32(1000.0 / stopWatch.Elapsed.Milliseconds);
            }        
        }
        public int loopIter = 0;
        public int basex = 0;
        public int baseShift => this.GetBaseWidth - this.GetBackgroundWidth;

        private void Recalcula()
        {
            foreach(var player in Players)
            {
                player.Tic();
            }

            // playerIndex basex change

            loopIter = (loopIter + 1) % 30;
            basex = -((-basex + 100) % baseShift);

            MovePipes();

        }

        public readonly List<Bird> Players = new List<Bird>();
        public readonly List<Printable> PrintablePiles = new List<Printable>();

        #region Event
        public static event EventHandler<TicEventArgs> Tic; 

        protected virtual void OnTic()
        {
            EventHandler<TicEventArgs> handler = Tic;
            var e = new TicEventArgs(Players.ToList(), PrintablePiles.ToList(),  this);
            handler?.Invoke(this, e);
        }
        #endregion

        #region MainLoop

        #endregion

        public int pipeVelX = -4;
        public List<Dictionary<string,int>> upperPipes;
        public List<Dictionary<string,int>> lowerPipes;
            
        public const int FPS = 30;
        public int FPS_DELAY => Convert.ToInt32( 1000.0 / FPS );

        public const int SCREENWIDTH  = 288;
        public const int SCREENHEIGHT = 512;
        public override int Width => SCREENHEIGHT;
        public override int Height => SCREENHEIGHT;

        public const int PIPEGAPSIZE  = 150; // gap between upper and lower part of pipe
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


        public ( List<Dictionary<string,int>>, List<Dictionary<string,int>> ) GetNewPipes()
        {
            var newPipe1 = getRandomPipe();
            var newPipe2 = getRandomPipe();

            // list of upper pipes
            var upperPipes = new List<Dictionary<string,int>>()
            {
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 250, 
                    ["y"] = newPipe1[0]["y"] 
                },  
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 250 + (Universe.SCREENWIDTH / 2), 
                    ["y"] = newPipe2[0]["y"] 
                }, 
            };

            // list of lowerpipe
            var lowerPipes = new List<Dictionary<string,int>>()
            {
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 250, 
                    ["y"] = newPipe1[1]["y"] 
                },  
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 250 + (Universe.SCREENWIDTH / 2), 
                    ["y"] = newPipe2[1]["y"] 
                }, 
            };

            return (upperPipes, lowerPipes);
        }

        private void MovePipes()
        {
            // move pipes to left
            for( int i = 0; i< this.upperPipes.Count(); i++ )
            {
                var (uPipe, lPipe) = ( this.upperPipes[i], this.lowerPipes[i]);
                uPipe["x"] += this.pipeVelX;
                lPipe["x"] += this.pipeVelX;
            }

            // add new pipe when first pipe is about to touch left of screen
            if ( !upperPipes.Any() || (  0 < upperPipes[0]["x"] &&  upperPipes[0]["x"] < 5 ) )
            {
                var newPipe = getRandomPipe();
                upperPipes.Add(newPipe[0]);
                lowerPipes.Add(newPipe[1]);
            }

            // remove first pipe if its out of the screen
            if (upperPipes[0]["x"] < - this.GetPipeWidth )
            {
                upperPipes.RemoveAt(0);
                lowerPipes.RemoveAt(0);
            }   

            //update pritable pipes
            lock(PrintablePiles)
            {
                PrintablePiles.Clear();
                for( int i = 0; i< this.upperPipes.Count(); i++ )
                {
                    var (uPipe, lPipe) = ( this.upperPipes[i], this.lowerPipes[i]);

                    PrintablePiles.Add(
                        new Printable( uPipe["x"], uPipe["y"], Universe.PIPES_LIST[0], -180 )
                    );
                    PrintablePiles.Add(
                        new Printable( lPipe["x"], lPipe["y"], Universe.PIPES_LIST[1] )
                    );
                }            
            }
        }

        internal void PleaseRestart()
        {

            lock(Players) 
            if (Players.All(p=>p.IsDead)) 
            lock(upperPipes) 
            lock(lowerPipes) 
            (upperPipes, lowerPipes) = GetNewPipes();

        }

        private Dictionary<string,int>[] getRandomPipe()
        {
            //returns a randomly generated pipe
            // y of gap between upper and lower pipe

            Random random = new Random();
            var gapY = random.Next(0, Convert.ToInt32(Universe.BASEY * 0.6 - Universe.PIPEGAPSIZE) );
            gapY += Convert.ToInt32(Universe.BASEY * 0.2);
            var pipeHeight = this.GetPipeHeight;
            var pipeX = Universe.SCREENWIDTH + 10;
            var pipe = new [] {
                new Dictionary<string,int>() {["x"] = pipeX, ["y"] = gapY - pipeHeight },  // upper pipe
                new Dictionary<string,int>() {["x"] = pipeX, ["y"] = gapY + Universe.PIPEGAPSIZE }, // lower pipe
            };
            return pipe;
        }

    }
}