using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DBConnectionLayerFrontEnd.ViewModel;

namespace DBConnectionLayerFrontEnd
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrontEndViewModel viewModel = new FrontEndViewModel();
            MainWindow window = new MainWindow();

            EventHandler handler = null;

            //handler = delegate
            //{
            //    ViewModel.RequestClose -=
            //}
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                window.Close();
            };
            viewModel.RequestClose += handler;
            window.DataContext = viewModel;
            window.Show();

        }

    }
}
