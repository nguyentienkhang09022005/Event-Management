    using Demo1.Models;
using Demo1.Pages;
using Demo1.Pages.SignUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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

namespace Demo1
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
            
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
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

        private void txtGuest_Click(object sender, RoutedEventArgs e) // Sự kiện là khách ghé qua
        {
            App.CurrentUserRole = 0;
            MainWindow mainwindow = new MainWindow();
            mainwindow.ApplyRoleTemplate(0);
            mainwindow.Show();
            this.Close();
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)// Sự kiện click button đăng ký
        {
            SignUpPage page = new SignUpPage();
            page.Show();
            this.Close();
        }

        private void btnSignIn_CLick(object sender, RoutedEventArgs e)// Sự kiện click button đăng nhập
        {
            // Kiểm tra nhập đầy đủ thông tin
            if (!AllowUserName())
            {
                return;
            }
            else
            {
                string Email = txtUsername.Text.Trim();
                string PassWord = txtPassword.Password.Trim();
                using (var context = new QuanlysukienContext())
                {
                    var NguoiDung = context.Nguoidungs.FirstOrDefault(u => u.Email == Email && u.Passworduser == PassWord); bool isUserFound = false; // Kiểm tra trạng thái đăng nhập
                    if (NguoiDung != null)
                    {
                        isUserFound = true;
                        App.CurrentUserMand = NguoiDung.Mand;// Lưu lại Mand để dùng cho việc khác
                        if (NguoiDung.Roleuser == 0)
                        {
                            App.CurrentUserRole = NguoiDung.Roleuser ?? -1;
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.ApplyRoleTemplate(NguoiDung.Roleuser.Value);
                            mainwindow.Show();
                            this.Close();
                        }
                        else if (NguoiDung.Roleuser == 1)
                        {
                            App.CurrentUserRole = NguoiDung.Roleuser ?? -1;
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.ApplyRoleTemplate(NguoiDung.Roleuser.Value);
                            mainwindow.Show();
                            this.Close();
                        }
                        else if (NguoiDung.Roleuser == 2)
                        {
                            App.CurrentUserRole = NguoiDung.Roleuser ?? -1;
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.ApplyRoleTemplate(NguoiDung.Roleuser.Value);
                            mainwindow.Show();
                            this.Close();
                        }
                        else if (NguoiDung.Roleuser == 3)
                        {
                            App.CurrentUserRole = NguoiDung.Roleuser ?? -1;
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.ApplyRoleTemplate(NguoiDung.Roleuser.Value);
                            mainwindow.Show();
                            this.Close();
                        }
                    }
                    // Không tìm thấy tài khoản nào khớp
                    else
                    {
                        MessageBox.Show("Email hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private bool AllowUserName()
        {
            if (txtUsername.Text.Trim().Length == 0)// Sự kiện chưa nhập tài khoản
            {
                MessageBox.Show("Bạn chưa nhập UserName", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUsername.Focus();
                return false; // Nếu chưa đăng nhập thông tin thì thoát
            }
            if (txtPassword.Password.Trim().Length == 0)// Sự kiện chưa nhập mật khẩu
            {
                MessageBox.Show("Bạn chưa nhập Password", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Focus();
                return false;// Nếu chưa đăng nhập thông tin thì thoát
            }
            return true;
        }

        // Sự kiện quên mật khẩu
        private void txtForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            string Email = txtUsername.Text.Trim();
            if (Email.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập Email!", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using (var context = new QuanlysukienContext())
                {
                    var Check = context.Nguoidungs.FirstOrDefault(u => u.Email == Email);
                    if (Check != null)
                    {
                        string PassWord = Check.Passworduser;
                        SendPasswordEmail(Email, PassWord);
                    }
                    else
                    {
                        MessageBox.Show("Email chưa được đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        // Gửi mật khẩu về mail của người dùng
        private void SendPasswordEmail(string toEmail, string password)
        {
            try
            {
                // Cấu hình SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("23520699@gm.uit.edu.vn", "cbzk sqrd gxtl lycv\r\n"),
                    EnableSsl = true
                };

                // Tạo email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("23520699@gm.uit.edu.vn"),
                    Subject = "Lấy lại mật khẩu",
                    Body = $"Mật khẩu của bạn là: {password}",
                    IsBodyHtml = false
                };
                // Thêm người nhận
                mail.To.Add(toEmail);
                // Gửi email
                smtp.Send(mail);
                MessageBox.Show("Mật khẩu đã được gửi đến email của bạn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi email: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

