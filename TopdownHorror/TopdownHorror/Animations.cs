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
    public class Animations
    {
        public static Dictionary<string, string> TypeFromWeapon = new Dictionary<string, string>()
        {
            {"Pistol", "handgun"},
            {"Submachinegun", "handgun"},
            {"AK-47", "rifle"},
            {"Rocket Launcher", "rifle"},
            {"Shotgun", "shotgun"}
        };

        public static Dictionary<string, Animation> Blood = new Dictionary<string, Animation>()
        {
            {"Blood01", Game.LoadAnimation("Textures/Blood/bloodanim01")},
            {"Blood02", Game.LoadAnimation("Textures/Player/handgun/move")}
        };

        public static Dictionary<string, Animation> Player = new Dictionary<string, Animation>()
        {
            {"Idle", Game.LoadAnimation("Textures/Player/handgun/idle")},
            {"Move", Game.LoadAnimation("Textures/Player/handgun/move")}
        };

        public static Dictionary<string, Animation> PlayerFeet = new Dictionary<string, Animation>()
        {
            {"Idle", Game.LoadAnimation("Textures/Player/feet/idle")},
            {"Run", Game.LoadAnimation("Textures/Player/feet/run")}
        };

        public static Dictionary<string, Animation> BasicEnemy = new Dictionary<string, Animation>()
        {
            {"idle", Game.LoadAnimation("Textures/Enemy/idle")},
            {"walk", Game.LoadAnimation("Textures/Enemy/walk")},
            {"run", Game.LoadAnimation("Textures/Enemy/run")},
            {"attack01", Game.LoadAnimation("Textures/Enemy/attack01")},
            {"attack02", Game.LoadAnimation("Textures/Enemy/attack02")},
            {"attack03", Game.LoadAnimation("Textures/Enemy/attack03")},
            {"death01", Game.LoadAnimation("Textures/Enemy/death01")},
            {"death02", Game.LoadAnimation("Textures/Enemy/death02")}
        };
    }
}
