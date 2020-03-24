using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomShops
{
    public class GameState
    {
        public StarSystem CurrentSystem { get; internal set; }
        public Sprite SystemShopSprite { get; internal set; }
        public Sprite BlacMarketSprite { get; internal set; }
        public SimGameState Sim { get; internal set; }
}
}
