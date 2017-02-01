using System;
using System.Collections.Generic;
using System.Linq;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

namespace TopdownHorror
{
    public class ThisGame : PhysicsGame
    {
        public Player Player;
        public List<Key> KeysDown = new List<Key>();
        public SoundEffect fireSndFx = LoadSoundEffect("pistol_fire");
        public Time GameTime;
        public int CurrentWave = 1;
        public float ElapsedTime = 0f;
        public float Delta = 0f;
        public List<Enemy> Enemies = new List<Enemy>();
        public List<DroppableTool> DroppedToolObjects = new List<DroppableTool>();
        public List<BloodPool> BloodPools = new List<BloodPool>();
        public bool TestEnemyKilled = true;
        public const float BLOODPOOL_LIFETIME = 5f;
        public const float CORPSE_LIFETIME = 5f;

        public const float ENEMY_BASE_SPEED = 32f;
        public const float ENEMY_BASE_DAMAGE = 9f;
        public const float ENEMY_BASE_HEALTH = 100f;
        public const float ENEMY_MAX_SPEED = 148f;
        public const float ENEMY_MAX_DAMAGE = 9f;
        public const float ENEMY_MAX_HEALTH = 256f;

        public const int ENEMY_BASE_COUNT = 8;
        public const int ENEMY_MAX_COUNT = 64;
        public const int ENEMY_MAX_RENDERCOUNT = 64;

        public const int WAVE_BASE = 1;
        public const int WAVE_MAX = 10;

        public int WaveEnemiesSpawned = 0;

        public int GetEnemiesAlive()
        {
            int count = 0;
            foreach (Enemy e in Enemies)
            {
                if (e.Health > 0f)
                {
                    count++;
                }
            }
            return count;
        }

        public RayResult FindEnemyOnRay(Raycast ray)
        {
            RayResult res = new RayResult();

            foreach(Enemy e in Enemies)
            {
                res = ray.Intersects(e);
                if (res.Hit != null)
                {
                    Console.WriteLine(res.Hit.ToString());
                    return res;
                }
            }
            Console.WriteLine("Pos: " + res.Position.ToString());
            Console.WriteLine("Hit: null");
            return res;
        }

        public void SpawnEnemy()
        {
            Enemy newEnemy = Enemy.FromEnemyType(Player, EnemyType.Basic);
            newEnemy.Position = RandomGen.NextVector(0.0, 0.0, 5120.0, 5120.0);
            float ratio = (CurrentWave - WAVE_BASE) / (WAVE_MAX - WAVE_BASE);
            newEnemy.MoveSpeed = ENEMY_BASE_COUNT + ratio * (ENEMY_MAX_COUNT - ENEMY_BASE_COUNT);
            newEnemy.Health = ENEMY_BASE_HEALTH + ratio * (ENEMY_MAX_HEALTH - ENEMY_BASE_HEALTH);
            newEnemy.HitDamage = ENEMY_BASE_DAMAGE + ratio * (ENEMY_MAX_DAMAGE - ENEMY_BASE_DAMAGE);
            Enemies.Add(newEnemy);
            Add(newEnemy, 1);
            newEnemy.Died += new Enemy.DiedEventHandler(Player.AwardPoints);
        }

        protected override void Update(Time time)
        {
            base.Update(time);
            Delta = (float)(time.SinceLastUpdate.Milliseconds / 1000.0);
            ElapsedTime += Delta;

            int aliveEnemies = GetEnemiesAlive();
            float ratio = (CurrentWave - WAVE_BASE) / (WAVE_MAX - WAVE_BASE);
            float ets = ENEMY_BASE_COUNT + ratio * (ENEMY_MAX_COUNT - ENEMY_BASE_COUNT);
            float enemiesToSpawn = (int)ets;


            if (aliveEnemies < enemiesToSpawn && aliveEnemies < ENEMY_MAX_RENDERCOUNT && WaveEnemiesSpawned <= ENEMY_MAX_COUNT)
            {
                for (int i = 1; i <= ENEMY_MAX_RENDERCOUNT - GetEnemiesAlive(); i++)
                {
                    SpawnEnemy();
                    WaveEnemiesSpawned++;
                }
            }
            else if (WaveEnemiesSpawned >= ENEMY_MAX_COUNT)
            {
                CurrentWave++;
                Player.Wave++;
                WaveEnemiesSpawned = 0;
            }


            if (ElapsedTime > 5f && !TestEnemyKilled)
            {
                Enemies[2].TakeDamage(300f);
                TestEnemyKilled = true;
            }

            if (Player != null)
            {
                Camera.Position = Player.Position;
            }
            GameTime = time;
            if (Player != null)
            {
                Player.Update(time);
                foreach(Tool t in Player.Tools)
                {
                    if (t.Equipped)
                    {
                        t.Update(time);
                    }
                }
            }

            foreach (BloodPool blood in BloodPools)
            {
                if (blood.Age < BLOODPOOL_LIFETIME)
                {
                    if (blood.Size.X < 128.0)
                    {
                        blood.Size += new Vector(10.0, 10.0) * Delta;
                    }
                }
                else
                {
                    //BloodPools.Remove(blood);
                    blood.Destroy();
                }
            }

            foreach(Enemy e in Enemies)
            {
                if (e.TimeSinceDeath < CORPSE_LIFETIME)
                {
                    e.Update(time);
                }
                else
                {
                    e.IsUpdated = false;
                    //Enemies.Remove(e);
                    e.Destroy();
                }
            }

            if (KeysDown.Contains(Key.Space) && Player.Health > 0f)
            {
                Player.EquippedTool.Fire(ElapsedTime, Delta);
            }
        }

        public void KeyEventHandle(Key key)
        {
            if (Player.Health > 0f)
            {
                if (KeysDown.Contains(Key.LeftShift))
                {
                    Player.MoveSpeed = 168f;
                }
                else
                {
                    Player.MoveSpeed = 128f;
                }

                if (KeysDown.Contains(Key.W)) //Up
                {
                    Player.MoveTowards(Player.MoveTarget + new Vector(0, 1));
                }
                else { Player.MoveTowards(Player.MoveTarget + new Vector(0, -1)); }

                if (KeysDown.Contains(Key.S)) //Down
                {
                    Player.MoveTowards(Player.MoveTarget + new Vector(0, -1));
                }
                else { Player.MoveTowards(Player.MoveTarget + new Vector(0, 1)); }

                if (KeysDown.Contains(Key.A)) //Left
                {
                    Player.MoveTowards(Player.MoveTarget + new Vector(-1, 0));
                }
                else { Player.MoveTowards(Player.MoveTarget + new Vector(1, 0)); }

                if (KeysDown.Contains(Key.D)) //Right
                {
                    Player.MoveTowards(Player.MoveTarget + new Vector(1, 0));
                }
                else { Player.MoveTowards(Player.MoveTarget + new Vector(-1, 0)); }
            }
        }

        public void KeyDown(Key key)
        {
            if (!KeysDown.Contains(key))
            {
                KeysDown.Add(key);
            }
            KeyEventHandle(key);
        }

        public void KeyUp(Key key)
        {
            if (KeysDown.Contains(key))
            {
                KeysDown.Remove(key);
            }
            if (!KeysDown.Contains(Key.W) && !KeysDown.Contains(Key.A) && !KeysDown.Contains(Key.S) && !KeysDown.Contains(Key.D))
            {
                Player.StopMoving();
            }
            else
            {
                KeyEventHandle(key);
            }
        }

        public void PlayerDied()
        {
            Label display = new Label();
            display.Text = "GAME OVER.";
            //display.Font = LoadFont("diablo_h");
            display.Position = new Vector(25.0, 50.0);
            display.TextScale = new Vector(2, 2);
            display.TextColor = Color.Red;
            Label gore = new Label();
            gore.Image = LoadImage("Textures/bloodblur");
            gore.Size = new Vector(Screen.Width, Screen.Height);
            Add(display);
            Add(gore);
            Player.Animation = null;
            Player.Image = LoadImage("headstone");
            Player.Angle = Angle.Zero;
        }

        public override void Begin()
        {
            //Level.Background.Image = LoadImage("grass");
            SoundEffect.MasterVolume = 0.1;
            MediaPlayer.Volume = 0.25;
            //MediaPlayer.Play("redlining_6th");

            //Generate grass
            for (int x = 0; x <= 10; x++)
            {
                for (int y = 0; y <= 10; y++)
                {
                    GameObject grasstile = new GameObject(512.0, 512.0);
                    grasstile.Position = new Vector(x * 512.0, y * 512.0);
                    grasstile.Image = LoadImage("grass");
                    Add(grasstile);
                }
            }

            //Generate walls
            //Top
            for (int x = 0; x <= 10; x++)
            {
                GameObject wallTile = new GameObject(512.0, 512.0);
                wallTile.Position = new Vector(x * 512.0, 0);
                wallTile.Image = LoadImage("stone");
                Add(wallTile);
            }
            for (int y = 0; y <= 10; y++)
            {
                GameObject wallTile = new GameObject(512.0, 512.0);
                wallTile.Position = new Vector(0, y * 512.0);
                wallTile.Image = LoadImage("stone");
                Add(wallTile);
            }


            Player = Player.FromAnimation(this, Animations.Player["Idle"], Animations.PlayerFeet["Idle"]);
            Player.Position = new Vector(1200, 1200);
            //Player = Player.FromAnimation(Animations.Player.Idle);
            Player.Died += new Player.DiedEventHandler(PlayerDied);

            Add(Player.Feet, 2);
            Add(Player, 3);

            //Generate trees
            for (int i = 0; i <= 50; i++ )
            {
                PhysicsObject tree = new PhysicsObject(128.0, 128.0);
                tree.Position = RandomGen.NextVector(0.0, 0.0, 5120.0, 5120.0);
                tree.Image = LoadImage("tree");
                Add(tree, 3);
            }

            Level.CreateBorders();



            //Give starter weapon
            //Tool ak47 = new Tool(Player, LoadSoundEffect("pistol_fire"));
            Tool ak47 = new Tool(Player, LoadSoundEffect("ak47"));
            ak47.Name = "AK-47";
            ak47.Damage = 100f;
            ak47.IsHitScan = false;
            ak47.Automatic = false;
            ak47.MaxAmmo = 30;
            ak47.Ammo = 30;
            Player.Tools.Add(ak47);
            Player.EquipTool(0);


            Keyboard.Listen(Key.W, ButtonState.Pressed, KeyDown, "Move up", Key.W);
            Keyboard.Listen(Key.W, ButtonState.Released, KeyUp, "Stop moving up", Key.W);
            Keyboard.Listen(Key.A, ButtonState.Pressed, KeyDown, "Move left", Key.A);
            Keyboard.Listen(Key.A, ButtonState.Released, KeyUp, "Stop moving left", Key.A);
            Keyboard.Listen(Key.S, ButtonState.Pressed, KeyDown, "Move down", Key.S);
            Keyboard.Listen(Key.S, ButtonState.Released, KeyUp, "Stop moving down", Key.S);
            Keyboard.Listen(Key.D, ButtonState.Pressed, KeyDown, "Move right", Key.D);
            Keyboard.Listen(Key.D, ButtonState.Released, KeyUp, "Stop moving right", Key.D);
            Keyboard.Listen(Key.Space, ButtonState.Down, KeyDown, "Fire weapon", Key.Space);
            Keyboard.Listen(Key.Space, ButtonState.Released, KeyUp, "Fire weapon", Key.Space);
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Quit game");
        }
    }
}