using System.Windows;
using InstrumentalSystem.Client.View.Modals;

namespace InstrumentalSystem.Client.View
{
    //Создано окно для того, чтобы отображать его при запуске приложения
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
            ContentGrid.Children.Add(new AuthenticationModal(this));
        }
    }
}