using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    public class EvaluateBeginner : EvaluateBase
    {
        public override string Name()
        {
            return "今日子";
        }

        public override string Comments()
        {
            return "初心者";
        }

        public override int ExactDepth()
        {
            return 0;
        }

        public override EvaluateBase NewCopy()
        {
        	EvaluateBase gc = new EvaluateBeginner();
	        gc.CopyFrom(this);
    	    return gc;
        }

        public override int Evaluate()
        {
    	    return (counts[turn]-counts[Enemy]) << 24;
        }

    }
}
