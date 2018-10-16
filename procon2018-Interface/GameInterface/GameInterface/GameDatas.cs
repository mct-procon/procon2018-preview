using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    class GameData
    {
        Point[] agents = new Point[] {
            new Point(),
            new Point(),
            new Point(),
            new Point(),
        };
        public int MassHeight { get; private set; }
        public int MassWidth { get; private set; }
        public GameManager(MainWindowViewModel _viewModel)
        {
            this.MassDatasInit();
            this.viewModel = _viewModel;
        }
        void MassDatasInit()
        {
            System.Random rand = new System.Random();

            while (true)
            {
                this.MassHeight = rand.Next(7, 12);
                this.MassWidth = rand.Next(7, 12);
                if (this.MassHeight * this.MassWidth > 80) break;
            }

            for (int i = 0; i < this.MassHeight; i++)
            {
                //リストの配列は宣言時はNullだから、インスタンスを入れて初期化
                this.MassData[i] = new List<Mass>();
                for (int j = 0; j < MassWidth; j++)
                {
                    //10%の確率で値を0以下にする
                    if (rand.Next(1, 100) > 10)
                        this.MassData[i].Add(new Mass(rand.Next(1, 14)));
                    else
                        this.MassData[i].Add(new Mass(rand.Next(-14, 0)));
                }
            }
        }
    }
}
