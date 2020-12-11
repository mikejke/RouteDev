using RouteDev.Commands;
using RouteDev.Data;
using RouteDev.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RouteDev.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;

        private Cargo _selected;
        private const int StorageX = 17;
        private const int StorageY = 14;
        public ObservableCollection<Shop> ShopList { get; set; }
        public List<Transport> Cars { get; set; }


        private RelayCommand _drawMap;
        public RelayCommand DrawMap
        {
            get
            {
                return _drawMap ??= new RelayCommand(obj =>
                {
                    if (!ShopList.Any()) MessageBox.Show("Таблица магазинов пуста.", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    for (var i = 0; i < Cars.Count; i++)
                    {
                        Cars[i].CalculateRoutes(ShopList);
                        
                        if (i == Cars.Count() - 1 && ShopList.Any(x => x.AnyNeed()))
                        {
                            Cars.Add(
                                new Transport(TransportType.Hired)
                                {
                                    Products = 50,
                                    Chemistry = 50,
                                    Drinks = 50
                                });
                        }
                    }

                    var rm = new RouteMap(ShopList, Cars);
                    rm.Show();
                });
            }
        }

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        if (_dialogService.OpenFileDialog() == true)
                        {
                            var phones = _fileService.Open(_dialogService.FilePath);
                            ShopList.Clear();
                            foreach (var p in phones)
                                ShopList.Add(p);
                            //_dialogService.ShowMessage("Файл открыт");
                        }
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage(ex.Message);
                    }
                });
            }
        }

        public Cargo Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            this._dialogService = dialogService;
            this._fileService = fileService;
            //moq
            ShopList = new ObservableCollection<Shop>();

            Cars = new List<Transport>
            {
                new Transport(TransportType.Own)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                },
                new Transport(TransportType.Own)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                },
                new Transport(TransportType.Own)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                },
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}