using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    sealed public class EvaluateNovice : EvaluateFatLib
    {
        public override string Name()
        {
            return "聖子";
        }

        public override string Comments()
        {
            return "初級";
        }

        public override int ExactDepth()
        {
            return 0;
        }

        public override EvaluateBase NewCopy()
        {
    	    EvaluateFatLib gc = new EvaluateNovice();
    	    gc.CopyFrom(this);
     	    return gc;
        }

        protected override int EvaluateOpenCorner(int corner, int major, int minor)
        {
    	    return EvaluateOpenCornerLv0(corner, major, minor);
        }

        protected override int EvaluateBody()
        {
    	    return ((counts[turn]-counts[Enemy]) << 16)
                + openCornerScore
                + ((playersFixedCount - enemysFixedCount) << 24);
        }

    }
}
