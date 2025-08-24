using Demo1.Models;
using Demo1.Pages.General;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Globalization;
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;
namespace Demo1.Pages.Dean
{
    /// <summary>
    /// Interaction logic for CreateEventPage.xaml
    /// </summary>
    public partial class CreateEventPage : Page
    {
        public ObservableCollection<int> EventQuantities { get; set; }

        public CreateEventPage()
        {
            InitializeComponent();
            // Khởi tạo ObservableCollection và thêm các số từ 1 đến 1000
            EventQuantities = new ObservableCollection<int>();

            for (int i = 1; i <= 1000; i++)
            {
                EventQuantities.Add(i);
            }

            // Liên kết ObservableCollection vào ComboBox
            ParticipantsComboBox.ItemsSource = EventQuantities;
        }

        // Sự kiện in đậm
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentRichTextBox.Selection.IsEmpty)
            {
                return;
            }
            var currentBold = ContentRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            if (currentBold != DependencyProperty.UnsetValue && currentBold.Equals(FontWeights.Bold))
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
            else
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }
        // Sự kiện in nghiêng
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentRichTextBox.Selection.IsEmpty)
            {
                return;
            }
            var currentItalic = ContentRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            if (currentItalic != DependencyProperty.UnsetValue && currentItalic.Equals(FontStyles.Italic))
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
            }
            else
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            }
        }
        // Sự kiện gạch chân
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentRichTextBox.Selection.IsEmpty)
            {
                return;
            }
            var currentUnderline = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (currentUnderline != DependencyProperty.UnsetValue && currentUnderline == TextDecorations.Underline)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            else
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }
        // Sự kiện đóng, mở hộp chọn màu
        private void OpenColorPicker_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị ColorPicker khi nhấn vào nút chọn màu
            FontColorPicker.Visibility = FontColorPicker.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        // Căn lề trái
        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            // Căn lề trái cho văn bản trong RichTextBox
            ContentRichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
        }

        // Căn lề giữa
        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            // Căn lề giữa cho văn bản trong RichTextBox
            ContentRichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        }

        // Căn lề phải
        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            // Căn lề phải cho văn bản trong RichTextBox
            ContentRichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }
        // Sự kiện thêm phông chữ cho text trong RichTextbox
        private void ThemeFontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentRichTextBox != null)
            {
                var selectedFont = (ThemeFontComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? ThemeFontComboBox.Text;
                if (!string.IsNullOrEmpty(selectedFont))
                {
                    ContentRichTextBox.FontFamily = new FontFamily(selectedFont);
                }
            }
        }
        // Sự kiện thêm màu cho text của RichTextBox
        private void FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue && ContentRichTextBox != null)
            {
                Color color = e.NewValue.Value;
                TextRange selectedText = new TextRange(ContentRichTextBox.Selection.Start, ContentRichTextBox.Selection.End);
                selectedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
                FontColorPicker.Visibility = Visibility.Collapsed; // Ẩn ColorPicker sau khi chọn màu
            }
        }

        // Tạo mã sự kiện tăng dần
        private string CreateMaSK(QuanlysukienContext context)
        {
            // Lấy mã cao nhất từ cơ sở dữ liệu
            var maxMaSK = context.Sukiens
                                 .OrderByDescending(sk => sk.Mask)
                                 .Select(sk => sk.Mask)
                                 .FirstOrDefault();

            // Tạo mã mới dựa trên giá trị lớn nhất
            if (maxMaSK != null && maxMaSK.StartsWith("SK"))
            {
                int maxNumber = int.Parse(maxMaSK.Substring(2)); // Bỏ "SK" và chuyển phần số
                return $"SK{(maxNumber + 1).ToString("D3")}";    // Tăng giá trị và định dạng
            }
            return "SK001";
        }
        // Kiểm tra nhập đầy đủ thông tin
        private bool AllowEvent()
        {
            if (EventNameTextBox.Text.Trim().Length == 0 || LocationTextBox.Text.Trim().Length == 0 || CategoryComboBox.SelectedItem == null || RegistrationOpenDate.SelectedDate == null || KhoaCombobox.SelectedItem == null ||
                RegistrationCloseDate.SelectedDate == null || RegistrationOpenTime.SelectedTime == null || RegistrationCloseTime.SelectedTime == null || EventStartTime.SelectedTime == null || EventEndTime.SelectedTime == null ||
                EventStartDate.SelectedDate == null || EventEndDate.SelectedDate == null ||  RewardTextBox.Text.Trim().Length == 0 || ParticipantsComboBox.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        // Sự kiện chuyển đổi tên khoa sang mã khoa
        private string ChangeFaculty()
        {
            using (var context = new QuanlysukienContext())
            {
                var Faculty = context.Khoas.FirstOrDefault(u => u.Tenkhoa == KhoaCombobox.Text);
                if (Faculty != null)
                {
                    return Faculty.Makhoa;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // Sự kiện nút tạo bài đăng
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AllowEvent())
            {
                return;
            }
            // Lấy ảnh từ Image và chuyển thành byte
            BitmapSource bitmapSource = AvatarPreview.Source as BitmapSource;

            byte[] AvatarEvent = CoversionAvatarEvent(bitmapSource);
            string TenSK = EventNameTextBox.Text.Trim();
            string Venue = LocationTextBox.Text.Trim();
            string TheLoai = CategoryComboBox.Text.Trim();
            string BeginRegisterDate = RegistrationOpenDate.Text.Trim();
            string EndRegisterDate = RegistrationCloseDate.Text.Trim();
            string BeginRegisterTime = RegistrationOpenTime.SelectedTime?.ToString(@"hh\:mm") ?? string.Empty;
            string EndRegisterTime = RegistrationCloseTime.SelectedTime?.ToString(@"hh\:mm") ?? string.Empty;
            string BeginTime = EventStartTime.SelectedTime?.ToString(@"hh\:mm") ?? string.Empty;
            string EndTime = EventEndTime.SelectedTime?.ToString(@"hh\:mm") ?? string.Empty;
            string BeginDate = EventStartDate.Text.Trim();
            string EndDate = EventEndDate.Text.Trim();
            string NhaTaiTro = SponsorNameTextBox.Text.Trim();
            string PhanThuong = RewardTextBox.Text.Trim();
            int SoLuongThamGia = ParticipantsComboBox.Items.Count;

            TextRange textrange = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
            string MoTa = GetRichTextBoxContentAsXaml(textrange);

            using (var context = new QuanlysukienContext())
            {
                // Chuyển kiểu dữ liệu sang dd/MM/yy để lưu vào csdl
                string DateFormat = "dd/MM/yyyy";
                DateTime.TryParseExact(BeginRegisterDate, DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime beginRegisterDate);
                DateTime.TryParseExact(EndRegisterDate, DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endRegisterDate);
                DateTime.TryParseExact(BeginDate, DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime beginDate);
                DateTime.TryParseExact(EndDate, DateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endDate);
                // Chuyển kiểu dữ liệu sang HH:mm để lưu vào csdl
                string TimeFormat = "HH:mm";
                TimeOnly.TryParseExact(BeginTime, TimeFormat, null, DateTimeStyles.None, out TimeOnly beginTime);
                TimeOnly.TryParseExact(EndTime, TimeFormat, null, DateTimeStyles.None, out TimeOnly endTime);
                TimeOnly.TryParseExact(BeginRegisterTime, TimeFormat, null, DateTimeStyles.None, out TimeOnly beginRegisterTime);
                TimeOnly.TryParseExact(EndRegisterTime, TimeFormat, null, DateTimeStyles.None, out TimeOnly endRegisterTime);

                // Hợp ngày và thời gian lại
                DateTime BeginRegisterDateTime = beginRegisterDate.Add(beginRegisterTime.ToTimeSpan());
                DateTime EndRegisterDateTime = endRegisterDate.Add(endRegisterTime.ToTimeSpan());
                DateTime EventStartDateTime = beginDate.Add(beginTime.ToTimeSpan());
                DateTime EventEndDateTime = endDate.Add(endTime.ToTimeSpan());

                if (beginRegisterDate > endRegisterDate)
                {
                    System.Windows.MessageBox.Show("Ngày mở đăng ký phải nhỏ hơn ngày đóng đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    RegistrationOpenDate.Focus();
                }
                else if (beginDate > endDate)
                {
                    System.Windows.MessageBox.Show("Ngày bắt đầu sự kiện phải nhỏ hơn hoặc bằng ngày kết thúc sự kiện!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    RegistrationOpenDate.Focus();
                }
                else if (AvatarEvent == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng chọn hình ảnh cho sự kiện!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string MaSK = CreateMaSK(context);
                    Sukien sukien = new Sukien()
                    {
                        Mask = MaSK,
                        Tensk = TenSK,
                        Dvtc = ChangeFaculty(),
                        Theloai = TheLoai,
                        Mota = MoTa,
                        Mandb = App.CurrentUserMand,
                        Soluongthamgia = (short)SoLuongThamGia,
                        Venue = Venue,
                        Phanthuong = PhanThuong,
                        Ngaymodangky = BeginRegisterDateTime,
                        Ngaydongdangky = EndRegisterDateTime,
                        Ngaybatdau = EventStartDateTime,
                        Ngayketthuc = EventEndDateTime,
                        Imageevent = AvatarEvent,
                        Nhataitro = NhaTaiTro,
                        Duyet = 0
                    };
                    context.Sukiens.Add(sukien);
                    context.SaveChanges();
                    System.Windows.MessageBox.Show("Đã tạo sự kiện thành công!", "Thông Báo", MessageBoxButton.OK);
                    // Xoá các dữ liệu cho lần tạo mới
                    EventNameTextBox.Clear();
                    LocationTextBox.Clear();
                    CategoryComboBox.Items.Clear();
                    KhoaCombobox.Items.Clear();
                    RegistrationOpenDate.Text = string.Empty;
                    RegistrationCloseDate.Text = string.Empty;
                    RegistrationOpenTime.Text = string.Empty;
                    RegistrationCloseTime.Text = string.Empty;
                    EventStartTime.Text = string.Empty;
                    EventEndTime.Text = string.Empty;
                    EventStartDate.Text = string.Empty;
                    EventEndDate.Text = string.Empty;
                    SponsorNameTextBox.Clear();
                    RewardTextBox.Clear();
                    CategoryComboBox.Text = string.Empty;
                    ContentRichTextBox.Document.Blocks.Clear();
                    AvatarPreview.Source = null;
                }
            }
        }
        // Chuyển dữ liệu được markup trong Richtexbox sang dạng chuỗi XAML
        private string GetRichTextBoxContentAsXaml(TextRange textrange)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                textrange.Save(ms, DataFormats.Xaml);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        // Sự kiện up ảnh lên Image lúc tạo sự kiện
        private void UploadAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bmi = new BitmapImage(new Uri(openFileDialog.FileName));
                    // Hiển thị ảnh được chọn
                    AvatarPreview.Source = bmi;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Lỗi tải ảnh!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        // Chuyển đổi hình ảnh sang chuỗi binary
        private byte[] CoversionAvatarEvent(BitmapSource bms)
        {
            if (bms == null)
            {
                return null;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bms));  // Chuyển đổi BitmapSource thành mảng byte
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private void ThemeFontComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow users to type in any font name
            ThemeFontComboBox.IsDropDownOpen = true; // Keeps dropdown open while typing
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem có văn bản nào được chọn trong RichTextBox không
            if (ContentRichTextBox.Selection.IsEmpty)
            {
                return;
            }
            // Lấy giá trị từ ComboBox và kiểm tra xem có phải số hợp lệ không
            if (FontSizeComboBox.SelectedItem is ComboBoxItem selectedItem && double.TryParse(selectedItem.Content.ToString(), out double fontSize))
            {
                // Áp dụng kích thước font cho đoạn văn bản được chọn
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
            else
            {
                return;
            }
        }    
    }
}
