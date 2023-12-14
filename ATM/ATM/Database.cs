using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATM
{
    internal class Database
    {
        public Database()
        {
            //niks
        }

        public bool VerifySHA256Hash(string dataToVerify, string expectedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify));
                string computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return computedHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
            }
        }

        public string CreateSHA256Hash(string dataToHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString;
            }
        }

        public bool isNotBlocked(string accountNumber)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Connection is established

                // Example: Execute a SQL query
                string sqlQuery = "SELECT blocked FROM rekening WHERE rekening_nummer = '" + accountNumber + "'";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Move to the first row
                    {
                        // Check if the 'blocked' field is equal to 1
                        if (reader.GetInt16(0) == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // No rows were returned, handle this case accordingly
                        return false;
                    }
                }
            }
        }

        public bool findAccountNumber(string accountNumber)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Connection is established

                // Example: Execute a SQL query
                string sqlQuery = "SELECT * FROM rekening WHERE rekening_nummer = '" + accountNumber + "'";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public string getPinCode(string accountNumberString)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT pincode FROM rekening WHERE rekening_nummer = @accountNumber";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                cmd.Parameters.AddWithValue("@accountNumber", accountNumberString);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Check if a result was returned
                    {
                        return reader.GetString(0);
                    }
                }
            }

            return null;
        }

        public int getBalance(string accountNumberString)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT saldo FROM rekening WHERE rekening_nummer = @accountNumber";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                cmd.Parameters.AddWithValue("@accountNumber", accountNumberString);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
            }

            return 0;
        }

        public bool withdraw(string accountNumberString, int amount)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            int newSaldo = getBalance(accountNumberString) - amount;

            if (newSaldo < 0)
            {
                return false;
            } else {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery2 = "UPDATE rekening SET saldo = " + newSaldo + " WHERE rekening_nummer = '" + accountNumberString + "'";
                    string sqlQuery = "INSERT INTO `transactions`(`rekening_nummer`, `amount`, `type`) VALUES ('" + accountNumberString + "','" + amount.ToString() + "','OPNEMEN')";

                    MySqlCommand cmd = new MySqlCommand(sqlQuery2, connection);
                    MySqlCommand cmd2 = new MySqlCommand(sqlQuery, connection);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    int result = cmd2.ExecuteNonQuery();

                    if (rowsAffected > 0 && result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool deposite(string accountNumberString, int amount)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";

            int newSaldo = getBalance(accountNumberString) + amount;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery2 = "UPDATE rekening SET saldo = " + newSaldo + " WHERE rekening_nummer = '" + accountNumberString + "'";
                string sqlQuery = "INSERT INTO `transactions`(`rekening_nummer`, `amount`, `type`) VALUES ('" + accountNumberString + "','" + amount.ToString() + "','STORTEN')";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                MySqlCommand cmd2 = new MySqlCommand(sqlQuery2, connection);

                int rowsAffected = cmd.ExecuteNonQuery();
                int result = cmd2.ExecuteNonQuery();

                if (rowsAffected > 0 && result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool checkLimit(string accountNumberString)
        {
            string connectionString = "Server=localhost;Uid=root;Pwd=;Database=atm";
            string date = DateTime.Now.ToString("yyyy/MM/dd");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT COUNT(*) FROM transactions WHERE date LIKE '" + date + "%' AND rekening_nummer = '" + accountNumberString + "' AND type = 'OPNEMEN'";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Check if a result was returned
                    {
                        int number = int.Parse(reader.GetString(0));

                        if(number > 3)
                        {
                            return true;
                        } else
                        {
                            return false;
                        }
                    } else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
