using System;
using Demo1.Pages.Dean;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Demo1.Models;
using Demo1.Pages.Admin;
using System.IO;

namespace Demo1.Pages.Dean
{
    public partial class HistoryCreatePage : Page
    {
        public ObservableCollection<DsPheDuyet.Event> CreatedEvents { get; set; }

        public HistoryCreatePage()
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
                    var events = context.Sukiens
                        .Where(s => s.Duyet == 1 && s.Mandb == IDCreator)
                        .Select(s =>
                        new DsPheDuyet.Event
                        {
                            IdEvent = s.Mask,
                            Title = s.Tensk,
                            StartDate = s.Ngaybatdau.ToString("dd/MM/yyyy"),
                            StartTime = s.Ngaybatdau.ToString("HH:mm"),
                            Venue = s.Venue,
                            Faculty = FacultyMapping.ContainsKey(s.Dvtc) ? FacultyMapping[s.Dvtc] : "Khoa không xác định",
                            ImagePath = ConvertByteArrayToImage(s.Imageevent)
                        }).ToList();
                   CreatedEvents = new ObservableCollection<DsPheDuyet.Event>(events);
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
