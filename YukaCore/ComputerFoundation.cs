using System;
using System.Collections.Generic;
using System.Text;


namespace YukaCore
{
    public class ComputerFoundation : LeafCounter
    {
        const string panic = "Application Panic";

        protected const int max_score_value = 64 << 24;

        protected EvaluateBase topGC;

        protected string commonMessage;

        protected string analyzedString;

        public string AnalyzedString { get { return analyzedString; } }

        protected string chatString;
        public string ChatString { get { return chatString; } }

        protected bool randomFlag;
        protected int exactDepth;
        protected int depth;
 
        protected SortedList<int, object> best_moves;
        protected int max_score;
        public int ticks;
        protected Random rand;

        public ComputerFoundation(EvaluateBase evaltype, bool randomFlag, int depth, int exactDepth)
        {
            this.best_moves = new SortedList<int, object>();
            this.rand = new Random();
            this.topGC = evaltype;
            this.randomFlag = randomFlag;
            this.depth = depth;
            this.exactDepth = exactDepth;
        }

        public string Name
        { get { return topGC.Name(); } }

        public string LongPlayerName
        {
            get
            {
                    return topGC.Name()  +"(" + topGC.Comments() + ")" + (randomFlag ? "/r" : "/n") + "/" + depth + "/" + exactDepth;
            }
        }


        private static string StrNum(int i)
        {
            if (i >= 0)
                return "+" + i.ToString();
            else
                return i.ToString();
        }

        protected static string StrScore(int score)
        {
            double d = score;
            d = d / (1 << 24);
            int c1 = (int)(Math.Round(d));
            d = (d - c1) * 256;
            if (d == 0) return StrNum(c1);
            int c2 = (int)(Math.Round(d));
            d = (d - c2) * 256;
            if (d == 0) return StrNum(c1) + StrNum(c2);
            int c3 = (int)(Math.Round(d));
            d = (d - c3) * 256;
            if (d == 0) return StrNum(c1) + StrNum(c2) + StrNum(c3);
            int c4 = (int)(Math.Round(d));
            return StrNum(c1) + StrNum(c2) + StrNum(c3) + StrNum(c4);
        }

        public void StartThinking(GameContext gc)
        {
            topGC.CopyFromGC(gc);
            Count = 0;
            ForceBreak = false;
            ticks = 0;
            max_score = -0x7fffffff;
            best_moves.Clear();
        }

        protected void ComputerMoveNormal()
        {
            this.max_score = -0x7fffffff;
            best_moves.Clear();

            EvaluateBase wgc = topGC.NewCopy();
            for (int i = 0; i < wgc.SpaceCount; i++)
            {
                int move = wgc.SpaceTable(i);
                if (wgc.Reverse(move) > 0)
                {
                    int score = -wgc.Max(-max_score + 1, -max_score_value,
                                    depth, this);
                    if (score > this.max_score)
                    {
                        best_moves.Clear();
                        best_moves.Add(move, move);
                        this.max_score = score;
                    }
                    else if (score == this.max_score)
                    {
                        best_moves.Add(move, move);
                    }
                    wgc.CopyFrom(topGC);
                }
            }
        }

        protected void ComputerMovePerfect()
        {
            max_score = -0x7fffffff;
            best_moves.Clear();
            GameContext wgc = new GameContext();
            wgc.CopyFromGC(topGC);
            for (int i = 0; i < wgc.Counts(GameContext.space); i++)
            {
                if (ForceBreak) return;
                int move = wgc.SpaceTable(i);
                if (wgc.FastReverse(move) > 0)
                {
                    int score = -wgc.Exact(-max_score + 1, -max_score_value, this);
                    if (score > max_score)
                    {
                        best_moves.Clear();
                        best_moves.Add(move, move);
                        max_score = score;
                    }
                    else if (this.randomFlag && (score == max_score))
                    {
                        best_moves.Add(move, move);
                    }
                    wgc.CopyFromGC(topGC);
                }
            }
        }

        public void Think()
        {
            if (topGC.SpaceCount > exactDepth)
            {
                ComputerMoveNormal();
            }
            else
            {
                ComputerMovePerfect();
            }
        }


        public int GetMove()
        {

            if (best_moves.Count == 0)
            {
                commonMessage = topGC.Name() + "：どこにも打てない（x_x)";
                analyzedString = commonMessage;
                chatString = commonMessage;

                return 0;
            }

            int ix = randomFlag ? rand.Next(best_moves.Count) : 0;
            int selected_move = best_moves.Keys[ix];

            commonMessage = topGC.Name() + "は " + GameContext.StrMove(selected_move)
                + " に打ちました。";
            analyzedString = commonMessage;
            chatString = commonMessage;

            analyzedString += ": score: " + StrScore(max_score)
                    + "; Ticks: " + ticks + "; leaves: " + Count;

            if (topGC.SpaceCount > exactDepth)
            {
                if (max_score > 0)
                {
                    if (max_score < (1 << 23))
                        chatString = commonMessage + ":ルンルン♪";
                    else if (max_score < (4 << 24))
                        chatString = commonMessage + ":ちょっと有利かな？";
                    else
                        chatString = commonMessage + ":勝てそうだ♪";
                }
                else
                {
                    if (max_score > (-1 << 23))
                        chatString = commonMessage + ":むー(@_@)";
                    else if (max_score > (-4 << 24))
                        chatString = commonMessage + ":ちょっと難しいか？";
                    else
                        chatString = commonMessage + ":きびしいなあ(T_T)";
                }

            }
            else
            {
                if (max_score > (1 << 23))
                {
                    chatString = topGC.Name() + ":私の" + (max_score >> 24) + "石勝ちですね(^-^)";
                }
                else if (max_score < (-1 << 23))
                {
                    chatString = topGC.Name() + ":やばい、負けそう(T_T)";
                }
                else
                {
                    chatString = topGC.Name() + ":引き分け？";
                }
            }
            return selected_move;
        }

        public void ShowAssist(bool[] assistMoves)
        {
            // asert(best_moves.Count !=0)

            for (int i = 0; i < GameContext.boardSize; i++)
            {
                assistMoves[i] = false;
            }

            string message = topGC.Name() + "の次の一手は、[＃]です。";

            if (randomFlag)
            {
                for (int i = 0; i < best_moves.Count; i++)
                {
                    assistMoves[best_moves.Keys[i]] = true;
                }
            }
            else
            {
                assistMoves[best_moves.Keys[0]] = true;
            }

            analyzedString = message + " score: " + StrScore(max_score) + "; leaves: " + Count;
            chatString = message;

            if (topGC.SpaceCount > exactDepth)
            {
                if (max_score > (1 << 23))
                {
                    chatString += "\r\nちょっと有利かな？";
                }
                else if (max_score < (-1 << 23))
                {
                    chatString += "\r\nちょっと不利かな？";
                }
            }
            else
            {
                if (max_score > (1 << 23))
                {
                    chatString += "\r\nうまく打てば" + (max_score >> 24) + "石勝ちです(^o^)";
                }
                else if (max_score < (-1 << 23))
                {
                    chatString += "\r\n完璧に打っても負けるかも(T_T)";
                }
            }
        }
    }
}
