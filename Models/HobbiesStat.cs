using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet.Models
{
    public class HobbiesStat
    {
        private int HOBBY_ID, USER_ID;

        public int IdUserHobby { get; set; }

        public int HobbyID
        {
            get { return HOBBY_ID; }
            set { HOBBY_ID = value; }
        }

        public int UserID
        {
            get { return USER_ID; }
            set { USER_ID = value; }
        }
    }
}
