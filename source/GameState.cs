using BattleTech;
using UnityEngine;

namespace CustomShops
{
    public class GameState
    {
        private StarSystem _currentStarSystem;
        public StarSystem CurrentSystem
        {
            get
            {
                if (_currentStarSystem == null)
                {
                    Control.Log($"Control.State.CurrentSystem was null. Setting to Control.Sim.CurSystem: {Sim.CurSystem.Name}");
                    CurrentSystem = Sim.CurSystem;
                }
                return _currentStarSystem;
            }
            internal set => _currentStarSystem = value;
        }

        public Sprite SystemShopSprite { get; internal set; }
        public Sprite BlacMarketSprite { get; internal set; }
        public SimGameState Sim { get; internal set; }
    }
}
