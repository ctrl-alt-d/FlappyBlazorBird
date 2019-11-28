using Microsoft.AspNetCore.Components;
using FlappyBlazorBird.Client.Data;
using FlappyBlazorBird.Client.Helpers;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using Microsoft.JSInterop;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Pages
{

    public class Statistics
    {
        public int fps;
        public int totalPlayers;
    }

    public class IndexBase: ComponentBase, IDisposable
    {
        [Inject] protected Universe Universe {get; set; }      
        [Inject] protected IJSRuntime JSRuntime {get; set; }       

        protected void KeyDown(KeyboardEventArgs e)
        {
            MyBird.KeyPressed.Enqueue(e);
        }
        protected void OnClick()
        {
            var e = new KeyboardEventArgs();
            e.Key = MyBird.IsDead?"p":"ArrowUp";
            MyBird.KeyPressed.Enqueue(e);
        }

        protected ElementReference OuterDiv;
        protected Bird MyBird;

        protected override void OnInitialized()
        {
            MyBird = new Bird(Universe);
            Universe.Tic += Render;
        }

        protected Statistics Statistics = new Statistics() {};


        protected List<Printable> ToRender = new List<Printable>();

        private void Render(object sender, TicEventArgs e)
        {
            var toRender = new List<Printable>();

            // background
            var background = new Printable( 0, 0, e.Universe.CurrentBackgroundImage );
            toRender.Add(background);

            // pipes
            var pipes = e.PrintablePipes;
            
            if (MyBird.IsDead || MyBird.CurrentGraceInterval > 0)
            {
                pipes = pipes.Select(p=>new Printable(Convert.ToInt32(p.X),Convert.ToInt32(p.Y),p.Image,Convert.ToInt32(p.R),0.4) ).ToList();
            }
            toRender.AddRange(pipes);

            // the base
            var theBase = new Printable( e.Universe.basex, Convert.ToInt32( Universe.BASEY), Universe.IMAGES["base"]  );
            toRender.Add(theBase);

            // score
            toRender.AddRange( GetPrintableScore(MyBird.score) );

            // other players
            foreach(var bird in e.Players)
            {
                if (bird != MyBird)
                {
                    var otherBirdIndex = bird.IsDead?0:bird.playerIndex;
                    var otherBird = new Printable( bird.playerx, bird.playery,  bird.player_images[otherBirdIndex] , -bird.visibleRot, opacity: 0.5);
                    toRender.Add(otherBird);
                }
            }

            // myBird
            var myBirdIndex = MyBird.IsDead?0:MyBird.playerIndex;
            var ocell = new Printable( MyBird.playerx, MyBird.playery,  MyBird.player_images[myBirdIndex] , -MyBird.visibleRot);
            toRender.Add(ocell);

            // play again
            if (MyBird.IsDead && MyBird.CurrentPenaltyTime==0)
            {
                var playAgain = new Printable( (Universe.SCREENWIDTH - 192)/2 , Universe.SCREENHEIGHT/2,  Universe.IMAGES["pressptoplayagain"]);
                toRender.Add(playAgain);
            } else if (MyBird.IsDead && MyBird.CurrentPenaltyTime>0)
            {
                var gameOver = new Printable( (Universe.SCREENWIDTH - 192)/2 , Universe.SCREENHEIGHT/2,  Universe.IMAGES["gameover"]);
                toRender.Add(gameOver);
            } 

            lock(ToRender) 
            {
                Statistics.totalPlayers = e.Players.Count();
                Statistics.fps = Universe.CurrentFps;
                ToRender.Clear();
                ToRender.AddRange(toRender);
            }
            
            InvokeAsync( StateHasChanged );
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("SetFocusToElement", OuterDiv);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Universe.Tic -= Render;
                    Universe.Players.Remove(MyBird);
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion




    }

}