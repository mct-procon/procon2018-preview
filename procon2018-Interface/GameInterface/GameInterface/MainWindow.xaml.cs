using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using GameInterface.Cells;
using System.Collections.Generic;

namespace GameInterface
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private GameManager gameManager;
        public MainWindow()
        {
            InitializeComponent();
            this.viewModel = new MainWindowViewModel();
            this.viewModel.MainWindowDispatcher = Dispatcher;
            this.DataContext = this.viewModel;
            this.gameManager = new GameManager(viewModel);
            this.viewModel.gameManager = this.gameManager;
            CreateOrderButtonsOnPlayerGrid();
        }

        void InitGame(GameSettings.SettingStructure settings)
        {
            gameManager.InitGameData(settings);
            CreateCellOnCellGrid(gameManager.data.BoardWidth, gameManager.data.BoardHeight);
        }

        void CreateCellOnCellGrid(int boardWidth, int boardHeight)
        {
            // Clear before game.
            cellGrid.RowDefinitions.Clear();
            cellGrid.ColumnDefinitions.Clear();

            List<Cells.CellUserControl> cells = new List<CellUserControl>();
            foreach(var ctrl in cellGrid.Children)
            {
                if (ctrl is Cells.CellUserControl)
                    cells.Add((Cells.CellUserControl)ctrl);
            }
            foreach (var ctrl in cells)
                cellGrid.Children.Remove(ctrl);
            // end

            //Gridに列、行を追加
            for (int i = 0; i < boardHeight; i++)
                cellGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < boardWidth; i++)
                cellGrid.ColumnDefinitions.Add(new ColumnDefinition());

            //i行目のj列目にテキストを追加
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    var cellUserControl = new CellUserControl();
                    cellUserControl.DataContext = gameManager.data.CellData[i, j];
                    cellGrid.Children.Add(cellUserControl);
                    Grid.SetColumn(cellUserControl, i);
                    Grid.SetRow(cellUserControl, j);

                    var changeColorUserCtrl = new ChangeColorUserCtrl(new Point(i, j));
                    cellGrid.Children.Add(changeColorUserCtrl);
                    Grid.SetColumn(changeColorUserCtrl, i);
                    Grid.SetRow(changeColorUserCtrl, j);
                }
            }
        }
        
        void CreateOrderButtonsOnPlayerGrid()
        {
            OrderButtonUserControl.Viewmodel = viewModel;
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                var currentGrid = i < 2 ? player1Grid : player2Grid;
                var currentStyle = (Style)(i < 2 ? this.FindResource("BlueButton") : this.FindResource("RedButton"));

                for (int j = 0; j < Constants.OrderButtonsNum; j++)
                {
                    var orderButtonUserControl = new OrderButtonUserControl(i, j);
                    ((FrameworkElement)orderButtonUserControl.Content).Style = currentStyle;
                    currentGrid.Children.Add(orderButtonUserControl);
                    Grid.SetRow(orderButtonUserControl, GetButtonSpaceRow(i, j));
                    Grid.SetColumn(orderButtonUserControl, GetButtonSpaceColumn(j));
                    viewModel.orderButtonUserControls[i, j] = orderButtonUserControl;
                }
            }
            viewModel.Agents = gameManager.data.Agents;
        }

        int GetButtonSpaceRow(int agentNum, int buttonId)
        {
            const int BUTTON_MARGIN_TOP = 2;
            const int BUTTON_MARGIN_MIDDLE = 1;
            const int BUTTON_SPACE_RANGE = 3;
            int row = buttonId / BUTTON_SPACE_RANGE + BUTTON_MARGIN_TOP;
            if (agentNum % 2 == 1) row += BUTTON_SPACE_RANGE + BUTTON_MARGIN_MIDDLE;
            return row;
        }

        int GetButtonSpaceColumn(int buttonId)
        {
            const int BUTTON_MARGIN_LEFT = 1;
            const int BUTTON_SPACE_RANGE = 3;
            int column = buttonId % BUTTON_SPACE_RANGE + BUTTON_MARGIN_LEFT;
            return column;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.ContextMenu.IsOpen = true;
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            //まだEndTurn()していないなら、しておく
            if (gameManager.data.IsNextTurnStart) gameManager.EndTurn();
            gameManager.StartTurn();
        }

        private void BreakMenu_Clicked(object sender, RoutedEventArgs e)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                if (!System.Diagnostics.Debugger.Launch())
                    return;
            System.Diagnostics.Debugger.Break();
        }

        private void NewGameMenu_Clicked(object sender, RoutedEventArgs e)
        {
            viewModel.gameManager.TimerStop();
            if (GameSettings.GameSettingDialog.ShowDialog(out var result))
            {
                viewModel.gameManager.server.SendGameEnd();
                InitGame(result);
                if (!(result.IsUser1P & result.IsUser2P))
                    (new GameSettings.WaitForAIDialog(viewModel.gameManager.server, result)).ShowDialog();
                gameManager.StartGame();
            }
        }
    }
}
