using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace FlappyBlazorBird.Client.Helpers
{
    public class Cycle<T>:IEnumerable<T>
    {
        private readonly T[] Data;

        public Cycle(IEnumerable<T> data)
        {
            Data = data.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            while(true)
            {
                foreach(var x in Data)
                {
                    yield return x;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}