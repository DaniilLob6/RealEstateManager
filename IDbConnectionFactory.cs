using System.Data;

namespace RealEstateManager
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}