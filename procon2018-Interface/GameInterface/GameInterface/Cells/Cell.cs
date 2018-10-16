using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.Cells
{
    public class Cell : ViewModels.ViewModelBase
    {
        private int score;
        public int Score
        {
            get => score; 
            set => RaisePropertyChanged(ref score, value);
        }

        private TeamColor areaState = TeamColor.Free;
        public TeamColor AreaState_
        {
            get => areaState;
            set => RaisePropertyChanged(ref areaState, value);
        }

        private TeamColor agentState = TeamColor.Free;
        public TeamColor AgentState 
        {
            get => agentState;
            set => RaisePropertyChanged(ref agentState, value);
        }

        public Cell() { }
        public Cell(int _score)
        {
            Score = _score;
        }
    }
}
