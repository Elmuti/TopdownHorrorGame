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
    /// Meant for droppable tools such as exploding barrels
    /// </summary>
    public class DroppableTool : Tool
    {
        public Image ItemSprite = null;

        /// <summary>
        /// Is Dropped tool triggered by enemy touch
        /// </summary>
        public bool IsTouchTriggered = false;

        /// <summary>
        /// Is Dropped tool triggered by bullets
        /// </summary>
        public bool IsBulletTriggered = false;

        /// <summary>
        /// Is Dropped tool triggered by explosions
        /// </summary>
        public bool IsExplosionTriggered = false;


        public DroppableTool()
        {

        }
        public DroppableTool(SoundEffect useSound)
        {
            UseSound = useSound;
        }

        public override void Update(Time time)
        {

        }
    }
}
