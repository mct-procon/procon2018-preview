using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCTProcon29Protocol.Methods;

namespace MCTProcon29Protocol.AIFramework
{
    public abstract class AIBase : IIPCClientReader
    {
        private IPCManager ipc;
        protected CancellationTokenSource Canceller;
        protected CancellationToken CancellationToken;

        protected Decided SolverResult;

        protected Task SolverTask;

        private System.Timers.Timer timer;

        public bool IsWriteLog { get; set; } = false;
        public bool IsWriteBoard { get; set; } = false;

        public sbyte[,] ScoreBoard { get; set; }
        public Point MyAgent1 { get; set; }
        public Point MyAgent2 { get; set; }
        public Point EnemyAgent1 { get; set; }
        public Point EnemyAgent2 { get; set; }

        public ColoredBoardSmallBigger MyBoard { get; set; }
        public ColoredBoardSmallBigger EnemyBoard { get; set; }

        public int CurrentTurn { get; set; }
        public int TurnCount { get; set; }

        private volatile bool SendingFinished = false;

        public object LogSyncRoot = new object();

        private ManualResetEventSlim SynchronizeStopper = new ManualResetEventSlim(false);

        public AIBase()
        {
            ipc = new IPCManager(this);
            timer = new System.Timers.Timer();
            timer.Elapsed += this.EndSolve;
            timer.AutoReset = false;
        }

        public virtual void StartSync(int port, bool isWriteLog = false, bool isWriteBoard = false)
        {
            Start(port, isWriteLog, isWriteBoard);
            SynchronizeStopper.Wait();
        }
        public virtual void Start(int port, bool isWriteLog = false, bool isWriteBoard = false)
        {
            SynchronizeStopper.Reset();
            IsWriteLog = isWriteLog;
            IsWriteBoard = isWriteBoard;
            Task.Run(() => ipc.Start(port));
            {
                var proc = System.Diagnostics.Process.GetCurrentProcess();
                ipc.Write(DataKind.Connect, new Connect(ProgramKind.AI) { ProcessId = proc.Id });
                proc.Dispose();
            }
            Log("[IPC] Sended Connect");
        }

        public virtual void End()
        {
            ipc?.Shutdown();
            SynchronizeStopper.Set();
        }

        public void OnGameInit(GameInit init)
        {
            Log("[IPC] Receive GameInit");

            ScoreBoard = init.Board;
            MyAgent1 = init.MeAgent1;
            MyAgent2 = init.MeAgent2;
            EnemyAgent1 = init.EnemyAgent1;
            EnemyAgent2 = init.EnemyAgent2;
            TurnCount = init.Turns;

        }

        public void OnTurnStart(TurnStart turn)
        {
            MyBoard = turn.MeColoredBoard;
            EnemyBoard = turn.EnemyColoredBoard;
            MyAgent1 = turn.MeAgent1;
            MyAgent2 = turn.MeAgent2;
            EnemyAgent1 = turn.EnemyAgent1;
            EnemyAgent2 = turn.EnemyAgent2;
            CurrentTurn = turn.Turn;
            SendingFinished = false;

            Log("[IPC] Receive TurnStart turn = {0}", turn.Turn);
            DumpBoard(turn.MeColoredBoard, turn.EnemyColoredBoard, MyAgent1, MyAgent1, EnemyAgent1, EnemyAgent2);

            StartSolve();
            timer.Interval = CalculateTimerMiliSconds(turn.WaitMiliSeconds);
            timer.Enabled = true;
        }

        public void OnTurnEnd(TurnEnd turn)
        {
            if (IsWriteLog)
                Console.WriteLine("[IPC] Receive TurnEnd");
            Canceller?.Cancel();
        }

        public void Log(string str)
        {
            if (!IsWriteLog) return;
            lock (LogSyncRoot)
                Console.WriteLine(str);
        }

        public void Log(string format, params object[] objects)
        {
            if (!IsWriteLog) return;
            lock (LogSyncRoot)
                Console.WriteLine(format, objects);
        }

        public virtual void OnGameEnd(GameEnd end)
        {
            Log("[IPC] Received GameEnd");
        }

        public virtual void OnPause(Pause pause)
        {
            Log("[IPC] Received Pause");
        }

        public virtual void OnInterrupt(Interrupt interrupt)
        {
            Log("[IPC] Received Interrupt isError = {0}", interrupt.IsError);
            if (interrupt.IsError)
            {
                //DoSomething.
            }

            End();
        }

        public void OnRebaseByUser(RebaseByUser rebase)
        {
            Log("[IPC] Received Rebase By User");
        }

        private void StartSolve()
        {
            Canceller = new CancellationTokenSource();
            CancellationToken = Canceller.Token;
            SolverTask = Task.Run((Action)Solve, CancellationToken);
            SolverTask.ContinueWith(ContinuationAction);
            Log("[SOLVER] Solver Started.");
        }

        private void ContinuationAction(Task prevTask) {
            if (SendingFinished) return;
            if (!prevTask.IsCompleted || prevTask.IsCanceled) return;
            Log("[SOLVER] Solver Finished.");
            SendDecided();
        }

        protected virtual void DumpBoard(in ColoredBoardSmallBigger MyBoard, in ColoredBoardSmallBigger EnemyBoard, Point Me1, Point Me2, Point Enemy1, Point Enemy2 )
        {
            if (!IsWriteBoard) return;
            lock (LogSyncRoot)
            {
                for (uint y = 0; y < ScoreBoard.GetLength(1); ++y)
                {
                    for (uint x = 0; x < ScoreBoard.GetLength(0); ++x)
                    {
                        if ((x == Me1.X && y == Me1.Y) || (x == Me2.X && y == Me2.Y))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                        else if ((x == Enemy1.X && y == Enemy1.Y) || (x == Enemy2.X && y == Enemy2.Y))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Blue;
                        }
                        if (MyBoard[x, y])
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        else if (EnemyBoard[x, y])
                        {

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                        else if (((x + y) & 1) == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        string str = ScoreBoard[x, y].ToString();
                        if (str.Length != 3)
                            Console.Write(new string(' ', 3 - str.Length));
                        Console.Write(str);
                    }
                    Console.WriteLine();
                }
            }
        }

        protected abstract void EndGame(GameEnd end);
        protected abstract void Solve();
        protected virtual int CalculateTimerMiliSconds(int miliseconds) => miliseconds - 1500;
        protected virtual void EndSolve(object sender, EventArgs e)
        {
            timer.Enabled = false;
            if (SendingFinished) return;
            if (SolverTask.IsFaulted)
            {
                lock (LogSyncRoot)
                {
                    Console.WriteLine("[SOLVER] An exception is thrown.====");
                    Console.WriteLine(SolverTask.Exception);
                    Console.WriteLine("======");
                }
            }
            else
            {
                Log("[SOLVER] State is {0}", SolverTask.Status);
                if (SolverTask.IsCompleted)
                    Canceller?.Dispose();
                else
                    Canceller?.Cancel();
                SendDecided();
            }
            Log("[SOLVER] Thinking Stop.");
        }

        protected virtual void SendDecided()
        {
            SendingFinished = true;
            if (SolverResult != null)
            {
                ipc.Write<Methods.Decided>(DataKind.Decided, SolverResult);
                Log("[IPC] Decided Sended");
            }
            else
                Log("[SOLVER] Decision is NULL!!\n[IPC] Decided Sending Failed");
        }
    }
}
