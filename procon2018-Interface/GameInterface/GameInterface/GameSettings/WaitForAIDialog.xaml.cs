using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameInterface.GameSettings
{
    /// <summary>
    /// WaitForAIDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class WaitForAIDialog : Window
    {

        public WaitForAIDialog()
        {
            InitializeComponent();
        }

        internal WaitForAIDialog(Server serverData, SettingStructure settings)
        {
            this.DataContext = serverData;
            InitializeComponent();
            serverData.PropertyChanged += (ss, ee) =>
            {
                if (ee.PropertyName.StartsWith("IsConnected") && ((serverData.IsConnected1P | settings.IsUser1P) & (serverData.IsConnected2P | settings.IsUser2P)))
                    Dispatcher.BeginInvoke((Action)(() => this.Close()));
            };
            if (settings.IsUser1P)
                Text1P.Visibility = ( Progress1P.Visibility =  Visibility.Hidden);
            if (settings.IsUser2P)
                Text2P.Visibility = (Progress2P.Visibility = Visibility.Hidden);
        }
    }
}
