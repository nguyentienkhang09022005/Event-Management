using Demo1.Pages.Guest;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

namespace Demo1.Pages.General
{
    /// <summary>
    /// Interaction logic for FeaturedEventsControl.xaml
    /// </summary>
    public partial class FeaturedEventsControl : UserControl
    {
        public FeaturedEventsControl()
        {
            InitializeComponent();
        }
        private void EventDetails_Click(object sender, MouseButtonEventArgs e)
        {
            // Lấy item đang được click
            if (App.CurrentUserRole == 0)
            {
                var errorPage = new ErrorPage();
                NavigationService.GetNavigationService(this).Navigate(errorPage);
            }
            else
            {
                var homePage = new HomePage();
                var textBlock = sender as TextBlock;
                var dataContext = textBlock?.DataContext;

                if (dataContext != null)
                {
                    // Xử lý logic dựa trên item
                    var eventItem = dataContext as HomePage.Event;
                    EventDetailsPage page = new EventDetailsPage(eventItem.Mask);
                    NavigationService.GetNavigationService(this).Navigate(page);
                }
            }

        }
    }
}
