namespace RouteDev.Interfaces
{
    public interface IDialogService
    {
        string FilePath { get; set; }  
        bool OpenFileDialog();
        bool SaveFileDialog();
        void ShowMessage(string message);
    }
}
