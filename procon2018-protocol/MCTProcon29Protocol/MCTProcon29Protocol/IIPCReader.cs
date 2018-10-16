using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol
{
    public interface IIPCClientReader
    {
        void OnGameInit(Methods.GameInit init);
        void OnTurnStart(Methods.TurnStart turn);
        void OnTurnEnd(Methods.TurnEnd turn);
        void OnGameEnd(Methods.GameEnd end);
        void OnPause(Methods.Pause pause);
        void OnInterrupt(Methods.Interrupt interrupt);
        void OnRebaseByUser(Methods.RebaseByUser rebase);
    }

    public interface IIPCServerReader
    {
        void OnConnect(Methods.Connect connect);
        void OnDecided(Methods.Decided decided);
        void OnInterrupt(Methods.Interrupt interrupt);
    }
}
