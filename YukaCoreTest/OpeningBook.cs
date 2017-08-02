using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using YukaCore;

namespace YukaCoreTest
{
    class OpeningBook
    {
          	public static readonly string [] book = {
//			斜め取り定石			
			"f5f6e6f4e3","牛定石",
//			"f5f6e6f4e3d3f3c5c4e2","牛定石変化、図2",
//			"f5f6e6f4e3d6g4d3c3f2","牛定石変化、図3",

//			"f5f6e6f4e3c5c4e7b5e2","ヨット定石", // 牛定石からの変化
//			"f5f6e6f4e3c5c4e7b5e2f2d2f3d3c3c2","ヨット定石その後、図5",
//			"f5f6e6f4e3c5c4e7b5e2f2d2f3d3g5c3g4","ヨット定石変化、図6",
//			"f5f6e6f4e3c5c4e7b5e2f2d2f3g4g5d6g3","ヨット定石変化、図7",
//			"f5f6e6f4e3c5c4e7b5e2f3f2g5g4g6d6","白やや有利、ヨット定石変化、図8",
			
//			"f5f6e6f4e3c5c4e7g4","飛行機", // 牛定石からの変化
//			"f5f6e6f4e3c5c4e7g4g3g5f3g6d6e2","飛行機その後、図10",
//			"f5f6e6f4e3c5c4e7g4g3g5f3d7d3","飛行機変化、図11",
//			"f5f6e6f4e3c5c4e7g4g3g6h3d6f7c6","飛行機変化、図12",
//			"f5f6e6f4e3c5c4e7g4g3d7h4f7","黒優勢、飛行機変化、谷口流",
//			"f5f6e6f4e3c5c4e7g4g3d7g5b5d3c3e2","谷口流変化、図14",
			
//			"f5f6e6f4e3c5g5","こうもり", // 牛定石からの変化
//			"f5f6e6f4e3c5g5f3g4g6g3d6f7h4d3h5","こうもりその後、図17", 
//			"f5f6e6f4e3c5g5f3g6d7d6e7e8","黒有利だが難解、こうもり変化、図18",
//			"f5f6e6f4e3c5g5d6g6g4e7f7c6d3f3","黒やや有利、こうもり変化、図19",
			
			"f5f6e6f4c3","バッファロー定石",
//			"f5f6e6f4c3d6f3c4c5b4a5a3c6b3","バッファロー定石その後、図21",
//			"f5f6e6f4c3d6f3c5g4g3","バッファロー定石変化、図22",
//			"f5f6e6f4c3e7f3e3d3e2g5c5d2","バッファロー定石変化、図23",
//			"f5f6e6f4c3d7f3d6g5g4e3","バッファロー定石変化、図24",

			"f5f6e6f4g4","白有利、たぬき定石",

			"f5f6e6f4g5","飛び出し",
//			"f5f6e6f4g5e7f7h5d3c6","飛び出しその後、図30",
//			"f5f6e6f4g5e7f7h5e3d3","飛び出し変化、図31",
//			"f5f6e6f4g5e7f7h5g4h4e3d6","飛び出し変化、図32",
//			"f5f6e6f4g5e7f7h5e8c5g4h4","飛び出し変化、図33",
//			"f5f6e6f4g5e7f7c5f3g3h3h5g4","黒やや有利、飛び出し変化、図34",
//			"f5f6e6f4g5e7f7g6h5c5d8","飛び出し変化、図35",
//			"f5f6e6f4g5g6e7c5","飛び出し変化、図36",
//			"f5f6e6f4g5d6e7g4f3h5","黒やや有利、飛び出し変化、図37",

			"f5f6e6f4g6","へび定石",
			
//			"f5f6c4g5e6d7e7d6c5c6d8", "図52",
			
//			縦取り定石
			"f5d6c5f4e3","ウサギ定石",
//			"f5d6c5f4e3c6d3f6e6d7e7c7c4f3d8","ウサギ定石その後、54",
//			"f5d6c5f4e3c6d3f6e6d7g3c4g6g5","ウサギ定石その後、55",
//			"f5d6c5f4e3c6d3g5f6c4g6f3c3","ウサギ定石その後、56",
//			"f5d6c5f4e3c6d3g5f6f3g4h3","ウサギ定石その後、57",
//			"f5d6c5f4e3c6d3g5f6e7g6f3e6f7d7","ウサギ定石その後、58",

			"f5d6c5f4d3","馬定石",
			"f5d6c4d3c3","とら定石",
//			"f5d6c4d3e6f4e3f3c3","図75",
//			"f5d6c4g5c6c5e6","図76",
//			"f5d6c4g5c6d3e6d7f6c7","黒やや有利、図77",
			
//			並び取り定石
			"f5f4e3f6d3","黒有利、ねずみ定石",
			
//          bug check 1 for Exact method
			"f5f6e6f4e3","バグ1",
        };

        public static int ComVsCom(string opening, string comment,
            ComputerFaundation blackPlayer, ComputerFaundation whitePlayer)
            // return winner (bkack or white or zero)
	    {
            ComputerFaundation[] playerType = new ComputerFaundation[3];
            playerType[GameContext.black] = blackPlayer;
            playerType[GameContext.white] = whitePlayer;
            int result;

            GameContext gc = new GameContext();
            gc.Init();

            // build GC from opening move string

            for (int i = 0; i < opening.Length; i += 2)
            {
                int row = 0;
                int col = 0;
                char ch0 = opening[i];
                char ch1 = opening[i + 1];
                if ('a' <= ch0 && ch0 <= 'h')
                {
                    col = ch0 - 'a';
                }
                if ('1' <= ch1 && ch1 <= '8')
                {
                    row = ch1 - '1';
                }
                int move = GameContext.Move(row, col);
                int dummy = gc.Reverse(move);
                if (dummy == 0)
                {
                    throw new Exception("Opening Book Error");
                }
            }

            // braw board to stdout

            string [] board = gc.BoardPrint();
            for (int i=0; i<=8; i++)
		    {
    			Console.WriteLine(board[i]);
		    }
	   	
    		while (!gc.IsGameEnd())
	    	{
		        ComputerFaundation player = playerType[gc.Turn];
                player.StartThinking(gc);
                player.Think();
                int move = player.GetMove();
                Console.WriteLine(player.AnalyzedString);
			    if (move == 0)
			    {
				    gc.Pass();
			    }
			    else
			    {
				    int dummy = gc.Reverse(move);
				    if (dummy == 0) 
				    {
                        throw new Exception("Panic");
	    			}
				    board = gc.BoardPrint();
				    board[1] += blackPlayer.LongPlayerName;
				    board[5] += whitePlayer.LongPlayerName;
				    for (int i=0; i<=8; i++)
				    {
				        Console.WriteLine(board[i]);
				    }
			    }
		    }
		    int bcnt = gc.Counts(GameContext.black);
		    int wcnt = gc.Counts(GameContext.white);
		    string winner;
		    if (bcnt>wcnt)
		    {
			    winner = "黒:" + blackPlayer.LongPlayerName	+ " " + bcnt + " vs " + wcnt + " " + whitePlayer.LongPlayerName;
		        result = GameContext.black;
            }
    		else if (bcnt<wcnt)
		    {
			    winner = "白:" + whitePlayer.LongPlayerName	+ " " + wcnt + " vs " + bcnt + " " + blackPlayer.LongPlayerName;
		        result = GameContext.white;
		    }
		    else
		    {
			    winner = "draw.";
                result = 0;
		    }
		    Console.WriteLine("! " + winner);
            return result;
	    }
	
	    public static void ComVsCom(ComputerFaundation player1, ComputerFaundation player2)
	    {
            GameContext gc = new GameContext();

            int win = 0;
            int lose = 0;
            int draw = 0;

		    for (int i=0; i<book.Length; i+=2)
		    {
			    gc.Init();

                int play1 = ComVsCom(book[i], book[i+1], player1, player2);

			    int play2 = ComVsCom(book[i], book[i+1], player2, player1);

                if (play1 == black)
                {
                    win++;
                }
                else if (play1== white)
                {
                    lose--;
                }

                if (play2 == black)
                {
                    lose--;
                }
                else if (play2 == white)
                {
                    win++;
                }
            }
            Console.WriteLine("!results for " + player1.LongPlayerName);
            Console.WriteLine("!total win:" + win.ToString());
            Console.WriteLine("!total lose:" + lose.ToString());
            Console.WriteLine("!total draw:" + draw.ToString());
    	}		    

    	public static void Check() // 定石のチェック用メインルーチン
	    {
		    for (int i=0; i<book.Length; i+=2)
		    {
			    GameContext gc = new GameContext();
			    gc.Init();

			    for (int j=0; j<book[i].Length; j+=2)
			    {
				    int row = 0;
				    int col = 0;
				    char ch0 = book[i][j];
				    char ch1 = book[i][j+1];
				    if ('a' <= ch0 && ch0 <= 'h')
				    {
					    col = ch0 - 'a' + 1;
				    }
				    if ('1' <= ch1 && ch1 <='8')
				    {
					    row = ch1 - '0';
				    }
				    int move = GameContext.Move(row,col);
				    int dummy = gc.Reverse(move);
				    if (dummy == 0) 
				    {
					    Console.WriteLine("Opening Book Error at:" + book[i].Substring(0,j+2) + " in "+ book[i+1]);
					    break;
				    }
			    }

			    Console.WriteLine(book[i+1]);
			    String [] board = gc.BoardPrint();
			    for (int k=0; k<=8; k++)
			    {
				    Console.WriteLine(board[k]);
			    }
			    Console.WriteLine();
		    }
	    }

    }
}
