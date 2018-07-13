using System;
using System.Collections;

//Yehya Mubaideen   يحيى مبيضين         20102171058
namespace MyGame
{
    /// <summary>
    /// Comments below may be inconsistant because I'm in a hurry (5 hours till submission time).
    /// To gain speedups the code below was first {all static with no parameter passing} then I had to convert to OOP (to search upcoming positions).
    /// 
    /// Still needs debugging
    /// </summary>
    public class Cpe480
    {
        /// <summary>
        /// The following class can be used in two ways:
        /// Either as an agent for the game from the start in which it will figure if it's white or black given (start = true;)
        /// Or as an agent for the game in an arbitrary turn for which you must change ->(start, im, remainingTime).
        /// 
        /// P.S. The game will be a variant of chess. The details of the game are as follows:
        ///      The starting position is as shown in the below diagram. One side wins the game win it finishes off all the pieces of the opponent.
        ///      Pawns reaching the last rank are promoted to queens only.
        ///      
        /// The 2D array position will include the state of the game at the time. It will be of size 8x8.
        /// Each cell represents the corresponding square in the game with a value of 
        ///     0 representing an empty square, 1 for a white Pawn, 2 for a white queen, 3 for a black Pawn, 4 for black queen.
        ///     
        /// So simply the starting position will be :
        /// 
        ///             0 0 0 0 0 0 0 0
        ///             3 3 3 3 3 3 3 3
        ///             0 0 0 0 0 0 0 0
        ///             0 0 0 0 0 0 0 0
        ///             0 0 0 0 0 0 0 0
        ///             0 0 0 0 0 0 0 0
        ///             1 1 1 1 1 1 1 1
        ///             0 0 0 0 0 0 0 0
        /// </summary>
        /// 

        static bool start = true;//To determine if it's the start of a game or not.
        static bool im = true;//True means I'm white, false means I'm black.
        static double remainingTime = 290;//Initial remaining time 5 minutes = 300 seconds, but just to be on the safe side 290.
        static DateTime timerStart;//start timer, aids in setting the remaining time.
        static DateTime timerEnd;//end timer, aids in setting the remaining time.
        static DateTime endEstimation;//A time to end estimating the best move.
        static double initialAlpha = double.MinValue;//alpha negative infinity.
        static double initialBeta = double.MaxValue;//beta positive infinity.
        static string move;//Holds the actual move to be made.
        static Stack fringe;

        //The following start with lowercase character p to represent that they are for a "play" which is an arbitrary turn of playing the game (whether real or imaginary).
        bool pWhite;//To determine if playing white or black.
        int pStartLine;//To determine which row is the starting line.
        int pOrientation;//To guide the movement of pawns.
        int pPromotionLine;//To determine at which row a pawn promotes to queen.
        int pPawn;//To identify a pawn.
        int pQueen;//To identify a queen.
        int pOpponentPawn;//To identify an opponent's pawn.
        int pOpponentQueen;//To identify an opponent's queen.
        bool pDefensive;//True means defensive-strategy(no unprotected pieces), false means offensive-strategy.
        int pFrontline;//To determine which row holds the closest pawn(s) to the opposite side.
        int[] pBestMove;//Used by CurrentBestMove() to hold {current position row, current position column, next position row, next position column} of the best calculated move(so far).
        double pBestMoveEvaluation;//Used by CurrentBestMove() to hold the evaluation of the best calculated move(so far).
        char pFirst;//Used by BestMoveString() to hold the first character.
        int pSecond;//Used by BestMoveString() to hold the second character.
        char pThird;//Used by BestMoveString() to hold the third character.
        int pFourth;//Used by BestMoveString() to hold the fourth character.
        string pBestMoveString;//Used by BestMoveString() to hold the final value of the best move in the required string notation.
        int[][] pEvaluatePosition;//Used by EvaluateMove(), holds a position(state) to be evaluated.
        double pEvaluation;//Used by EvaluateMove() for the summation of all 64 Tile evaluations.
        double pTileEvaluation;//Used by EvaluateMove() for a Tile's evaluation.
        bool[][] pQueenSight;//Used by QueenSight() to hold the sight of a specific queen.
        bool pDefended;//Used by PieceDefended() to aid in knowing a piece is defended or not.
        bool[][] pCBMQueenSight;//Used by CurrentBestMove() to hold the sight of a specific queen.
        bool[][] pEMQueenSight;//Used by EvaluateMove() to hold the sight of a specific queen.
        bool[][] pPDQueenSight;//Used by PieceDefended() to hold the sight of a specific queen.

        int[] pEstimatedMove;//
        double pEstimatedMoveEvaluation;//
        int pDepthLimit;//Depth Limited Search algorithm.
        Stack pFringe;//Depth Limited Search algorithm.



        /// <summary>
        /// Predicts the best move by looking at future Positions {using RecursiveDLS + ALPHA-BETA-SEARCH algorithm}.
        /// </summary>
        /// <param name="State"></param>
        void EstimateBestMove(int[][] State/*position*/)
        {
            int limit = 2;
            double error = double.NaN;
            while (endEstimation > DateTime.Now)
            {
                CurrentBestMove(State);
                pDepthLimit = limit;
                error = RecursiveAlphaBetaDLS(this, State, pDepthLimit, initialAlpha, initialBeta);
                if (error == double.NaN)
                    return;
                //limit += 2;
            }
            if (pEstimatedMoveEvaluation != 0 && error != double.NaN)
            {
                pBestMove[0] = pEstimatedMove[0];
                pBestMove[1] = pEstimatedMove[1];
                pBestMove[2] = pEstimatedMove[2];
                pBestMove[3] = pEstimatedMove[3];
            }
        }
        /// <summary>
        /// Recursive Alpha Beta Depth Limited Search (if only i had more time {submission due time is in 13 minutes})
        /// </summary>
        /// <param name="Problem"></param>
        /// <param name="State"></param>
        /// <param name="Limit"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        double RecursiveAlphaBetaDLS(Cpe480 Problem, int[][] State/*position*/, int Limit/*current recursion limit*/, double alpha, double beta)
        {
            if (endEstimation > DateTime.Now)
                return double.NaN;
            if (pDepthLimit - Limit == (pDepthLimit - 1))
            {
                Cpe480 node = new Cpe480(pWhite);
                node.MakeMove(State);
                fringe.Push(node);
                return RecursiveAlphaBetaDLS((Cpe480)fringe.Peek(), (int[][])pFringe.Peek(), 0, initialAlpha, initialBeta);
            }
            else
            {
                if (pDepthLimit - Limit != pDepthLimit)
                {
                    if (pWhite == im)
                    {
                        double maxValue = double.MinValue;

                        while (pFringe.Count != 0)
                        {
                            maxValue = Math.Max(maxValue, RecursiveAlphaBetaDLS(this, (int[][])pFringe.Peek(), Limit - 1, alpha, beta));
                            pFringe.Pop();
                            //if (maxValue == double.NaN)
                            //    goto l1;
                            if (endEstimation > DateTime.Now)
                                return double.NaN;
                            
                            if (maxValue >= beta)
                            {
                                pEstimatedMoveEvaluation = maxValue;
                                return maxValue;
                            }
                            alpha = Math.Max(alpha, maxValue);
                        //l1:
                        }
                        pEstimatedMoveEvaluation = maxValue;
                        return maxValue;
                    }
                    else
                    {
                        double minValue = double.MaxValue;

                        while (pFringe.Count != 0)
                        {
                            minValue = Math.Min(minValue, RecursiveAlphaBetaDLS(this, (int[][])pFringe.Peek(), Limit - 1, alpha, beta));
                            if (endEstimation > DateTime.Now)
                                return double.NaN;
                            pFringe.Pop();
                            if (minValue <= alpha)
                            {
                                pEstimatedMoveEvaluation = minValue;
                                return minValue;
                            }
                            beta = Math.Min(beta, minValue);
                        }
                        pEstimatedMoveEvaluation = minValue;
                        return minValue;
                    }
                }
                return double.NaN;
            }
        }
        /// <summary>
        /// Iterates all pieces of a given position for the best move.
        /// </summary>
        /// <param name="State"></param>
        void CurrentBestMove(int[][] State/*position*/)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    //Evaluates all possible moves for a piece with given position
                    //And compares to last known best move (if better update best move)
                    if (State[i][j] == pPawn)//Pawn
                    {
                        //Pawn moves 2 steps if at startline
                        if (i == pStartLine)
                        {
                            if (State[i + (2 * pOrientation)][j] == 0 && State[i + (1 * pOrientation)][j] == 0)
                            {
                                for (int k = 0; k < 8; k++)
                                    for (int l = 0; l < 8; l++)
                                        pEvaluatePosition[k][l] = State[k][l];

                                pEvaluatePosition[i][j] = 0;
                                pEvaluatePosition[i + (2 * pOrientation)][j] = pPawn;

                                EvaluateMove();
                                if (pEvaluation > pBestMoveEvaluation)
                                {
                                    //start {for EstimateBestMove()}
                                    int[][] toKeep = new int[8][];
                                    for (int k = 0; k < 8; k++)
                                        toKeep[k] = new int[8];

                                    for (int k = 0; k < 8; k++)
                                        for (int l = 0; l < 8; l++)
                                            toKeep[k][l] = pEvaluatePosition[k][l];

                                    pFringe.Push(toKeep);
                                    //end {for EstimateBestMove()}

                                    pBestMoveEvaluation = pEvaluation;
                                    pBestMove[0] = i;
                                    pBestMove[1] = j;
                                    pBestMove[2] = i + (2 * pOrientation);
                                    pBestMove[3] = j;
                                }
                            }
                        }

                        //Pawn moves 1 step
                        if (State[i + (1 * pOrientation)][j] == 0)
                        {
                            for (int k = 0; k < 8; k++)
                                for (int l = 0; l < 8; l++)
                                    pEvaluatePosition[k][l] = State[k][l];

                            pEvaluatePosition[i][j] = 0;
                            pEvaluatePosition[i + (1 * pOrientation)][j] = pPawn;
                            if (i + (1 * pOrientation) == pPromotionLine)
                                pEvaluatePosition[i + (1 * pOrientation)][j] = pQueen;

                            EvaluateMove();
                            if (pEvaluation > pBestMoveEvaluation)
                            {
                                //start {for EstimateBestMove()}
                                int[][] toKeep = new int[8][];
                                for (int k = 0; k < 8; k++)
                                    toKeep[k] = new int[8];

                                for (int k = 0; k < 8; k++)
                                    for (int l = 0; l < 8; l++)
                                        toKeep[k][l] = pEvaluatePosition[k][l];

                                pFringe.Push(toKeep);
                                //end {for EstimateBestMove()}

                                pBestMoveEvaluation = pEvaluation;
                                pBestMove[0] = i;
                                pBestMove[1] = j;
                                pBestMove[2] = i + (1 * pOrientation);
                                pBestMove[3] = j;
                            }
                        }

                        //Pawn kill diagonal left step
                        if (j != 0)
                        {
                            if (State[i + (1 * pOrientation)][j - 1] == pOpponentPawn || State[i + (1 * pOrientation)][j - 1] == pOpponentQueen)
                            {
                                for (int k = 0; k < 8; k++)
                                    for (int l = 0; l < 8; l++)
                                        pEvaluatePosition[k][l] = State[k][l];

                                pEvaluatePosition[i][j] = 0;
                                pEvaluatePosition[i + (1 * pOrientation)][j - 1] = pPawn;
                                if (i + (1 * pOrientation) == pPromotionLine)
                                    pEvaluatePosition[i + (1 * pOrientation)][j - 1] = pQueen;

                                EvaluateMove();
                                if (pEvaluation > pBestMoveEvaluation)
                                {
                                    //start {for EstimateBestMove()}
                                    int[][] toKeep = new int[8][];
                                    for (int k = 0; k < 8; k++)
                                        toKeep[k] = new int[8];

                                    for (int k = 0; k < 8; k++)
                                        for (int l = 0; l < 8; l++)
                                            toKeep[k][l] = pEvaluatePosition[k][l];

                                    pFringe.Push(toKeep);
                                    //end {for EstimateBestMove()}

                                    pBestMoveEvaluation = pEvaluation;
                                    pBestMove[0] = i;
                                    pBestMove[1] = j;
                                    pBestMove[2] = i + (1 * pOrientation);
                                    pBestMove[3] = j - 1;
                                }
                            }
                        }

                        //Pawn kill diagonal right step
                        if (j != 7)
                        {
                            if (State[i + (1 * pOrientation)][j + 1] == pOpponentPawn || State[i + (1 * pOrientation)][j + 1] == pOpponentQueen)
                            {
                                for (int k = 0; k < 8; k++)
                                    for (int l = 0; l < 8; l++)
                                        pEvaluatePosition[k][l] = State[k][l];

                                pEvaluatePosition[i][j] = 0;
                                pEvaluatePosition[i + (1 * pOrientation)][j + 1] = pPawn;
                                if (i + (1 * pOrientation) == pPromotionLine)
                                    pEvaluatePosition[i + (1 * pOrientation)][j + 1] = pQueen;

                                EvaluateMove();
                                if (pEvaluation > pBestMoveEvaluation)
                                {
                                    //start {for EstimateBestMove()}
                                    int[][] toKeep = new int[8][];
                                    for (int k = 0; k < 8; k++)
                                        toKeep[k] = new int[8];

                                    for (int k = 0; k < 8; k++)
                                        for (int l = 0; l < 8; l++)
                                            toKeep[k][l] = pEvaluatePosition[k][l];

                                    pFringe.Push(toKeep);
                                    //end {for EstimateBestMove()}

                                    pBestMoveEvaluation = pEvaluation;
                                    pBestMove[0] = i;
                                    pBestMove[1] = j;
                                    pBestMove[2] = i + (1 * pOrientation);
                                    pBestMove[3] = j + 1;
                                }
                            }
                        }
                    }
                    if (State[i][j] == pQueen)//Queen
                    {
                        //Get sight and copy.
                        QueenSight(i, j);
                        for (int k = 0; k < 8; k++)
                            for (int l = 0; l < 8; l++)
                                pCBMQueenSight[k][l] = pQueenSight[k][l];

                        //Iterate over all true values in sight if corresponding given position tile free or holds an enemy piece
                        for (int k = 0; k < 8; k++)
                            for (int l = 0; l < 8; l++)
                            {
                                if (pCBMQueenSight[k][l])
                                    if (State[k][l] == 0 || State[k][l] == pOpponentPawn || State[k][l] == pOpponentQueen)
                                    {
                                        for (int m = 0; m < 8; m++)
                                            for (int n = 0; n < 8; n++)
                                                pEvaluatePosition[m][n] = State[m][n];

                                        pEvaluatePosition[i][j] = 0;
                                        pEvaluatePosition[k][l] = pQueen;

                                        EvaluateMove();
                                        if (pEvaluation > pBestMoveEvaluation)
                                        {
                                            //start {for EstimateBestMove()}
                                            int[][] toKeep = new int[8][];
                                            for (int m = 0; m < 8; m++)
                                                toKeep[m] = new int[8];

                                            for (int m = 0; m < 8; m++)
                                                for (int n = 0; n < 8; n++)
                                                    toKeep[m][n] = pEvaluatePosition[m][n];

                                            pFringe.Push(toKeep);
                                            //end {for EstimateBestMove()}

                                            pBestMoveEvaluation = pEvaluation;
                                            pBestMove[0] = i;
                                            pBestMove[1] = j;
                                            pBestMove[2] = k;
                                            pBestMove[3] = l;
                                        }
                                    }
                            }
                    }
                }
        }
        /// <summary>
        /// Evaluates a game position (pEvaluatePosition), used by CurrentBestMove().
        /// </summary>
        void EvaluateMove()
        {
            //reset
            pEvaluation = 0;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (pEvaluatePosition[i][j] == pOpponentPawn)//Opponent Pawn
                    {
                        pEvaluation += -5;//value of killing a pawn is +5
                        if (!pDefensive)
                            pEvaluation += -1;//if offensive then the value of killing a pawn is +6
                        continue;
                    }
                    if (pEvaluatePosition[i][j] == pOpponentQueen)//Opponent Queen
                    {
                        pEvaluation += -48;//value of killing a queen is +48
                        if (!pDefensive)
                            pEvaluation += -26;//if offensive then the value of killing a queen is +74
                        continue;
                    }

                    if (pEvaluatePosition[i][j] == pPawn)//Pawn
                    {
                        pTileEvaluation = 1;//value of a normal pawn

                        if (j == 0 || j == 7)
                            pTileEvaluation = 0.75;//value of an edge pawn

                        if (i == pPromotionLine)
                            pTileEvaluation = 7;//value for promoting to a queen
                        else
                        {
                            if (i == pFrontline)
                            {
                                pTileEvaluation *= 2;//multiplication for moving to the frontline while offensive

                                if (pDefensive)
                                {
                                    if (j != 0)
                                        if (pEvaluatePosition[i][j - 1] == pPawn)
                                            pTileEvaluation *= 3;//another multiplication for barricading the frontline while defensive

                                    if (j != 7)
                                        if (pEvaluatePosition[i][j + 1] == pPawn)
                                            pTileEvaluation *= 3;//another multiplication for barricading the frontline while defensive
                                }
                            }
                            else
                            {
                                if (i == pFrontline + pOrientation)
                                    pTileEvaluation *= 3;//multiplication for advancing frontline one step
                                else
                                    if (i == pFrontline + (2 * pOrientation))
                                        pTileEvaluation *= 4;//multiplication for advancing frontline two steps
                            }
                        }

                        if (pDefensive)
                        {
                            PieceDefended(i, j);

                            if (!pDefended/*pawn not defended*/)
                            {
                                pEvaluation += -1;
                                pEvaluation += -1 / pTileEvaluation;//Higher piece evaluation means lower effect on the total evaluation.
                                continue;
                            }
                        }
                        pEvaluation += pTileEvaluation;
                        continue;
                    }
                    if (pEvaluatePosition[i][j] == pQueen)//Queen
                    {
                        pTileEvaluation = 7;//value of a queen

                        //Get sight and copy.
                        QueenSight(i, j);
                        for (int k = 0; k < 8; k++)
                            for (int l = 0; l < 8; l++)
                                pEMQueenSight[k][l] = pQueenSight[k][l];

                        //Iterate over all true values in sight to see if it's in the sight of an opponent queen
                        for (int k = 0; k < 8; k++)
                            for (int l = 0; l < 8; l++)
                            {
                                if (pEMQueenSight[k][l])
                                    if (pEvaluatePosition[k][l] == pOpponentQueen)
                                        pTileEvaluation = Math.Abs(pTileEvaluation) * -49;//Because it'll mostly be killed (-7^2)
                            }

                        //No way to be killed by an opponent pawn if at promotionLine or behined the promotionLine (opponent's startLine)
                        if (i != (pPromotionLine - pOrientation) && i != pPromotionLine)
                        {
                            //front-left
                            if (j != 0)
                                if (pEvaluatePosition[i + pOrientation][j - 1] == pOpponentPawn)
                                    pTileEvaluation = Math.Abs(pTileEvaluation) * -343;//Because it'll mostly be killed & an insult (-7^3)

                            //front-right
                            if (j != 7)
                                if (pEvaluatePosition[i + pOrientation][j + 1] == pOpponentPawn)
                                    pTileEvaluation = Math.Abs(pTileEvaluation) * -343;//Because it'll mostly be killed & an insult (-7^3)
                        }

                        if (pDefensive)
                        {
                            PieceDefended(i, j);

                            if (!pDefended/*queen not defended*/)
                            {
                                pEvaluation += -7;
                                pEvaluation += -1 / pTileEvaluation;//Higher piece evaluation means lower effect on the total evaluation.
                                continue;
                            }
                        }
                        pEvaluation += pTileEvaluation;
                    }
                }
            //Note: I could've quantified free tiles too.
        }
        /// <summary>
        /// Sets pDefended to true if the piece with coordinates (PieceRow,PieceCol) is defended by another piece, used by EvaluateMove().
        /// </summary>
        /// <param name="PieceRow"></param>
        /// <param name="PieceCol"></param>
        void PieceDefended(int PieceRow/*Holds the row coordinate of a specific piece.*/, int PieceCol/*Holds the column coordinate of a specific piece.*/)
        {
            //if defended by a pawn
            if (PieceRow != (pStartLine - pOrientation) && PieceRow != pStartLine)//No way to be defended by a pawn if at startline or behined the startline
            {
                //back-left
                if (PieceCol != 0)
                    if (pEvaluatePosition[PieceRow - pOrientation][PieceCol - 1] == pPawn)
                    {
                        pDefended = true;
                        return;
                    }

                //back-right
                if (PieceCol != 7)
                    if (pEvaluatePosition[PieceRow - pOrientation][PieceCol + 1] == pPawn)
                    {
                        pDefended = true;
                        return;
                    }
            }

            //if defended by a queen
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (pEvaluatePosition[i][j] == pQueen)
                    {
                        //Get sight and copy.
                        QueenSight(i, j);
                        for (int k = 0; k < 8; k++)
                            for (int l = 0; l < 8; l++)
                                pPDQueenSight[k][l] = pQueenSight[k][l];

                        if (pPDQueenSight[PieceRow][PieceCol])
                        {
                            pDefended = true;
                            return;
                        }
                    }
                }
            pDefended = false;
        }
        /// <summary>
        /// Sets all corresponding tiles that a queen with coordinates (QueenRow,QueenCol) "sees".
        /// </summary>
        /// <param name="QueenRow"></param>
        /// <param name="QueenCol"></param>
        void QueenSight(int QueenRow/*Holds the row coordinate of a specific queen.*/, int QueenCol/*Holds the column coordinate of a specific queen.*/)
        {
            int rowsAbove = QueenRow;//Holds the count of rows above a specific queen.
            int rowsBelow = 7 - QueenRow;//Holds the count of rows below a specific queen.
            int columnsLeft = QueenCol;//Holds the count of columns to the left of a specific queen.
            int columnsRight = 7 - QueenCol;//Holds the count of columns to the right of a specific queen.

            //Clear old data
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    pQueenSight[i][j] = false;

            //Set sight above
            if (rowsAbove != 0)
            {
                for (int i = 0; i < rowsAbove; i++)
                {
                    pQueenSight[QueenRow - (1 + i)][QueenCol] = true;
                    if (pEvaluatePosition[QueenRow - (1 + i)][QueenCol] != 0)
                        break;
                }
            }

            //Set sight above&right
            if (rowsAbove != 0 && columnsRight != 0)
            {
                for (int i = 0; i < rowsAbove && i < columnsRight; i++)
                {
                    pQueenSight[QueenRow - (1 + i)][QueenCol + (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow - (1 + i)][QueenCol + (1 + i)] != 0)
                        break;
                }
            }

            //Set sight right
            if (columnsRight != 0)
            {
                for (int i = 0; i < columnsRight; i++)
                {
                    pQueenSight[QueenRow][QueenCol + (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow][QueenCol + (1 + i)] != 0)
                        break;
                }
            }

            //Set sight below&right
            if (rowsBelow != 0 && columnsRight != 0)
            {
                for (int i = 0; i < rowsBelow && i < columnsRight; i++)
                {
                    pQueenSight[QueenRow + (1 + i)][QueenCol + (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow + (1 + i)][QueenCol + (1 + i)] != 0)
                        break;
                }
            }

            //Set sight below
            if (rowsBelow != 0)
            {
                for (int i = 0; i < rowsBelow; i++)
                {
                    pQueenSight[QueenRow + (1 + i)][QueenCol] = true;
                    if (pEvaluatePosition[QueenRow + (1 + i)][QueenCol] != 0)
                        break;
                }
            }

            //Set sight below&left
            if (rowsBelow != 0 && columnsLeft != 0)
            {
                for (int i = 0; i < rowsBelow && i < columnsLeft; i++)
                {
                    pQueenSight[QueenRow + (1 + i)][QueenCol - (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow + (1 + i)][QueenCol - (1 + i)] != 0)
                        break;
                }
            }

            //Set sight left
            if (columnsLeft != 0)
            {
                for (int i = 0; i < columnsLeft; i++)
                {
                    pQueenSight[QueenRow][QueenCol - (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow][QueenCol - (1 + i)] != 0)
                        break;
                }
            }

            //Set sight above&left
            if (rowsAbove != 0 && columnsLeft != 0)
            {
                for (int i = 0; i < rowsAbove && i < columnsLeft; i++)
                {
                    pQueenSight[QueenRow - (1 + i)][QueenCol - (1 + i)] = true;
                    if (pEvaluatePosition[QueenRow - (1 + i)][QueenCol - (1 + i)] != 0)
                        break;
                }
            }
        }
        /// <summary>
        /// Converts bestMove array to corresponding string notation.
        /// </summary>
        void BestMoveString()
        {
            switch (pBestMove[1])
            {
                case 0:
                    pFirst = 'a';
                    break;
                case 1:
                    pFirst = 'b';
                    break;
                case 2:
                    pFirst = 'c';
                    break;
                case 3:
                    pFirst = 'd';
                    break;
                case 4:
                    pFirst = 'e';
                    break;
                case 5:
                    pFirst = 'f';
                    break;
                case 6:
                    pFirst = 'g';
                    break;
                case 7:
                    pFirst = 'h';
                    break;
                default://Just a precaution
                    pFirst = 'a';
                    break;
            }

            switch (pBestMove[0])
            {
                case 0:
                    pSecond = 8;
                    break;
                case 1:
                    pSecond = 7;
                    break;
                case 2:
                    pSecond = 6;
                    break;
                case 3:
                    pSecond = 5;
                    break;
                case 4:
                    pSecond = 4;
                    break;
                case 5:
                    pSecond = 3;
                    break;
                case 6:
                    pSecond = 2;
                    break;
                case 7:
                    pSecond = 1;
                    break;
                default://Just a precaution
                    pSecond = 8;
                    break;
            }

            switch (pBestMove[3])
            {
                case 0:
                    pThird = 'a';
                    break;
                case 1:
                    pThird = 'b';
                    break;
                case 2:
                    pThird = 'c';
                    break;
                case 3:
                    pThird = 'd';
                    break;
                case 4:
                    pThird = 'e';
                    break;
                case 5:
                    pThird = 'f';
                    break;
                case 6:
                    pThird = 'g';
                    break;
                case 7:
                    pThird = 'h';
                    break;
                default://Just a precaution
                    pThird = 'a';
                    break;
            }

            switch (pBestMove[2])
            {
                case 0:
                    pFourth = 8;
                    break;
                case 1:
                    pFourth = 7;
                    break;
                case 2:
                    pFourth = 6;
                    break;
                case 3:
                    pFourth = 5;
                    break;
                case 4:
                    pFourth = 4;
                    break;
                case 5:
                    pFourth = 3;
                    break;
                case 6:
                    pFourth = 2;
                    break;
                case 7:
                    pFourth = 1;
                    break;
                default://Just a precaution
                    pFourth = 8;
                    break;
            }

            pBestMoveString = (pFirst.ToString() + pSecond.ToString() + pThird.ToString() + pFourth.ToString());
        }
        /// <summary>
        /// The entry point of the class returns a move in a timely manner..
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static string Project(int[][] Position/*game State*/)
        {
            timerStart = DateTime.Now;

            endEstimation = DateTime.Now;
            if (remainingTime <= 0)
                return "a1a1";
            endEstimation = endEstimation.AddSeconds(remainingTime / 64);

            //Only for the first turn
            if (start)
            {
                start = false;
                for (int j = 0; j < 8; j++)
                    if (Position[6/*white startLine*/][j] == 0)
                    {
                        im = false;//I'm black
                        break;
                    }
                timerEnd = DateTime.Now;
                remainingTime = remainingTime - ((timerEnd - timerStart).TotalSeconds);
                if (!im)//I'm black
                    return "h7h6";
                return "h2h3";
            }

            Cpe480 me = new Cpe480(im);
            move = me.MakeMove(Position);

            timerEnd = DateTime.Now;
            remainingTime = remainingTime - ((timerEnd - timerStart).TotalSeconds);
            return move;
        }
        /// <summary>
        /// Calculates best action to take, given a certain position.
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        string MakeMove(int[][] State/*position*/)
        {
            //Initialize data
            bool whiteFrontLineSet = false;//aids in setting the value of frontline if playing white.
            int countPawns = 0;//Holds the number of pawns.
            int countQueens = 0;//Holds the number of queens.
            int countOpponentPawns = 0;//Holds the number of opponent pawns.
            int countOpponentQueens = 0;//Holds the number of opponent queens.

            //reset
            pBestMoveEvaluation = double.MinValue;


            //Instate game
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (State[i][j] == pPawn)
                    {
                        if (!whiteFrontLineSet)
                        {
                            pFrontline = i;
                            whiteFrontLineSet = true;
                        }

                        if (!pWhite)
                            pFrontline = i;

                        countPawns++;
                        continue;
                    }
                    if (State[i][j] == pQueen)
                    {
                        countQueens++;
                        continue;
                    }
                    if (State[i][j] == pOpponentPawn)
                    {
                        countOpponentPawns++;
                        continue;
                    }
                    if (State[i][j] == pOpponentQueen)
                    {
                        countOpponentQueens++;
                    }
                }

            //Choose path (strategy)
            //I can use the difference (countPawns - countOpponentPawns) to make another path.
            if (countQueens > 0 && countOpponentQueens > 0)
                goto stage4;
            if (countOpponentQueens > 0)
                goto stage3n;
            if (countQueens > 0)
                goto stage3p;
            if (countPawns < 8 /*&& countQueens == 0*/)
                goto stage2;
            if (countPawns == 8 /*&& countQueens == 0*/)
                goto stage1;
            goto stageDefault;

        //No queens, all pawns -> defensive
        stage1:
            pDefensive = true;
            CurrentBestMove(State);//Because while being defensive & all pawns are present no need to predict time saving.
            BestMoveString();
            return pBestMoveString;

        //No queens, some pawns -> defensive
        stage2:
            pDefensive = true;
            EstimateBestMove(State);
            BestMoveString();
            return pBestMoveString;

        //Queens for this side -> offensive
        stage3p:
            pDefensive = false;
            EstimateBestMove(State);
            BestMoveString();
            return pBestMoveString;

        //Queens for other side -> defensive
        stage3n:
            pDefensive = true;
            EstimateBestMove(State);
            BestMoveString();
            return pBestMoveString;

        //Queens for both sides -> defensive
        stage4:
            pDefensive = true;
            EstimateBestMove(State);
            BestMoveString();
            return pBestMoveString;

        //Precaution stage
        stageDefault:
            BestMoveString();
            return pBestMoveString;
        }
        /// <summary>
        /// The static constructor of this class.
        /// </summary>
        static Cpe480()
        {
            timerStart = new DateTime();
            timerEnd = new DateTime();
            endEstimation = new DateTime();
            fringe = new Stack();
        }
        /// <summary>
        /// The constructor of this class.
        /// </summary>
        /// <param name="White"></param>
        Cpe480(bool White/*turn?*/)
        {
            this.pDefensive = true;
            pFringe = new Stack();

            if (White)
            {
                this.pOpponentPawn = 3;
                this.pOpponentQueen = 4;
                this.pOrientation = -1;
                this.pPawn = 1;
                this.pPromotionLine = 0;
                this.pQueen = 2;
                this.pStartLine = 6;
                this.pWhite = true;
            }
            else
            {
                this.pOpponentPawn = 1;
                this.pOpponentQueen = 2;
                this.pOrientation = 1;
                this.pPawn = 3;
                this.pPromotionLine = 7;
                this.pQueen = 4;
                this.pStartLine = 1;
                this.pWhite = false;
            }

            //Create new arrays
            pEstimatedMove = new int[4];
            this.pBestMove = new int[4];
            this.pEvaluatePosition = new int[8][];
            this.pQueenSight = new bool[8][];
            this.pCBMQueenSight = new bool[8][];
            this.pEMQueenSight = new bool[8][];
            this.pPDQueenSight = new bool[8][];
            for (int i = 0; i < 8; i++)
            {
                this.pEvaluatePosition[i] = new int[8];
                this.pQueenSight[i] = new bool[8];
                this.pCBMQueenSight[i] = new bool[8];
                this.pEMQueenSight[i] = new bool[8];
                this.pPDQueenSight[i] = new bool[8];
            }
        }
        Cpe480(bool White/*turn?*/, int depthLimit)
            : this(White)
        {
            pDepthLimit = depthLimit;
        }
    }
}