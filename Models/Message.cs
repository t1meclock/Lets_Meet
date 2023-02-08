using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet.Models
{
    public class Message
    {
        private int ID_SENDER, ID_RECIEVER, ID_CHAT;
        private string TEXT_MESSAGE;

        public int MessageID { get; set; }

        public int IdSender
        {
            get { return ID_SENDER; }
            set { ID_SENDER = value; }
        }

        public int IdReciever
        {
            get { return ID_RECIEVER; }
            set { ID_RECIEVER = value; }
        }

        public int IdChat
        {
            get { return ID_CHAT; }
            set { ID_CHAT = value; }
        }

        public string TextMessage
        {
            get { return TEXT_MESSAGE; }
            set { TEXT_MESSAGE = value; }
        }
    }
}
