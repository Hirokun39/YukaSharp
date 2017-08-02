using System;
using System.Collections.Generic;
using System.Text;

namespace YukaCore
{
    public abstract class EvaluateFatLib : EvaluateBase
    {
        protected abstract int EvaluateBody();
        protected abstract int EvaluateOpenCorner(int corner, int major, int minor);

        private byte [] fixedDiscs;

        protected int openCornerScore;   // read only
        protected int playersFixedCount; // read only
        protected int enemysFixedCount;  // read only

        protected EvaluateFatLib() : base()
	    {
        	fixedDiscs = new byte[3]; // [0] is dummy;
	    }


    	private int EvaluateCorner(int corner, int scanDirection1,
	    	     int scanDirection2)
	    {
            if (board[corner] == space)
            {
                return EvaluateOpenCorner(corner, scanDirection1,
                                            scanDirection2)
                     + EvaluateOpenCorner(corner, scanDirection2,
                                            scanDirection1);
            }
            else
            {
                return 0;
            }
	    }

	    private int EvaluateAllCorner()
	    {
		    return EvaluateCorner(upperLeftCorner, right, down)
                 + EvaluateCorner(upperRightCorner, left, down)
                 + EvaluateCorner(bottomLeftCorner, right, up)
                 + EvaluateCorner(bottomRightCorner, left, up);
        }
	
        public override int Evaluate()
        {
    	    if (counts[turn] == 0)
    	    {
    		    return -64 << 24;
    	    }
    	    else if (counts[Enemy] ==0)
    	    {
    		    return 64 << 24;
    	    }
        	else
    	    {
                openCornerScore = EvaluateAllCorner();
                
    		    fixedDiscs[black] = 0;
    		    fixedDiscs[white] = 0;
		
    		    for (int i=1; i<9; i++)
    		    {
    			    for (int j=1; j<9; j++)
    			    {
    				    if (IsFixed(i*down+j*right))
    				    {
    					    fixedDiscs[board[i*down+j*right]]++;
    				    }
    			    }
    		    }
                playersFixedCount = fixedDiscs[turn];
                enemysFixedCount  = fixedDiscs[Enemy];

                return EvaluateBody();
		    }
        }


    	protected int EvaluateOpenCornerLv0(int corner,
            int scanDirectionMajor, int scanDirectionMinor)
	    // もっとも簡単な開き隅の評価ルーチン
        {

		    int scanPlace = corner + scanDirectionMajor;

    		byte xDisc = board[scanPlace+scanDirectionMinor];
	    	if (xDisc != space)
		    {
			    int xScore = -1 << 23;
			    return (xDisc == turn) ? xScore : -xScore;
		    }

		    byte cDisc = board[scanPlace];
		    int count=0;
		    if (cDisc != space)
		    {
			    count = 1;
			    scanPlace += scanDirectionMajor;
			    while (board[scanPlace] == cDisc)
			    {
				    count++;
				    scanPlace += scanDirectionMajor;
			    }
			    if (board[scanPlace] == outside) return 0;
		    }

		    if (count==0) return 0;

		    int cScore;
		    if (count==6)
		    {
			    cScore = 1;
		    }
		    else
		    {
			    cScore = -2;
		    }

		    return ((cDisc == turn) ? cScore : -cScore) << 23;
        }

        protected int EvaluateOpenCornerLv1(int corner,
            int scanDirectionMajor, int scanDirectionMinor)
        {
    	    int scanPlace = corner + scanDirectionMajor;
    	    byte cDisc = board[scanPlace];	
    	    byte xDisc = board[scanPlace+scanDirectionMinor];
    	    int count=0;
    	    if (cDisc != space)
    	    {
    		    count = 1;
    		    scanPlace += scanDirectionMajor;
    		    while (board[scanPlace] == cDisc)
    		    {
    			    count++;
    			    scanPlace += scanDirectionMajor;
    		    }
    		    if (board[scanPlace] == outside) count = 0;
    	    }

    	    if (board[scanPlace] == EnemyOf(cDisc)
                && turn == EnemyOf(cDisc)) 
    	    {
        	    return (cDisc == turn) ? (-count << 24)
                                             : ( count << 24);
    	    }
       	
        	if (xDisc != space)
    	    {
    		    if (xDisc != cDisc)
    		    {
    			    // 山で両側にX打ちされている時のチェック
    			    if (count == 6 && board[scanPlace - scanDirectionMajor + scanDirectionMinor] == xDisc)
    				count = 3;
    		
    			    int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
       			    return (xDisc == turn) ? xScore : -xScore;
    		    }
        		else
        		{
    	    		int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
       		    	return (xDisc == turn) ? xScore : -xScore;
    		    }
        	}

    	    if (count==0) return 0;

    	    int cScore;
    	    if (count==6)
    	    {
    		    cScore = 1 << 24;
    		
    	    }
    	    else
    	    {
    		    cScore = -1 << 24;
    	    }
    	    return (cDisc == turn) ? cScore : -cScore;
        }

        protected int EvaluateOpenCornerLv2(int corner,
            int scanDirectionMajor, int scanDirectionMinor)
        {
  	        int scanPlace = corner + scanDirectionMajor;
  	        byte cDisc = board[scanPlace];	
  	        byte xDisc = board[scanPlace+scanDirectionMinor];
  	        int count=0;
  	        if (cDisc != space)
  	        {
  		        count = 1;
  		        scanPlace += scanDirectionMajor;
  		        while (board[scanPlace] == cDisc)
  		        {
  			        count++;
  			        scanPlace += scanDirectionMajor;
  		        }
      		    if (board[scanPlace] == outside) count = 0;
  	        }

      	    if (board[scanPlace] == EnemyOf(cDisc) && turn == EnemyOf(cDisc)) 
  	        {
      	        return (cDisc == turn) ? (-count << 24) : (count << 24);
  	        }
  	
  	        if (xDisc != space)
  	        {
  		        if (xDisc != cDisc)
  		        {
  			        // 山で両側にX打ちされている時のチェック
  			        if (count == 6 && board[scanPlace - scanDirectionMajor + scanDirectionMinor] == xDisc)
  				    count = 3;
  		
  			        int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
     		    	return (xDisc == turn) ? xScore : -xScore;
  		        }
  		        else
  		        {
  			        int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
     			    return (xDisc == turn) ? xScore : -xScore;
  		        }
  	        }

  	        if (count==0) return 0;

  	        int cScore;
  	        if (count==6)
  	        {
  		        cScore = 1 << 24;
      	        return (cDisc == turn) ? cScore : -cScore;
  	        }

  	        int spaceCount = 0;
  	        while (board[scanPlace]==space)
  	        {
  		        spaceCount++;
  		        scanPlace += scanDirectionMajor;
  	        }
  	
          	if (board[scanPlace]==outside)
  	        {
  		        switch (count)
		        {
	    	    case 1:
		        case 2:	
			        cScore = -1 << 24;
  			        break;
		        case 3:
		        case 4:	
			        cScore = -1 << 16;
			        break;
		        case 5:
			        byte keyDisc = board[corner+scanDirectionMajor*5+scanDirectionMinor];
			        if (keyDisc == space)
			        {
				        cScore = -1 << 16;
			        }
			        else if (keyDisc == cDisc)
			        {
				        cScore = -4 << 16;
			        }
			        else
			        {
				        // this is case of enemyOF(cDisc):
				        cScore = 2 << 16;
			        }
			        break;
		        default:
                    // Application Panic if flow reaches here
                    cScore = 0;
                    break;
		        }
  	        }
  	        else
  	        {
  		        cScore = -1 << 24;
  	        }
  	        return (cDisc == turn) ? cScore : -cScore;
  	    }

        protected int EvaluateOpenCornerLv3(int corner,
            int scanDirectionMajor, int scanDirectionMinor)
        {
    	    int scanPlace = corner + scanDirectionMajor;
    	    byte cDisc = board[scanPlace];	
    	    byte xDisc = board[scanPlace+scanDirectionMinor];
    	    int count=0;
    	    if (cDisc != space)
    	    {
    		    count = 1;
    		    scanPlace += scanDirectionMajor;
    		    while (board[scanPlace] == cDisc)
    		    {
    			    count++;
    			    scanPlace += scanDirectionMajor;
    		    }
    		    if (board[scanPlace] == outside) count = 0;
    	    }

    	    if (board[scanPlace] == EnemyOf(cDisc) && turn == EnemyOf(cDisc)) 
    	    {
    		    return (cDisc == turn) ? (-count << 24) : (count << 24);
    	    }
	
    	    if (xDisc != space)
    	    {
    		    if (xDisc != cDisc)
    		    {
    			    // 山で両側にX打ちされている時のチェック
    			    if (count == 6 && board[scanPlace - scanDirectionMajor + scanDirectionMinor] == xDisc)
    				    count = 3;
		
    			    int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
    			    return (xDisc == turn) ? xScore : -xScore;
    		    }
    		    else
    		    {
    			    int xScore = (-count << 24) + (-1 << 23) + (1 << 15);
    			    return (xDisc == turn) ? xScore : -xScore;
    		    }
        	}

    	    if (count==0) return 0;

    	    int cScore;
    	    if (count==6)
    	    {
    		    cScore = 1 << 24;
    		    return (cDisc == turn) ? cScore : -cScore;
    	    }

    	    int spaceCount = 0;
    	    while (board[scanPlace]==space)
    	    {
    		    spaceCount++;
    		    scanPlace += scanDirectionMajor;
    	    }
	
    	    if (board[scanPlace]==outside)
    	    {
                switch (count)
                {
                    case 1:
                    case 2:
                        cScore = -1 << 24;
                        break;
                    case 3:
                    case 4:
                        cScore = 0;
                        break;
                    case 5:
                        byte keyDisc = board[corner + scanDirectionMajor * 5 + scanDirectionMinor];
                        byte a_xDisc = board[corner + scanDirectionMajor * 6 + scanDirectionMinor];
                        if (a_xDisc != space)
                        {
                            if ((a_xDisc != cDisc)
                                && Feasible(corner + scanDirectionMajor * 6, cDisc, scanDirectionMinor - scanDirectionMajor)
                                && !Feasible(corner + scanDirectionMajor * 6, cDisc, scanDirectionMinor))
                            {
                                cScore = 1 << 24;
                            }
                            else
                            {
                                cScore = -1 << 24;
                            }
                        }
                        else if (keyDisc == space)
                        {
                            cScore = 1 << 16;
                        }
                        else if (keyDisc == cDisc)
                        {
                            cScore = (-1 << 24) + (2 << 16);
                        }
                        else
                        {
                            // this is case of enemyOF(cDisc):
                            if (Feasible(corner + scanDirectionMajor * 6, cDisc, scanDirectionMinor - scanDirectionMajor))
                                cScore = 1 << 24;
                            else
                                cScore = 1 << 16;
                        }
                        break;
                    default:
                        //　"Panic at EvaluateFreeCornerLv3");
                        cScore = 0;
                        break;
                }
    	    }
    	    else if (board[scanPlace] == cDisc)
    	    {
                switch (spaceCount)
                {
                    case 1:
                    case 3:
                    case 5:
                        cScore = -count << 24;
                        break;
                    case 2:
                        cScore = 1 << 16;
                        break;
                    default:
                        cScore = -1 << 24;
                        break;
                }
    	    }
    	    else
    	    {
                switch (spaceCount)
                {
                    case 0:
                        cScore = 0;
                        break;
                    case 1:
                        cScore = -1 << 24;
                        break;
                    default:
                        cScore = (-count - 1) << 24;
                        break;
                }
    	    }
    	    return (cDisc == turn) ? cScore : -cScore;
        }
    }
}
