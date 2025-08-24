using Demo1.Models;
using Demo1.Pages.General;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Demo1.Pages
{
    public class DateInListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date && parameter is ObservableCollection<DateTime> specialDates)
            {
                // Kiểm tra nếu ngày nằm trong danh sách đặc biệt
                return specialDates.Any(d => d.Date == date.Date);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public partial class CalendarPage : Page
    {
        public ObservableCollection<Event> FilteredEvents { get; set; }
        public ObservableCollection<Event> FilteredRegistrationEvents { get; set; }
        public ObservableCollection<Event> RegisteredEvents { get; set; }

        // Một danh sách sự kiện mẫu (có thể lấy từ cơ sở dữ liệu hoặc nguồn khác)
        public ObservableCollection<Event> AllEvents { get; set; }
        public ObservableCollection<DateTime> SpecialDates { get; set; }

        public CalendarPage()
        {
            InitializeComponent();
            // Khởi tạo danh sách sự kiện (hoặc lấy từ nguồn khác)
            SpecialDates = new ObservableCollection<DateTime>();
            using var context = new QuanlysukienContext();
            var allevent = (from c in context.Sukiens
                            where c.Duyet == 1
                            select c).ToList();
            AllEvents = new ObservableCollection<Event>();
            foreach (var item in allevent)
            {
                AllEvents.Add(new Event
                {
                    Title = item.Tensk,
                    EventStartDate = item.Ngaybatdau.Date,
                    EventStartTime = item.Ngaybatdau.ToString("HH:mm", CultureInfo.InvariantCulture),
                    EventEndDate = item.Ngayketthuc.Date,
                    EventEndTime = item.Ngayketthuc.ToString("HH:mm", CultureInfo.InvariantCulture),
                    RegistrationStartDate = item.Ngaymodangky.Date,
                    RegistrationStartTime = item.Ngaymodangky.ToString("HH:mm", CultureInfo.InvariantCulture),
                    RegistrationEndDate = item.Ngaydongdangky.Date,
                    RegistrationEndTime = item.Ngaydongdangky.ToString("HH:mm", CultureInfo.InvariantCulture),
                    Organizer = (from c in context.Khoas
                                 where item.Dvtc == c.Makhoa
                                 select c.Tenkhoa).FirstOrDefault(),
                    ImagePath = EventDetailsPage.ConvertByteArrayToImage(item.Imageevent)
                });
            }


            var mask = (from c in context.Dangkysukiens
                        where c.Mand == App.CurrentUserMand
                        select c.Mask).ToList();
            RegisteredEvents = new ObservableCollection<Event>();
            foreach (var item in mask)
            {
                var qr = (from c in context.Sukiens
                          where c.Mask == item
                          select c).FirstOrDefault();
                SpecialDates.Add(qr.Ngaybatdau.Date);
                if (qr.Ngayketthuc >= DateTime.Now)
                {
                    RegisteredEvents.Add(new Event
                    {
                        Title = qr.Tensk,
                        EventStartDate = qr.Ngaybatdau.Date,
                        EventStartTime = qr.Ngaybatdau.ToString("HH:mm", CultureInfo.InvariantCulture),
                        EventEndDate = qr.Ngayketthuc.Date,
                        EventEndTime = qr.Ngayketthuc.ToString("HH:mm", CultureInfo.InvariantCulture),
                        RegistrationStartDate = qr.Ngaymodangky.Date,
                        RegistrationStartTime = qr.Ngaymodangky.ToString("HH:mm", CultureInfo.InvariantCulture),
                        RegistrationEndDate = qr.Ngaydongdangky.Date,
                        RegistrationEndTime = qr.Ngaydongdangky.ToString("HH:mm", CultureInfo.InvariantCulture),
                        Organizer = (from c in context.Khoas
                                     where qr.Dvtc == c.Makhoa
                                     select c.Tenkhoa).FirstOrDefault(),
                        ImagePath = EventDetailsPage.ConvertByteArrayToImage(qr.Imageevent)
                    });
                }
            }
            // Khởi tạo FilteredEvents và FilteredRegistrationEvents
            FilteredEvents = new ObservableCollection<Event>();
            FilteredRegistrationEvents = new ObservableCollection<Event>();

            // Lọc các sự kiện đang trong thời gian đăng ký (thời gian hiện tại nằm trong khoảng đăng ký)
            var eventsForRegistration = AllEvents.Where(ev => ev.RegistrationStartDate <= DateTime.Now && ev.RegistrationEndDate >= DateTime.Now).ToList();
            // Cập nhật FilteredRegistrationEvents để hiển thị sự kiện đang trong thời gian đăng ký
            FilteredRegistrationEvents.Clear();
            foreach (var eventItem in eventsForRegistration)
            {
                FilteredRegistrationEvents.Add(eventItem);
            }
            DataContext = this;
        }
        // Xử lý sự kiện khi ngày được chọn
        private void Calendar_SelectedDatesChanged(object sender, RoutedEventArgs e)
            {
            using var context = new QuanlysukienContext();
                // Kiểm tra xem có ngày được chọn không
                if (calendar.SelectedDate.HasValue)
                {
                    var selectedDate = calendar.SelectedDate.Value;
                    
                    // Lọc các sự kiện theo ngày đã chọn (dựa vào EventStartDate và EventEndDate)
                    var eventsForSelectedDay = AllEvents.Where(ev => ev.EventStartDate.Date <= selectedDate.Date && ev.EventEndDate.Date >= selectedDate.Date).ToList();

                    // Cập nhật FilteredEvents để hiển thị sự kiện cho ngày đã chọn
                    FilteredEvents.Clear();
                    foreach (var eventItem in eventsForSelectedDay)
                    {
                        FilteredEvents.Add(eventItem);
                    }
                }
            }

        public class Event
        {
                    public string Title { get; set; }
                    public DateTime EventStartDate { get; set; }
                    public string EventStartTime { get; set; }
                    public DateTime EventEndDate { get; set; }
                    public string EventEndTime { get; set; }

                    public string Organizer { get; set; }
                    public BitmapImage ImagePath { get; set; }
                    public DateTime RegistrationStartDate { get; set; }
                    public string RegistrationStartTime { get; set; }
                    public DateTime RegistrationEndDate { get; set; }
                    public string RegistrationEndTime { get; set; }
        }
        private void Calendar_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCalendarDayButtons();
        }

        private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            UpdateCalendarDayButtons();
        }

        private void UpdateCalendarDayButtons()
        {
            foreach (var child in FindVisualChildren<Button>(calendar))
            {
                if (child is CalendarDayButton dayButton && dayButton.DataContext is DateTime date)
                {
                    // Kiểm tra nếu ngày nằm trong SpecialDates
                    var isSpecialDate = SpecialDates.Any(d => d.Date == date.Date);

                    // Gán style dựa trên điều kiện
                    dayButton.Style = isSpecialDate
                        ? (Style)FindResource("CalendarDayButtonStyle2")
                        : (Style)FindResource("CalendarDayButtonStyle1");
                }
            }
        }



        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }

}
