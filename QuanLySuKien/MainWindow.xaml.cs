using Demo1.Models;
using Demo1.Pages;
using Demo1.Pages.General;
using System.IO;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string HoTen { get; set; }
        public string Email { get; set; }
        public BitmapImage Avatar { get; set; }
        public DataTemplate SidebarTemplate { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            PagesNavigation.Navigate(new Uri("Pages/General/HomePage.xaml", UriKind.RelativeOrAbsolute));
            DataContext = this;
            if (App.CurrentUserRole == 0)
                GuestLoad();
            else
                UserLoad();

        }

        // Sự kiện load Thông tin người dùng
        private void GuestLoad()
        {
            HoTen = "Khách";
            Avatar = new BitmapImage(new Uri("pack://application:,,,/Images/avtdefault.jpg"));
        }
        private void UserLoad()
        {
            QuanlysukienContext db = new QuanlysukienContext();
            var currentUser = db.Nguoidungs.Find(App.CurrentUserMand);
            HoTen = currentUser.Hoten;
            Email = currentUser.Email;
            //Load avatar
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(currentUser.Imageuser))
            {
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            Avatar = bitmap;
            gridMenu.DataContext = this;
        }

        // Sự kiện phân quyền cho mỗi tài khoản
        public void ApplyRoleTemplate(int role)
        {
            if (role == 0)
            {
                SidebarTemplate = (DataTemplate)FindResource("Guest");
            }
            else if (role == 1)
            {
                SidebarTemplate = (DataTemplate)FindResource("Admin");
            }
            else if (role == 2)
            {
                SidebarTemplate = (DataTemplate)FindResource("User");
            }
            else
            {
                SidebarTemplate = (DataTemplate)FindResource("Dean");
            }
        }
        // Sự kiện đóng form
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Sự kiện phóng to form
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        // Sự kiện thu nhỏ form
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Sự kiện navigate đến các trang
        private void NavigateToPage(string pageUri)
        {
            PagesNavigation.Navigate(new System.Uri(pageUri, UriKind.RelativeOrAbsolute));
        }
        // Navigite đến Home Page trong General
        private void rdHome_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/General/HomePage.xaml");
        }
        // Navigate đến Calendar Page trong General
        private void rdCalendar_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/General/CalendarPage.xaml");
        }
        // Navigate đến HistoryCreatePage trong Dean
        private void rdHistoryCreate_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Dean/HistoryCreatePage.xaml");
        }
        // Navigate đến CreateEventPage trong Dean
        private void rdCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Dean/CreateEventPage.xaml");
        }
        // Navigate đến HistoryCreatePage trong User
        private void rdCalendarCheck_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/User/HistorySignupPage.xaml");
        }
        // Navigate đến ErrorPage trong Guest
        private void rdError_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Guest/ErrorPage.xaml");
        }
        // Navigate đến DsPheDuyet trong Admin
        private void rdPheDuyet_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Admin/DsPheDuyet.xaml");
        }
        // Navigate đến DsPheDuyet trong Admin
        private void rdMemberList_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Admin/MemberListPage.xaml");
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUserRole == 0)
                NavigateToPage("Pages/Guest/ErrorPage.xaml");
            else
                NavigateToPage("Pages/General/PersonalProfilePage.xaml");
        }
        private void rdLogout_Click(object sender, RoutedEventArgs e)
        {
                var signUpWindow = new Demo1.SignUp();
                signUpWindow.Show();
                // Đóng cửa sổ hiện tại
                this.Close();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Xử lý logic khi nội dung thanh tìm kiếm thay đổi
            string searchText = txtSearch.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                HomePage homePage = new HomePage();
                PagesNavigation.Navigate(homePage);
                return;
            }
            else
            {
                HomePage homePage = new HomePage(searchText);
                PagesNavigation.Navigate(homePage);
            }
        }
    }
}