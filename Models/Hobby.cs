using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet.Models
{
    public class Hobby
    {
        private string HOBBY_NAME;
        
        public int HobbyID { get; set; }

        public string HobbyName
        {
            get { return HOBBY_NAME; }
            set { HOBBY_NAME = value; }
        }
    }
}
