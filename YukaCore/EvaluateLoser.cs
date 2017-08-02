using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YukaCore
{
    public class EvaluateLoser : EvaluateBase
    {
        public override string Name()
        {
            return "ローザ";
        }

        public override string Comments()
        {
            return "わざと負け";
        }

        public EvaluateLoser()
            : base()
        {
        }

        public override EvaluateBase NewCopy()
        {
            EvaluateBase gc = new EvaluateLoser();
            gc.CopyFrom(this);
            return gc;
        }

        public override int Evaluate()
        {
            int [] fixedDiscs = new int[3];
            fixedDiscs[black] = 0;
            fixedDiscs[white] = 0;

            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if (IsFixed(i * down + j * right))
                    {
                        fixedDiscs[board[i * down + j * right]]++;
                    }
                }
            }

            return -((counts[nextPlayer] - counts[Enemy]) << 16)
                   -((fixedDiscs[nextPlayer] - fixedDiscs[Enemy]) << 24);
        }
    }
}
