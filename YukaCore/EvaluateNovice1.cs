using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    sealed public class EvaluateNovice1 : EvaluateFatLib
    {
        public override string Name()
        {
            return "美和子";
        }

        public override string Comments()
        {
            return "初級＋";
        }

        public override bool NoRandom()
        {
            return true;
        }

        public override EvaluateBase NewCopy()
        {
            EvaluateFatLib gc = new EvaluateNovice1();
            gc.CopyFrom(this);
            return gc;
        }

        protected override int EvaluateOpenCorner(int corner, int major, int minor)
        {
            return EvaluateOpenCornerLv0(corner, major, minor);
        }

        protected override int EvaluateBody()
        {
            return openCornerScore + ((playersFixedCount - enemysFixedCount) << 24);
        }

    }
}
