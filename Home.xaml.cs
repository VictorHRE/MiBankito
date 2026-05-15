using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MiBankito.Models;
using MiBankito.Services;
using MaterialDesignThemes.Wpf;

namespace MiBankito
{
    public partial class Home : Window
    {
        public string pantalla { get; set; }
        private Button _activeButton;
        private string _currentCategory;
        private bool _isAscending = true;

        public Home(string pantalla)
        {
            InitializeComponent();
            // KPIs removed from Home header; no subscription here
            this.WindowState = WindowState.Maximized;
            this.pantalla = pantalla;

            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial active button and load services
            //_activeButton = ServiciosBtn;
            //_activeButton.Tag = "Active";
            _currentCategory = "Bancos";
            if (SearchTextBox != null) SearchTextBox.Text = string.Empty;
            if (SortAscendingButton != null) SortAscendingButton.Tag = "Active";
            if (SortDescendingButton != null) SortDescendingButton.Tag = "";
            LoadServices(_currentCategory);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowInMainContent(UserControl control)
        {
            MainContent.Content = control;
            pantalla = control.GetType().Name;
            ServicesView.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
            HeaderBorder.Visibility = Visibility.Collapsed;
            var grid = HeaderBorder.Parent as Grid;
            if (grid != null)
            {
                grid.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        public void HideMainContent()
        {
            MainContent.Content = null;
            MainContent.Visibility = Visibility.Collapsed;
            ServicesView.Visibility = Visibility.Visible;
            HeaderBorder.Visibility = Visibility.Visible;
            var grid = HeaderBorder.Parent as Grid;
            if (grid != null)
            {
                grid.RowDefinitions[0].Height = new GridLength(72);
            }
        }

        private void ReporteCard_Click(object sender, RoutedEventArgs e)
        {
            ShowInMainContent(new Reporteria());
            SetActiveButton(sender as Button);
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null || clickedButton == _activeButton) return;

            SetActiveButton(clickedButton);

            string category = clickedButton.Content.ToString();
            _currentCategory = category;
            if (SearchTextBox != null) SearchTextBox.Text = string.Empty; // reset search when switching tabs
            LoadServices(category);
            HideMainContent();
        }

        private void SetActiveButton(Button button)
        {
            if (_activeButton != null)
            {
                _activeButton.Tag = ""; // Deactivate old
            }

            _activeButton = button;

            if (_activeButton != null)
            {
                _activeButton.Tag = "Active"; // Activate new
            }
        }

        private void LoadServices(string category)
        {
            _currentCategory = category;
            RenderCurrentServices();
        }

        private void RenderServices(IEnumerable<Service> services)
        {
            if (ServicesPanel == null) return;
            ServicesPanel.Children.Clear();

            foreach (var service in services)
            {
                var card = new Card
                {
                    Style = (Style)FindResource("ServiceCardStyle"),
                    Tag = service
                };

                var stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

                var image = new Image
                {
                    Source = new BitmapImage(new Uri($"pack://application:,,,/MiBankito;component/{service.logo}", UriKind.Absolute)),
                    Width = 200,
                    Height = 180,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                var textBlock = new TextBlock
                {
                    Text = (service.name ?? string.Empty).ToUpper(),
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0,0, 0, 0),
                    FontSize = 14
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);
                card.Content = stackPanel;
                card.MouseLeftButtonUp += ServiceCard_Click;

                ServicesPanel.Children.Add(card);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RenderCurrentServices();
        }

        private void SortAscendingButton_Click(object sender, RoutedEventArgs e)
        {
            _isAscending = true;
            if (SortAscendingButton != null) SortAscendingButton.Tag = "Active";
            if (SortDescendingButton != null) SortDescendingButton.Tag = "";
            RenderCurrentServices();
        }

        private void SortDescendingButton_Click(object sender, RoutedEventArgs e)
        {
            _isAscending = false;
            if (SortAscendingButton != null) SortAscendingButton.Tag = "";
            if (SortDescendingButton != null) SortDescendingButton.Tag = "Active";
            RenderCurrentServices();
        }

        private void RenderCurrentServices()
        {
            var category = _currentCategory ?? (_activeButton?.Content?.ToString() ?? "Servicios Básicos");
            var services = GetServicesByCategory(category);

            var query = (SearchTextBox?.Text ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(query))
            {
                services = services.Where(s => (s.name ?? string.Empty).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (_isAscending)
            {
                services = services.OrderBy(s => s.name).ToList();
            }
            else
            {
                services = services.OrderByDescending(s => s.name).ToList();
            }

            RenderServices(services);
        }

        private void ServiceCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Card card && card.Tag is Service service)
            {
                BitmapImage logoImg = null;
                try
                {
                    logoImg = new BitmapImage(new Uri($"pack://application:,,,/MiBankito;component/{service.logo}", UriKind.Absolute));
                }
                catch { /* si falla deja null */ }

                if (service.category == "Servicios Básicos")
                {
                    var servicios = new ServiciosBasicos();
                    servicios.SelectedServiceName = service.name;
                    servicios.SetHeader(service.name?.ToUpper(), logoImg);
                    ShowInMainContent(servicios);
                }
                else if (service.category == "Bancos")
                {
                    var bancos = new Bancos();
                    bancos.SetHeader(service.name?.ToUpper(), logoImg);
                    ShowInMainContent(bancos);
                }
                else if (service.category == "Remesas")
                {
                    var remesas = new Remesas();
                    remesas.SetHeader(service.name?.ToUpper(), logoImg);
                    ShowInMainContent(remesas);
                }
            }
        }

        private void WhatsAppBtn_Click(object sender, RoutedEventArgs e)
        {
            // Open Chatbot user control in main content
            ShowInMainContent(new Chatbot());
        }

        private List<Service> GetServicesByCategory(string category)
        {
            var allServices = new List<Service>
            {
                // Servicios Básicos
                new Service { name = "Arabela", logo = "MiBankito/resources/ARABELA.png", category = "Servicios Básicos" },
                new Service { name = "Avon", logo = "MiBankito/resources/AVON.png", category = "Servicios Básicos" },
                new Service { name = "Claro", logo = "MiBankito/resources/CLARO.png", category = "Servicios Básicos" },
                new Service { name = "Credex", logo = "MiBankito/resources/CREDEX.png", category = "Servicios Básicos" },
                new Service { name = "Crediq", logo = "MiBankito/resources/CREDIQ.png", category = "Servicios Básicos" },
                new Service { name = "Credisiman", logo = "MiBankito/resources/CREDISIMAN.png", category = "Servicios Básicos" },
                new Service { name = "Disnorte", logo = "MiBankito/resources/DISNORTE.png", category = "Servicios Básicos" },
                new Service { name = "Embajada Americana", logo = "MiBankito/resources/EE_UU.png", category = "Servicios Básicos" },
                new Service { name = "El Mundo", logo = "MiBankito/resources/ELMUNDO.png", category = "Servicios Básicos" },
                new Service { name = "El Verdugo", logo = "MiBankito/resources/ELVERDUGO.png", category = "Servicios Básicos" },
                new Service { name = "Gallo Mas Gallo", logo = "MiBankito/resources/GALLOMASGALLO.png", category = "Servicios Básicos" },
                new Service { name = "Globex", logo = "MiBankito/resources/GLOBEX.png", category = "Servicios Básicos" },
                new Service { name = "Instacredit", logo = "MiBankito/resources/INSTACREDIT.png", category = "Servicios Básicos" },
                new Service { name = "Mi Credito", logo = "MiBankito/resources/MICREDITO.png", category = "Servicios Básicos" },
                new Service { name = "Monge Pay", logo = "MiBankito/resources/MONGEPAY.png", category = "Servicios Básicos" },
                new Service { name = "Paisa", logo = "MiBankito/resources/PAISA.png", category = "Servicios Básicos" },
                new Service { name = "Picap", logo = "MiBankito/resources/PICAP.png", category = "Servicios Básicos" },
                new Service { name = "Promujer", logo = "MiBankito/resources/PROMUJER.png", category = "Servicios Básicos" },
                new Service { name = "Yota", logo = "MiBankito/resources/YOTA.png", category = "Servicios Básicos" },

                // Bancos
                new Service { name = "Avanz", logo = "MiBankito/resources/AVANZ.png", category = "Bancos" },
                new Service { name = "BAC", logo = "MiBankito/resources/BAC.png", category = "Bancos" },
                new Service { name = "Banco Lafise", logo = "MiBankito/resources/LAFISE.png", category = "Bancos" },
                new Service { name = "BDF", logo = "MiBankito/resources/BDF.png", category = "Bancos" },
                new Service { name = "Ficohsa", logo = "MiBankito/resources/FICOHSA.png", category = "Bancos" },
                new Service { name = "Banpro", logo = "MiBankito/resources/BANPRO.png", category = "Bancos" },
                new Service { name = "FDL", logo = "MiBankito/resources/FDL.png", category = "Bancos" },
                new Service { name = "Fundeser", logo = "MiBankito/resources/FUNDESER.png", category = "Bancos" },
                new Service { name = "BFP", logo = "MiBankito/resources/BFP.png", category = "Bancos" },

                // Remesas

                new Service { name = "Western", logo = "MiBankito/resources/WESTERN.png", category = "Remesas" },
                new Service { name = "AirPak", logo = "MiBankito/resources/AIRPARK.png", category = "Remesas" },
            };

            return allServices.Where(s => s.category == category).ToList();
        }
    }

    // Modelo simple para el servicio, puedes moverlo a su propio archivo.
    public class Service
    {
        public string name { get; set; }
        public string logo { get; set; }
        public string category { get; set; }
    }
}
