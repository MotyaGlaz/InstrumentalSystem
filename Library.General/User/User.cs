using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.User
{
    public class User
    {
        public string Name { get; private set; }

        public string Picture { get; private set; }
        
        public User(string name)
        {
            Name = name;
        }

        public User(string name, string picture) : this(name)
        {
            Picture = picture;
        }
    }
}
