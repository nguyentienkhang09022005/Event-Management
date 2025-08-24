using Demo1.Models;
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

namespace Demo1.Pages.Dean
{
    /// <summary>
    /// Interaction logic for HistoryCreatePage.xaml
    /// </summary>
    public partial class HistoryCreateControl : UserControl
    {
        public HistoryCreateControl()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        // Nút xem thông tin chi tiết sự kiện
        private void btnInf_Click(object sender, RoutedEventArgs e)
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
        
        // Nút thay đổi thông tin sự kiện
        private void btnChangeInf_Click(Object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var SelectedEvent = button.DataContext as DsPheDuyet.Event;
            if (SelectedEvent == null)
                return;
            CheckEvent page = new CheckEvent(SelectedEvent.IdEvent);
            var navigationService = NavigationService.GetNavigationService(this);
            navigationService.Navigate(page);
        }

        // Nút xoá sự kiện
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var SelectedEvent = button.DataContext as DsPheDuyet.Event;
            if (SelectedEvent == null)
                return;
            var Check = MessageBox.Show("Bạn muốn xoá sự kiện?", "Thông Báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Check == MessageBoxResult.Yes)
            {
                using (var context = new QuanlysukienContext())
                {
                    var CheckEvent = context.Sukiens.FirstOrDefault(e => e.Mask == SelectedEvent.IdEvent);
                    if (CheckEvent != null)
                    {
                        // Xoá danh sách người đăng ký sự kiện
                        var RegisterList = context.Dangkysukiens.Where(e => e.Mask == SelectedEvent.IdEvent).ToList();
                        {
                            context.Dangkysukiens.RemoveRange(RegisterList);
                        }
                        context.Sukiens.Remove(CheckEvent);
                        context.SaveChanges();

                        var eventToRemove = App.SharedData.CreatedEvents.FirstOrDefault(e => e.IdEvent == SelectedEvent.IdEvent);
                        if (eventToRemove != null)
                        {
                            App.SharedData.CreatedEvents.Remove(eventToRemove);  // Xoá sự kiện khỏi DanhsachPheDuyet đồng bộ
                        }
                        MessageBox.Show("Sự kiện đã được xoá", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}
