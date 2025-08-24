using Demo1.Models;
using System;
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
using Demo1;
using System.IO;
using System.Diagnostics;


namespace Demo1.Pages.Admin
{
    /// <summary>
    /// Interaction logic for MemberListPage.xaml
    /// </summary>
    public partial class DsPheDuyet : Page
    {
        public ObservableCollection<Event> DanhsachPheDuyet { get; set; }

        public DsPheDuyet()
        {
            InitializeComponent();
            // Gán DataContext của trang để có thể truy cập MemberList từ XAML
            LoadEvent();
            DataContext = this;
            App.SharedData.DanhsachPheDuyet = DanhsachPheDuyet; // Đồng bộ dữ liệu
        }

        // Load sự kiện lên danh sách phê duyệt
        private void LoadEvent()
        {
            try
            {
                using (var context = new QuanlysukienContext())
                {
                    var events = context.Sukiens
                    .Where(s => s.Duyet == 0)  // Chỉ lấy các sự kiện chưa được phê duyệt
                    .Select(s =>
                    new Event
                    {
                        IdEvent = s.Mask,
                        Title = s.Tensk,
                        StartDate = s.Ngaybatdau.ToString("dd/MM/yyyy"),
                        StartTime = s.Ngaybatdau.ToString("HH:mm"),
                        Venue = s.Venue,
                        Faculty = FacultyMapping.ContainsKey(s.Dvtc) ? FacultyMapping[s.Dvtc] : "Khoa không xác định",
                        ImagePath = ConvertByteArrayToImage(s.Imageevent)
                    }).ToList();
                    DanhsachPheDuyet = new ObservableCollection<Event>(events);
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
        //// Xữ lý sự kiện được duyệt
        //public void ApproveEvent(Event EventItem)
        //{
        //    if (EventItem != null)
        //    {
        //        // Xoá sự kiện đang phê duyệt khỏi danh sách
        //        DanhsachPheDuyet.Remove(EventItem);
        //        // Cập nhật danh sách sự kiện đã tạo
        //        DanhsachDaTao.Add(EventItem);
        //    }
        //}

        public class Event
        {
            public string IdEvent { get; set; }
            public string Title { get; set; }
            public BitmapImage ImagePath { get; set; }
            public string StartDate { get; set; }
            public string StartTime { get; set; }
            public string Faculty { get; set; }
            public string Venue { get; set; }
        }
    }
}
