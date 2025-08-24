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
    public partial class PostersListPage : Page
    {
        private ICollectionView _deanCollectionView;
        public ObservableCollection<Dean> MembersList { get; set; }

        public PostersListPage()
        {
            InitializeComponent();
            this.DataContext = AppState.Instance; // Liên kết DataContext với AppState
            LoadMembers();
            _deanCollectionView = CollectionViewSource.GetDefaultView(MembersList);
            _deanCollectionView.Filter = FilterDeans;

            // Lắng nghe sự thay đổi của FilterText
            AppState.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppState.FilterText))
                {
                    _deanCollectionView.Refresh();
                }
            };
        }

        // Hàm tìm kiếm dean
        private bool FilterDeans(object item)
        {
            if (item is Dean dean)
            {
                return string.IsNullOrEmpty(AppState.Instance.FilterText) ||
                       dean.Name.Contains(AppState.Instance.FilterText, StringComparison.OrdinalIgnoreCase) ||
                       dean.Email.Contains(AppState.Instance.FilterText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private void LoadMembers()
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {
                    var Dean = context.Nguoidungs
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
                        .Where(dean => dean.Roleuser == '3')
                        .Select(dean =>
                        new Dean
                        {
                            Id = dean.Mand,
                            Name = dean.Hoten,
                            PhoneNumber = dean.Sdt,
                            Email = dean.Email,
                            Khoa = dean.Tenkhoa

                        }).ToList();
                    MembersList = new ObservableCollection<Dean>(Dean);
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

        public class Dean
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Khoa { get; set; }
        }

        // Sự kiện thay đổi thông tin cá nhân của người đăng bài
        private void btninfo_Click(object sender, RoutedEventArgs e)
        {
            var SelectedMember = (Dean)CurrentDataGrid.SelectedItem;
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

        // Sự kiện xoá người đăng bài khỏi datagrid
        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            var SelectedItem = CurrentDataGrid.SelectedItem as Dean;
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một người đăng bài để xoá!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var Result = MessageBox.Show("Bạn có muốn xoá người đăng bài này?", "Xác Nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                // Xoá dữ liệu người đăng bài trong cơ sở dữ liệu
                try
                {
                    using (var context = new QuanlysukienContext())
                    {
                        var DeleteUser = context.Nguoidungs.FirstOrDefault(u => u.Email == SelectedItem.Email);
                        if (DeleteUser != null)
                        {
                            // Lấy danh sách sự kiện của người đăng bài
                            var DeanEvents = context.Sukiens.Where(u => u.Mandb == DeleteUser.Mand).ToList();
                            foreach (var eventItem in DeanEvents)
                            {
                                // Xoá các đăng ký sự kiện liên quan
                                var EventRegistrations = context.Dangkysukiens.Where(r => r.Mask == eventItem.Mask).ToList();
                                //if (EventRegistrations != null)
                                    context.Dangkysukiens.RemoveRange(EventRegistrations);
                                // Xoá sự kiện
                                context.Sukiens.Remove(eventItem);
                            }
                            // Xoá người dùng
                            context.Nguoidungs.Remove(DeleteUser);
                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();
                            // Xoá dữ liệu người đăng bài trong DataGrid
                            MembersList.Remove(SelectedItem);
                            MessageBox.Show("Đã xoá người đăng bài và các sự kiện liên quan thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy người đăng bài trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
