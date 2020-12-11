using RouteDev.ViewModels;
using System.Windows;
using RouteDev.Services;

namespace RouteDev
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // We use MVVM pattern in this application, which means we are using view model to display bonded with a view data:
            DataContext = new ApplicationViewModel(new DefaultDialogService(), new TxtFileService());
        }
    }
}
