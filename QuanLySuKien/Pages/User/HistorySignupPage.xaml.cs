using System;
using Demo1.Pages.User;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Demo1.Models;
using Demo1.Pages.Admin;
using System.IO;

namespace Demo1.Pages.User
{
    /// <summary>
    /// Interaction logic for HistorySignupPage.xaml
    /// </summary>
    public partial class HistorySignupPage : Page
    {
        public ObservableCollection<DsPheDuyet.Event> SignupEvents { get; set; }

        public HistorySignupPage()
        {
            InitializeComponent();
            DataContext = this;
            string IDCreator = App.CurrentUserMand;
            LoadEvent(IDCreator);
        }
        // Load sự kiện lên lịch sử sự kiện đã tạo
        private void LoadEvent(string IDCreator)
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {
                    // Join bảng Dangkysukiens và Sukiens
                    var result = context.Dangkysukiens
                        .Where(dk => dk.Mand == IDCreator && dk.Xacnhanthamgia == "Có") // Lọc theo người tạo và xác nhận tham gia
                        .Join(
                            context.Sukiens, 
                            dk => dk.Mask,   
                            sk => sk.Mask,   
                            (dk, sk) => new DsPheDuyet.Event
                            {
                                IdEvent = sk.Mask,
                                Title = sk.Tensk,
                                Venue = sk.Venue,
                                StartDate = sk.Ngaybatdau.ToString("dd/MM/yyyy"),
                                StartTime = sk.Ngaybatdau.ToString("HH:mm"),
                                Faculty = FacultyMapping.ContainsKey(sk.Dvtc) ? FacultyMapping[sk.Dvtc] : "Khoa không xác định",
                                ImagePath = ConvertByteArrayToImage(sk.Imageevent)
                            }
                        )
                        .ToList(); // Chuyển kết quả thành danh sách

                    // Gán dữ liệu cho ObservableCollection
                    SignupEvents = new ObservableCollection<DsPheDuyet.Event>(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}");
            }
        }

        // Ánh xạ mã khoa và tên khoa
        private static Dictionary<string, string> FacultyMapping = new Dictionary<string, string>
        {
            { "KH0001", "Công Nghệ Phần Mềm" },
            { "KH0002", "Hệ Thống Thông Tin" },
            { "KH0003", "Khoa Học Máy Tính" },
            { "KH0004", "Kỹ Thuật Máy Tính" },
            { "KH0005", "Mạng Máy Tính Và Truyền Thông" },
            { "KH0006", "Khoa Học Và Kỹ Thuật Thông Tin" },
        };

        // Chuyển đổi hình ảnh từ csdl lên BitmapImage
        public static BitmapImage ConvertByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            // Tạo MemoryStream từ byte[]
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load ngay vào bộ nhớ
                bitmap.StreamSource = ms; // Nguồn dữ liệu là MemoryStream
                bitmap.EndInit();
                return bitmap;
            }
        }
    }
}

