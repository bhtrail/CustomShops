using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomShops
{
    public interface ISpriteIcon
    {
        Sprite Sprite { get; }
    }

    public interface ITextIcon
    {
        string SpriteID { get; }
    }
}
