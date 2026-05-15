using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MiBankito.Models;
using MiBankito.Services;

namespace MiBankito
{
    /// <summary>
    /// Lógica de interacción para Chatbot.xaml
    /// </summary>
    public partial class Chatbot : UserControl
    {
        public Chatbot()
        {
            InitializeComponent();
            AppState.Changed += AppState_Changed;
            this.Unloaded += Chatbot_Unloaded;
            RefreshGrid();
        }

        private void AppState_Changed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(RefreshGrid);
        }

        private void Chatbot_Unloaded(object sender, RoutedEventArgs e)
        {
            AppState.Changed -= AppState_Changed;
            this.Unloaded -= Chatbot_Unloaded;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this) as Home;
            parent?.HideMainContent();
        }

        private void LoadSampleBtn_Click(object sender, RoutedEventArgs e)
        {
            var samples = new List<TransactionRecord>
            {
                new TransactionRecord { Timestamp = DateTime.Now.AddMinutes(-10), Platform = "WhatsApp", Service = "Pago Luz", Reference = "ABC123", Amount = 125.50m, Currency = "NIO", CashierId = "WM01", Status = "Pendiente" },
                new TransactionRecord { Timestamp = DateTime.Now.AddMinutes(-30), Platform = "WhatsApp", Service = "Recarga Celular", Reference = "REF456", Amount = 50.00m, Currency = "NIO", CashierId = "WM02", Status = "Pendiente" },
                new TransactionRecord { Timestamp = DateTime.Now.AddHours(-1), Platform = "WhatsApp", Service = "Pago Agua", Reference = "XYZ789", Amount = 75.75m, Currency = "NIO", CashierId = "WM03", Status = "Pendiente" }
            };

            foreach (var s in samples)
            {
                AppState.Add(s);
            }
        }

        private void RefreshGrid()
        {
            var data = AppState.Filter("WhatsApp", string.Empty, null, null).ToList();
            ChatTransactionsGrid.ItemsSource = data;
        }

        private TransactionRecord GetRowTransaction(object sender)
        {
            if (sender is DependencyObject dep)
            {
                var row = FindAncestor<DataGridRow>(dep);
                if (row != null)
                {
                    return row.Item as TransactionRecord;
                }
            }
            return null;
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var txn = GetRowTransaction(sender);
            if (txn == null) return;
            txn.Status = "Confirmado";
            AppState.NotifyChanged();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            var txn = GetRowTransaction(sender);
            if (txn == null) return;
            txn.Status = "Rechazado";
            AppState.NotifyChanged();
        }
    }
}