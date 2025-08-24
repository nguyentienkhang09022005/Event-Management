using Demo1.Models;
using Microsoft.VisualBasic;
using Microsoft.Win32;
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
namespace Demo1.Pages.SignUp
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Window
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        // Đóng form
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Mở rộng form
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        // Thu nhỏ form
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Kiểm tra nhập đầy đủ thông tin
        private bool AllowUserName()
        {
            // Sự kiện chill fill đủ các ô 
            if (txtName.Text.Trim().Length == 0 || txtMSSV.Text.Trim().Length == 0 || txtSDT.Text.Trim().Length == 0 || txtGmail.Text.Trim().Length == 0 || txtPassWord.Password.Trim().Length == 0 || 
                txtAgainPassWord.Password.Trim().Length == 0 || RadioBtnNam.IsChecked == false && RadioBtnNu.IsChecked == false || txtKhoa.Text.Trim().Length == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                if (txtPassWord.Password.Trim() != txtAgainPassWord.Password.Trim())
                {
                    MessageBox.Show("Hai mật khẩu không khớp. Vui lòng nhập lại!", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }
        // Tạo mã người dùng tăng dần
        private string CreateMaND(QuanlysukienContext context)
        {
            // Lấy mã cao nhất từ cơ sở dữ liệu
            var maxMaND = context.Nguoidungs
                                 .OrderByDescending(nd => nd.Mand)
                                 .Select(nd => nd.Mand)
                                 .FirstOrDefault();

            // Tạo mã mới dựa trên giá trị lớn nhất
            if (maxMaND != null && maxMaND.StartsWith("ND"))
            {
                int maxNumber = int.Parse(maxMaND.Substring(2)); // Bỏ "ND" và chuyển phần số
                return $"ND{(maxNumber + 1).ToString("D3")}";    // Tăng giá trị và định dạng
            }
            return "ND001";
        }
        // Thêm người dùng mới vào csdl
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nhập đầy đủ thông tin
            if (!AllowUserName())
            {
                return;
            }
            string MaSVGV = txtMSSV.Text.Trim();
            string HoTen = txtName.Text.Trim();
            string SDT = txtSDT.Text.Trim();
            string Gmail = txtGmail.Text.Trim();
            string PassWord = txtPassWord.Password.Trim();
            string AgainPassWord = txtAgainPassWord.Password.Trim();
            string Sex;
            if (RadioBtnNam.IsChecked == true)
            {
                Sex = RadioBtnNam.Content.ToString();
            }
            else
            {
                Sex = RadioBtnNu.Content.ToString();

            }
            string Khoa = txtKhoa.Text switch
            {
                "Công Nghệ Phần Mềm" => "KH0001",
                "Hệ Thống Thông Tin" => "KH0002",
                "Khoa Học Máy Tính" => "KH0003",
                "Kỹ Thuật Máy Tính" => "KH0004",
                "Mạng Máy Tính Và Truyền Thông" => "KH0005",
                "Khoa Học Và Kỹ Thuật Thông Tin" => "KH0006",
            };

            // Lấy hình ảnh từ Ellipe
            ImageBrush imb = ImageEllipse.Fill as ImageBrush;
            BitmapImage bmi = imb?.ImageSource as BitmapImage;
            byte[] Avatar = CoversionAvatar(bmi);
            using (var context = new QuanlysukienContext())
            {
                // Kiểm tra kích thước họ tên > 50
                if (HoTen.Length > 50)
                {
                    MessageBox.Show("Tên không vượt quá 50 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtName.CaretIndex = txtName.Text.Length;

                }
                else if (MaSVGV.Length > 10) // Kiểm tra MSSVGV > 10
                {
                    MessageBox.Show("MSSV/MSGV không vượt quá 10 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtMSSV.CaretIndex = txtMSSV.Text.Length;
                }
                else if (!SDT.All(char.IsDigit) || SDT.Length < 9 || SDT.Length > 11) // Kiểm tra SĐT không có ký tự chữ và kích thước từ 9 đến 11 số
                {
                    MessageBox.Show("Số điện thoại phải từ 9 đến 11 số và phải là số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtSDT.CaretIndex = txtSDT.Text.Length;
                }
                else if (Avatar == null)
                {
                    MessageBox.Show("Vui lòng chọn ảnh đại diện!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var existingUser = context.Nguoidungs.FirstOrDefault(u => u.Email == Gmail);
                    if (existingUser != null)// Kiểm tra Email đã tồn tại trong csdl
                    {
                        MessageBox.Show("Email hoặc Mật Khẩu đã tồn tại. Vui lòng nhập lại!", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string MaND = CreateMaND(context);
                        Nguoidung NewUser = new Nguoidung()
                        {
                            Mand = MaND,
                            Hoten = HoTen,
                            Gioitinh = Sex,
                            Makhoa = Khoa,
                            Passworduser = PassWord,
                            Email = Gmail,
                            Sdt = SDT,
                            Masvgv = MaSVGV,
                            Roleuser = 2,
                            Imageuser = Avatar

                        };
                        context.Nguoidungs.Add(NewUser);
                        context.SaveChanges();
                        MessageBox.Show("Đã đăng ký thành công!", "Thông Báo", MessageBoxButton.OK);
                        var signUpWindow = new Demo1.SignUp();
                        signUpWindow.Show();
                        this.Close();
                    }
                }
            }
        }

        // Sự kiện thay đổi ảnh đại diện
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

        // Sự kiện quay trở lại trang đăng nhập
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo và hiển thị cửa sổ mới
            var signUpWindow = new Demo1.SignUp();
            signUpWindow.Show();

            // Đóng cửa sổ hiện tại
            this.Close();
        }
    }
}
