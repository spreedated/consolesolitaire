using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSolitaire.EventArgs
{
    internal class ResourcePreloadedEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public ResourcePreloadedEventArgs(string name) : base()
        {
            this.Name = name;
        }
    }
}
