using System;
using System.ComponentModel;
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
            //binding view model to data context:
            //TODO: also find a way to chose formats
            DataContext = new ApplicationViewModel(new DefaultDialogService(), new XlsFileService());
        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(200);
        }
    }
}
