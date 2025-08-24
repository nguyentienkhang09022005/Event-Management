using Demo1.Pages.Admin;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Demo1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string CurrentUserMand { get; set; }
        public static int CurrentUserRole { get; set; }
        public static class SharedData
        {
            public static ObservableCollection<DsPheDuyet.Event> DanhsachPheDuyet { get; set; } = new ObservableCollection<DsPheDuyet.Event>();
            public static ObservableCollection<DsPheDuyet.Event> CreatedEvents { get; set; } = new ObservableCollection<DsPheDuyet.Event>();

        }
    }

}
