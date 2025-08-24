using Demo1.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Demo1.Pages.Dean
{
    /// <summary>
    /// Interaction logic for InfoUser.xaml
    /// </summary>
    public partial class InfoUser : Window
    {
        public class RoleItem
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }
        public class FacultyItem
        {
            public string IdFaculty { get; set; }
            public string NameFaculty { get; set; }
        }
        private Nguoidung CurrentUser;
        public InfoUser(Nguoidung selecteduser)
        {

            InitializeComponent();
            CurrentUser = selecteduser;
            txtName.Text = CurrentUser.Hoten;
            txtMSSV.Text = CurrentUser.Masvgv;
            txtSDT.Text = CurrentUser.Sdt;
            if (CurrentUser.Gioitinh == "Nam")
            {
                RadioBtnNam.IsChecked = true;
            }
            else
            {
                RadioBtnNu.IsChecked = true;
            }
            // Hiển thị danh sách khoa kèm id mỗi khoa
            var Faculty = new List<FacultyItem>
            {
                new FacultyItem { IdFaculty = "KH0001", NameFaculty = "Công Nghệ Phần Mềm" },
                new FacultyItem { IdFaculty = "KH0002", NameFaculty = "Hệ Thống Thông Tin" },
                new FacultyItem { IdFaculty = "KH0003", NameFaculty = "Khoa Học Máy Tính" },
                new FacultyItem { IdFaculty = "KH0004", NameFaculty = "Kỹ Thuật Máy Tính" },
                new FacultyItem { IdFaculty = "KH0005", NameFaculty = "Mạng Máy Tính Và Truyền Thông" },
                new FacultyItem { IdFaculty = "KH0006", NameFaculty = "Khoa Học Và Kỹ Thuật Thông Tin" },
            };
            txtKhoa.ItemsSource = Faculty;
            txtKhoa.DisplayMemberPath = "NameFaculty"; // Hiển thị tên khoa
            txtKhoa.SelectedValuePath = "IdFaculty";  // Lấy ID khoa
            var currentFaculty = Faculty.FirstOrDefault(f => f.IdFaculty == CurrentUser.Makhoa); // so sánh với mã khoa hiện tại của người dùng để hiển thị
            if (currentFaculty != null)
            {
                txtKhoa.SelectedItem = currentFaculty;
            }
            // Hiển thị danh sách quyền kèm id của mỗi quyền
            var roles = new List<RoleItem>
            {
                new RoleItem { RoleId = 1, RoleName = "Admin" },
                new RoleItem { RoleId = 2, RoleName = "Sinh Viên" },
                new RoleItem { RoleId = 3, RoleName = "Người Đăng Bài" },
            };
            txtRole.ItemsSource = roles;
            txtRole.DisplayMemberPath = "RoleName"; // Hiển thị tên vai trò
            txtRole.SelectedValuePath = "RoleId";  // Lấy ID vai trò
            var currentRole = roles.FirstOrDefault(r => r.RoleId == CurrentUser.Roleuser); // so sánh với id vai trò của người dùng hiện tại để hiện tên vai trò
            if (currentRole != null)
            {
                txtRole.SelectedItem = currentRole;
            }

            // Hiển thị avt của user lên ellipes
            if (CurrentUser.Imageuser != null)
            {
                BitmapImage bitmap = new BitmapImage();// còn bug
                using (MemoryStream stream = new MemoryStream(CurrentUser.Imageuser))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                ImageBrush brush = new ImageBrush()
                {
                    ImageSource = bitmap
                };
                ImageEllipse.Fill = brush;
            }
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
    }
}
