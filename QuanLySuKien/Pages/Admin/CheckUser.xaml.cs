using Demo1.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Demo1.Pages.Admin
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
    /// <summary>
    /// Interaction logic for CheckUser.xaml
    /// </summary>
    public partial class CheckUser : Window
    {
        // Gán dữ liệu của User được chọn
        private bool isEditing = false;
        private Nguoidung CurrentUser;
        public CheckUser(Nguoidung selecteduser)
        {
            InitializeComponent();
            EnableEditing(false); // Mặc định tất cả các trường không chỉnh sửa
            CurrentUser = selecteduser;
            txtName.Text = CurrentUser.Hoten;
            txtMSSV.Text = CurrentUser.Masvgv;
            txtSDT.Text = CurrentUser.Sdt;
            txtEmail.Text = CurrentUser.Email;
            txtPassword.Text = CurrentUser.Passworduser;
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
        private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Chỉ cho phép thay đổi khi ở chế độ chỉnh sửa
            if (!isEditing)
            {
                e.Handled = true; // Chặn sự kiện nếu không ở chế độ chỉnh sửa
            }
        }

        // Bật/tắt chế độ chỉnh sửa thông tin người dùng
        private void EnableEditing(bool isEnabled)
        {
            txtName.IsReadOnly = !isEnabled;
            txtEmail.IsReadOnly = !isEnabled;
            txtSDT.IsReadOnly = !isEnabled;
            txtMSSV.IsReadOnly = !isEnabled;
            txtKhoa.IsEnabled = isEnabled;
            txtRole.IsEnabled = isEnabled;
            RadioBtnNam.IsEnabled = isEnabled;
            RadioBtnNu.IsEnabled = isEnabled;
            txtPassword.IsReadOnly = !isEnabled;
            txtAvtChange.IsEnabled = isEnabled; 
        }
        private void btn_Edit(object sender, RoutedEventArgs e)
        {
            isEditing = true;
            EnableEditing(true);

            // Ẩn nút chỉnh sửa khi bắt đầu chỉnh sửa
            btnChinhsua.Visibility = Visibility.Collapsed;

            // Hiện nút xác nhận
            btnXacnhan.Visibility = Visibility.Visible;
        }
        private void btn_Xacnhan(object sender, RoutedEventArgs e)
        {
            try
            {
                // Đặt lại các trường để không thể chỉnh sửa
                EnableEditing(false);
                isEditing = false;

                EnableEditing(false);
                isEditing = false;
                // Ẩn nút Xác nhận sau khi xác nhận xong
                btnXacnhan.Visibility = Visibility.Collapsed;

                // Hiện nút Chỉnh sửa để có thể chỉnh sửa lại
                btnChinhsua.Visibility = Visibility.Visible;

                // Chuyển đổi hình ảnh sang chuỗi binary
                ImageBrush imb = ImageEllipse.Fill as ImageBrush;
                BitmapImage bmi = imb?.ImageSource as BitmapImage;
                byte[] Avatar = CoversionAvatar(bmi);
                // Lưu thông tin vào cơ sở dữ liệu
                using (var context = new QuanlysukienContext())
                {
                   
                    var UserUpdate = context.Nguoidungs.FirstOrDefault(u => u.Mand == CurrentUser.Mand);
                    if (UserUpdate != null)
                    {
                        UserUpdate.Hoten = txtName.Text;
                        UserUpdate.Masvgv = txtMSSV.Text;
                        UserUpdate.Sdt = txtSDT.Text;
                        UserUpdate.Email = txtEmail.Text;
                        UserUpdate.Passworduser = txtPassword.Text;
                        UserUpdate.Gioitinh = RadioBtnNam.IsChecked == true ? "Nam" : "Nữ";
                        UserUpdate.Imageuser = Avatar;
                        UserUpdate.Roleuser = txtRole.Text switch // Đưa vai trò mới của người dùng vào database
                        {
                            "Admin" => 1,
                            "Sinh Viên" => 2,
                            "Người Đăng Bài" => 3,
                            _ => throw new ArgumentException("Giá trị Role không hợp lệ.")
                        };
                        UserUpdate.Makhoa = txtKhoa.Text switch // Đưa khoa mới của người dùng vào database
                        {
                            "Công Nghệ Phần Mềm" => "KH0001",
                            "Hệ Thống Thông Tin" => "KH0002",
                            "Khoa Học Máy Tính" => "KH0003",
                            "Kỹ Thuật Máy Tính" => "KH0004",
                            "Mạng Máy Tính Và Truyền Thông" => "KH0005",
                            "Khoa Học Và Kỹ Thuật Thông Tin" => "KH0006",
                            _ => null
                        };
                        context.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
                        CurrentUser = UserUpdate;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        // Sự kiện up ảnh mới lên Image
        private void txtAvtChange_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog // Tạo đối tượng lưu ảnh
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp" // Giới hạn file
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bmi = new BitmapImage(new Uri(openFileDialog.FileName)); // Lấy URL cùa file ảnh gán vào bmi
                    ImageEllipse.Fill = new ImageBrush(bmi); // Đưa ảnh lên Ellipse
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải ảnh!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Chuyển đổi hình ảnh sang chuỗi binary
        private byte[] CoversionAvatar(BitmapImage bmi)
        {
            if (bmi == null)
                return null;
            else
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder(); // chuyển đổi ảnh qua kiểu PNG
                    encoder.Frames.Add(BitmapFrame.Create(bmi));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
