using Demo1.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Markup;
using System.Xml.Linq;


namespace Demo1.Pages.Dean
{
    /// <summary>
    /// Interaction logic for CheckEvent.xaml
    /// </summary>
    /// 
    public class FacultyItem
    {
        public string IdFaculty { get; set; }
        public string NameFaculty { get; set; }
    }

    public partial class CheckEvent : Page
    {
        private Sukien GetEventById(string idEvent)
        {
            using (var dbContext = new QuanlysukienContext())
            {
                return dbContext.Sukiens.FirstOrDefault(e => e.Mask == idEvent);
            }
        }
        private Sukien CurrentEvent;
        public CheckEvent(string idEvent)
        {
            InitializeComponent();
            EnableEditing(false); // Mặc định tất cả các trường không chỉnh sửa
            CurrentEvent = GetEventById(idEvent);
            EventNameTextBox.Text = CurrentEvent.Tensk;
            LocationTextBox.Text = CurrentEvent.Venue;
            SponsorNameTextBox.Text = CurrentEvent.Nhataitro;
            RewardTextBox.Text = CurrentEvent.Phanthuong;
            ParticipantsComboBox.Text = CurrentEvent.Soluongthamgia.ToString();
            CategoryComboBox.Text = CurrentEvent.Theloai;
            // Hiển thị danh sách khoa kèm id mỗi khoa
            var Faculty = new List<FacultyItem>
            {
                new FacultyItem { IdFaculty = "KH0001", NameFaculty = "Công Nghệ Phần Mềm" },
                new FacultyItem { IdFaculty = "KH0002", NameFaculty = "Hệ Thống Thông Tin" },
                new FacultyItem { IdFaculty = "KH0003", NameFaculty = "Khoa Học Máy Tính" },
                new FacultyItem { IdFaculty = "KH0004", NameFaculty = "Kỹ Thuật Máy Tính" },
                new FacultyItem { IdFaculty = "KH0005", NameFaculty = "Mạng Máy Tính Và Truyền Thông" },
                new FacultyItem { IdFaculty = "KH0006", NameFaculty = "Khoa Học Và Kỹ Thuật Thông Tin" },
            };
            KhoaCombobox.ItemsSource = Faculty;
            KhoaCombobox.DisplayMemberPath = "NameFaculty"; // Hiển thị tên khoa
            KhoaCombobox.SelectedValuePath = "IdFaculty";  // Lấy ID khoa
            var currentFaculty = Faculty.FirstOrDefault(f => f.IdFaculty == CurrentEvent.Dvtc); // so sánh với mã khoa hiện tại của người dùng để hiển thị
            if (currentFaculty != null)
            {
                KhoaCombobox.SelectedItem = currentFaculty;
            }

            // Lấy ngày mở đăng và đóng đăng ký
            RegistrationOpenDate.SelectedDate = CurrentEvent.Ngaymodangky.Date;
            RegistrationCloseDate.SelectedDate = CurrentEvent.Ngaydongdangky.Date;
            // Lấy thời gian mở và đóng đăng ký
            RegistrationOpenTime.SelectedTime = DateTime.Today + CurrentEvent.Ngaymodangky.TimeOfDay;
            RegistrationCloseTime.SelectedTime = DateTime.Today + CurrentEvent.Ngaydongdangky.TimeOfDay;

            // Lấy ngày mở đăng và đóng đăng ký
            EventStartDate.SelectedDate = CurrentEvent.Ngaymodangky.Date;
            EventEndDate.SelectedDate = CurrentEvent.Ngaydongdangky.Date;
            // Lấy thời gian mở và đóng đăng ký
            EventStartTime.SelectedTime = DateTime.Today + CurrentEvent.Ngaymodangky.TimeOfDay;
            EventEndTime.SelectedTime = DateTime.Today + CurrentEvent.Ngaydongdangky.TimeOfDay;

            // Đưa avt từ csdl lên Image
            AvatarPreview.Source = ConvertByteArrayToImage(CurrentEvent.Imageevent);

            // Đưa mô tả từ csdl lên RichTextBox
            SetRichTextBoxContentFromXaml(ContentRichTextBox, CurrentEvent.Mota);
        }

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

        // Đưa nội dung mô tả sự kiện từ database lên và giữ nguyên format
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

        // Bật/tắt chế độ chỉnh sửa thông tin người dùng
        private void EnableEditing(bool isEnabled)
        {
            EventNameTextBox.IsReadOnly = !isEnabled;
            LocationTextBox.IsReadOnly = !isEnabled;
            SponsorNameTextBox.IsReadOnly = !isEnabled;
            RewardTextBox.IsReadOnly = !isEnabled;

            ParticipantsComboBox.IsEnabled = isEnabled;
            CategoryComboBox.IsEnabled = isEnabled;
            KhoaCombobox.IsEnabled = isEnabled;

            RegistrationOpenTime.IsEnabled = isEnabled;
            RegistrationOpenDate.IsEnabled = isEnabled;
            RegistrationCloseTime.IsEnabled = isEnabled;
            RegistrationCloseDate.IsEnabled = isEnabled;
            EventStartTime.IsEnabled = isEnabled;
            EventStartDate.IsEnabled = isEnabled;
            EventEndTime.IsEnabled = isEnabled;
            EventEndDate.IsEnabled = isEnabled;

            bntImage.IsEnabled = isEnabled;
            ContentRichTextBox.IsEnabled = isEnabled;
        }

        // Nút chỉnh sửa nội dung sự kiện
        private void ChangeInfEvent_Click(object sender, RoutedEventArgs e)
        {
            EnableEditing(true);
            // Ẩn nút chỉnh sửa khi bắt đầu chỉnh sửa
            btnChinhSua.Visibility = Visibility.Collapsed;
            // Hiện nút xác nhận
            btnXacnhan.Visibility = Visibility.Visible;
        }

        // Nút xác nhận chỉnh sửa xong
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Đặt lại các trường để không thể chỉnh sửa
                EventNameTextBox.IsReadOnly = true;
                LocationTextBox.IsReadOnly = true;
                SponsorNameTextBox.IsReadOnly = true;
                RewardTextBox.IsReadOnly = true;
                ParticipantsComboBox.IsReadOnly = true;
                CategoryComboBox.IsReadOnly = true  ;
                KhoaCombobox.IsReadOnly = true;
                RegistrationOpenTime.IsEnabled = true;
                RegistrationOpenDate.IsEnabled = true;
                RegistrationCloseTime.IsEnabled = true;
                RegistrationCloseDate.IsEnabled = true;
                EventStartTime.IsEnabled = true;
                EventStartDate.IsEnabled = true;
                EventEndTime.IsEnabled = true;
                EventEndDate.IsEnabled = true;
                bntImage.IsEnabled = true;
                ContentRichTextBox.IsEnabled = true;

                EnableEditing(false); // Vô hiệu hóa chỉnh sửa
                // Ẩn nút Xác nhận sau khi xác nhận xong
                btnXacnhan.Visibility = Visibility.Collapsed;

                // Hiện nút Chỉnh sửa để có thể chỉnh sửa lại
                btnChinhSua.Visibility = Visibility.Visible;

                // Lưu thông tin vào cơ sở dữ liệu
                using (var context = new QuanlysukienContext())
                {
                    BitmapSource bitmapSource = AvatarPreview.Source as BitmapSource;
                    TextRange textrange = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
                    var EventUpdate = context.Sukiens.FirstOrDefault(e => e.Mask == CurrentEvent.Mask);
                    if (EventUpdate != null)
                    {
                        EventUpdate.Tensk = EventNameTextBox.Text;
                        EventUpdate.Venue = LocationTextBox.Text;
                        EventUpdate.Phanthuong = RewardTextBox.Text;
                        EventUpdate.Nhataitro = SponsorNameTextBox.Text;
                        EventUpdate.Theloai = CategoryComboBox.Text;
                        EventUpdate.Imageevent = CoversionAvatarEvent(bitmapSource);
                        EventUpdate.Mota = GetRichTextBoxContentAsXaml(textrange);
                        EventUpdate.Dvtc = KhoaCombobox.Text switch
                        {
                            "Công Nghệ Phần Mềm" => "KH0001",
                            "Hệ Thống Thông Tin" => "KH0002",
                            "Khoa Học Máy Tính" => "KH0003",
                            "Kỹ Thuật Máy Tính" => "KH0004",
                            "Mạng Máy Tính Và Truyền Thông" => "KH0005",
                            "Khoa Học Và Kỹ Thuật Thông Tin" => "KH0006",
                            _ => throw new ArgumentException("Mã khoa không hợp lệ.")
                        };
                        if (int.TryParse(ParticipantsComboBox.Text, out int soluongThamgia))
                        {
                            EventUpdate.Soluongthamgia = soluongThamgia;
                        }
                        else
                        {
                            MessageBox.Show("Số lượng tham gia không hợp lệ.", "Lỗi nhập liệu");
                            return;
                        }

                        // Lấy ngày và giờ từ các TimePicker và DatePicker
                        DateTime? registrationOpenDate = RegistrationOpenDate.SelectedDate; // Lấy ngày từ DatePicker
                        DateTime? registrationOpenTime = RegistrationOpenTime.SelectedTime; // Lấy giờ từ TimePicker
                        DateTime? registrationCloseDate = RegistrationCloseDate.SelectedDate; // Lấy ngày từ DatePicker
                        DateTime? registrationCloseTime = RegistrationCloseTime.SelectedTime; // Lấy giờ từ TimePicker
                        DateTime? eventStartDate = EventStartDate.SelectedDate; // Lấy ngày từ DatePicker
                        DateTime? eventStartTime = EventStartTime.SelectedTime; // Lấy giờ từ TimePicker
                        DateTime? eventEndDate = EventEndDate.SelectedDate; // Lấy ngày từ DatePicker
                        DateTime? eventEndTime = EventEndTime.SelectedTime; // Lấy giờ từ TimePicker

                        // Kết hợp ngày và giờ
                        DateTime? registrationOpenDateTime = null;
                        DateTime? registrationCloseDateTime = null;
                        DateTime? eventStartDateTime = null;
                        DateTime? eventEndDateTime = null;
                        registrationOpenDateTime = registrationOpenDate.Value.Date + registrationOpenTime.Value.TimeOfDay;
                        registrationCloseDateTime = registrationCloseDate.Value.Date + registrationCloseTime.Value.TimeOfDay;
                        eventStartDateTime = eventStartDate.Value.Date + eventStartTime.Value.TimeOfDay;
                        eventEndDateTime = eventEndDate.Value.Date + eventEndTime.Value.TimeOfDay;

                        // Lưu ngày và giờ vào cơ sở dữ liệu
                        EventUpdate.Ngaymodangky = registrationOpenDateTime.Value;
                        EventUpdate.Ngaydongdangky = registrationCloseDateTime.Value;
                        EventUpdate.Ngaybatdau = eventStartDateTime.Value;
                        EventUpdate.Ngayketthuc = eventEndDateTime.Value;

                        if (EventUpdate.Ngaymodangky > EventUpdate.Ngaydongdangky)
                        {
                            System.Windows.MessageBox.Show("Ngày mở đăng ký phải nhỏ hơn ngày đóng đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else if (EventUpdate.Ngaybatdau > EventUpdate.Ngayketthuc)
                        {
                            System.Windows.MessageBox.Show("Ngày bắt đầu sự kiện phải nhỏ hơn hoặc bằng ngày kết thúc sự kiện!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        context.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
                        CurrentEvent = EventUpdate;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
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

        private void ThemeFontComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow users to type in any font name
            ThemeFontComboBox.IsDropDownOpen = true; // Keeps dropdown open while typing
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentRichTextBox.Selection.IsEmpty)
            {
                return;
            }
            // Lấy giá trị nhập vào từ ComboBox
            if (double.TryParse(FontSizeComboBox.Text, out double fontSize))
            {
                // Áp dụng kích thước văn bản cho đoạn văn bản được chọn
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
            else
            {
                // Nếu nhập không phải số, có thể hiển thị cảnh báo hoặc reset lại giá trị
                System.Windows.MessageBox.Show("Vui lòng nhập một giá trị số hợp lệ cho kích thước văn bản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                FontSizeComboBox.Text = string.Empty;
            }
        }
    }
}
