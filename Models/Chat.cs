using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet.Models
{
    public class Chat
    {
        private int ID_SENDER, ID_RECIEVER;
        private string CHATNAME;
        private bool IS_DELETED;

        public int ChatID { get; set; }

        public int IDSender
        {
            get { return ID_SENDER; }
            set { ID_SENDER = value; }
        }
        
        public int IDReciever
        {
            get { return ID_RECIEVER; }
            set { ID_RECIEVER = value; }
        }

        public string ChatName
        {
            get { return CHATNAME; }
            set { CHATNAME = value; }
        }

        public bool IsDeleted
        {
            get { return IS_DELETED; }
            set { IS_DELETED = value; }
        }
    }
}
