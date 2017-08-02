using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    sealed public class EvaluateMiddle : EvaluateFatLib
    {
        public override string Name()
        {
            return "明菜";
        }

        public override string Comments()
        {
            return "中級－";
        }

        public override EvaluateBase NewCopy()
        {
    	    EvaluateFatLib gc = new EvaluateMiddle();
    	    gc.CopyFrom(this);
    	    return gc;
        }

        protected override int EvaluateOpenCorner(int corner, int major, int minor)
        {
    	    return EvaluateOpenCornerLv0(corner, major, minor);
        }

        protected override int EvaluateBody()
        {
    	    return openCornerScore
                + ((playersFixedCount - enemysFixedCount) << 24)
                + ((counts[Enemy]-counts[turn]) << 16);
        }
    }
}
