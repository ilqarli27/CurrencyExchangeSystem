using System.Windows;
using Wpfclient.ServiceReference1;


namespace WpfClient
{
    public partial class MainWindow : Window
    {
        Service1Client client = new Service1Client();

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
            txtAccountResult.Text = client.Login(txtUsername.Text, txtPassword.Password);
        }

        private void TopUp_Click(object sender, RoutedEventArgs e)
        {
            decimal amount = decimal.Parse(txtTopUp.Text);
            txtAccountResult.Text = client.TopUp(txtUsername.Text, amount);
        }

        private void GetBalance_Click(object sender, RoutedEventArgs e)
        {
            txtAccountResult.Text = client.GetBalance(txtUsername.Text);
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            decimal amount = decimal.Parse(txtExAmount.Text);
            txtExResult.Text = client.BuyCurrency(txtExUsername.Text, txtExCurrency.Text, amount);
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            decimal amount = decimal.Parse(txtExAmount.Text);
            txtExResult.Text = client.SellCurrency(txtExUsername.Text, txtExCurrency.Text, amount);
        }

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            txtHistResult.Text = client.GetTransactionHistory(txtHistUsername.Text);
        }

        private void GetHistoricalRates_Click(object sender, RoutedEventArgs e)
        {
            txtHistResult.Text = client.GetHistoricalRates(txtHistCurrency.Text, txtStartDate.Text, txtEndDate.Text);
        }
    }
}