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