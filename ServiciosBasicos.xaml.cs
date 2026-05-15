using System;
using System.Globalization;
using System.Linq;
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
    /// <summary>
    /// Lógica de interacción para Pagos.xaml
    /// </summary>
    public partial class ServiciosBasicos : UserControl
    {
        private bool _isLoaded;
        private ObservableCollection<TransactionRecord> _detalleRecords = new ObservableCollection<TransactionRecord>();
        private TransactionRecord _currentRecord;
        private string _selectedCode = "";
        private string _selectedService = "";

        public string SelectedServiceName { get; set; }

        public ServiciosBasicos()
        {
            InitializeComponent();
            Loaded += (s, e) => { _isLoaded = true; LoadServiceChips(); UpdateDetail(); };
            DetalleDataGrid.ItemsSource = _detalleRecords;
            ClearDetail();
        }

        private void ClearDetail()
        {
            _selectedCode = "";
            _selectedService = "";
            if (ReferenceText != null) ReferenceText.Text = string.Empty;
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

            // Asegurar que el filtro se actualice al recibir el encabezado
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                if (string.IsNullOrWhiteSpace(SelectedServiceName))
                    SelectedServiceName = serviceName; // no sobrescribir si ya lo estableció Home
                if (_isLoaded)
                    LoadServiceChips();
            }
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
                Reference = ReferenceText?.Text ?? "",
                DocumentNumber = "",
                Currency = cur,
                Amount = amt
            };
            _detalleRecords.Clear();
            _detalleRecords.Add(record);
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
                if ((CurCordobas?.IsChecked != true) && (CurDolares?.IsChecked != true))
                {
                    MessageBox.Show("Seleccione una divisa.");
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
                var reference = ReferenceText?.Text;
                _currentRecord = new TransactionRecord
                {
                    Platform = plat,
                    Service = srv,
                    Code = code,
                    Currency = cur,
                    Amount = amt,
                    Reference = reference,
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
        }

        private void ServiceChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                SetServiceChip(btn);
                var tag = btn.Tag?.ToString() ?? "";
                var parts = tag.Split('|');
                _selectedCode = parts.Length > 0 ? parts[0] : "";
                _selectedService = parts.Length > 2 ? parts[2] : btn.Content?.ToString();
                // UpdateDetail(); // Removed to keep grid empty until Consultar is clicked
            }
        }

        private void SetServiceChip(ToggleButton active)
        {
            foreach (var child in ServiceWrapPanel.Children)
            {
                if (child is ToggleButton tb)
                {
                    tb.IsChecked = tb == active;
                }
            }
        }

        private void LoadServiceChips()
        {
            ServiceWrapPanel.Children.Clear();

            // Normalizar para comparación (sin espacios y mayúsculas)
            Func<string, string> Normalize = v => string.IsNullOrWhiteSpace(v)
                ? string.Empty
                : new string(v.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();

            var selectedNorm = Normalize(SelectedServiceName);
            var filtered = DataCatalog.Services;

            if (!string.IsNullOrWhiteSpace(selectedNorm))
            {
                filtered = DataCatalog.Services
                    .Where(s => Normalize(s.name).Contains(selectedNorm))
                    .ToArray();
            }

            // Si no hubo coincidencias, no mostrar todos; mantener la intención del filtro
            // pero hacer la coincidencia más flexible usando IndexOf ignorando mayúsculas
            if (filtered.Length == 0 && !string.IsNullOrWhiteSpace(SelectedServiceName))
            {
                filtered = DataCatalog.Services
                    .Where(s => (s.name ?? string.Empty)
                        .IndexOf(SelectedServiceName, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .ToArray();
            }

            foreach (var s in filtered.OrderBy(s => s.name))
            {
                var chip = new ToggleButton
                {
                    Content = s.name,
                    Style = (Style)FindResource("CapsuleChipStyle"),
                    Margin = new Thickness(4),
                    Tag = $"{s.code}|{s.platform}|{s.name}"
                };
                chip.Click += ServiceChip_Click;
                ServiceWrapPanel.Children.Add(chip);
            }
        }

        private void CloseHostWindow()
        {
            var wnd = Window.GetWindow(this);
            if (wnd != null)
            {
                wnd.Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
