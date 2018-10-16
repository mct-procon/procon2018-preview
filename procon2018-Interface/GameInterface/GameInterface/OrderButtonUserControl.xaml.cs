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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameInterface
{
    /// <summary>
    /// OrderButtonUserControl.xaml の相互作用ロジック
    /// </summary>
    /// 

    public partial class OrderButtonUserControl : UserControl
    {
        public static MainWindowViewModel Viewmodel { get; set; }
        readonly string[] buttonText = new string[] {
            "↖", "↑", "↗",
            "←", "・", "→",
            "↙", "↓", "↘",
        };

        readonly Agent.Direction[] buttonDir = new Agent.Direction[] {
            Agent.Direction.UP_LEFT,    Agent.Direction.UP ,    Agent.Direction.UP_RIGHT,
            Agent.Direction.LEFT,       Agent.Direction.NONE,   Agent.Direction.RIGHT,
            Agent.Direction.DOWN_LEFT,  Agent.Direction.DOWN,   Agent.Direction.DOWN_RIGHT,
        };
        public OrderButtonUserControl(int agentNum,int buttonId)
        {
            InitializeComponent();
            InitOrderButton(agentNum,buttonId);
        }
        private void InitOrderButton(int agentNum, int buttonId)
        {
            var oB = this.orderButton;
            oB.Content = buttonText[buttonId];
            oB.Command = Viewmodel.OrderToAgentCommand;
            oB.CommandParameter = new Order(agentNum, buttonDir[buttonId], Agent.State.MOVE);
        }
    }
}
