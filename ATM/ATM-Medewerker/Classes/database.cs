using ATM_Medewerker.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATM_Medewerker.Classes
{

    internal class database
    {
        dns dns = new dns();
        public bool insertAccoount(string accountNumberString, string pincode)
        {

            using (MySqlConnection connection = new MySqlConnection(dns.connectionString))
            {
                connection.Open();

                string sqlQuery = "INSERT INTO `rekening`(`rekening_nummer`, `pincode`) VALUES ('" + accountNumberString + "','" + pincode + "')";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool blockAccount(string accountNumberString)
        {
            using (MySqlConnection connection = new MySqlConnection(dns.connectionString))
            {
                connection.Open();

                string sqlQuery = "UPDATE rekening SET blocked = 0 WHERE rekening_nummer = '" + accountNumberString + "'";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool deBlockAccount(string accountNumberString)
        {
            using (MySqlConnection connection = new MySqlConnection(dns.connectionString))
            {
                connection.Open();

                string sqlQuery = "UPDATE rekening SET blocked = 1 WHERE rekening_nummer = '" + accountNumberString + "'";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool checkEmployee(string name, string password)
        {
            return true;
        }
        public int getBalance(string accountNumberString)
        {
            string connectionString = dns.connectionString;

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
            string connectionString = dns.connectionString;

            int newSaldo = getBalance(accountNumberString) - amount;

            if (newSaldo < 0)
            {
                MessageBox.Show("U kan niet zoveel geld van rekening afhalen, U kan maximaal " + getBalance(accountNumberString) + " euro ophalen.");

                return false;
            }
            else
            {
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
            string connectionString = dns.connectionString;

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
    }
}
