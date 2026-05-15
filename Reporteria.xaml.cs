using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MiBankito.Services;

namespace MiBankito
{
    /// <summary>
    /// Lógica de interacción para Reporteria.xaml
    /// </summary>
    public partial class Reporteria : UserControl
    {
        public Reporteria()
        {
            InitializeComponent();
            BindData();
            AppState.Changed += AppState_Changed;
            this.Unloaded += Reporteria_Unloaded;
        }

        private void AppState_Changed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => UpdateKpis());
        }

        private void UpdateKpis()
        {
            if (KpiTodayText != null) KpiTodayText.Text = AppState.TransactionsTodayCount().ToString();
            if (KpiIncomeText != null) KpiIncomeText.Text = $"C$ {AppState.TotalAmountToday():N2}";
        }

        private void BindData()
        {
            ResultsList.ItemsSource = AppState.Transactions.ToList();
            UpdateKpis();
        }

        private string GetPlatformFilter()
        {
            if (PlatformCombo?.SelectedItem is ComboBoxItem item)
            {
                var tag = item.Tag as string;
                return tag == "*" ? "*" : item.Content?.ToString();
            }
            return "*";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var home = Window.GetWindow(this) as Home;
            if (home != null)
            {
                home.HideMainContent();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var plat = GetPlatformFilter();
            DateTime? from = FromDate.SelectedDate;
            DateTime? to = null; // No longer a ToDate picker in the new UI
            var data = AppState.Filter(plat, string.Empty, from, to).ToList();
            ResultsList.ItemsSource = data;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var data = ResultsList.Items.Cast<object>().OfType<MiBankito.Models.TransactionRecord>().ToList();
            if (data.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("Proveedor,Servicio,Referencia,Documento Cliente,Monto,Moneda,Fecha de Creación,ID Cajero,Autorización,Transacción RMH");
            foreach (var r in data)
            {
                var fecha = r.Timestamp.ToString(CultureInfo.CurrentCulture);
                sb.AppendLine($"\"{r.Platform}\",\"{r.Service}\",\"{r.Reference}\",\"{r.CustomerDocument}\",{r.Amount.ToString("0.00", CultureInfo.InvariantCulture)},\"{r.Currency}\",\"{fecha}\",\"{r.CashierId}\",\"{r.Authorization}\",\"{r.RmhTransactionId}\"");
            }
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "sbf-transacciones.csv");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"Exportado a {path}", "Exportación", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reporteria_Unloaded(object sender, RoutedEventArgs e)
        {
            AppState.Changed -= AppState_Changed;
            this.Unloaded -= Reporteria_Unloaded;
        }
    }
}
