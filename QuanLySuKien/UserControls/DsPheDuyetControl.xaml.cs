using Demo1.Pages.Dean;
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
using Demo1.Models;
using System.Collections.ObjectModel;
using Demo1.Pages.Admin;
using Demo1.Pages.General;

namespace Demo1.Pages.Admin
{
    /// <summary>
    /// Interaction logic for MemberListControl.xaml
    /// </summary>
    public partial class DsPheDuyetControl : UserControl
    {
        public DsPheDuyetControl()
        {
            InitializeComponent();
            this.DataContext = this;
            
        }

        // Sự kiện phê duyệt sự kiện
        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var SelectedEvent = button.DataContext as DsPheDuyet.Event;
            if (SelectedEvent == null)
                return;
            ApproveEvent(SelectedEvent);
        }

        // Duyệt sự kiện
        public void ApproveEvent(DsPheDuyet.Event SelectedEvent)
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {
                    var CheckEvent = context.Sukiens.FirstOrDefault(e => e.Mask == SelectedEvent.IdEvent);
                    if (CheckEvent != null)
                    {
                        CheckEvent.Duyet = 1;
                        context.SaveChanges();

                        var eventToRemove = App.SharedData.DanhsachPheDuyet.FirstOrDefault(e => e.IdEvent == SelectedEvent.IdEvent);
                        if (eventToRemove != null)
                        {
                            App.SharedData.DanhsachPheDuyet.Remove(eventToRemove);  // Xoá sự kiện khỏi DanhsachPheDuyet đồng bộ
                        }
                        App.SharedData.CreatedEvents.Add(SelectedEvent);
                        MessageBox.Show("Sự kiện đã được phê duyệt", "Thông Báo", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Sự kiện không tồn tại trong sơ sở dữ liệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi phê duyệt: {ex.Message}");
            }
        }

        // Nút xem chi tiết sự kiện
        private void btnInfor_Click(object sender, EventArgs e)
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

        // Nút xoá sự kiện
        private void btnDelete_Click(object sender, EventArgs e)
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
                        context.Sukiens.Remove(CheckEvent);
                        context.SaveChanges();

                        var eventToRemove = App.SharedData.DanhsachPheDuyet.FirstOrDefault(e => e.IdEvent == SelectedEvent.IdEvent);
                        if (eventToRemove != null)
                        {
                            App.SharedData.DanhsachPheDuyet.Remove(eventToRemove);  // Xoá sự kiện khỏi DanhsachPheDuyet đồng bộ
                        }
                    }
                }
            }
        }
    }
}
