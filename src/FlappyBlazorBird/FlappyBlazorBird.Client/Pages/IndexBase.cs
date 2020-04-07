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
        public string totalSessions;
        public string startedAt;

        public int maxScore ;
    }

    public class IndexBase: ComponentBase, IDisposable
    {
        [Inject] protected Universe Universe {get; set; }      
        [Inject] protected IJSRuntime JSRuntime {get; set; }       

        protected void KeyDown(KeyboardEventArgs e)
        {
            if (e.Key == " " || e.Key == "p" || e.Key == "P" )
            {
                CheckIsRunning();                
                MyBird.KeyPressed.Enqueue(e);                
            }
        }
        protected void OnClick()
        {

            CheckIsRunning();
            var e = new KeyboardEventArgs();
            e.Key = MyBird.IsDead?"p":"ArrowUp";
            MyBird.KeyPressed.Enqueue(e);            
        }

        protected void CheckIsRunning()
        {
            if (MyBird.IsDead)
            {
                Universe.PleaseWeakUp();
                PleaseStopSent = false;
            }
        }

        protected ElementReference OuterDiv;
        protected Bird MyBird = null;

        protected override void OnInitialized()
        {
        }

        protected bool MyBirdIsSet = false;
        protected string birdname {set; get;} = "unnamed";
        protected async Task OnNickIsSet()
        {
            MyBird = new Bird(Universe);
            MyBird.Name = birdname;

            MyBirdIsSet = true;          
            Universe.PleaseWeakUp();
            Universe.Tic += Render;            
            PleaseStopSent = false;  
        }

        protected Statistics Statistics = new Statistics() {};
        protected bool PleaseStopSent = false;


        protected List<Printable> ToRender = new List<Printable>();

        private void Render(object sender, TicEventArgs e)
        {
            var toRender = new List<Printable>();

            // background
            var background = e.Universe.PrintableBackground;
            toRender.Add(background);

            // pipes
            var pipes = e.PrintablePipes;
            
            if (MyBird.IsDead || MyBird.CurrentGraceInterval > 0)
            {
                pipes = pipes.Select(p=>new Printable(p.X,p.Y,p.Image,Convert.ToInt32(p.R),0.4, p.GuidKey) ).ToList();
            }
            toRender.AddRange(pipes);
            

            // the base
            var theBase = e.Universe.TheBase;
            toRender.Add(theBase);

            // score
            toRender.AddRange( GetPrintableScore(MyBird.score) );

            // other players
            foreach(var bird in e.Players)
            {
                if (bird != MyBird)
                {
                    var otherBirdIndex = bird.IsDead?0:bird.playerIndex;
                    var otherBird = new Printable( bird.playerx, bird.playery,  bird.player_images[otherBirdIndex] , -bird.visibleRot, opacity: 0.5, guidKey: bird.GuidKey);
                    toRender.Add(otherBird);
                }
            }

            // myBird
            var myBirdIndex = MyBird.IsDead?0:MyBird.playerIndex;
            var ocell = new Printable( MyBird.playerx, MyBird.playery,  MyBird.player_images[myBirdIndex] , -MyBird.visibleRot, null, MyBird.GuidKey);
            toRender.Add(ocell);

            // play again
            if (MyBird.IsDead && MyBird.CurrentPenaltyTime==0)
            {
                var playAgain = e.Universe.PlayAgain;
                toRender.Add(playAgain);
                if (!PleaseStopSent) 
                {                  
                    PleaseStopSent = true;  
                    Universe.PleaseStop();
                }
            } else if (MyBird.IsDead && MyBird.CurrentPenaltyTime>0)
            {
                var gameOver = e.Universe.GameOver;
                toRender.Add(gameOver);
            } 

            Statistics.totalPlayers = e.Players.Count();
            Statistics.fps = Universe.CurrentFps;
            Statistics.totalSessions = Universe.TotalSessions.ToString();
            Statistics.startedAt = Universe.StartedAt;
            Statistics.maxScore = Universe.MaxScore;
            lock(ToRender) 
            {
                ToRender.Clear();
                ToRender.AddRange(toRender);
            }
            
            InvokeAsync( StateHasChanged );
            GoToSetFocus = true;
        }

        private bool GoToSetFocus = false;
        private bool GoToSetFocusAlreadySet = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (GoToSetFocus && !GoToSetFocusAlreadySet)
            {
                await JSRuntime.InvokeVoidAsync("SetFocusToElement", OuterDiv);
                GoToSetFocusAlreadySet = true;
            }
        }

        private int previousScore = -1;
        private List<Printable> previousPrintableScore = null;
        private List<Printable> GetPrintableScore(int score)
        {
            if (score == previousScore) return previousPrintableScore;
            previousScore=score;

            var result = new List<Printable>();
            var scoreDigits = score.ToString().ToCharArray().Select(x=>x-'0');
            var totalWidth = 0;
            foreach(var digit in scoreDigits) totalWidth += Universe.GetDigitWidth(digit);

            var Xoffset = (Universe.SCREENWIDTH - totalWidth) / 2;
            foreach(var digit in scoreDigits)
            {
                result.Add(
                    new Printable(Xoffset, Convert.ToInt32( Universe.SCREENHEIGHT * 0.1), Universe.IMAGESS["numbers"][digit],null,null,Guid.NewGuid())
                );
                Xoffset += Universe.GetDigitWidth(digit);
            }
            previousPrintableScore = result;
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