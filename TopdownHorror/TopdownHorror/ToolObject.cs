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
    /// Represents spawned objects such as explosive barrels or grenades.
    /// </summary>
    public class ToolObject
    {
        /// <summary>
        /// Texture of dropped tool
        /// </summary>
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

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="itemSprite">Texture of dropped tool</param>
        /// <param name="isTouch">Is dropped tool triggered by enemy touch</param>
        /// <param name="isBullet">Is dropped tool triggered by bullets</param>
        /// <param name="isExp">Is dropped tool triggered by explosions</param>
        public ToolObject(Image itemSprite, bool isTouch = false, bool isBullet = true, bool isExp = true)
        {
            ItemSprite = itemSprite;
            IsTouchTriggered = isTouch;
            IsBulletTriggered = isBullet;
            IsExplosionTriggered = isExp;
        }


        public void Update(Time time)
        {

        }
    }
}
