using System.ComponentModel;
using System.Windows;

namespace InstrumentalSystem.Client.View
{
    public partial class ClosingProjectEditWindow : Window
    {
        public ClosingProjectEditWindow()
        {
            InitializeComponent();
        }
        
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Perform actions for 'Yes' and then close
            this.DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // Perform actions for 'No' and then close
            this.DialogResult = false;
        }
    }
}