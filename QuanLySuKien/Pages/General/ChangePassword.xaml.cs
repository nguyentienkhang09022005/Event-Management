using Demo1.Models;
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
using System.Windows.Shapes;

namespace Demo1.Pages.General
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QuanlysukienContext context = new QuanlysukienContext();
            var currentuser = context.Nguoidungs.Find(App.CurrentUserMand);
            if (txtOldPassword.Password != currentuser.Passworduser)
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (txtNewPassword.Password != txtAgainPassword.Password)
            {
                MessageBox.Show("Mật khẩu mới không khớp", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                currentuser.Passworduser = txtNewPassword.Password;
                context.SaveChanges();
                MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
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
