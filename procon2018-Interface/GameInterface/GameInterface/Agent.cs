using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameInterface
{
    public class Agent
    {
        public int playerNum; //0,1
        public Point Point { get; set; }
        public enum Direction { NONE = 0, UP, UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT };
        private Direction agentDirection;
        public Direction AgentDirection
        {
            get => agentDirection; 
            set
            {
                agentDirection = value;
            }
        }

        //UP_LEFT は左上だから 0 というように、
        //0 1 2                                 
        //3 4 5                                  
        //6 7 8
        //となるようなIDを定める(viewmodel内のボタンの処理をわかりやすくするため)
        readonly int[] directionId = new int[]
        {
            4,1,2,
            5,8,7,
            6,3,0,
        };
        public int GetDirectionIdFromDirection()
        {
            return directionId[(int)this.AgentDirection];
        }

        public enum State { MOVE, REMOVE_TILE };
        private State agentState;
        public State AgentState
        {
            get => agentState; 
            set
            {
                agentState = value;
            }
        }
        public Point GetNextPoint()
        {
            int x = this.Point.X, y = this.Point.Y;
            switch (AgentDirection)
            {
                case Direction.NONE:
                    break;
                case Direction.UP:
                    y -= 1;
                    break;
                case Direction.UP_RIGHT:
                    x += 1;
                    y -= 1;
                    break;
                case Direction.RIGHT:
                    x += 1;
                    break;
                case Direction.DOWN_RIGHT:
                    x += 1;
                    y += 1;
                    break;
                case Direction.DOWN:
                    y += 1;
                    break;
                case Direction.DOWN_LEFT:
                    x -= 1;
                    y += 1;
                    break;
                case Direction.LEFT:
                    x -= 1;
                    break;
                case Direction.UP_LEFT:
                    x -= 1;
                    y -= 1;
                    break;
                default:
                    break;
            }
            return new Point(x, y);
        }

        static public Direction CastPointToDir(Point p)
        {
            int x = p.X, y = p.Y;
            if (x == 1)
            {
                if (y == -1)
                    return Direction.UP_RIGHT;
                if (y == 0)
                    return Direction.RIGHT;
                if (y == 1)
                    return Direction.DOWN_RIGHT;
            }
            else if (x == 0)
            {
                if (y == -1)
                    return Direction.UP;
                if (y == 0)
                    return Direction.NONE;
                if (y == 1)
                    return Direction.DOWN;
            }
            else if (x == -1)
            {
                if (y == -1)
                    return Direction.UP_LEFT;
                if (y == 0)
                    return Direction.LEFT;
                if (y == 1)
                    return Direction.DOWN_LEFT;
            }
            return Direction.NONE;
        }
    }
}
