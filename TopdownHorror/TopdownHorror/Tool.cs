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
    /// Class meant for items and weapons
    /// </summary>
    public class Tool
    {
        protected Player CurrentPlayer;
        public bool Constructed = false;
        public bool Firing = false;

        /// <summary>
        /// Name of the tool to be displayed
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Sound to play when Tool is used
        /// </summary>
        public SoundEffect UseSound;

        /// <summary>
        /// Sound to play when tool is used when ammo is 0
        /// </summary>
        public SoundEffect EmptySound = Game.LoadSoundEffect("clipempty");


        public bool Equipped = false;

        /// <summary>
        /// 
        /// </summary>
        public bool IsHitScan = false;

        /// <summary>
        /// Does the tool automatically keep using itself while use key is pressed
        /// </summary>
        public bool Automatic = false;

        /// <summary>
        /// Should the Tool be drawn
        /// </summary>
        public bool Drawable = true;

        /// <summary>
        /// Player sprite when tool is equipped
        /// </summary>
        public Image PlayerSprite = null;

        /// <summary>
        /// Maximum ammunition
        /// </summary>
        public int MaxAmmo = 20;

        /// <summary>
        /// Current ammunition
        /// </summary>
        public int Ammo = 20;

        /// <summary>
        /// Number of projectiles per shot
        /// </summary>
        public int Projectiles = 1;

        /// <summary>
        /// The Elapsed game time when the gun was last shot
        /// </summary>
        public float LastShot = 0f;

        /// <summary>
        /// Shots per minute
        /// </summary>
        public float FireRate = 700f;

        /// <summary>
        /// Damage dealt to enemies per projectile
        /// </summary>
        public float Damage = 10f;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="plrSprite">Player sprite when tool is equipped</param>
        /// <param name="drawable">Should the Tool be drawn</param>
        /// <param name="projectiles">Number of projectiles per shot</param>
        /// <param name="ammo">Current and Maximum ammunition</param>
        /// <param name="damage">Damage dealt to enemies per projectile</param>
        public Tool(Image plrSprite, bool drawable = true, int projectiles = 1, int ammo = 20, float damage = 10f)
        {
            PlayerSprite = plrSprite;
            Drawable = drawable;
            Projectiles = projectiles;
            MaxAmmo = ammo;
            Ammo = ammo;
            Damage = damage;
            Constructed = true;
        }
        public Tool()
        {

        }

        public Tool(Player plr, SoundEffect useSound)
        {
            CurrentPlayer = plr;
            UseSound = useSound;
            Constructed = true;
        }

        public void Equip()
        {
            Equipped = true;
        }

        public void UnEquip()
        {
            Equipped = false;
        }

        /// <summary>
        /// Fire a bullet in the shape of a line that instantly hits the target.
        /// </summary>
        protected void FireHitScan()
        {
            Raycast ray = new Raycast(CurrentPlayer.Position, CurrentPlayer.MoveTarget, 1024.0);
            RayResult res = CurrentPlayer.CurrentGame.FindEnemyOnRay(ray);
            if (res.Hit != null)
            {
                if (res.Hit is Enemy)
                {
                    res.Hit.TakeDamage(CurrentPlayer.EquippedTool.Damage);
                }
            }
        }

        void ProjectileHit(PhysicsObject p, PhysicsObject hit)
        {
            if (hit is Enemy)
            {
                Enemy e = hit as Enemy;
                e.TakeDamage(CurrentPlayer.EquippedTool.Damage);
            }
        }

        protected void FireProjectile()
        {
            Projectile p = new Projectile(1.0, 1.0, Color.Transparent);
            //p.Color = Color.Transparent;

            Direction d = CurrentPlayer.Angle.MainDirection;
            CurrentPlayer.CurrentGame.AddCollisionHandler(p, ProjectileHit);
            p.Hit(d.GetVector() * 1000.0);
        }


        public void Fire(float elapsed, float delta)
        {
            if (CurrentPlayer.Health > 0f && !Firing && (elapsed - LastShot >= (60f / FireRate)))
            {
                if (Ammo > 0)
                {
                    Firing = true;
                    CurrentPlayer.Firing = true;
                    UseSound.Play();
                    if (IsHitScan)
                    {
                        FireHitScan();
                    }
                    else
                    {
                        FireProjectile();
                    }
                    CurrentPlayer.Animation = CurrentPlayer.GetAnimationFromName("shoot");
                    CurrentPlayer.Animation.Start(1);
                    Ammo--;
                    LastShot = elapsed;
                    Firing = false;
                    CurrentPlayer.Firing = false;
                }
                else
                {
                    EmptySound.Play();
                }
            }
        }

        /// <summary>
        /// Update method to be called per game update
        /// </summary>
        /// <param name="time">Current GameTime</param>
        public virtual void Update(Time time)
        {
            //float delta = (float)(time.SinceLastUpdate.Milliseconds / 1000.0);
            //LastShot += delta;
        }
    }
}
