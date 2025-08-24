using Demo1.Pages.Admin;
using Demo1.Pages.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo1.Pages.User
{
    /// <summary>
    /// Interaction logic for HistoryEventSignup.xaml
    /// </summary>
    public partial class SignupEventsControl : UserControl
    {
        public SignupEventsControl()
        {
            InitializeComponent();
        }
        // Button xem thông tin chi tiết sự kiện
        private void btnInf_Click (object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var SelectedEvent = button.DataContext as DsPheDuyet.Event;
            if (SelectedEvent == null)
                return;
            EventDetailsPage page = new EventDetailsPage(SelectedEvent.IdEvent);
            var navigationService = NavigationService.GetNavigationService(this);
            navigationService.Navigate(page);
        }
    }
}
