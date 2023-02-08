using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lets_Meet
{
    //
    // Шифрование пароля
    //
    class CryptPass
    {
        public string Crypt (string password)
        {
            string cryptPass = String.Empty;

            foreach (char c in password)
            {
                int cp = Convert.ToInt32 (c);
                cp++;
                cryptPass += Convert.ToChar (cp);
            }

            return cryptPass;
        }
    }
}
