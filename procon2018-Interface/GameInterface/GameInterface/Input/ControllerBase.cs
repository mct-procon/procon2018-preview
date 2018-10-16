using System;

namespace GameInterface.Input {
    public interface ControllerBase {
        InputData Get();
        void GameInitialize(GameInit data);
        void EndTurn(int turnNum);
        void BeginTurn(TurnStart data);
        void GameEnd(int meScore, int enemyScore);
        void PauseGame(bool isEnter);
        void ResumeController(Resume data);
        void Interrupt();
    }
}