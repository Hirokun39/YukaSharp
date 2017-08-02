using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    sealed class EvaluateMiddle1 : EvaluateFatLib
    {
        public override string Name()
        {
            return "ちえみ";
        }

        public override string Comments()
        {
            return "中級";
        }

        public override EvaluateBase NewCopy()
        {
            EvaluateBase gc = new EvaluateMiddle1();
            gc.CopyFrom(this);
            return gc;
        }

        protected override int EvaluateOpenCorner(int corner, int major, int minor)
        {
            return EvaluateOpenCornerLv0(corner, major, minor);
        }

        protected override int EvaluateBody()
        {
            int playerFCount = 0;
            int enemyFCount = 0;
            byte enemy = Enemy;
            int nLoop = counts[space];
            for (int i = 0; i < nLoop; i++)
            {
                int place = spaceTable[i];
                if (Feasible(place))
                {
                    playerFCount++;
                }
                if (Feasible(place, enemy))
                {
                    enemyFCount++;
                }
            }

            return openCornerScore
                + ((playersFixedCount - enemysFixedCount) << 24)
                + ((playerFCount - enemyFCount) << 16);

        }
    }
}
