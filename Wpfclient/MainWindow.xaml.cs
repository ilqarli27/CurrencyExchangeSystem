using System.Windows;
using Wpfclient.ServiceReference1;


namespace WpfClient
{
    public partial class MainWindow : Window
    {
        Service1Client client = new Service1Client();
        private string loggedInUser = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetRate_Click(object sender, RoutedEventArgs e)
        {
            txtRateResult.Text = client.GetRate(txtCurrency.Text);
        }

        private void GetAllRates_Click(object sender, RoutedEventArgs e)
        {
            txtRateResult.Text = client.GetAllRates();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            txtAccountResult.Text = client.Register(txtUsername.Text, txtPassword.Password);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string result = client.Login(txtUsername.Text, txtPassword.Password);
            txtAccountResult.Text = result;

            if (result.StartsWith("Success"))
            {
                loggedInUser = txtUsername.Text;
                txtExUsername.Text = loggedInUser;
                txtHistUsername.Text = loggedInUser;
                this.Title = "Currency Exchange Office — " + loggedInUser;
                MessageBox.Show("Welcome " + loggedInUser + "!", "Login successful");
            }
        }

        private void TopUp_Click(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == "")
            {
                MessageBox.Show("Please login first!", "Error");
                return;
            }
            decimal amount = decimal.Parse(txtTopUp.Text);
            txtAccountResult.Text = client.TopUp(loggedInUser, amount);
        }

        private void GetBalance_Click(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == "")
            {
                MessageBox.Show("Please login first!", "Error");
                return;
            }
            txtAccountResult.Text = client.GetBalance(loggedInUser);
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == "")
            {
                MessageBox.Show("Please login first!", "Error");
                return;
            }

            if (txtExCurrency.Text == "" || txtExAmount.Text == "")
            {
                MessageBox.Show("Please enter currency code and amount!", "Error");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtExAmount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount!", "Error");
                return;
            }

            string result = client.BuyCurrency(loggedInUser, txtExCurrency.Text, amount);
            txtExResult.Text = result;

            if (result.StartsWith("Success"))
                MessageBox.Show(result, "Purchase successful");
            else
                MessageBox.Show(result, "Error");
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == "")
            {
                MessageBox.Show("Please login first!", "Error");
                return;
            }

            if (txtExCurrency.Text == "" || txtExAmount.Text == "")
            {
                MessageBox.Show("Please enter currency code and amount!", "Error");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtExAmount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount!", "Error");
                return;
            }

            string result = client.SellCurrency(loggedInUser, txtExCurrency.Text, amount);
            txtExResult.Text = result;

            if (result.StartsWith("Success"))
                MessageBox.Show(result, "Sale successful");
            else
                MessageBox.Show(result, "Error");
        }

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == "")
            {
                MessageBox.Show("Please login first!", "Error");
                return;
            }
            txtHistResult.Text = client.GetTransactionHistory(loggedInUser);
        }

        private void GetHistoricalRates_Click(object sender, RoutedEventArgs e)
        {
            txtHistResult.Text = client.GetHistoricalRates(txtHistCurrency.Text, txtStartDate.Text, txtEndDate.Text);
        }
    }
}