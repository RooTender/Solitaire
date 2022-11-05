using System;
using System.Windows;
using System.Windows.Controls;

namespace Solitaire
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

            OkButton.Click += (sender, e) =>
            {
                var parentWindow = Window.GetWindow(this);
                parentWindow?.Close();
            };
        }
    }
}
