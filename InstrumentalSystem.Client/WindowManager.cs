using System.Windows;

namespace InstrumentalSystem.Client
{
    //Класс создан для того, чтобы контролировать какая сейчас страница должна отображаться пользователю
    public class WindowManager
    {
        private static Window _currentWindow;

        public static Window CurrentWindow
        {
            get { return _currentWindow; }
            set
            {
                if (_currentWindow != null)
                {
                    _currentWindow.Close();
                }

                _currentWindow = value;
                _currentWindow.Show();
            }
        }
    }
}