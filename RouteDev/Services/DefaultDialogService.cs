using Microsoft.Win32;
using RouteDev.Interfaces;
using System.Windows;

namespace RouteDev.Services
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog 
            {
                Filter = "Excel|*.xlsx;*.xls;*.xlsm"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel|*.xlsx;*.xls;*.xlsm",
                FileName = "output",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
