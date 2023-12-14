using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ATM_Medewerker.Classes;
using MySql.Data.MySqlClient;

namespace ATM_Medewerker.Windows
{
    /// <summary>
    /// Interaction logic for index.xaml
    /// </summary>
    public partial class index : Window
    {
        dns dns = new dns();
        hashing hashing = new hashing();
        database database = new database();

        private string accountName = null;

        public index()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            MySqlConnection connection = new MySqlConnection(dns.connectionString);

            try
            {
                connection.Open();

                string query = "SELECT * FROM rekening";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Set the DataTable as the ItemsSource for the ListView
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void searchKeyUp(object sender, KeyEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(dns.connectionString);

            try
            {
                dataGrid.ItemsSource = null;

                connection.Open();

                string query = "SELECT * FROM rekening WHERE rekening_nummer LIKE '" + searchBox.Text + "%'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Set the DataTable as the ItemsSource for the ListView
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void makeAccountButton(object sender, RoutedEventArgs e)
        {
            string hash = hashing.CreateSHA256Hash(pincodeBox.Text);

            if (database.insertAccoount(accountNumberBox.Text, hash))
            {
                MessageBox.Show("Gelukt");

                accountNumberBox.Text = null;
                pincodeBox.Text = null;

                searchAccount.Visibility = Visibility.Visible;
                makeAccountGrid.Visibility = Visibility.Hidden;
            }
        }

        private void closeMainGrid(object sender, RoutedEventArgs e)
        {
            searchAccount.Visibility = Visibility.Hidden;
            makeAccountGrid.Visibility = Visibility.Visible;
        }

        private void blockAcountClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView selectedItem = (DataRowView)button.DataContext;
            string rekeningNummer = selectedItem["rekening_nummer"].ToString();

            MessageBoxResult result = MessageBox.Show("Weet je zeker dat je rekening " + rekeningNummer + " wil blokkeren?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                database.deBlockAccount(rekeningNummer);

                LoadData();
            }
        }

        private void deBlockAccountClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView selectedItem = (DataRowView)button.DataContext;
            string rekeningNummer = selectedItem["rekening_nummer"].ToString();

            database.blockAccount(rekeningNummer);

            LoadData();
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            searchAccount.Visibility = Visibility.Visible;
            makeAccountGrid.Visibility = Visibility.Hidden;

            accountNumberBox.Text = null;
            pincodeBox.Text = null;

            LoadData();
        }

        private void backClick2(object sender, RoutedEventArgs e)
        {
            changeAccountGrid.Visibility = Visibility.Hidden;
            searchAccount.Visibility = Visibility.Visible;

            newSaldoBox.Text = null;

            LoadData();
        }

        private void showChangeSaldoGrid(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView selectedItem = (DataRowView)button.DataContext;
            string rekeningNummer = selectedItem["rekening_nummer"].ToString();

            accountName = rekeningNummer;

            changeAccountGrid.Visibility = Visibility.Visible;
            searchAccount.Visibility = Visibility.Hidden;

            LoadData();
        }

        private void changeSaldo(object sender, RoutedEventArgs e)
        {
            if(OpnemenItem.IsSelected)
            {
                if(database.withdraw(accountName, int.Parse(newSaldoBox.Text))) {
                    changeAccountGrid.Visibility = Visibility.Hidden;
                    searchAccount.Visibility = Visibility.Visible;

                    newSaldoBox.Text = null;
                }

                LoadData();
            }
            else if (StortenItem.IsSelected)
            {
                if (database.deposite(accountName, int.Parse(newSaldoBox.Text)))
                {
                    changeAccountGrid.Visibility = Visibility.Hidden;
                    searchAccount.Visibility = Visibility.Visible;

                    newSaldoBox.Text = null;
                }

                LoadData();
            } else
            {
                MessageBox.Show("Selecteer een optie");
            }
        }
    }
}