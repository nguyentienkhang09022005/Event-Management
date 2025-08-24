using Demo1.Models;
using Demo1.Pages.General;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo1.Pages
{
    public partial class HomePage : Page
    {
        // Danh sách sự kiện
        public ObservableCollection<Event> FeaturedEvents { get; set; }
        public ObservableCollection<Event> RegisteredEvents { get; set; }
        public ObservableCollection<Event> CreatedEvents { get; set; }  // Danh sách sự kiện đã tạo
        public ObservableCollection<Event> SearchedEvents { get; set; }

        public HomePage()
        {
            InitializeComponent();
            // Load danh sách sự kiện
            LoadData();
            // Gọi hàm để thiết lập nội dung phù hợp với quyền người dùng
            SetContentBasedOnUserRole();
        }

        public HomePage(string SearchText)
        {
            InitializeComponent();
            MainContentControl.Children.Clear();
            SearchedEvents = new ObservableCollection<Event>();
            QuanlysukienContext db = new QuanlysukienContext();

            // Chuyển SearchText thành chữ thường để so sánh không phân biệt hoa thường
            string searchTextLower = SearchText.ToLower();

            foreach (var item in db.Sukiens)
            {
                // So sánh Tensk với SearchText không phân biệt hoa thường
                if (item.Tensk.ToLower().Contains(searchTextLower) || item.Theloai.ToLower().Contains(searchTextLower))
                {
                    var Event = (from sk in db.Sukiens
                                 join khoa in db.Khoas
                                 on sk.Dvtc equals khoa.Makhoa
                                 where sk.Mask == item.Mask && sk.Duyet == 1
                                 select new Event
                                 {
                                     Mask = sk.Mask,
                                     Title = sk.Tensk,
                                     Date = sk.Ngaybatdau.ToString(),
                                     Organizer = khoa.Tenkhoa,
                                     ImagePath = ConvertByteArrayToImage(sk.Imageevent)
                                 }).FirstOrDefault();
                    SearchedEvents.Add(Event);
                }
            }

            var searchedEventsControl = new SearchedEventsControl();
            searchedEventsControl.DataContext = this;
            MainContentControl.Children.Add(searchedEventsControl);
        }

        private void SetContentBasedOnUserRole()
        {
            // Clear existing content
            MainContentControl.Children.Clear();
            using (var context = new QuanlysukienContext())
            {
                if (App.CurrentUserRole == 0) // Guest
                {
                    var featuredEventsControl = new FeaturedEventsControl();
                    featuredEventsControl.DataContext = this;
                    MainContentControl.Children.Add(featuredEventsControl);
                }
                else if (App.CurrentUserRole == 1) // Admin
                {
                    // Admin có quyền xem cả Featured Events và Created Events
                    var createdEventsControl = new CreatedEventsControl();
                    createdEventsControl.DataContext = this;
                    MainContentControl.Children.Add(createdEventsControl);

                    var featuredEventsControl = new FeaturedEventsControl();
                    featuredEventsControl.DataContext = this;
                    MainContentControl.Children.Add(featuredEventsControl);
                }
                else if (App.CurrentUserRole == 2) // User
                {
                    var registeredEventsControl = new RegisteredEventsControl();
                    registeredEventsControl.DataContext = this;
                    MainContentControl.Children.Add(registeredEventsControl);

                    var featuredEventsControl = new FeaturedEventsControl();
                    featuredEventsControl.DataContext = this;
                    MainContentControl.Children.Add(featuredEventsControl);
                }
                else if (App.CurrentUserRole == 3) // Dean
                {
                    // Dean có quyền xem cả Featured Events và Created Events
                    var createdEventsControl = new CreatedEventsControl();
                    createdEventsControl.DataContext = this;
                    MainContentControl.Children.Add(createdEventsControl);

                    var featuredEventsControl = new FeaturedEventsControl();
                    featuredEventsControl.DataContext = this;
                    MainContentControl.Children.Add(featuredEventsControl);
                }
            }
        }
        public void LoadData()
        {
            QuanlysukienContext db = new QuanlysukienContext();
            // Lấy danh sách sự kiện đã tạo
            var ListCreateEvent = (from c in db.Sukiens 
                                   where c.Mandb == App.CurrentUserMand
                                   select c).ToList();
            CreatedEvents = new ObservableCollection<Event>();
            // Chuyển dữ liệu từ List sang ObservableCollection
            foreach (var item in ListCreateEvent)
            {
                var Event = (from sk in db.Sukiens
                            join khoa in db.Khoas
                            on sk.Dvtc equals khoa.Makhoa
                            where sk.Mask == item.Mask && sk.Duyet == 1
                            select new Event
                            {
                                Mask = sk.Mask,
                                Title = sk.Tensk,
                                Date = sk.Ngaybatdau.ToString("HH:mm dd/MM/yyyy"),
                                Organizer = khoa.Tenkhoa,
                                ImagePath = ConvertByteArrayToImage(sk.Imageevent)
                            }).FirstOrDefault();
                if (Event != null)
                    CreatedEvents.Add(Event);
            }

            var ListRegisteredEvent = (from c in db.Dangkysukiens
                                       where c.Mand == App.CurrentUserMand
                                       select c).ToList();
            RegisteredEvents = new ObservableCollection<Event>();
            foreach (var item in ListRegisteredEvent)
            {
                var Event = (from sk in db.Sukiens
                             join khoa in db.Khoas
                             on sk.Dvtc equals khoa.Makhoa
                             where sk.Mask == item.Mask && sk.Duyet == 1
                             select new Event
                             {
                                 Mask = sk.Mask,
                                 Title = sk.Tensk,
                                 Date = sk.Ngaybatdau.ToString("HH:mm dd/MM/yyyy"),
                                 Organizer = khoa.Tenkhoa,
                                 ImagePath = ConvertByteArrayToImage(sk.Imageevent)
                             }).FirstOrDefault();
                if (Event != null)
                    RegisteredEvents.Add(Event);
            }

            var ListFeaturedEvent = (from c in db.Sukiens
                                     select c).ToList();
            FeaturedEvents = new ObservableCollection<Event>();
            foreach(var item in ListFeaturedEvent)
            {
                var Event = (from sk in db.Sukiens
                             join khoa in db.Khoas
                             on sk.Dvtc equals khoa.Makhoa
                             where sk.Mask == item.Mask && sk.Duyet == 1
                             select new Event
                             {
                                 Mask = sk.Mask,
                                 Title = sk.Tensk,
                                 Date = sk.Ngaybatdau.ToString("HH:mm dd/MM/yyyy"),
                                 Organizer = khoa.Tenkhoa,
                                 ImagePath = ConvertByteArrayToImage(sk.Imageevent)
                             }).FirstOrDefault();
                if (Event != null)
                    FeaturedEvents.Add(Event);
            }
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

        public class Event
        {
            public string Mask { get; set; }
            public string Title { get; set; }
            public string Date { get; set; }
            public string Organizer { get; set; }
            public BitmapImage ImagePath { get; set; }
        }
    }
}
