using Microsoft.AspNetCore.Components;
using FlappyBlazorBird.Client.Data;
using FlappyBlazorBird.Client.Helpers;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FlappyBlazorBird.Client.Pages
{

    public class IndexBase: ComponentBase
    {
        [Inject] protected Universe Universe {get; set; }      
        [Inject] protected IJSRuntime JSRuntime {get; set; }       

        protected Queue<KeyboardEventArgs> KeyPressed = new Queue<KeyboardEventArgs>();
        protected void KeyDown(KeyboardEventArgs e)
        {
            KeyPressed.Enqueue(e);
        }
        protected ElementReference OuterDiv;

        protected bool IsDead = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("SetFocusToElement", OuterDiv);
                MainGame( Render );
            }
        }

        protected async Task Render()
        {
            await InvokeAsync(StateHasChanged);
            await Task.Delay(Universe.FPS_DELAY);
        }

        protected async void MainGame(Func<Task> render)
        {
            var score = 0;
            var playerIndex = 0;
            var loopIter = 0;

            var playerIndexGen = new Cycle<int>(new [] {0, 1, 2, 1}).GetEnumerator();

            var playerx = Convert.ToInt32( Universe.SCREENWIDTH * 0.2);
            var playery = Convert.ToInt32((Universe.SCREENHEIGHT - Universe.GetPlayerHeight) / 2);

            var basex = 0;
            var baseShift = Universe.GetBaseWidth - Universe.GetBackgroundWidth;

            var newPipe1 = getRandomPipe();
            var newPipe2 = getRandomPipe();

            // list of upper pipes
            var upperPipes = new List<Dictionary<string,int>>()
            {
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 200, 
                    ["y"] = newPipe1[0]["y"] 
                },  
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 200 + (Universe.SCREENWIDTH / 2), 
                    ["y"] = newPipe2[0]["y"] 
                }, 
            };

            // list of lowerpipe
            var lowerPipes = new List<Dictionary<string,int>>()
            {
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 200, 
                    ["y"] = newPipe1[1]["y"] 
                },  
                new Dictionary<string,int>() 
                {
                    ["x"] = Universe.SCREENWIDTH + 200 + (Universe.SCREENWIDTH / 2), 
                    ["y"] = newPipe2[1]["y"] 
                }, 
            };

            var pipeVelX = -4;

            // player velocity, max velocity, downward accleration, accleration on flap
            var playerVelY    =  -9   ;// player's velocity along Y, default same as playerFlapped
            var playerMaxVelY =  10   ;// max vel along Y, max descend speed
            var playerAccY    =   1   ;// players downward accleration
            var playerRot     =  45   ;// player's rotation
            var playerVelRot  =   3   ;// angular speed
            var playerRotThr  =  20   ;// rotation threshold
            var playerFlapAcc =  -9   ;// players speed on flapping
            var playerFlapped = false ;// True when player flaps

            var random = new Random();
            var randPlayer = random.Next(0, Universe.PLAYERS_LIST.Count() - 1);
            var player_images = new [] {
                Universe.PLAYERS_LIST[randPlayer][0],
                Universe.PLAYERS_LIST[randPlayer][1],
                Universe.PLAYERS_LIST[randPlayer][2],
            };

            while (true)
            {
                while (KeyPressed.Any())
                {
                    var k = KeyPressed.Dequeue();
                    if (!IsDead && (k.Key == "ArrowUp" || k.Key == " "  ))
                    {
                        if (playery > -2 * Universe.GetPlayerHeight)
                        {
                            playerVelY = playerFlapAcc;
                            playerFlapped = true;
                            //SOUNDS['wing'].play()
                        }
                    } else if (IsDead && ( k.Key == "P" || k.Key == "p") )
                    {
                        playerx = Convert.ToInt32( Universe.SCREENWIDTH * 0.2);
                        playery = Convert.ToInt32((Universe.SCREENHEIGHT - Universe.GetPlayerHeight) / 2);
                        upperPipes[0]["x"] = Universe.SCREENWIDTH + 200;
                        lowerPipes[0]["x"] = Universe.SCREENWIDTH + 200;
                        upperPipes[1]["x"] = Universe.SCREENWIDTH + 200 + (Universe.SCREENWIDTH / 2);
                        lowerPipes[1]["x"] = Universe.SCREENWIDTH + 200 + (Universe.SCREENWIDTH / 2);
                        score = 0;
                        IsDead = false;
                    }
                }


                var crashTest = CheckCrash( ( x: playerx, y: playery, index: playerIndex ),
                                            upperPipes, lowerPipes);
                                    
                if (crashTest.collPipe) IsDead = true;

                var playerMidPos = playerx + Universe.GetPlayerWidth / 2;

                // check for score
                foreach(var pipe in upperPipes)
                {
                    var pipeMidPos = pipe["x"] + Universe.GetPipeWidth / 2;
                    if (pipeMidPos <= playerMidPos && playerMidPos < pipeMidPos + 4)
                    {
                        score += 1;
                        //SOUNDS['point'].play()                    
                    }
                }

                // playerIndex basex change
                if (!IsDead)
                {
                    if ((loopIter + 1) % 3 == 0)
                    {
                        playerIndexGen.MoveNext();
                        playerIndex = playerIndexGen.Current;
                    }
                    loopIter = (loopIter + 1) % 30;
                    basex = -((-basex + 100) % baseShift);
                }

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
                playery += new int[] { playerVelY, Convert.ToInt32( Universe.BASEY - playery - playerHeight) }.Min();

                // move pipes to left
                if (!IsDead) //foreach( var (uPipe, lPipe) in upperPipes.Zip(lowerPipes) )
                             for( int i = 0; i< upperPipes.Count(); i++ )
                {
                    var (uPipe, lPipe) = ( upperPipes[i], lowerPipes[i]);
                    uPipe["x"] += pipeVelX;
                    lPipe["x"] += pipeVelX;
                }

                // add new pipe when first pipe is about to touch left of screen
                if ( !upperPipes.Any() || (  0 < upperPipes[0]["x"] &&  upperPipes[0]["x"] < 5 ) )
                {
                    var newPipe = getRandomPipe();
                    upperPipes.Add(newPipe[0]);
                    lowerPipes.Add(newPipe[1]);
                }

                // remove first pipe if its out of the screen
                if (upperPipes[0]["x"] < -Universe.GetPipeWidth )
                {
                    upperPipes.RemoveAt(0);
                    lowerPipes.RemoveAt(0);
                }   

                // Rendering

                Universe.ToRender.Clear();
                Universe.ToRender.Add(
                    new Printable( 0, 0, Universe.CurrentBackgroundImage )
                );

                //foreach( var (uPipe, lPipe) in upperPipes.Zip(lowerPipes))
                for( int i = 0; i< upperPipes.Count(); i++ )
                {
                    var (uPipe, lPipe) = ( upperPipes[i], lowerPipes[i]);

                    Universe.ToRender.Add(
                        new Printable( uPipe["x"], uPipe["y"], Universe.PIPES_LIST[0], -180 )
                    );
                    Universe.ToRender.Add(
                        new Printable( lPipe["x"], lPipe["y"], Universe.PIPES_LIST[1] )
                    );
                }

                //score
                var printableScore = GetPrintableScore(score);

                Universe.ToRender.Add(
                    new Printable( basex, Convert.ToInt32( Universe.BASEY), Universe.IMAGES["base"]  )
                );

                // print score so player overlaps the score
                Universe.ToRender.AddRange(printableScore);

                var visibleRot = playerRotThr;
                if (playerRot <= playerRotThr)
                {
                    visibleRot = playerRot;
                }
                
                //  playerSurface = pygame.transform.rotate(IMAGES['player'][playerIndex], visibleRot)  Simplify

                var ocell = new Printable( playerx, playery,  player_images[playerIndex] , -visibleRot);
                Universe.ToRender.Add(ocell);

                if (IsDead)
                {
                    var playAgain = new Printable( (Universe.SCREENWIDTH - 192)/2 , Universe.SCREENHEIGHT/2,  Universe.IMAGES["pressptoplayagain"]);
                    Universe.ToRender.Add(playAgain);

                }

                await render();

            }

        }

        private List<Printable> GetPrintableScore(int score)
        {
            var result = new List<Printable>();
            var scoreDigits = score.ToString().ToCharArray().Select(x=>x-'0');
            var totalWidth = 0;
            foreach(var digit in scoreDigits) totalWidth += Universe.GetDigitWidth(digit);

            var Xoffset = (Universe.SCREENWIDTH - totalWidth) / 2;
            foreach(var digit in scoreDigits)
            {
                result.Add(
                    new Printable(Xoffset, Convert.ToInt32( Universe.SCREENHEIGHT * 0.1), Universe.IMAGESS["numbers"][digit])                    
                );
                Xoffset += Universe.GetDigitWidth(digit);
            }
            return result;
        }

        private (bool collPipe, bool collBase) CheckCrash((int x, int y, int index) player, List<Dictionary<string, int>> upperPipes, List<Dictionary<string, int>> lowerPipes)
        {
            var pi = player.index;
            
            if (player.y + Universe.GetPlayerHeight >= Universe.BASEY - 1)
            {
                return (true, true);
            }
            else
            {
                var playerCenterX=player.x-(Universe.GetPlayerWidth/2);
                var playerUpY=player.y;
                var playerLoY=Convert.ToInt32( player.y+Universe.GetPlayerHeight*0.8 );
                //foreach( var (uPipe, lPipe) in upperPipes.Zip(lowerPipes))
                for( int i = 0; i< upperPipes.Count(); i++ )
                {                    
                    var (uPipe, lPipe) = ( upperPipes[i], lowerPipes[i]);
                    var uCollide = InRectangle( playerCenterX, playerUpY, uPipe["x"]+2, uPipe["y"], uPipe["x"] + Universe.GetPipeWidth, uPipe["y"] -4 + Universe.GetPipeHeight   );
                    var lCollide = InRectangle( playerCenterX, playerLoY, lPipe["x"]+2, lPipe["y"], lPipe["x"] + Universe.GetPipeWidth, lPipe["y"] -4 + Universe.GetPipeHeight   );

                    if (uCollide || lCollide)
                    {
                        return (true, false);
                    }
                }
            }

             return (collPipe: false, collBase: false);
        }

        private bool InRectangle(int pX, int pY, int lX, int uY, int rX, int lY)
        {
            bool isAtLeft = pX < lX;
            bool isAtRight = pX > rX;
            bool isDown = pY > lY;
            bool isUp = pY < uY;
            bool isOut = isAtLeft || isAtRight || isDown || isUp;
            return !isOut;
        }

        private Dictionary<string,int>[] getRandomPipe()
        {
            //returns a randomly generated pipe
            // y of gap between upper and lower pipe

            Random random = new Random();
            var gapY = random.Next(0, Convert.ToInt32(Universe.BASEY * 0.6 - Universe.PIPEGAPSIZE) );
            gapY += Convert.ToInt32(Universe.BASEY * 0.2);
            var pipeHeight = Universe.GetPipeHeight;
            var pipeX = Universe.SCREENWIDTH + 10;
            var pipe = new [] {
                new Dictionary<string,int>() {["x"] = pipeX, ["y"] = gapY - pipeHeight },  // upper pipe
                new Dictionary<string,int>() {["x"] = pipeX, ["y"] = gapY + Universe.PIPEGAPSIZE }, // lower pipe
            };
            return pipe;
        }
    }

}