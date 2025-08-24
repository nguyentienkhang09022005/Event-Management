using Demo1.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Demo1.Pages.Admin
{
    public partial class MemberListPage : Page, INotifyPropertyChanged
    {
        public event Action<string> SearchTextChanged; // Lưu text tìm kiếm
        public MemberListPage()
        {
            InitializeComponent();
            PagesNavigation.Navigate(new Uri("Pages/Admin/StudentListPage.xaml", UriKind.RelativeOrAbsolute));
            this.DataContext = AppState.Instance;
        }

        // Đảm bảo RowCount có thể được binding
        private int _rowCount;
        public int RowCount
        {
            get { return _rowCount; }
            set
            {
                if (_rowCount != value)
                {
                    _rowCount = value;
                    OnPropertyChanged(nameof(RowCount));  // Thông báo UI cập nhật
                }
            }
        }

        // Sự kiện PropertyChanged được gọi khi thuộc tính thay đổi
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NavigateToPage(string pageUri)
        {
            PagesNavigation.Navigate(new System.Uri(pageUri, UriKind.RelativeOrAbsolute));
        }

        private void Student_Click(object sender, RoutedEventArgs e)
        {
            var studentPage = new StudentListPage();
            studentPage.CurrentDataGrid.Loaded += (s, args) => UpdateRowCount(studentPage.CurrentDataGrid);
            NavigateToPage("/Pages/Admin/StudentListPage.xaml");
        }

        private void Posters_Click(object sender, RoutedEventArgs e)
        {
            var postersPage = new PostersListPage();
            postersPage.CurrentDataGrid.Loaded += (s, args) => UpdateRowCount(postersPage.CurrentDataGrid);
            NavigateToPage("/Pages/Admin/PostersListPage.xaml");
        }

        // Sự kiện đếm số lượng bên trong datagrid
        private void UpdateRowCount(DataGrid dataGrid)
        {
            RowCount = dataGrid.Items.Count;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo một instance của cửa sổ AddUser
            AddUser addUserWindow = new AddUser();

            // Hiển thị cửa sổ
            addUserWindow.ShowDialog(); // Sử dụng ShowDialog() nếu bạn muốn cửa sổ mới là modal.
        }

        // Sự kiện tìm kiếm Dean hoặc Student
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                AppState.Instance.FilterText = textBox.Text; // Cập nhật FilterText
            }
        }
    }
}