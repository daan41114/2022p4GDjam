﻿namespace GameJam.Game
{
    public class GameContext
    {
        internal int scaleunit = 4;

        internal int tileSize = 16;
        internal RenderObject player = new RenderObject();
        internal RenderObject enemy = new RenderObject();
        internal SpriteMap spriteMap = new SpriteMap();
        internal Room room;
    }
}