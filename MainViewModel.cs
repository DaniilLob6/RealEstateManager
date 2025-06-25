using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Npgsql;

namespace RealEstateManager
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly RealEstateObjectRepository _repository;
        private ObservableCollection<RealEstateObject> _objects;
        private RealEstateObject _selectedObject;
        private RealEstateObject _newObject;

        public ObservableCollection<RealEstateObject> Objects
        {
            get => _objects;
            set { _objects = value; OnPropertyChanged(); }
        }

        public RealEstateObject SelectedObject
        {
            get => _selectedObject;
            set { _selectedObject = value; OnPropertyChanged(); }
        }

        public RealEstateObject NewObject
        {
            get => _newObject;
            set { _newObject = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ImportJsonCommand { get; }

        public MainViewModel()
        {
            try
            {
                var connectionString = "Host=localhost;Port=5432;Database=real_estate_db;Username=postgres;Password=sa;";
                var factory = new NpgsqlConnectionFactory(connectionString);
                _repository = new RealEstateObjectRepository(factory);
                Objects = _repository.GetAll();
                NewObject = new RealEstateObject();

                AddCommand = new RelayCommand(Add, CanAdd);
                UpdateCommand = new RelayCommand(Update, CanUpdate);
                DeleteCommand = new RelayCommand(Delete, CanDelete);
                ExportJsonCommand = new RelayCommand(ExportJson);
                ImportJsonCommand = new RelayCommand(ImportJson);
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAdd(object parameter)
        {
            return NewObject != null &&
                   !string.IsNullOrEmpty(NewObject.Address) &&
                   NewObject.Area > 0 &&
                   NewObject.OwnerId > 0 &&
                   NewObject.StatusId > 0 &&
                   NewObject.CityId > 0 &&
                   NewObject.PropertyTypeId > 0;
        }

        private void Add(object parameter)
        {
            try
            {
                _repository.Add(NewObject);
                Objects.Add(NewObject);
                NewObject = new RealEstateObject();
                MessageBox.Show("Объект успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdate(object parameter)
        {
            return SelectedObject != null &&
                   !string.IsNullOrEmpty(SelectedObject.Address) &&
                   SelectedObject.Area > 0 &&
                   SelectedObject.OwnerId > 0 &&
                   SelectedObject.StatusId > 0 &&
                   SelectedObject.CityId > 0 &&
                   SelectedObject.PropertyTypeId > 0;
        }

        private void Update(object parameter)
        {
            try
            {
                _repository.Update(SelectedObject);
                MessageBox.Show("Объект успешно обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete(object parameter)
        {
            return SelectedObject != null;
        }

        private void Delete(object parameter)
        {
            try
            {
                _repository.Delete(SelectedObject.Id);
                Objects.Remove(SelectedObject);
                MessageBox.Show("Объект успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportJson(object parameter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _repository.ExportToJson(dialog.FileName, Objects);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportJson(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var importedObjects = _repository.ImportFromJson(dialog.FileName);
                    Objects.Clear();
                    foreach (var obj in importedObjects)
                    {
                        Objects.Add(obj);
                    }
                    MessageBox.Show("Данные импортированы из JSON.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при импорте JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}