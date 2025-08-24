using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Demo1.Models;
using Demo1.ViewModels;

namespace Demo1.Pages.Admin
{
    public partial class StudentListPage : Page
    {
        private ICollectionView _studentCollectionView;
        public ObservableCollection<Member> MembersList { get; set; }

        public StudentListPage()
        {
            InitializeComponent();
            this.DataContext = AppState.Instance; // Liên kết DataContext với AppState
            LoadMembers();
            _studentCollectionView = CollectionViewSource.GetDefaultView(MembersList);
            _studentCollectionView.Filter = FilterStudents;

            // Lắng nghe sự thay đổi của FilterText
            AppState.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppState.FilterText))
                {
                    _studentCollectionView.Refresh();
                }
            };
        }

        // Hàm tìm kiếm student
        private bool FilterStudents(object item)
        {
            if (item is Member member)
            {
                return string.IsNullOrEmpty(AppState.Instance.FilterText) ||
                       member.Name.Contains(AppState.Instance.FilterText, StringComparison.OrdinalIgnoreCase) ||
                       member.Email.Contains(AppState.Instance.FilterText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private void LoadMembers()
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {
                    var Member = context.Nguoidungs
                        .Join(
                        context.Khoas, nguoidung => nguoidung.Makhoa,
                        khoa => khoa.Makhoa,
                        (nguoidung, khoa) => new
                        {
                            nguoidung.Mand,
                            nguoidung.Hoten,
                            nguoidung.Sdt,
                            nguoidung.Email,
                            nguoidung.Makhoa,
                            nguoidung.Roleuser,
                            khoa.Tenkhoa
                        }
                     )
                        .Where(member => member.Roleuser != '3' && member.Roleuser != '1')
                        .Select(member =>
                        new Member
                        {
                            Id = member.Mand,
                            Name = member.Hoten,
                            PhoneNumber = member.Sdt,
                            Email = member.Email,
                            Khoa = member.Tenkhoa

                        }).ToList();
                    MembersList = new ObservableCollection<Member>(Member);
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

        public class Member
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Khoa { get; set; }
        }

        // Sự kiện thay đổi thông tin cá nhân của người dùng
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
                    CheckUser checkUserWindow = new CheckUser(selectedUser);
                    checkUserWindow.Show();
                }
            }
        }

        // Sự kiện xoá sinh viên khỏi datagrid
        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            var SelectedItem = CurrentDataGrid.SelectedItem as Member;
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xoá!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            var Result = MessageBox.Show("Bạn có muốn xoá sinh viên này?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                // Xoá dữ liệu sinh viên trong cơ sở dữ liệu
                try
                {
                    using (var context = new QuanlysukienContext())
                    {

                        var DeleteUser = context.Nguoidungs.FirstOrDefault(u => u.Email == SelectedItem.Email);
                        if (DeleteUser != null)
                        {
                            // Xoá người dùng khỏi bảng đăng ký sự kiện nếu có
                            var UserRegiste = context.Dangkysukiens.Where(u => u.Mand == DeleteUser.Mand).ToList();
                                context.Dangkysukiens.RemoveRange(UserRegiste);                            
                            context.Nguoidungs.Remove(DeleteUser);
                            context.SaveChanges();
                            // Xoá dữ liệu sinh viên trong datagrid
                            MembersList.Remove(SelectedItem);

                            MessageBox.Show("Đã xoá sinh viên thành công");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
