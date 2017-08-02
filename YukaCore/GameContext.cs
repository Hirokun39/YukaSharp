using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    public class GameContext
    {
        protected const int up = -1;
        protected const int down = 1;
        protected const int left = -9;
        protected const int right = 9;

        protected const int upperLeftCorner = down + right;
        protected const int upperRightCorner = down + right * 8;
        protected const int bottomLeftCorner = down * 8 + right;
        protected const int bottomRightCorner = down * 8 + right * 8;
        public const int boardSize = down * 9 + right * 9 + 1;

        public const byte space = 0;
        public const byte black = 1;
        public const byte white = 2;
        public const byte outside = 3;

        public const byte spaceTableSize = 60;

        protected readonly int[] DirectionTable =
        {
            up,
            up+left,
            left,
            left+down,
            down,
            down+right,
            right,
            right+up
        };

        private readonly string [] StrDisc = {"□", "●", "○", "×"};

        protected byte [] board;
        protected byte [] spaceTable;
        protected int[] counts;
        protected byte turn;
        protected bool passFlag;
        protected int passCount;
        protected int blackScore;
  
        public byte Turn            { get { return turn; } }
        public int  PassCount       { get { return passCount; } }
        public int  SpaceCount      { get { return counts[space]; } }
        public bool IsGameEndByPass { get { return passCount > 1; } }
        public byte Enemy           { get { return (byte)(turn ^ 3); } }
        public int BlackScore       { get { return blackScore; } }

        public byte Board(int move) { return board[move]; }
        public int Counts(int disc) { return counts[disc]; }
        public byte SpaceTable(int i) { return spaceTable[i]; }

        protected static byte EnemyOf(byte player)
        {
            return (byte)(player ^ 3);
        }

        public GameContext()
        {
	        board = new byte[boardSize];
	        spaceTable = new byte[spaceTableSize];
	        counts = new int[3];
        }
    
        public void CopyFromGC(GameContext src)
        {
            src.board.CopyTo(this.board, 0);
            src.spaceTable.CopyTo(this.spaceTable, 0);
            src.counts.CopyTo(this.counts, 0);
	        this.turn = src.turn;
            this.blackScore = src.blackScore;
	        this.passCount = src.passCount;
            this.passFlag = true; // this will be leared when any moves found in FastReverse
        }

        private void SpaceDef(int i, int j)
        {
            spaceTable[counts[space]++] = (byte)(9 * (9 - i) + (9 - j));
            spaceTable[counts[space]++] = (byte)(9 * (9 - i) + (j));
            spaceTable[counts[space]++] = (byte)(9*(  i)+(9-j));
            spaceTable[counts[space]++] = (byte)(9 * (i) + (j));
            if (i != j)
            {
                spaceTable[counts[space]++] = (byte)(9 * (9 - j) + (9 - i));
                spaceTable[counts[space]++] = (byte)(9 * (9 - j) + (i));
                spaceTable[counts[space]++] = (byte)(9 * (j) + (9 - i));
                spaceTable[counts[space]++] = (byte)(9 * (j) + (i));
            }
        }

        public virtual void Init()
        {
    	    for (int i=0; i<boardSize; i++)
    	    {	
    		    board[i] = outside;
    	    }
      
            for (int i=1; i<9; i++)
                for (int j=1; j<9; j++)
                {
        	        board[i*down+j*right] = space;
                }
        	
      	    board[4*right+4*down] = white;
      	    board[4*right+5*down] = black;
      	    board[5*right+4*down] = black;
      	    board[5*right+5*down] = white;
      	    turn = black;
      	    passCount = 0;
            passFlag = true; // will be cleared in FastReverse
      	    counts[space]=0;
      	    counts[black]=2;
      	    counts[white]=2;
      	    SpaceDef(1,1);
      	    SpaceDef(1,3);
      	    SpaceDef(1,4);
      	    SpaceDef(3,3);
      	    SpaceDef(3,4);
      	    SpaceDef(2,3);
      	    SpaceDef(2,4);
      	    SpaceDef(1,2);
      	    SpaceDef(2,2);      
        }

        public bool Feasible(int move, byte player, int scanDirection)
        {
    	    int j = move + scanDirection;
    	    byte enemy = EnemyOf(player);
    	    if (board[j] != enemy) return false;
	
    	    do
    	    {
    		    j += scanDirection;
    	    } while (board[j] == enemy);
	
    	    if (board[j] == player)
    	    {
    		    return true;
    	    }
    	    return false;
        }

        public bool Feasible(int move, byte player)
        {
    	    if ( Board(move) != space )
    		    return false;
    
    	    for (int i = 0; i<8; i++)
    	    {
    		    int scanDirection = DirectionTable[i];
    		    if (Feasible(move, player, scanDirection))
    	    		return true;
    	    }
    	    return false;
        }

        public bool Feasible(int move)
        {
    	    return Feasible(move, turn);
        }

        public int FastReverse(int move)
        {
            if (Board(move) != space) return 0;

            int count = 0;
            byte player = turn;
            byte enemy = Enemy;
            for (int i = 0; i < 8; i++)
            {
                int searchDirection = DirectionTable[i];
                int j = move + searchDirection;
                int tempcount = 0;
                while (Board(j) == enemy)
                {
                    j += searchDirection;
                    tempcount++;
                }
                if (tempcount != 0 && board[j] == player)
                {
                    count += tempcount;
                    j = move + searchDirection;
                    while (tempcount != 0)
                    {
                        board[j] = player;
                        j += searchDirection;
                        tempcount--;
                    }
                }
            }

            if (count == 0) return 0;

            counts[space]--;
            int spaceCounts = counts[space];
            counts[turn] += count + 1;
            counts[enemy] -= count;

            board[move] = turn;
            turn = enemy;
            passFlag = false;

            {
                int i = 0;
                while (spaceTable[i] != move) i++;
                while (i < spaceCounts)
                {
                    spaceTable[i] = spaceTable[i + 1];
                    i++;
                }
            }

            spaceTable[spaceCounts] = (byte)move;
            passCount = 0;
            return count * 2 + 1;
        }
   	
        public virtual int Reverse(int move)
        {
  	        return FastReverse(move);
        }
  
        public bool MustBePassed()
        {
  	        for (int i=0; i<SpaceCount; i++)
  	        {
  		        if (Feasible(spaceTable[i]))
  			        return false;
  	        }
      	    return true;
        }
  
        public void Pass()
        {
            passCount ++;
            turn = EnemyOf(turn);
        }
  
        public static int Row(int move)
        {
            return (move - upperLeftCorner) % right;
        }

        public static int Col(int move)
        {
            return (move - upperLeftCorner) / right;
        }

        public static string StrMove(int row, int col)
        {
            return "" + (char)('a' + col) + (char)('1' + row);
        }

        public static string StrMove(int move)
        {
            return StrMove(Row(move), Col(move));
        }

        public static byte Move(int row, int col)
        {
            return (byte)(row * down + col * right + upperLeftCorner);
        }

        public byte Board(int row, int col)
        {
            return Board(Move(row, col));
        }

        protected void SetBoard(int row, int col, byte disc)
        {
            board[Move(row, col)] = disc;
        }
  
        private string ArrowIfturn(int b_or_w)
        {
            return (turn == b_or_w) ? "⇒" : "　";
        }

        public string[] BoardPrint()
        {
            string[] result = new string[9];
            result[0] = "  ａｂｃｄｅｆｇｈ";
            for (int i = 0; i < 8; i++)
            {
                String line = (i+1).ToString() + " ";
                for (int j = 0; j < 8; j++)
                {
                    line += StrDisc[Board(i, j)];
                }
                result[i+1] = line;
            }
            result[1] += " " + ArrowIfturn(black) + StrDisc[black]
                        + ":" + counts[black];
            result[5] += " " + ArrowIfturn(white) + StrDisc[white]
                        + ":" + counts[white];

            return result;
        }

        public int GetHistory(int spaces)
        {
            return spaceTable[spaces - 1];
        }

        public bool IsGameEnd()
        {
            for (int i = 0; i < SpaceCount; i++)
            {
                int move = spaceTable[i];
                if (Feasible(move))
                {
                    return false;
                }
                else if (Feasible(move, EnemyOf(turn)))
                {
                    return false;
                }
            }

            return true;
        }

        public int Exact(int up_limit, int low_limit, LeafCounter lc)
        {
            byte player = turn;
            byte enemy  = Enemy;

            if (SpaceCount == 0)
            {
                lc.Count++;
                return (Counts(player) - Counts(enemy)) << 24;
            }
            else if (Counts(player) == 0)
            {
                lc.Count++;
                return -64 << 24;
            }
            else if (IsGameEndByPass)
            {
                lc.Count++;
                return (Counts(player) - Counts(enemy)) << 24;
            }

            GameContext wgc = new GameContext();
            wgc.CopyFromGC(this);

            int max_score = low_limit;
            bool passFlag = true;

            for (int i = 0; i < wgc.SpaceCount; i++)
            {
                int move = wgc.SpaceTable(i);
                if (wgc.FastReverse(move) != 0)
                {
                    passFlag = false;
                    int score = -wgc.Exact(-max_score, -up_limit, lc);
                    if (score > max_score)
                    {
                        max_score = score;
                        if (max_score >= up_limit)
                            return max_score;
                    }
                    wgc.CopyFromGC(this);
                }
            }
            if (passFlag)
            {
                wgc.Pass();
                max_score = -wgc.Exact(-low_limit, -up_limit, lc);
            }
            return max_score;
        }

        public GameContext BestGC4Black(int max, int min, LeafCounter lc)
        {
            if ((SpaceCount == 0) || IsGameEndByPass)
            {
                blackScore = (counts[black] - counts[white]) << 24;
                lc.Count++;
                return this;
            }

            // turn must be black

            GameContext bestGC = new GameContext();
            GameContext endGC = null;
            GameContext wgc = new GameContext();

            wgc.CopyFromGC(this);
            int best_score = min;

            for (int i = 0; i < wgc.SpaceCount; i++)
            {
                int move = wgc.SpaceTable(i);
                if (wgc.FastReverse(move) != 0)
                {
                    endGC = wgc.BestGC4Black(max, best_score, lc);

                    if (endGC.BlackScore > best_score)
                    {
                        bestGC.CopyFromGC(endGC);
                        best_score = endGC.BlackScore;
                    　  if (best_score >= max)　return bestGC;
                    }
                }
                wgc.CopyFromGC(this);
            }

            if (endGC == null)
            {
                wgc.Pass();
                return wgc.BestGC4Black(max, min, lc);
            }
            return bestGC;
        }

        public GameContext BestGC4White(int max, int min, LeafCounter lc)
        {
            byte player = turn;
            byte enemy = Enemy;
            GameContext bestGC = null;

            if ((SpaceCount == 0) || IsGameEndByPass)
            {
                blackScore = (counts[black] - counts[white]) << 24;
                lc.Count++;
                return this;
            }
            else if (counts[black] == 0)
            {
                blackScore = -64 << 24;
                lc.Count++;
                return this;
            }

            GameContext wgc = new GameContext();
            wgc.CopyFromGC(this);
            int best_score = min;

            bool passFlag = true;

            for (int i = 0; i < wgc.SpaceCount; i++)
            {
                int move = wgc.SpaceTable(i);
                if (wgc.FastReverse(move) != 0)
                {
                    passFlag = false;
                    GameContext tmpGC = wgc.BestGC4Black(max, min, lc);

                    if (player == black)
                    {
                        if (tmpGC.BlackScore > best_score)
                        {
                            bestGC = tmpGC;
                            best_score = tmpGC.BlackScore;
                            if (best_score >= max)
                                return bestGC;
                        }
                    }
                }
                wgc.CopyFromGC(this);
            }

            if (passFlag)
            {
                wgc.Pass();
                return wgc.BestGC4Black(max, min, lc);
            }
            throw new Exception("Panic!");
        }
    }
}