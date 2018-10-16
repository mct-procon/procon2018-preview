using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using GameInterface.Cells;
using System.Windows.Threading;

namespace GameInterface
{
    public class MainWindowViewModel : ViewModels.ViewModelBase
    {
        public GameManager gameManager;

        public Dispatcher MainWindowDispatcher;

        //---------------------------------------
        //画面に表示する変数
        private Cell[,] cellData = new Cell[12,12];
        public Cell[,] CellData
        {
            get => cellData;
            set => RaisePropertyChanged(ref cellData, value);
        }
        private string timerStr;
        public string TimerStr
        {
            get => timerStr; 
            set
            {
                timerStr = value;
                RaisePropertyChanged("TimerStr");
            }
        }
        private Agent[] agents = new Agent[]{
            new Agent(),new Agent(),new Agent(),new Agent()
        };
        public Agent[] Agents
        {
            get => agents; 
            set
            {
                agents = value;
                UpdateOrderButton();
                RaisePropertyChanged();
                RaisePropertyChanged("Point");
            }
        }
        public OrderButtonUserControl[,] orderButtonUserControls = new OrderButtonUserControl[4, 9];
        private int[] playerScores = new int[2];
        public int[] PlayerScores
        {
            get => playerScores;
            set
            {
                playerScores = value;
                RaisePropertyChanged();
            }
        }
        private bool[] isRemoveMode =new bool[4];
        public bool[] IsRemoveMode {
            get => isRemoveMode;
            set => isRemoveMode = value;
        }
        private string turnStr;
        public string TurnStr
        {
            get => turnStr;
            set => RaisePropertyChanged(ref turnStr, value);
        }
        //---------------------------------------
        //ボタン等を押された時の関数
        public DelegateCommand<Order> OrderToAgentCommand { get; private set; }
        public DelegateCommand<Point> ChangeColorCommand { get; private set; }

        public MainWindowViewModel()
        {
            InitCommands();
        }
        private void InitCommands()
        {
            OrderToAgentCommand = new DelegateCommand<Order>(
                OrderToAgentFromVM
            );
            ChangeColorCommand = new DelegateCommand<Point>
            (
                ChangeColor
            );
        }

        private void OrderToAgentFromVM(Order order)
        {
            if (isRemoveMode[order.agentNum]) order.state = Agent.State.REMOVE_TILE;
            gameManager.OrderToAgent(order);
        }

        private void ChangeColor(Point point)
        {
            gameManager.ChangeCellToNextColor(point);
        }

        private void UpdateOrderButton()
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                for (int j = 0; j < Constants.OrderButtonsNum; j++)
                {
                    orderButtonUserControls[i, j].IsEnabled = true;
                }
                orderButtonUserControls[i, agents[i].GetDirectionIdFromDirection()].IsEnabled = false;
            }
        }
    }
}
