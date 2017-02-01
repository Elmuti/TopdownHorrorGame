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
    /// Player object
    /// </summary>
    public class Player : PhysicsObject
    {
        public delegate void DiedEventHandler();
        public event DiedEventHandler Died;

        public ThisGame CurrentGame;

        public PhysicsObject Feet;
        /// <summary>
        /// Should the player move
        /// </summary>
        public bool Moving = false;

        public bool Firing = false;

        public float _timeMoving = 0f;
        /// <summary>
        /// Time spent moving
        /// </summary>
        public float TimeMoving
        {
            get
            {
                return _timeMoving;
            }
            set
            {
                _timeMoving = Utilities.Clamp(value, 0f, MoveMaxAccel);
            }
        }

        /// <summary>
        /// Current wave
        /// </summary>
        public int Wave = 1;

        /// <summary>
        /// 
        /// </summary>
        public int Points = 0;

        /// <summary>
        /// Time it takes until player has reached maximum running speed.
        /// </summary>
        public float MoveMaxAccel = 0.25f;


        /// <summary>
        /// Player's moving direction
        /// </summary>
        public Vector MoveTarget = Vector.Zero;

        /// <summary>
        /// Player move speed, in units per second
        /// </summary>
        public float MoveSpeed = 128f;

        /// <summary>
        /// Player maximum health points
        /// </summary>
        public float MaxHealth = 100f;

        /// <summary>
        /// Player health points
        /// </summary>
        public float Health = 100f;

        /// <summary>
        /// Player inventory
        /// </summary>
        public List<Tool> Tools = new List<Tool>();

        /// <summary>
        /// Currently equipped tool.
        /// </summary>
        public Tool EquippedTool;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            Health = Utilities.Clamp(Health - damage, 0f, MaxHealth);
            if (Health == 0)
            {
                GameObject bloodSplash = new GameObject(128.0, 128.0);
                bloodSplash.Position = Position;
                //bloodSplash.Animation = Animations.Blood["Blood0" + bloodAnimID];
                bloodSplash.Animation = Animations.Blood["Blood01"];
                CurrentGame.Add(bloodSplash);
                bloodSplash.Animation.Start(1);
                bloodSplash.Animation.StopOnLastFrame = true;
                Died();
            }
        }        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points">Number of points to award</param>
        public void AwardPoints(float killingBlowDamage, float maxHp)
        {
            float mult = killingBlowDamage / maxHp;
            int awardedPoints = (int)(10f * Wave * mult);
            Points += awardedPoints;
            Console.WriteLine("Player was awarded " + awardedPoints + " pts.");
        }

        public Animation GetAnimationFromName(string name)
        {
            string spriteCategory = Animations.TypeFromWeapon[EquippedTool.Name];
            return Game.LoadAnimation("Textures/Player/"+ spriteCategory +"/" + name);
        }

        /// <summary>
        /// Make player move towards a direction
        /// </summary>
        /// <param name="dir">Direction to move towards</param>
        public void MoveTowards(Vector dir)
        {
            Moving = true;
            MoveTarget = new Vector(Utilities.Clamp(dir.X, -1.0, 1.0), Utilities.Clamp(dir.Y, -1.0, 1.0));
            if (Animation != GetAnimationFromName("move") && !Firing)
            {
                ChangeAnimationTo(GetAnimationFromName("move"), Animations.PlayerFeet["Run"]);
            }
        }

        /// <summary>
        /// Stop player from moving
        /// </summary>
        public void StopMoving()
        {
            Moving = false;
            TimeMoving = 0f;
            MoveTarget = Utilities.PolarToCartesian(Angle.Degrees);
            Animation.Stop();
            ChangeAnimationTo(GetAnimationFromName("idle"), Animations.PlayerFeet["Idle"]);
        }

        /// <summary>
        /// Make player equip a tool
        /// </summary>
        /// <param name="slot"></param>
        public void EquipTool(int slot)
        {
            Tool t = Tools.ElementAt(slot);
            t.Equip();
            EquippedTool = t;
        }

        /// <summary>
        /// Make player fire currently equipped tool.
        /// </summary>
        public Tool GetEquippedTool()
        {
            foreach(Tool t in Tools)
            {
                if (t.Equipped)
                {
                    return t;
                }
            }
            return new Tool();
        }
        public Player()
            :base(64.0, 64.0)
        {
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sprite">Player default sprite</param>
        public static Player FromSprite(Image sprite)
        {
            Player plr = new Player();
            //plr.Size = new Vector(64.0, 64.0);
            plr.Image = sprite;
            plr.Feet = new PhysicsObject(64.0, 64.0);
            return plr;
        }

        public static Player FromAnimation(ThisGame game, Animation anim, Animation feetAnim)
        {
            PhysicsObject feet = new PhysicsObject(48.0, 48.0);
            Player plr = new Player();
            //plr.Size = new Vector(64.0, 64.0);
            plr.Animation = anim;
            plr.Animation.Start();
            plr.Feet = feet;
            plr.Feet.Animation = feetAnim;
            plr.Feet.Animation.Start();
            plr.CurrentGame = game;
            return plr;
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
        public void ChangeAnimationTo(Animation anim, Animation feetAnim, bool looped = true)
        {
            if (!Firing)
            {
                Animation = anim;
                Feet.Animation = feetAnim;
                if (looped)
                {
                    Animation.Start();
                    Feet.Animation.Start();
                    return;
                }
                Animation.Start(1);
                Feet.Animation.Start(1);
            }
        }

        /// <summary>
        /// Update method to be called each frame
        /// </summary>
        /// <param name="time">GameTime</param>
        public override void Update(Time time)
        {
            base.Update(time);
            float delta = (float)(time.SinceLastUpdate.TotalMilliseconds / 1000.0);
            if (Moving)
            {
                //Body.Angle = Angle.FromDegrees((Math.Atan2(-MoveTarget.X, -MoveTarget.Y) * 180.0 / Math.PI));
                Angle = Utilities.GetAngleFromDirection(MoveTarget);
                Feet.Angle = Angle;
                TimeMoving += delta;
                Position += MoveTarget * (TimeMoving / MoveMaxAccel) * MoveSpeed * delta;
                Feet.Position = Position;
            }
        }
    }
}
