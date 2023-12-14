using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace ATM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Database database = new Database();
        Settings settings = new Settings();

        private string actions = null;
        private string secretPinConsoleCombi = null;

        public MainWindow()
        {
            InitializeComponent();
            circleButtonOne.Click += CircleButton_Click;
        }

        // Event handler for the button's Click event
        private void CircleButton_Click(object sender, RoutedEventArgs e)
        {
            if(database.isNotBlocked(account_number_value.Text.ToString()))
            {
                secondMainLabel.Content = "Fout: Rekening geblokkeerd";
            } else if (database.findAccountNumber(account_number_value.Text.ToString()))
            {
                settings.accountNumber = account_number_value.Text;

                startGrid.Visibility = Visibility.Hidden;
                secondGrid.Visibility = Visibility.Visible;

                PinConsoleConfirm.IsEnabled = true;
            }
        }

        private void addSecretPinConsoleCombi(string number)
        {
            secretPinConsoleCombi = secretPinConsoleCombi + number;
            pinConsoleScreen.Content = new string('*', secretPinConsoleCombi.Length);
        }

        private void PinConsoleDelete(object sender, RoutedEventArgs e)
        {
            deleteSecretPinCombi();
        }

        private void PinConsoleButttonOne(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("1");
        }

        private void PinConsoleButttonTwo(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("2");
        }

        private void PinConsoleButtonThree(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("3");
        }

        private void PinConsoleButttonFour(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("4");
        }

        private void PinConsoleButttonFive(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("5");
        }

        private void PinConsoleButttonSix(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("6");
        }

        private void PinConsoleButttonSeven(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("7");
        }

        private void PinConsoleButttonEight(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("8");
        }

        private void PinConsoleButttonNine(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("9");
        }

        private void PinConsoleButtonZero(object sender, RoutedEventArgs e)
        {
            addSecretPinConsoleCombi("0");
        }

        private void PinConsoleButtonConfirm(object sender, RoutedEventArgs e)
        {
            string enteredPin = secretPinConsoleCombi;
            string expectedPin = database.getPinCode(settings.accountNumber);

            if (database.VerifySHA256Hash(enteredPin, expectedPin))
            {
                secondGridLabel.Content = "Welkom, kies je functie";
                PinConsoleConfirm.IsEnabled = false;

                deleteSecretPinCombi();


                sideGridOne.Visibility = Visibility.Hidden;
                sideGridTwo.Visibility = Visibility.Hidden;
                sideGridThree.Visibility = Visibility.Visible;
                sideGridFour.Visibility = Visibility.Visible;
            }
            else
            {
                secondGridLabel.Content = "Incorrect pin code";
            }
        }

        private void deleteSecretPinCombi()
        {
            secretPinConsoleCombi = null;
            pinConsoleScreen.Content = "";
        }

        private void circleButton11Action(object sender, RoutedEventArgs e)
        {
            int balance = database.getBalance(settings.accountNumber);

            secondGridLabel.Content = "Uw saldo is: " + balance.ToString();
        }

        private void circleButton12Action(object sender, RoutedEventArgs e)
        {
            if (database.checkLimit(settings.accountNumber))
            {
                secondGridLabel.Content = "U heeft al 3x vandaag geld opgenomen";
            }
            else
            {
                actions = "Opnemen";

                sideGridThree.Visibility = Visibility.Hidden;
                sideGridFour.Visibility = Visibility.Hidden;
                sideGridFive.Visibility = Visibility.Visible;
                sideGridSix.Visibility = Visibility.Visible;

                secondGridLabel.Content = "Kies een bedrag";
            }
        }

        private void circleButton21Action(object sender, RoutedEventArgs e)
        {
            sideGridThree.Visibility = Visibility.Visible;
            sideGridFour.Visibility = Visibility.Visible;
            sideGridFive.Visibility = Visibility.Hidden;
            sideGridSix.Visibility = Visibility.Hidden;

            secondGridLabel.Content = "Kies een functie";
        }

        private void circleButton22Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 10))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            } else
            {
                if(database.deposite(settings.accountNumber, 10))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton23Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 20))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 20))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton24Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 50))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 50))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton25Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 100))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 100))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton27Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 200))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 200))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton28Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 300))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 300))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton29Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 400))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 400))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton30Action(object sender, RoutedEventArgs e)
        {
            if (actions == "Opnemen")
            {
                if (database.withdraw(settings.accountNumber, 500))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }
            else
            {
                if (database.deposite(settings.accountNumber, 500))
                {
                    sideGridThree.Visibility = Visibility.Visible;
                    sideGridFour.Visibility = Visibility.Visible;
                    sideGridFive.Visibility = Visibility.Hidden;
                    sideGridSix.Visibility = Visibility.Hidden;

                    secondGridLabel.Content = "Kies een functie";
                }
            }

            actions = null;
        }

        private void circleButton13Action(object sender, RoutedEventArgs e)
        {
            actions = "Storten";

            sideGridThree.Visibility = Visibility.Hidden;
            sideGridFour.Visibility = Visibility.Hidden;
            sideGridFive.Visibility = Visibility.Visible;
            sideGridSix.Visibility = Visibility.Visible;

            secondGridLabel.Content = "Kies een bedrag";
        }
    }
}
