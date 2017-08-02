using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    public class EvaluateBase : GameContext
    {
        // this class is not a real class, but I'm not choose abstract for implenmation reason.

        const string panic = "Application PANIC";

        private readonly byte[] unReversible = { 1, 2, 4, 8, 1, 2, 4, 8 };
        private byte[] unRevFlags;

        public virtual EvaluateBase NewCopy()
        {
            throw new Exception(panic);
        }

        public virtual int Evaluate()
        {
            throw new Exception(panic);
        }

        public virtual string Name()
        {
            throw new Exception(panic);
        }

        public virtual string Comments()
        {
            throw new Exception(panic);
        }

        public virtual int ExactDepth()
        {
            return 14; // this is default value of possible exact depth.
        }

        public virtual int DefaultDepth()
        {
            return 3; // this is default value of depth.
        }

        public virtual bool NoRandom()
        {
            return false;
        }

        public EvaluateBase()
            : base()
        {
            unRevFlags = new byte[boardSize];
        }

        public virtual void CopyFrom(EvaluateBase src)
        {
            CopyFromGC(src);
            src.unRevFlags.CopyTo(this.unRevFlags, 0);
        }

        public new void CopyFromGC(GameContext gc)
        {
            this.Init();
            while (this.SpaceCount != gc.SpaceCount)
            {
                if (MustBePassed())
                {
                    this.Pass();
                }
                if (!this.IsGameEnd())
                {
                    int dummy = Reverse(gc.GetHistory(this.SpaceCount));
                }
            }
            if (gc.PassCount != 0)
            {
                this.Pass();
            }
        }

        public bool IsFixed(int move)
        {
            return unRevFlags[move] == 0xf;
        }

        private void InitUnRevFlags(int corner, int scanDirection)
        {
            unRevFlags[corner] = 0x0f;

            int j = corner + scanDirection;
            int flags = 0;

            for (int i = 0; i < 8; i++)
            {
                if (board[j + DirectionTable[i]] == outside)
                {
                    flags |= unReversible[i];
                }
            }

            for (int n = 0; n < 6; n++)
            {
                unRevFlags[j] = (byte)flags;
                j += scanDirection;
            }
        }

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < boardSize; i++)
            {
                unRevFlags[i] = 0xf;
            }

            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    unRevFlags[i * down + j * right] = 0;
                }
            }

            InitUnRevFlags(upperLeftCorner, right);
            InitUnRevFlags(upperRightCorner, down);
            InitUnRevFlags(bottomRightCorner, left);
            InitUnRevFlags(bottomLeftCorner, up);
        }

        private void CheckFixed(int move)
        {
            if (Board(move) == space) return;
            if (IsFixed(move)) return;

            byte flags = 0;
            for (int i = 0; i < 8; i++)
            {
                int j = move + DirectionTable[i];
                if (IsFixed(j) && ((board[j] & board[move]) != 0))
                {
                    byte prevFlag = unRevFlags[move];
                    flags |= unReversible[i];
                }
            }
            if ((flags | unRevFlags[move]) == 0xf)
            {
                byte prevFlag = unRevFlags[move];
                unRevFlags[move] = 0xf;
                for (int i = 0; i < 8; i++)
                {
                    CheckFixed(move + DirectionTable[i]);
                }
            }
        }

        public override int Reverse(int move)
        {
            int result = FastReverse(move);

            switch (move)
            {
                case upperLeftCorner:
                    ScanCorner(upperLeftCorner, right, down);
                    break;
                case upperRightCorner:
                    ScanCorner(upperRightCorner, left, down);
                    break;
                case bottomLeftCorner:
                    ScanCorner(bottomLeftCorner, right, up);
                    break;
                case bottomRightCorner:
                    ScanCorner(bottomRightCorner, left, up);
                    break;
            }

            CheckFixed(move);
            for (int i = 0; i < 4; i++)
            {
                int scanDirection = DirectionTable[i];
                bool flag1 = true;
                for (int j = move + scanDirection; board[j] != outside; j += scanDirection)
                {
                    CheckFixed(j);
                    if (board[j] == space)
                    {
                        flag1 = false;
                        break;
                    }
                }
                bool flag2 = true;
                for (int j = move - scanDirection; board[j] != outside; j -= scanDirection)
                {
                    CheckFixed(j);
                    if (board[j] == space)
                    {
                        flag2 = false;
                        break;
                    }
                }
                if (flag1 & flag2)
                {
                    unRevFlags[move] |= unReversible[i];
                    for (int j = move - scanDirection; board[j] != outside; j -= scanDirection)
                    {
                        unRevFlags[j] |= unReversible[i];
                    }
                    for (int j = move + scanDirection; board[j] != outside; j += scanDirection)
                    {
                        unRevFlags[j] |= unReversible[i];
                    }
                }
            }
            return result;
        }

        private void ScanFixedDiscs(int corner, int major, int minor)
        {
            byte cornerDisc = board[corner];
            int length = 8;
            for (int minorscan = corner; board[minorscan] == cornerDisc; minorscan += minor)
            {
                int nextlength = 0;
                for (int majorscan = minorscan; board[majorscan] == cornerDisc; majorscan += major)
                {
                    if ((--length) <= 0)
                        break;
                    nextlength++;
                    unRevFlags[majorscan] = 0xf;
                }
                length = nextlength;
            }
        }

        private void ScanCorner(int corner, int scanDirection1,
                 int scanDirection2)
        {
            if (board[corner] == space) return;
            ScanFixedDiscs(corner, scanDirection1, scanDirection2);
            ScanFixedDiscs(corner, scanDirection2, scanDirection1);
        }

        public int Max(int up_limit, int low_limit, int depth, LeafCounter lc)
        {
            if (depth > 0)
            {
                bool passFlag = true;
                int max_score = low_limit;
                EvaluateBase wgc = NewCopy();

                for (int i = 0; i < wgc.SpaceCount; i++)
                {
                    if (lc.ForceBreak) return max_score;
                    int move = wgc.SpaceTable(i);
                    if (wgc.Reverse(move) != 0)
                    {
                        passFlag = false;
                        int eval = -wgc.Max(-max_score, -up_limit,
                                depth - 1, lc);
                        if (eval > max_score)
                        {
                            max_score = eval;
                            if (max_score >= up_limit)
                                return max_score;
                        }
                        wgc.CopyFrom(this);
                    }
                }
                if (passFlag)
                {
                    if (wgc.PassCount == 0)
                    {
                        wgc.Pass();
                        max_score = -wgc.Max( -max_score, -up_limit, depth - 1, lc);
                    }
                    else
                    {
                        lc.Count++;
                        if (wgc.Counts(turn) == 0)
                        {
                            return -64 << 24;
                        }
                        else if (wgc.Counts(Enemy) == 0)
                        {
                            return 64 << 24;
                        }
                        else
                        {
                            return (wgc.Counts(wgc.turn)
                                - wgc.Counts(wgc.Enemy)) << 24;
                        }
                    }
                }
                return max_score;
            }
            else
            {
                lc.Count++;
                return Evaluate();
            }
        }
    }
}
