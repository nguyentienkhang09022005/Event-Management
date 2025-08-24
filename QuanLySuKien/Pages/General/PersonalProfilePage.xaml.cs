using Demo1.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo1.Pages.General
{
    /// <summary>
    /// Interaction logic for PersonalProfilePage.xaml
    /// </summary>
    public partial class PersonalProfilePage : Page
    {
        private bool isEditing = false;
        public class Profile
        {
            public string Name { get; set; }
            public string MSSV { get; set; }
            public string Khoa { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public ImageBrush Avatar { get; set; }
        }
            public PersonalProfilePage()
        {
            InitializeComponent();
            btnXacnhan.Visibility = Visibility.Collapsed;
            LoadUserData();
        }
        private void LoadUserData()
        {
            using var context = new QuanlysukienContext();
            Nguoidung currentuser = context.Nguoidungs.Find(App.CurrentUserMand);
            if (currentuser.Gioitinh == "Nam")
                RadioBtnNam.IsChecked = true;
            else
                RadioBtnNu.IsChecked = true;
            ImageBrush imageBrush = ConvertByteArrayToImageBrush(currentuser.Imageuser);
            var Profile = new Profile
            {
                Name = currentuser.Hoten,
                MSSV = currentuser.Masvgv,
                Khoa = (from khoa in context.Khoas
                        where khoa.Makhoa == currentuser.Makhoa
                        select khoa.Tenkhoa).FirstOrDefault(),
                SDT = currentuser.Sdt,
                Email = currentuser.Email,
                Role = currentuser.Roleuser == 1 ? "Admin" : currentuser.Roleuser == 2 ? "Sinh Viên" : "Người Đăng Bài",
                Avatar = imageBrush
            };  
            this.DataContext = Profile;
        }
        public ImageBrush ConvertByteArrayToImageBrush(byte[] imageData)
        {
            // Tạo một MemoryStream từ mảng byte
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                // Tạo một BitmapImage từ MemoryStream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Đảm bảo tải hình ảnh vào bộ nhớ
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                // Tạo ImageBrush từ BitmapImage
                ImageBrush imageBrush = new ImageBrush(bitmapImage);
                return imageBrush;
            }
        }
            private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Chỉ cho phép thay đổi khi ở chế độ chỉnh sửa
            if (!isEditing)
            {
                e.Handled = true; // Chặn sự kiện nếu không ở chế độ chỉnh sửa
            }
        }
        private void btn_Edit(object sender, RoutedEventArgs e)
        {
            isEditing = true;
            txtName.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            txtSDT.IsReadOnly = false;
            txtMSSV.IsReadOnly = false;
            txtKhoa.IsReadOnly = false;
            txtKhoa.IsEnabled = true;
            RadioBtnNam.IsEnabled = true;
            RadioBtnNu.IsEnabled = true;
            txtAvtChange.IsEnabled = true;

            // Ẩn nút chỉnh sửa khi bắt đầu chỉnh sửa
            btnChinhsua.Visibility = Visibility.Collapsed;

            // Hiện nút xác nhận
            btnXacnhan.Visibility = Visibility.Visible;
        }
        private void btn_Xacnhan(object sender, RoutedEventArgs e)
        {

            if(AllowUser() == false)
            {
                return;
            }
            // Chuyển đổi hình ảnh sang chuỗi binary
            ImageBrush imb = ImageEllipse.Fill as ImageBrush;
            BitmapImage bmi = imb?.ImageSource as BitmapImage;
            byte[] Avatar = CoversionAvatar(bmi);
            // Đặt lại các trường để không thể chỉnh sửa
            txtName.IsReadOnly = true;
            txtSDT.IsReadOnly = true;
            txtMSSV.IsReadOnly = true;
            txtKhoa.IsEnabled = false;
            RadioBtnNam.IsEnabled = false;
            RadioBtnNu.IsEnabled = false;
            txtAvtChange.IsEnabled = false;
            // Ẩn nút Xác nhận sau khi xác nhận xong
            btnXacnhan.Visibility = Visibility.Collapsed;

            // Hiện nút Chỉnh sửa để có thể chỉnh sửa lại
            btnChinhsua.Visibility = Visibility.Visible;

            // Lưu thông tin vào cơ sở dữ liệu
            Nguoidung currentuser = null;
            using var context = new QuanlysukienContext();
            currentuser = context.Nguoidungs.Find(App.CurrentUserMand);
            currentuser.Hoten = txtName.Text;
            currentuser.Email = txtEmail.Text;
            currentuser.Sdt = txtSDT.Text;
            currentuser.Masvgv = txtMSSV.Text;
            currentuser.Imageuser = Avatar;
            currentuser.Makhoa = (from khoa in context.Khoas
                                  where khoa.Tenkhoa == txtKhoa.Text
                                  select khoa.Makhoa).FirstOrDefault();
            if (RadioBtnNam.IsChecked == true)
                currentuser.Gioitinh = "Nam";
            else
                currentuser.Gioitinh = "Nữ";
            context.SaveChanges();
        }

        public bool AllowUser()
        {
            if (txtName.Text.Length == 0 || txtSDT.Text.Length == 0 || txtMSSV.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (txtName.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Tên không được chứa số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (txtSDT.Text.Length != 10 || !txtSDT.Text.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changePassword = new ChangePassword();
            changePassword.ShowDialog();
        }

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
