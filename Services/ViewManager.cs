using Lets_Meet.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lets_Meet.Services
{
    interface IViewService
    {
        void ShowView (int viewID, int ID, int role);
    }

    public class ViewManager : IViewService
    {
        public void ShowView (int viewID, int ID, int role)
        {
            switch (viewID)
            {
                case 0:
                    AuthRegView authRegView = new AuthRegView ();
                    authRegView.Show ();
                    break;

                case 1:
                    AdministratorView administratorView = new AdministratorView (ID, role);
                    administratorView.Show ();
                    break;

                case 2:
                    MainView window = new MainView (ID, role);
                    window.Show ();
                    break;

                case 3:
                    UserProfileView userProfileView = new UserProfileView (ID);
                    userProfileView.Show ();
                    break;

                default:
                    MessageBox.Show ("View не существует. Некорректный ViewID.");
                    break;
            }
        }
    }
}
