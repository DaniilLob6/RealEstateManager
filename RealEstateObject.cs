using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateManager
{
    public class RealEstateObject : INotifyPropertyChanged
    {
        private int _id;
        private int _ownerId;
        private int _statusId;
        private int _cityId;
        private int _propertyTypeId;
        private string _address;
        private decimal _area;
        private int _floor;
        private int _roomCount;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }
        public int OwnerId
        {
            get => _ownerId;
            set { _ownerId = value; OnPropertyChanged(); }
        }
        public int StatusId
        {
            get => _statusId;
            set { _statusId = value; OnPropertyChanged(); }
        }
        public int CityId
        {
            get => _cityId;
            set { _cityId = value; OnPropertyChanged(); }
        }
        public int PropertyTypeId
        {
            get => _propertyTypeId;
            set { _propertyTypeId = value; OnPropertyChanged(); }
        }
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }
        public decimal Area
        {
            get => _area;
            set { _area = value; OnPropertyChanged(); }
        }
        public int Floor
        {
            get => _floor;
            set { _floor = value; OnPropertyChanged(); }
        }
        public int RoomCount
        {
            get => _roomCount;
            set { _roomCount = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}