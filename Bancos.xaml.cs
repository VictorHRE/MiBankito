using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MiBankito.Models;
using MiBankito.Services;
using RetailHero.POS.Core.Shared.Extensions;
using System.Collections.ObjectModel;

namespace MiBankito
{
    public partial class Bancos : UserControl
    {
        private bool _isLoaded;
        private ObservableCollection<TransactionRecord> _detalleRecords = new ObservableCollection<TransactionRecord>();
        private TransactionRecord _currentRecord;
        private string _selectedCode = "";
        private string _selectedService = "";

        public Bancos()
        {
            InitializeComponent();
            Loaded += (s, e) => { _isLoaded = true; UpdateDetail(); };
            DetalleDataGrid.ItemsSource = _detalleRecords;
            ClearDetail();
        }

        private void ClearDetail()
        {
            _selectedCode = "";
            _selectedService = "";
            if (AccountText != null) AccountText.Text = string.Empty;
            if (AmountText != null) AmountText.Text = "0.00";
            if (HeaderServiceName != null) HeaderServiceName.Text = string.Empty;
            if (HeaderServiceLogo != null) HeaderServiceLogo.Source = null;
            _detalleRecords.Clear();
            _currentRecord = null;
            if (ActionButton != null)
            {
                ActionButton.Content = "Consultar";
                ActionButton.Background = (Brush)FindResource("SuccessGreenBrush");
            }
        }

        public void SetHeader(string serviceName, ImageSource logoSource)
        {
            _selectedService = serviceName;
            if (HeaderServiceName != null) HeaderServiceName.Text = serviceName ?? string.Empty;
            if (HeaderServiceLogo != null) HeaderServiceLogo.Source = logoSource;
        }

        private string GetCurrency()
        {
            var item = CurrencyCombo?.SelectedItem as ComboBoxItem;
            if (item == null)
            {
                if (CurDolares != null && CurDolares.IsChecked == true) return "USD";
                return "NIO";
            }
            return item.Content?.ToString()?.Contains("USD") == true ? "USD" : "NIO";
        }

        private decimal GetAmount()
        {
            decimal.TryParse(AmountText?.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var amt);
            return amt;
        }

        private void UpdateDetail()
        {
            if (!_isLoaded) return;
            if (string.IsNullOrWhiteSpace(_selectedCode)) return;

            var cur = GetCurrency();
            var amt = GetAmount();

            // Poblar DataGrid de detalle
            var record = new TransactionRecord
            {
                Code = _selectedCode,
                Service = _selectedService,
                Reference = AccountText?.Text ?? "",
                DocumentNumber = DocNumberText?.Text ?? "",
                Currency = cur,
                Amount = amt
            };
            _detalleRecords.Clear();
            _detalleRecords.Add(record);
        }

        private void ServiceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceCombo?.SelectedItem is ComboBoxItem item)
            {
                var val = item.Content?.ToString();
                if (val == "PAGO_TARJETA")
                {
                    _selectedCode = "PTAR";
                    _selectedService = "PAGO TARJETA";
                    SetTransactionChip(TabPagoTarjeta);
                }
                else if (val == "DEPOSITO")
                {
                    _selectedCode = "DEP";
                    _selectedService = "DEPOSITO";
                    SetTransactionChip(TabDeposito);
                }
            }
        }

        private void AmountText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // UpdateDetail(); // Removed to keep grid empty until Consultar is clicked
        }

        private void CurrencyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrencyCombo?.SelectedIndex == 1) SetCurrencyChip(CurDolares); else SetCurrencyChip(CurCordobas);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var home = Window.GetWindow(this) as Home;
            if (home != null)
            {
                home.HideMainContent();
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionButton.Content.ToString() == "Consultar")
            {
                // Validar y mostrar en DataGrid
                if (string.IsNullOrWhiteSpace(_selectedCode))
                {
                    MessageBox.Show("Seleccione un servicio.");
                    return;
                }
                if ((CurCordobas?.IsChecked != true) && (CurDolares?.IsChecked != true) && (CurUsdToNio?.IsChecked != true))
                {
                    MessageBox.Show("Seleccione una divisa.");
                    return;
                }
                if (DocNumberText == null || string.IsNullOrWhiteSpace(DocNumberText.Text))
                {
                    MessageBox.Show("Ingrese el número de documento.");
                    return;
                }
                var amt = GetAmount();
                if (amt <= 0)
                {
                    MessageBox.Show("Ingrese un monto válido.");
                    return;
                }
                var plat = (PlatformCombo?.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var srv = _selectedService;
                var code = _selectedCode;
                var cur = GetCurrency();
                var reference = AccountText?.Text;
                var docType = (DocTypeCombo?.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var docNumber = DocNumberText?.Text;
                _currentRecord = new TransactionRecord
                {
                    Platform = plat,
                    Service = srv,
                    Code = code,
                    Currency = cur,
                    Amount = amt,
                    Reference = reference,
                    DocumentType = docType,
                    DocumentNumber = docNumber,
                    Timestamp = System.DateTime.Now
                };
                _detalleRecords.Clear();
                _detalleRecords.Add(_currentRecord);
                // Cambiar botón a Confirmar en azul
                ActionButton.Content = "Confirmar";
                ActionButton.Background = (Brush)FindResource("PrimaryIndigoBrush");
                MessageBox.Show("La consulta fue exitosa.");
            }
            else
            {
                // Confirmar: agregar a AppState
                if (_currentRecord != null)
                {
                    AppState.Add(_currentRecord);
                    ClearDetail();
                }
            }
        }

        private void TransactionChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                SetTransactionChip(btn);
                if (ServiceCombo != null)
                {
                    if (btn == TabPagoTarjeta)
                    {
                        ServiceCombo.SelectedIndex = 1;
                    }
                    else if (btn == TabDeposito)
                    {
                        ServiceCombo.SelectedIndex = 0;
                    }
                    else if (btn == TabRetiro)
                    {
                        _selectedCode = "RET";
                        _selectedService = "RETIRO";
                    }
                    else if (btn == TabPagoPrestamo)
                    {
                        _selectedCode = "PPRE";
                        _selectedService = "PAGO PRESTAMO";
                    }
                }
                // No llamar UpdateDetail aquí para mantener vacío hasta Consultar
            }
        }

        private void SetTransactionChip(ToggleButton active)
        {
            if (TabDeposito != null) TabDeposito.IsChecked = active == TabDeposito;
            if (TabPagoTarjeta != null) TabPagoTarjeta.IsChecked = active == TabPagoTarjeta;
            if (TabRetiro != null) TabRetiro.IsChecked = active == TabRetiro;
            if (TabPagoPrestamo != null) TabPagoPrestamo.IsChecked = active == TabPagoPrestamo;
        }

        private void CurrencyChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                SetCurrencyChip(btn);
                if (CurrencyCombo != null)
                {
                    if (btn == CurDolares) CurrencyCombo.SelectedIndex = 1; else CurrencyCombo.SelectedIndex = 0;
                }
                // UpdateDetail(); // Removed to keep grid empty until Consultar is clicked
            }
        }

        private void SetCurrencyChip(ToggleButton active)
        {
            if (CurCordobas != null) CurCordobas.IsChecked = active == CurCordobas;
            if (CurDolares != null) CurDolares.IsChecked = active == CurDolares;
            if (CurUsdToNio != null) CurUsdToNio.IsChecked = active == CurUsdToNio;
        }
    }
}
