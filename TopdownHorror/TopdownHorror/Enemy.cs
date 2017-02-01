using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

namespace TopdownHorror
{
    /// <summary>
    /// Enemy type. (Basic, Boss, etc.)
    /// </summary>
    public enum EnemyType
    {
        Basic = 1,
        Devil = 2,
        Mega = 3,
        Boss = 4
    }

    /// <summary>
    /// Enemy NPC
    /// </summary>
    public class Enemy : PhysicsObject
    {
        public delegate void DiedEventHandler(float killingBlowDamage, float maxHp);
        public event DiedEventHandler Died;

        /// <summary>
        /// The Movespeed where Animation FPS is the value of AnimFPS
        /// </summary>
        public double DefaultMoveSpeed = 64.0;

        /// <summary>
        /// The Animation FPS at DefaultMoveSpeed
        /// </summary>
        public double AnimFPS = 5.0;

        public Player CurrentPlayer;
        public bool Attacking = false;
        /// <summary>
        /// Enemy's moving direction
        /// </summary>
        public Vector MoveTarget = Vector.Zero;

        /// <summary>
        /// 
        /// </summary>
        public float TimeSinceDeath = 0f;

        /// <summary>
        /// Enemy's move speed, in units per second
        /// </summary>
        public float MoveSpeed = 108f;

        /// <summary>
        /// Enemy's maximum health points
        /// </summary>
        public float MaxHealth = 100f;

        /// <summary>
        /// Enemy's health points
        /// </summary>
        public float Health = 100f;

        /// <summary>
        /// Enemy attack range
        /// </summary>
        public double AttackRange = 64.0;

        /// <summary>
        /// Attack damage
        /// </summary>
        public float HitDamage = 20f;


        public void TakeDamage(float damage)
        {
            Health = Utilities.Clamp(Health - damage, 0f, MaxHealth);
            if (Health == 0)
            {
                int deathAnimID = RandomGen.NextInt(1, 2);
                ChangeAnimationTo(Animations.BasicEnemy["death0" + deathAnimID], false);
                Animation.StopOnLastFrame = true;
                Animation.Played += new Action(StopAttacking);

                //Create a shower of blood

                //int bloodAnimID = RandomGen.NextInt(1, 2);
                GameObject bloodSplash = new GameObject(128.0, 128.0);
                bloodSplash.Position = Position;
                //bloodSplash.Animation = Animations.Blood["Blood0" + bloodAnimID];
                bloodSplash.Animation = Animations.Blood["Blood01"];
                CurrentPlayer.CurrentGame.Add(bloodSplash);
                bloodSplash.Animation.Start(1);
                bloodSplash.Animation.StopOnLastFrame = true;

                //Create an expanding pool of blood

                int bloodPoolID = RandomGen.NextInt(1, 2);
                BloodPool blood = new BloodPool(1.0, 1.0);
                blood.Position = Position + MoveTarget * 48.0;
                blood.Angle = RandomGen.NextAngle();
                blood.Image = Game.LoadImage("Textures/Blood/blood0" + bloodPoolID);
                CurrentPlayer.CurrentGame.BloodPools.Add(blood);
                CurrentPlayer.CurrentGame.Add(blood, 0);

                Died(damage, MaxHealth);
            }
        }

        /// <summary>
        /// Creates a new enemy from an EnemyType
        /// </summary>
        /// <param name="type">What type of enemy to spawn</param>
        /// <returns></returns>
        public static Enemy FromEnemyType(Player player, EnemyType type)
        {
            //Image sprite;
            Animation anim = Game.LoadAnimation("Textures/Enemy/walk");;
            switch(type)
            {
                case EnemyType.Basic:
                    //sprite = Game.LoadImage("Textures/Enemy/idle/idle0000");
                    anim = Game.LoadAnimation("Textures/Enemy/walk");
                    break;
                case EnemyType.Devil:
                    //sprite = Game.LoadImage("skeleton-idle_0");
                    break;
                case EnemyType.Mega:
                    //sprite = Game.LoadImage("skeleton-idle_0");
                    break;
                case EnemyType.Boss:
                    //sprite = Game.LoadImage("skeleton-idle_0");
                    break;
                default:
                    //sprite = Game.LoadImage("skeleton-idle_0");
                    break;
            }
            //return Enemy.FromSprite(player, sprite);
            return Enemy.FromAnimation(player, anim);
        }


        public void StopAttacking()
        {
            Attacking = false;
            if ((CurrentPlayer.Position - Position).Magnitude < AttackRange)
            {
                CurrentPlayer.TakeDamage(HitDamage);
            }
        }

        public void AttackPlayer()
        {
            if (!Attacking)
            {
                Attacking = true;
                int attackAnimID = RandomGen.NextInt(1, 3);
                ChangeAnimationTo(Animations.BasicEnemy["attack0" + attackAnimID], false);
                Animation.Played += new Action(StopAttacking);
            }
        }

        public override void Update(Time time)
        {
            base.Update(time);
            if (CurrentPlayer != null)
            {
                if (Health > 0f && CurrentPlayer.Health > 0f)
                {
                    if ((CurrentPlayer.Position - Position).Magnitude < AttackRange)
                    {
                        AttackPlayer();
                    }
                    else
                    {
                        if (Animation != Animations.BasicEnemy["walk"])
                        {
                            ChangeAnimationTo(Animations.BasicEnemy["walk"]);
                        }
                        float delta = (float)(time.SinceLastUpdate.TotalMilliseconds / 1000.0);
                        Vector direction = (CurrentPlayer.Position - Position);
                        //Normalize direction
                        MoveTarget = direction / direction.Magnitude;
                        Angle = Angle.FromRadians(Math.Atan2(direction.Y, direction.X));
                        Position += MoveTarget * MoveSpeed * delta;
                    }
                }
                else
                {
                    if (Health > 0f)
                    {
                        if (Animation != Animations.BasicEnemy["idle"])
                        {
                            ChangeAnimationTo(Animations.BasicEnemy["idle"]);
                        }
                    }
                }
                if (Health <= 0f)
                {
                    TimeSinceDeath += (float)(time.SinceLastUpdate.TotalMilliseconds / 1000.0);
                }
            }
        }

        public void ChangeAnimationTo(string a, bool looped = true)
        {
            Animation anim = Game.LoadAnimation(a);
            Animation = anim;
            if (looped)
            {
                Animation.Start();
                return;
            }
            Animation.Start(1);
        }
        public void ChangeAnimationTo(Animation anim, bool looped = true)
        {
            Animation = anim;
            if (looped)
            {
                Animation.Start();
                return;
            }
            if (anim == Animations.BasicEnemy["walk"])
            {
                double ratio = DefaultMoveSpeed / AnimFPS;
                Animation.FPS = MoveSpeed * ratio;
            }
            Animation.Start(1);
        }

        /// <summary>
        /// Creates a new enemy with the given sprite
        /// </summary>
        /// <param name="player">Player instance</param>
        /// <param name="sprite">Sprite to be given to the enemy</param>
        /// <returns></returns>
        public static Enemy FromSprite(Player player, Image sprite)
        {
            Enemy e = new Enemy();
            e.Image = sprite;
            e.CurrentPlayer = player;
            return e;
        }

        /// <summary>
        /// Creates a new enemy with the given started animation
        /// </summary>
        /// <param name="player">Player instance</param>
        /// <param name="anim">Sprite to be given to the enemy</param>
        /// <returns></returns>
        public static Enemy FromAnimation(Player player, Animation anim)
        {
            Enemy e = new Enemy();
            e.Animation = anim;
            e.CurrentPlayer = player;
            e.Animation.Start();
            e.Animation.Step(RandomGen.NextInt(1, 30));
            return e;
        }




        public Enemy()
            : base(256.0, 256.0)
        {
        }
    }
}
