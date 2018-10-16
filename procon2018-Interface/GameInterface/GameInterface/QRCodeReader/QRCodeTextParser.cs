using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.QRCodeReader
{
    public class QRCodeTextParser
    {
        public static void Parse(string input, out Cells.Cell[,] CellResult, out Agent[] AgentResult)
        {
            CellResult = null;
            AgentResult = null;

            int height = 0;
            int width = 0;

            int itr = 0;
            int startItr = 0;

            int lineItr = 0;

            for(; itr < input.Length; ++itr)
                switch(input[itr])
                {
                    case ' ': goto end_height;
                    case ':': throw new FormatException("Board Width is missing.");
                }
            end_height:
            if (itr == 0) throw new FormatException("Invalid Text.");
            height = int.Parse(input.Substring(0, itr));

            itr++;
            startItr = itr;
            for(; itr < input.Length; ++itr)
                switch(input[itr])
                {
                    case ' ': throw new FormatException("Board Size Information is invalid.");
                    case ':': goto end_width;
                }
            end_width:
            if (itr == startItr) throw new FormatException("Invalid Text.");
            width = int.Parse(input.Substring(startItr, itr - startItr));

            itr++;
            startItr = itr;
            CellResult = new Cells.Cell[width, height];
            for(; itr < input.Length && lineItr < height; ++itr)
            {
                if(input[itr] == ':')
                {
                    lineItr++;
                    if (lineItr == height)
                        goto end_lines;
                }
            }
            end_lines:
            if (lineItr != height) throw new FormatException("Text is too short.");
            var cellData = input.Substring(startItr, itr - startItr).Split(':').Select((x, y) => x.Split().Select(xx => int.Parse(xx)).ToArray()).ToArray();
            for (int y = 0; y < cellData.Length; ++y)
            {
                var currentLine = cellData[y];
                if (currentLine.Length != width) throw new FormatException("Width is too short in y-" + y.ToString() + ".");
                for (int x = 0; x < currentLine.Length; ++x)
                    CellResult[x, y] = new Cells.Cell(currentLine[x]);
            }

            itr++;
            startItr = itr;
            AgentResult = new Agent[4];
            var nums = input.Substring(startItr).Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            AgentResult[0] = new Agent();
            AgentResult[1] = new Agent();
            AgentResult[0].Point = new Point(nums[1] - 1, nums[0] - 1);
            AgentResult[1].Point = new Point(nums[3] - 1, nums[2] - 1);
        }
    }
}
