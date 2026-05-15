using RetailHero.POS.Core.Shared.Contracts.Extensions;
using RetailHero.POS.Core.Shared.Contracts.Models;
using RetailHero.POS.Core.Shared.Contracts.ViewModels;
using RetailHero.POS.Core.Shared.Contracts.Views;
using RetailHero.POS.Core.Shared.MetaData;
using RetailHero.POS.Core.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBankito
{
    [Export(typeof(RetailHero.POS.Core.Shared.Contracts.Extensions.ICustomExtension))]
    [ExtensionMetadata("MiBankito", "RMH_Controller", "AMPM", "c9849fc9-edd7-464e-a0a4-a9202cff8d61", 1, 0, 0)]
    public class RMH_Controller : ICustomExtension
    {
        // current instance of the shared service provider
        private ServiceProvider serviceProvider = ServiceProvider.Instance;
        // instance of the Transaction model - the business layer
        private ITransaction Transaction { get; set; }
        // Instance of the Transaction View model - the UX interaction layer
        private ITransactionViewModel TransactionViewModel { get; set; }

        private IPOSMainWindow MainWindow { get; set; }

        public RMH_Controller()
        {
            //instantiates the core objects and binds into the transaction events
            if (serviceProvider.IsRegistered(typeof(ITransaction)))
            {
                Transaction = (ITransaction)serviceProvider.GetService(typeof(ITransaction));
            }

            if (serviceProvider.IsRegistered(typeof(ITransactionViewModel)))
            {
                TransactionViewModel = (ITransactionViewModel)serviceProvider.GetService(typeof(ITransactionViewModel));
            }

            if (serviceProvider.IsRegistered(typeof(IPOSMainWindow)))
            {
                MainWindow = (IPOSMainWindow)serviceProvider.GetService(typeof(IPOSMainWindow));
            }

            // bind a custom action button event
            Transaction.CustomActionEvent += Transaction_CustomActionEvent;
        }

        private void Transaction_CustomActionEvent(object parameter, EventArgs e)
        {
            if (!(parameter is string))
            {
                // sanity check
                return;
            }
            if (parameter.ToString() == "DEDE0403-5C7A-4A84-BB75-BANKITO")
            {
                // Custom Action 2
                ShowFullScreenForm();
                return;
            }
        }

        private void ShowFullScreenForm()
        {
            Home pantalla = new Home("");
            pantalla.Closed += Pantalla_Closed; 
            pantalla.ShowDialog();
        }

        private void Pantalla_Closed(object sender, EventArgs e)
        {
            Home pantalla = (Home)sender;
            if (pantalla.pantalla == "Bancos" || pantalla.pantalla == "ServiciosBasicos")
            {
                Transaction.TransactionItem_AddOrLookup("AMPMSAT026");  // Item LookUP
            }
            if (pantalla.pantalla == "Remesas")
            {
                Transaction.Drawer_CashDrop();
            }
            else
            {
                return;
            }
        }
    }
}
