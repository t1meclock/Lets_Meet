using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet.Models
{
    public class User
    {
        private string SURNAME, NAME, PATRONYMIC, AGE, EMAIL, LOGIN, PASSWORD, HOBBY_NAME;
        private int HOBBY_ID, ROLE_ID;
        private bool IS_DELETED;
        
        public int UserID { get; set; }

        public string Surname
        {
            get { return SURNAME; }
            set { SURNAME = value; }
        }

        public string Name
        {
            get { return NAME; }
            set { NAME = value; }
        }

        public string Patronymic
        {
            get { return PATRONYMIC; }
            set { PATRONYMIC = value; }
        }

        public string Age
        {
            get { return AGE; }
            set { AGE = value; }
        }

        public string Email
        {
            get { return EMAIL; }
            set { EMAIL = value; }
        }

        public string Login
        {
            get { return LOGIN; }
            set { LOGIN = value; }
        }

        public string Password
        {
            get { return PASSWORD; }
            set { PASSWORD = value; }
        }

        public int HobbyID
        {
            get { return HOBBY_ID; }
            set { HOBBY_ID = value; }
        }

        public string HobbyName
        {
            get { return HOBBY_NAME; }
            set { HOBBY_NAME = value; }
        }

        public bool IsDeleted
        {
            get { return IS_DELETED; }
            set { IS_DELETED = value; }
        }

        public int RoleID
        {
            get { return ROLE_ID; }
            set { ROLE_ID = value; }
        }
    }
}