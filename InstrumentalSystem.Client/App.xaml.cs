using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using InstrumentalSystem.Client.View;
using InstrumentalSystem.Client.View.Modals;

namespace InstrumentalSystem.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Поменял окно, которое открывается при запуске программы
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WindowManager.CurrentWindow = new Authorization();
        }
    }
}