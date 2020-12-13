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
using RouteDev.Services;

namespace RouteDev.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;

        private Cargo _selected;
        public ObservableCollection<Shop> ShopList { get; set; }
        public List<Transport> Cars { get; set; }


        private RelayCommand _drawCommand;
        public RelayCommand DrawCommand
        {
            get
            {
                return _drawCommand ??= new RelayCommand(obj =>
                {
                    if (!ShopList.Any()) MessageBox.Show("В списке нет магазинов.", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    for (var i = 0; i < Cars.Count; i++)
                    {
                        Cars[i].CalculateRoutes(ShopList);
                        //in case if its the last car and any shop still needs something we hire a new one
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

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        if (_dialogService.SaveFileDialog() == true)
                        {
                            _fileService.Save(_dialogService.FilePath, ShopList.ToList(), Cars);
                        }
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage(ex.Message);
                    }
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
                            var shopList = _fileService.Open(_dialogService.FilePath);
                            ShopList.Clear();
                            foreach (var shop in shopList)
                                ShopList.Add(shop);
                        }
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage(ex.Message);
                    }
                });
            }
        }

        private RelayCommand _showExpenses;
        public RelayCommand ShowExpenses
        {
            get
            {
                if (Cars.Sum(c => c.Distance) == 0)
                    return null;
                return _showExpenses ??= new RelayCommand(obj =>
                {
                    MessageBox.Show(string.Join('-', Cars.Select(car => car.Expenses)));
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
            this._fileService = fileService;
            this._dialogService = dialogService;
            //adding first three cars..
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