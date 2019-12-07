using System;
using System.Collections.Generic;
using System.Linq;
using FlappyBlazorBird.Client.Helpers;
using Microsoft.AspNetCore.Components.Web;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Data
{
    public class Bird: Printable
    {
        public readonly Universe Universe;

        public Bird(Universe universe) : base()
        {
            Universe = universe;
            randPlayer = random.Next(0, Universe.PLAYERS_LIST.Count() - 1);
            IsDead=true;
            InitializePlayer();
            Universe.Players.Add(this);
            Universe.TotalSessions++;
        }
        public int score = 0;
        public int playerIndex = 0;

        public int GraceInterval => 2000 / Universe.FPS_DELAY;
        public int CurrentGraceInterval = 0;

        public int PenaltyTime => 1500 / Universe.FPS_DELAY;
        public int CurrentPenaltyTime = 0;

        public IEnumerator<int> playerIndexGen = new Cycle<int>(new [] {0, 1, 2, 1}).GetEnumerator();

        public int playerx;
        public int playery;

        // player velocity, max velocity, downward accleration, accleration on flap
        public int  playerVelY    =  -9   ;// player's velocity along Y, default same as playerFlapped
        public int  playerMaxVelY =  10   ;// max vel along Y, max descend speed
        public int  playerAccY    =   1   ;// players downward accleration
        public int  playerRot     =  45   ;// player's rotation
        public int  playerVelRot  =   3   ;// angular speed
        public int  playerRotThr  =  20   ;// rotation threshold
        public int  playerFlapAcc =  -9   ;// players speed on flapping
        public bool  playerFlapped = false ;// True when player flaps
        public bool stopSent = false;
        public Random random = new Random();
        public int randPlayer = 0;
        public string[] player_images => new [] {
            Universe.PLAYERS_LIST[randPlayer][0],
            Universe.PLAYERS_LIST[randPlayer][1],
            Universe.PLAYERS_LIST[randPlayer][2],
        };

        public Queue<KeyboardEventArgs> KeyPressed = new Queue<KeyboardEventArgs>();
        public bool IsDead;
        public int visibleRot;
        public bool Tic()
        {
            var keysToProcess = new List<KeyboardEventArgs>(KeyPressed);
            KeyPressed.Clear();
            foreach(var k in keysToProcess)
            {
                if (!IsDead && (k.Key == "ArrowUp" || k.Key == " "  ))
                {
                    if (playery > -2 * Universe.GetPlayerHeight)
                    {
                        playerVelY = playerFlapAcc;
                        playerFlapped = true;
                        //SOUNDS['wing'].play()
                    }
                } else if (IsDead && CurrentPenaltyTime==0 && ( k.Key == "P" || k.Key == "p" || k.Key == "ArrowUp" || k.Key == " ") )
                {
                    InitializePlayer();
                }
            }

            var crashTest = CheckCrash( );
                                
            if (crashTest.collPipe)
            {
                if (!IsDead) CurrentPenaltyTime = PenaltyTime;  
                IsDead = true;                
            } 

            var playerMidPos = playerx + Universe.GetPlayerWidth / 2;

            // check for score
            if (!IsDead && CurrentGraceInterval==0) 
            foreach(var pipe in Universe.upperPipes.ToList())
            {
                var pipeMidPos = pipe.X + Universe.GetPipeWidth / 2;
                if (pipeMidPos <= playerMidPos && playerMidPos < pipeMidPos + 4)
                {
                    score += 1;
                    if (score > Universe.MaxScore) Universe.MaxScore = score;
                    //SOUNDS['point'].play()                    
                }
            }

            CurrentGraceInterval=CurrentGraceInterval>0?CurrentGraceInterval-1:0;
            CurrentPenaltyTime=CurrentPenaltyTime>0?CurrentPenaltyTime-1:0;

            // rotate the player
            if (playerRot > -90)
            {
                playerRot -= playerVelRot;
            }

            // player's movement
            if (playerVelY < playerMaxVelY && !playerFlapped)
            {
                playerVelY += playerAccY;
            }
                
            if (playerFlapped)
            {
                playerFlapped = false;
                // more rotation to cover the threshold (calculated in visible rotation)
                playerRot = 45;
            }

            var playerHeight = Universe.GetPlayerHeight;
            var bottom = Universe.BASEY - playerHeight;

            playery += playerVelY;

            // if is touching base
            if (playery > bottom)
            {
                playery=Convert.ToInt32(bottom);
                playerx += Universe.pipeVelX;
                if (playerx < -100) playerx = -100;
            }

            if ((Universe.loopIter + 1) % 3 == 0)
            {
                playerIndexGen.MoveNext();
                playerIndex = playerIndexGen.Current;
            }

            visibleRot = playerRotThr;
            if (playerRot <= playerRotThr)
            {
                visibleRot = playerRot;
            }

            return IsDead;

        }      

        private (bool collPipe, bool collBase) CheckCrash()
        {            
            if (playery + Universe.GetPlayerHeight >= Universe.BASEY - 1)
            {
                return (true, true);
            }
            else
            {
                var playerCenterX=playerx+(Universe.GetPlayerWidth/2);
                var playerUpY=playery;
                var playerLoY=Convert.ToInt32( playery+Universe.GetPlayerHeight*0.8 );
                //foreach( var (uPipe, lPipe) in upperPipes.Zip(lowerPipes))
                for( int i = 0; i< Universe.upperPipes.Count(); i++ )
                {                    
                    var (uPipe, lPipe) = ( Universe.upperPipes[i], Universe.lowerPipes[i]);
                    var uCollide = InRectangle( playerCenterX, playerUpY, uPipe.X+2, uPipe.Y, uPipe.X + Universe.GetPipeWidth -2, uPipe.Y + Universe.GetPipeHeight   );
                    var lCollide = InRectangle( playerCenterX, playerLoY, lPipe.X+2, lPipe.Y, lPipe.X + Universe.GetPipeWidth -2, lPipe.Y + Universe.GetPipeHeight   );

                    if (uCollide || lCollide)
                    {
                        return (CurrentGraceInterval==0 && true, false);
                    }
                }
            }

            return (collPipe: false, collBase: false);
        }

        private bool InRectangle(int pX, int pY, double lX, double uY, double rX, double lY)
        {
            bool isAtLeft = pX < lX;
            bool isAtRight = pX > rX;
            bool isDown = pY > lY;
            bool isUp = pY < uY;
            bool isOut = isAtLeft || isAtRight || isDown || isUp;
            return !isOut;
        }    

        private void InitializePlayer()
        {
            playerVelY    =  -9   ;// player's velocity along Y, default same as playerFlapped
            playerMaxVelY =  10   ;// max vel along Y, max descend speed
            playerAccY    =   1   ;// players downward accleration
            playerRot     =  45   ;// player's rotation
            playerVelRot  =   3   ;// angular speed
            playerRotThr  =  20   ;// rotation threshold
            playerFlapAcc =  -9   ;// players speed on flapping
            playerFlapped = false ;// True when player flaps            
            playerx = Convert.ToInt32( Universe.SCREENWIDTH * 0.2);
            playery = Convert.ToInt32((Universe.SCREENHEIGHT - Universe.GetPlayerHeight) / 2);
            score = 0;
            Universe.PleaseRestart();
            CurrentGraceInterval = GraceInterval;
            IsDead = false;
            stopSent = false;
        }
    }  
}
