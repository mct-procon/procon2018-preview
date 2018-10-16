using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTProcon29Protocol.Methods;
using MCTProcon29Protocol;
using System.Threading;

namespace GameInterface
{
    internal class ClientRennenend : IIPCServerReader
    {
        Server server;
        GameManager gameManager;
        private int managerNum;
        public ClientRennenend(Server server_, GameManager gameManager_, int managerNum_)
        {
            this.gameManager = gameManager_;
            this.managerNum = managerNum_;
            this.server = server_;
        }
        public void OnConnect(Connect connect)
        {
            gameManager.viewModel.MainWindowDispatcher.Invoke(connectMethod);
        }

        private void connectMethod()
        {
            if (managerNum == 0)
                server.IsConnected1P = true;
            else
                server.IsConnected2P = true;
        }

        private Decided _decided = null;

        public void OnDecided(Decided decided)
        {
            _decided = decided;
            gameManager.viewModel.MainWindowDispatcher.Invoke(decidedMethod);
        }

        private void decidedMethod()
        {
            var decided = _decided;
            Agent.Direction dir = Agent.CastPointToDir(new Point(decided.MeAgent1.X, decided.MeAgent1.Y));
            gameManager.OrderToAgent(new Order(managerNum * 2, dir, Agent.State.MOVE));
            dir = Agent.CastPointToDir(new Point(decided.MeAgent2.X, decided.MeAgent2.Y));
            gameManager.OrderToAgent(new Order(managerNum * 2 + 1, dir, Agent.State.MOVE));
        }

        public void OnInterrupt(Interrupt interrupt)
        {
            gameManager.viewModel.MainWindowDispatcher.Invoke(
                () => MessageBox.Show($"{managerNum + 1}P is disconnected."));
        }
    }

    class Server : ViewModels.ViewModelBase
    {
        IPCManager[] managers = new IPCManager[2];
        GameData data;
        GameManager gameManager;
        private bool[] isConnected = new bool[] { false, false };
        public bool IsConnected1P
        {
            get => isConnected[0];
            set => RaisePropertyChanged(ref isConnected[0], value);
        }

        public bool IsConnected2P
        {
            get => isConnected[1];
            set => RaisePropertyChanged(ref isConnected[1], value);
        }

        public bool[] IsDecidedReceived = new bool[] { false, false };

        public Server(GameManager gameManager)
        {
            this.gameManager = gameManager;
            data = gameManager.data;
            App.Current.Exit += (obj, e) =>
            {
                foreach (var man in managers)
                    man?.Shutdown();
            };
        }

        public void StartListening(GameSettings.SettingStructure settings)
        {
            if (!settings.IsUser1P)
            {
                managers[0] = new IPCManager(new ClientRennenend(this, gameManager, 0));
                Task.Run(() => managers[0].Start(settings.Port1P));
            }
            if (!settings.IsUser2P)
            {
                managers[1] = new IPCManager(new ClientRennenend(this, gameManager, 1));
                Task.Run(() => managers[1].Start(settings.Port2P));
            }
        }

        public void SendGameInit()
        {
            SendGameInit(0);
            SendGameInit(1);
        }

        private void SendGameInit(int playerNum)
        {
            if (!isConnected[playerNum]) return;
            sbyte[,] board = new sbyte[data.BoardWidth, data.BoardHeight];
            for (int i = 0; i < data.BoardWidth; i++)
            {
                for (int j = 0; j < data.BoardHeight; j++)
                {
                    board[i, j] = (sbyte)data.CellData[i, j].Score;
                }
            }
            managers[playerNum].Write(DataKind.GameInit, new GameInit((byte)data.BoardHeight, (byte)data.BoardWidth, board,
                new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[2 - playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[3 - playerNum * 2].Point.Y),
                data.FinishTurn));
        }

        public void SendTurnStart()
        {
            SendTurnStart(0);
            SendTurnStart(1);
        }

        private void SendTurnStart(int playerNum)
        {
            IsDecidedReceived[playerNum] = false;
            if (!isConnected[playerNum]) return;

            ColoredBoardSmallBigger colorBoardMe = new ColoredBoardSmallBigger((uint)data.BoardWidth, (uint)data.BoardHeight);
            ColoredBoardSmallBigger colorBoardEnemy = new ColoredBoardSmallBigger((uint)data.BoardWidth, (uint)data.BoardHeight);

            for (int i = 0; i < data.BoardWidth; i++)
            {
                for (int j = 0; j < data.BoardHeight; j++)
                {
                    if (data.CellData[i, j].AreaState_ == TeamColor.Area1P)
                        colorBoardMe[(uint)i, (uint)j] = true;
                    else if (data.CellData[i, j].AreaState_ == TeamColor.Area2P)
                        colorBoardEnemy[(uint)i, (uint)j] = true;
                }
            }
            if (playerNum == 1) Swap(ref colorBoardMe, ref colorBoardEnemy);
            managers[playerNum].Write(DataKind.TurnStart, new TurnStart((byte)data.NowTurn, data.TimeLimitSeconds * 1000,
                new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[2 - playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[3 - playerNum * 2].Point.Y),
                colorBoardMe,
                colorBoardEnemy));
        }

        public void SendTurnEnd()
        {
            SendTurnEnd(0);
            SendTurnEnd(1);
        }

        private void SendTurnEnd(int playerNum)
        {
            IsDecidedReceived[playerNum] = true;
            if (!isConnected[playerNum]) return;
            managers[playerNum].Write(DataKind. TurnEnd, new TurnEnd((byte)data.NowTurn));
        }

        public void SendGameEnd()
        {
            int score = data.PlayerScores[0], enemyScore = data.PlayerScores[1];
            for (int i = 0; i < Constants.PlayersNum; i++)
            {
                if (!isConnected[i]) continue;
                managers[i].Write(DataKind.GameEnd, new GameEnd(score, enemyScore));
                int n = i;
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    managers[n].Shutdown();
                });
                Swap<int>(ref score, ref enemyScore);
            }
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
