using System;
using System.Collections.ObjectModel;
using System.Data;
using Npgsql;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using NpgsqlTypes;

namespace RealEstateManager
{
    public class RealEstateObjectRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RealEstateObjectRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public ObservableCollection<RealEstateObject> GetAll()
        {
            var objects = new ObservableCollection<RealEstateObject>();
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("SELECT id, owner_id, status_id, city_id, property_type_id, address, area, floor, room_count FROM real_estate_objects", (NpgsqlConnection)connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                objects.Add(new RealEstateObject
                                {
                                    Id = reader.GetInt32(0),
                                    OwnerId = reader.GetInt32(1),
                                    StatusId = reader.GetInt32(2),
                                    CityId = reader.GetInt32(3),
                                    PropertyTypeId = reader.GetInt32(4),
                                    Address = reader.GetString(5),
                                    Area = reader.GetDecimal(6),
                                    Floor = reader.GetInt32(7),
                                    RoomCount = reader.GetInt32(8)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return objects;
        }

        public void Add(RealEstateObject obj)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "INSERT INTO real_estate_objects (owner_id, status_id, city_id, property_type_id, address, area, floor, room_count) " +
                        "VALUES (@ownerId, @statusId, @cityId, @propertyTypeId, @address, @area, @floor, @roomCount) RETURNING id",
                        (NpgsqlConnection)connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@ownerId", NpgsqlDbType.Integer) { Value = obj.OwnerId });
                        command.Parameters.Add(new NpgsqlParameter("@statusId", NpgsqlDbType.Integer) { Value = obj.StatusId });
                        command.Parameters.Add(new NpgsqlParameter("@cityId", NpgsqlDbType.Integer) { Value = obj.CityId });
                        command.Parameters.Add(new NpgsqlParameter("@propertyTypeId", NpgsqlDbType.Integer) { Value = obj.PropertyTypeId });
                        command.Parameters.Add(new NpgsqlParameter("@address", NpgsqlDbType.Text) { Value = obj.Address ?? "" });
                        command.Parameters.Add(new NpgsqlParameter("@area", NpgsqlDbType.Numeric) { Value = obj.Area });
                        command.Parameters.Add(new NpgsqlParameter("@floor", NpgsqlDbType.Integer) { Value = obj.Floor });
                        command.Parameters.Add(new NpgsqlParameter("@roomCount", NpgsqlDbType.Integer) { Value = obj.RoomCount });
                        obj.Id = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Update(RealEstateObject obj)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "UPDATE real_estate_objects SET owner_id = @ownerId, status_id = @statusId, city_id = @cityId, " +
                        "property_type_id = @propertyTypeId, address = @address, area = @area, floor = @floor, room_count = @roomCount " +
                        "WHERE id = @id",
                        (NpgsqlConnection)connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Integer) { Value = obj.Id });
                        command.Parameters.Add(new NpgsqlParameter("@ownerId", NpgsqlDbType.Integer) { Value = obj.OwnerId });
                        command.Parameters.Add(new NpgsqlParameter("@statusId", NpgsqlDbType.Integer) { Value = obj.StatusId });
                        command.Parameters.Add(new NpgsqlParameter("@cityId", NpgsqlDbType.Integer) { Value = obj.CityId });
                        command.Parameters.Add(new NpgsqlParameter("@propertyTypeId", NpgsqlDbType.Integer) { Value = obj.PropertyTypeId });
                        command.Parameters.Add(new NpgsqlParameter("@address", NpgsqlDbType.Text) { Value = obj.Address ?? "" });
                        command.Parameters.Add(new NpgsqlParameter("@area", NpgsqlDbType.Numeric) { Value = obj.Area });
                        command.Parameters.Add(new NpgsqlParameter("@floor", NpgsqlDbType.Integer) { Value = obj.Floor });
                        command.Parameters.Add(new NpgsqlParameter("@roomCount", NpgsqlDbType.Integer) { Value = obj.RoomCount });
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("DELETE FROM real_estate_objects WHERE id = @id", (NpgsqlConnection)connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Integer) { Value = id });
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportToJson(string filePath, ObservableCollection<RealEstateObject> objects)
        {
            try
            {
                var json = JsonConvert.SerializeObject(objects, Formatting.Indented);
                File.WriteAllText(filePath, json);
                MessageBox.Show("Данные экспортированы в JSON.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<RealEstateObject> ImportFromJson(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var importedObjects = JsonConvert.DeserializeObject<ObservableCollection<RealEstateObject>>(json);
                return importedObjects ?? new ObservableCollection<RealEstateObject>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<RealEstateObject>();
            }
        }
    }
}