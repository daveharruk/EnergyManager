using Dapper;
using System.Threading.Tasks;

namespace EnergyManager.Data.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Account(int accountId, string firstName, string lastName)
        {
            AccountId = accountId;
            FirstName = firstName;
            LastName = lastName;
        }

        public Account()
        {
            // Required for Dapper object initialisation
        }

        public void Persist()
        {
            var conn = DataHelper.GetConnection();
            var parameters = new DynamicParameters(this);
            conn.Execute("insert into Account(AccountId, FirstName, LastName) values(@AccountId, @FirstName, @LastName)", parameters);
        }

        public static List<Account> GetAll()
        {
            var conn = DataHelper.GetConnection();
            return conn.Query<Account>("select * from Account").ToList();
        }
    }
}
