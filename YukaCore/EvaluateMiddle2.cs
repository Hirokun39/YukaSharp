using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    sealed class EvaluateMiddle2 : EvaluateFatLib
    {
        public override string Name()
        {
            return "百恵";
        }

        public override string  Comments()
        {
            return "中級＋";
        }
        
        public override EvaluateBase NewCopy()
        {
            EvaluateBase gc = new EvaluateMiddle2();
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

            int score = openCornerScore + ((playersFixedCount - enemysFixedCount) << 24);

            if (playerFCount > enemyFCount)
            {
                score += ((64 - enemyFCount) << 16) + (playerFCount << 8);
            }
            else if (playerFCount < enemyFCount)
            {
                score += ((-64 + playerFCount) << 16) - (enemyFCount << 8);
            }

            return score;
        }
    }
}
