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
    public class BloodPool : GameObject
    {
        public float Age = 0f;



        public override void Update(Time time)
        {
            base.Update(time);
            Age += (float)time.SinceLastUpdate.TotalSeconds;
        }

        public BloodPool()
            :base(1.0, 1.0)
        {
        }
        public BloodPool(double width, double height)
            : base(width, height)
        {
        }

    }
}


