using GameJam.Game;
using GameJam.Tools;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GameJam
{
    public partial class RenderForm : Form
    {

        private LevelLoader levelLoader;
        private float frametime;
        private GameRenderer renderer;
        private SpriteMap spriteMap;
        //private Audio audio;
        private readonly GameContext gc = new GameContext();
        public RenderForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ResizeRedraw = true;

            //audio = new Audio();
            KeyDown += RenderForm_KeyDown;
            FormClosing += Form1_FormClosing;
            Load += RenderForm_Load;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.Dispose();
            //audio.Dispose();
        }
        private void RenderForm_Load(object sender, EventArgs e)
        {
            levelLoader = new LevelLoader(gc.tileSize, new FileLevelDataSource());
            levelLoader.LoadRooms(gc.spriteMap.GetMap());
            spriteMap = new SpriteMap();
            renderer = new GameRenderer(gc);
            Spawn();
            ClientSize =
             new Size(

                (gc.tileSize * gc.room.tiles[0].Length) * gc.scaleunit,
                (gc.tileSize * gc.room.tiles.Length) * gc.scaleunit
                );
        }
        private void Spawn()
        {
            gc.room = levelLoader.GetRoom(0, 0);

            gc.player = new RenderObject()
            {
                frames = gc.spriteMap.GetPlayerFrames(),

                rectangle = new Rectangle(2 * gc.tileSize, 2 * gc.tileSize, gc.tileSize, gc.tileSize),
            };
            gc.enemy = new RenderObject()
            {
                frames = gc.spriteMap.GetEnemyFrames(),

                rectangle = new Rectangle(11 * gc.tileSize, 10 * gc.tileSize, gc.tileSize, gc.tileSize),
            };
        }
        public void ReplaceEnemy()
        {
            gc.enemy = new RenderObject()
            {
                frames = gc.spriteMap.GetEnemyFrames(),

                rectangle = new Rectangle(11 * gc.tileSize, 10 * gc.tileSize, gc.tileSize, gc.tileSize),
            };
        }

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                MovePlayer(0, -1);
                MoveEnemy(0, 1);
            }
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                MovePlayer(0, 1);
                MoveEnemy(0, -1);
            }
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                MovePlayer(-1, 0);
                MoveEnemy(1, 0);
            }
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                MovePlayer(1, 0);
                MoveEnemy(-1, 0);
            }
        }

        private void MovePlayer(int x, int y)
        {
            RenderObject player = gc.player;
            float newx = player.rectangle.X + (x * gc.tileSize);
            float newy = player.rectangle.Y + (y * gc.tileSize);

            Tile next = gc.room.tiles.SelectMany(ty => ty.Where(tx => tx.rectangle.Contains((int)newx, (int)newy))).FirstOrDefault();

            if (next != null)
            {
                if (next.graphic == 'D')
                {
                    gc.room = levelLoader.GetRoom(gc.room.roomx + x, gc.room.roomy + y);
                    ReplaceEnemy();
                    if (y != 0)
                    {
                        player.rectangle.Y += -y * ((gc.room.tiles.Length - 2) * gc.tileSize);
                    }
                    else
                    {
                        player.rectangle.X += -x * ((gc.room.tiles[0].Length - 2) * gc.tileSize);

                    }
                }

                else if (next.graphic != '#' && next.graphic != '+')
                {
                    player.rectangle.X = newx;
                    player.rectangle.Y = newy;
                }
            }
        }
        private void MoveEnemy(int x, int y)
        {
            RenderObject enemy = gc.enemy;
            float newx = enemy.rectangle.X + (x * gc.tileSize);
            float newy = enemy.rectangle.Y + (y * gc.tileSize);

            Tile next = gc.room.tiles.SelectMany(ty => ty.Where(tx => tx.rectangle.Contains((int)newx, (int)newy))).FirstOrDefault();

            if (next != null)
            {
                if (next.graphic == 'D')
                {
                    gc.room = levelLoader.GetRoom(gc.room.roomx + x, gc.room.roomy + y);

                    if (y != 0)
                    {
                        enemy.rectangle.Y += -y * ((gc.room.tiles.Length - 2) * gc.tileSize);
                    }
                    else
                    {
                        enemy.rectangle.X += -x * ((gc.room.tiles[0].Length - 2) * gc.tileSize);

                    }
                }

                if (next.graphic != '#')
                {
                    enemy.rectangle.X = newx;
                    enemy.rectangle.Y = newy;
                }


                else if (next.graphic != '$')
                {
                    Dictionary<char, Rectangle> map = gc.spriteMap.GetMap();
                    //  = map['D'];
                    //next.rectangle['+'] = map['D'];
                }

                else if (next.graphic == '$')
                {
                }
            }
        }

        public void Logic(float frametime)
        {
            this.frametime = frametime;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.Render(e, frametime);
        }
    }

}


