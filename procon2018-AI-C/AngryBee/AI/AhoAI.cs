using System;
using System.Collections.Generic;
using System.Text;
using AngryBee.Boards;
using MCTProcon29Protocol;
using MCTProcon29Protocol.Methods;

namespace AngryBee.AI
{
	public class AhoAI : MCTProcon29Protocol.AIFramework.AIBase
	{
		Rule.MovableChecker Checker = new Rule.MovableChecker();
		PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

		VelocityPoint[] WayEnumerator = { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

		protected override void Solve()
		{
			var tmp = MoveOrderling(ScoreBoard, MyBoard, EnemyBoard, new Player(MyAgent1, MyAgent2), new Player(EnemyAgent1, EnemyAgent2))[0];
			SolverResult = new Decided(tmp.Value.Agent1, tmp.Value.Agent2);
		}

		//遷移順を決める.  「この関数においては」MeBoard…手番プレイヤのボード, Me…手番プレイヤ、とします。
		//(この関数におけるMeは、Maxi関数におけるMe, Mini関数におけるEnemyです）
		//newMe[0]が最初に探索したい行き先、nextMe[1]が次に探索したい行き先…として、nextMeに「次の行き先」を入れていきます。
		//以下のルールで優先順を決めます。
		//ルール1. Killer手があれば、それを優先する。(Killer手がなければ、Killer.Agent1 = (514, 514), Killer.Agent2 = (514, 514)のように範囲外の移動先を設定すること。)
		//ルール2. 次のmoveで得られる「タイルポイント」の合計値、が大きい移動(の組み合わせ)を優先する。
		//なお、ルールはMovableChecker.csに準ずるため、現在は、「タイル除去先にもう一方のエージェントが移動することはできない」として計算しています。
		private List<KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)>> MoveOrderling(sbyte[,] ScoreBoard, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
		{
			uint width = MeBoard.Width;
			uint height = MeBoard.Height;
			List<KeyValuePair<int, (VelocityPoint, VelocityPoint)>> orderling = new List<KeyValuePair<int, (VelocityPoint, VelocityPoint)>>();

			for (int i = 0; i < WayEnumerator.Length; i++)
			{
				for (int m = 0; m < WayEnumerator.Length; m++)
				{
					Player newMe = Me;
					newMe.Agent1 += WayEnumerator[i];
					newMe.Agent2 += WayEnumerator[m];

					int score = 0;         //優先度 (小さいほど優先度が高い）
					if (newMe.Agent1.X >= width || newMe.Agent1.Y >= height) score = 100;
					else if (newMe.Agent2.X >= width || newMe.Agent2.Y >= height) score = 100;
					else if (newMe.Agent1 == newMe.Agent2) score = 100;
					else if (newMe.Agent1 == Enemy.Agent1) score = 100;
					else if (newMe.Agent1 == Enemy.Agent2) score = 100;
					else if (newMe.Agent2 == Enemy.Agent1) score = 100;
					else if (newMe.Agent2 == Enemy.Agent2) score = 100;
					else
					{
						if (!MeBoard[newMe.Agent1.X, newMe.Agent1.Y] || EnemyBoard[newMe.Agent1.X, newMe.Agent1.Y])
							score += ScoreBoard[newMe.Agent1.X, newMe.Agent1.Y];
						if (!MeBoard[newMe.Agent2.X, newMe.Agent2.Y] || EnemyBoard[newMe.Agent2.X, newMe.Agent2.Y])
							score += ScoreBoard[newMe.Agent2.X, newMe.Agent2.Y];
						score = -score;
					}
					orderling.Add(new KeyValuePair<int, (VelocityPoint, VelocityPoint)>(score, (WayEnumerator[i], WayEnumerator[m])));
				}
			}
			orderling.Sort(impl_sorter);
			return orderling;
		}

		private int impl_sorter(KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)> a, KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)> b) => a.Key - b.Key;

		protected override int CalculateTimerMiliSconds(int miliseconds)
		{
			return miliseconds - 1000;
		}

		protected override void EndGame(GameEnd end)
		{
		}


	}
}
