using Demo1.Models;
using Demo1.Pages.Admin;
using Demo1.Pages.Dean;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Demo1.Pages.General
{
    public partial class EventDetailsPage : Page
    {
        public ObservableCollection<EventItem> SuggestedEvents { get; set; }

        public EventDetails EventDetailss { get; set; }
        public string thisMask { get; set; }

        public EventDetailsPage(string Mask)
        {
            InitializeComponent();
            thisMask = Mask;
            using var context = new QuanlysukienContext();
            // Kiểm tra đúng người đăng bài thì mới thấy nút xem danh sách người tham gia
            var Mandb = (from c in context.Sukiens
                         where c.Mask == Mask
                         select c.Mandb).FirstOrDefault().ToString();
            if (Mandb == App.CurrentUserMand)
            {
                btnShowParticipants.Visibility = Visibility.Visible;
            }

            // Set the EventDetails DataContext to bind values to the page.
            var Event = (from c in context.Sukiens
                         where c.Mask == Mask
                         select c).FirstOrDefault();
            if (Event != null)
            {
                SetRichTextBoxContentFromXaml(EventContentRichTextBox, Event.Mota);
                EventDetailss = new EventDetails
                {
                    Title = Event.Tensk,
                    Faculty = (from c in context.Khoas
                               where c.Makhoa == Event.Dvtc
                               select c.Tenkhoa).FirstOrDefault().ToString(),
                    Venue = Event.Venue,
                    StartTime = Event.Ngaymodangky.ToString("HH:mm"),
                    StartDate = Event.Ngaymodangky.Date.ToString("dd/MM/yyyy"),
                    EndTime = Event.Ngaydongdangky.ToString("HH:mm"),
                    EndDate = Event.Ngaydongdangky.Date.ToString("dd/MM/yyyy"),
                    StartTime1 = Event.Ngaybatdau.ToString("HH:mm"),
                    StartDate1 = Event.Ngaybatdau.Date.ToString("dd/MM/yyyy"),
                    EndTime1 = Event.Ngayketthuc.ToString("HH:mm"),
                    EndDate1 = Event.Ngayketthuc.ToString("dd/MM/yyyy"),
                    Reward = Event.Phanthuong.ToString(),
                    Sponsor = Event.Nhataitro.ToString(),
                    Image = ConvertByteArrayToImage(Event.Imageevent)
                };
                if (Event.Nhataitro == "")
                    Sponsor.Visibility = Visibility.Collapsed;
                // Set the DataContext for binding
                this.DataContext = EventDetailss;

                // Tạo list danh sách sự kiện gợi ý (sự kiện có cùng thể loại)
                var suggestedEventList = (from c in context.Sukiens
                                          where c.Theloai == Event.Theloai && c.Mask != Event.Mask
                                          select c).Take(4).ToList();
                SuggestedEvents = new ObservableCollection<EventItem>();
                foreach (var item in suggestedEventList)
                {
                    EventItem e = new EventItem
                    {
                        Mask = item.Mask,
                        Title = item.Tensk,
                        Date = item.Ngaybatdau.ToString("dd/MM/yyyy"),
                        Organizer = (from c in context.Khoas
                                     where c.Makhoa == item.Dvtc
                                     select c.Tenkhoa).FirstOrDefault().ToString(),
                        ImagePath = ConvertByteArrayToImage(item.Imageevent)
                    };
                    SuggestedEvents.Add(e);
                }
            }
            // Add SuggestedEventsControl to the page
            var suggestedEventsControl = new SuggestedEventsControl();
            suggestedEventsControl.DataContext = this;
            MainContentControl.Children.Add(suggestedEventsControl);
        }
        private void btnShowParticipants_Click(object sender, RoutedEventArgs e)
        {
            // Mở trang hoặc thực hiện hành động hiển thị danh sách người tham gia
            ParticipantList participantListWindow = new ParticipantList(thisMask); // Tạo một đối tượng cửa sổ mới
            participantListWindow.Show();  // Hiển thị cửa sổ ParticipantList
        }

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

        public void SetRichTextBoxContentFromXaml(RichTextBox richTextBox, string xamlContent)
        {
            if (string.IsNullOrWhiteSpace(xamlContent))
            {
                richTextBox.Document.Blocks.Clear();
                return;
            }
            try
            {
                // Chuyển chuỗi XAML thành đối tượng WPF
                using (var stringReader = new StringReader(xamlContent))
                using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                {
                    var content = XamlReader.Load(xmlReader);

                    if (content is FlowDocument flowDocument)
                    {
                        // Nếu là FlowDocument, gán trực tiếp
                        richTextBox.Document = flowDocument;
                    }
                    else if (content is Section section)
                    {
                        // Nếu là Section, thêm vào FlowDocument
                        var newFlowDocument = new FlowDocument();
                        newFlowDocument.Blocks.Add(section);
                        richTextBox.Document = newFlowDocument;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải nội dung: {ex.Message}");
            }
        }
        private string CreateMaDK(QuanlysukienContext context)
        {
            // Lấy mã cao nhất từ cơ sở dữ liệu
            var maxMaDK = context.Dangkysukiens
                                 .OrderByDescending(dk => dk.Madk)
                                 .Select(dk => dk.Madk)
                                 .FirstOrDefault();

            // Tạo mã mới dựa trên giá trị lớn nhất
            if (maxMaDK != null && maxMaDK.StartsWith("DK"))
            {
                int maxNumber = int.Parse(maxMaDK.Substring(2)); // Bỏ "DK" và chuyển phần số
                return $"DK{(maxNumber + 1).ToString("D3")}";    // Tăng giá trị và định dạng
            }
            return "DK001";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using var context = new QuanlysukienContext();
            var ev = context.Sukiens.Find(thisMask);
            if (ev.Ngaymodangky > DateTime.Now)
            {
                MessageBox.Show("Sự kiện chưa mở đăng ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (ev.Ngaydongdangky < DateTime.Now)
            {
                MessageBox.Show("Sự kiện đã đóng đăng ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Dangkysukien dk = new Dangkysukien();
            dk.Madk = CreateMaDK(new QuanlysukienContext());
            dk.Mask = thisMask;
            dk.Thoigiandangky = DateTime.Now;
            dk.Mand = App.CurrentUserMand;
            dk.Xacnhanthamgia = "Không";
            foreach (var item in context.Dangkysukiens)
            {
                if (item.Mask == thisMask && item.Mand == App.CurrentUserMand)
                {
                    MessageBox.Show("Bạn đã đăng ký sự kiện này rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            MessageBox.Show("Đăng ký sự kiện thành công");
            context.Dangkysukiens.Add(dk);
            context.SaveChanges();
        }

        public class EventDetails
        {
            public string Title { get; set; }
            public string Faculty { get; set; }
            public string Venue { get; set; }
            public string StartTime { get; set; }
            public string StartDate { get; set; }
            public string EndTime { get; set; }
            public string EndDate { get; set; }
            public string StartTime1 { get; set; }
            public string StartDate1 { get; set; }
            public string EndTime1 { get; set; }
            public string EndDate1 { get; set; }
            public string Reward { get; set; }
            public string Sponsor { get; set; }
            public BitmapImage Image { get; set; }
        }
        public class EventItem
        {
            public string Mask { get; set; }
            public string Title { get; set; }
            public string Date { get; set; }
            public string Organizer { get; set; }
            public BitmapImage ImagePath { get; set; }
        }

    }
}
