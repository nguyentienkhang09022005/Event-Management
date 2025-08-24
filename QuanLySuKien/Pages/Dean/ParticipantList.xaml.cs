using Demo1.Models;
using Demo1.Pages.Admin;
using Demo1.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Primitives;
using static Demo1.Pages.Admin.StudentListPage;

namespace Demo1.Pages.Dean
{
    /// <summary>
    /// Interaction logic for ParticipantList.xaml
    /// </summary>
    ///
    
    public partial class ParticipantList : Window
    {
        private string TargetEvent;
        public ObservableCollection<Member> MembersList { get; set; }

        public ParticipantList(string IdEvent)
        {
            InitializeComponent();
            this.DataContext = AppState.Instance; // Liên kết DataContext với AppState
            LoadMembers(IdEvent);
            TargetEvent = IdEvent;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)// Sự kiện đóng form 
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) // Sự kiện thu nhỏ form
        {
            WindowState = WindowState.Minimized;
        }

        // Load người đăng ký sk lên trên danh sách 
        private void LoadMembers(string TargetMask)
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {

                    // Kết 2 bảng người dùng và đăng ký sự kiện lại để lấy thông tin người dùng
                    var Member = context.Nguoidungs
                        .Join( 
                        context.Dangkysukiens, nguoidung => nguoidung.Mand,
                        dksk => dksk.Mand,
                        (nguoidung, dksk) => new
                        {
                            nguoidung.Mand,
                            nguoidung.Hoten,
                            nguoidung.Sdt,
                            nguoidung.Email,
                            nguoidung.Makhoa,
                            dksk.Thoigiandangky,
                            dksk.Mask,
                            dksk.Xacnhanthamgia
                        }
                     )
                        .Join(
                        context.Khoas, nguoidungdksk => nguoidungdksk.Makhoa,
                        khoa => khoa.Makhoa,
                        (nguoidungdksk, khoa) => new
                        {
                            nguoidungdksk.Mand,
                            nguoidungdksk.Hoten,
                            nguoidungdksk.Sdt,
                            nguoidungdksk.Email,
                            nguoidungdksk.Mask,
                            nguoidungdksk.Thoigiandangky,
                            nguoidungdksk.Xacnhanthamgia,
                            khoa.Tenkhoa,
                        }
                     )
                     .Where(result => result.Mask.Trim().ToLower() == TargetMask.Trim().ToLower())
                     .Select (Result => new Member
                     {
                         Id = Result.Mand,
                         Name = Result.Hoten,
                         PhoneNumber = Result.Sdt,
                         Email = Result.Email,
                         Khoa = Result.Tenkhoa,
                         Check = Result.Xacnhanthamgia == "Có"
                     }).ToList();
                    MembersList = new ObservableCollection<Member>(Member);
                    CurrentDataGrid.ItemsSource = null; // Reset nguồn dữ liệu
                    CurrentDataGrid.ItemsSource = MembersList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}");
            }
            // Cập nhật số lượng dòng vào AppState
            AppState.Instance.RowCount = MembersList.Count;
        }


        // Nút hiện thông tin người tham gia chương trình
        private void btninfo_Click(object sender, RoutedEventArgs e)
        {
            var SelectedMember = (Member)CurrentDataGrid.SelectedItem;
            if (SelectedMember != null)
            {
                // Truy vấn lại Nguoidung từ cơ sở dữ liệu dựa trên Email hoặc thông tin khác của Member
                Nguoidung selectedUser = null;
                using (var context = new QuanlysukienContext())
                {
                    selectedUser = context.Nguoidungs.FirstOrDefault(u => u.Mand == SelectedMember.Id);
                }
                if (selectedUser != null)
                {
                    // Tạo cửa sổ CheckUser với đối tượng Nguoidung
                    InfoUser checkUserWindow = new InfoUser(selectedUser);
                    checkUserWindow.Show();
                }
            }
        }

        // Nút xoá sinh viên khỏi datagrid
        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            var SelectedItem = CurrentDataGrid.SelectedItem as Member;
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xoá!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            var Result = MessageBox.Show("Bạn có muốn xoá người đăng ký này?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                // Xoá dữ liệu sinh viên trong cơ sở dữ liệu
                try
                {
                    using (var context = new QuanlysukienContext())
                    {

                        var DeleteUser = context.Dangkysukiens.FirstOrDefault(u => u.Mand == SelectedItem.Id);
                        if (DeleteUser != null)
                        {
                            context.Dangkysukiens.Remove(DeleteUser);
                            context.SaveChanges();
                            // Xoá dữ liệu sinh viên trong datagrid
                            MembersList.Remove(SelectedItem);

                            MessageBox.Show("Đã xoá người đăng ký thành công");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
      
        // Sự kiện nhất vào checkbox
        private void cbParticipant_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin checkbox được chọn
            var checkbox = sender as CheckBox;
            if (checkbox == null)
                return;

            var SelectedMember = checkbox.DataContext as Member;
            if (SelectedMember != null)
            {
                try
                {
                    using (var context = new QuanlysukienContext())
                    {

                        var UserChoice = context.Dangkysukiens.FirstOrDefault(u => u.Mand == SelectedMember.Id && u.Mask == TargetEvent);
                        if (UserChoice != null)
                        {
                            if (checkbox.IsChecked == true) // Nếu checkbox được tick thì cập nhật là có tham gia
                            {
                                UserChoice.Xacnhanthamgia = "Có";
                                context.SaveChanges();
                                MessageBox.Show("Người dùng đã được ghi nhận tham gia", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public class Member
        {
            public string Id { get; set; } 
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Khoa { get; set; }
            public bool Check { get; set; }
        }
    }
}
