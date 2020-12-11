using RouteDev.ViewModels;
using System;
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
