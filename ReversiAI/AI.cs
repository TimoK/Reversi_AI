using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ReversiAI
{
    interface AI
    {
        Point GetMove(ReversiBoard board, PlayerColor color);
    }

    class SimpleAI : AI
    {
        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.isLegalMove(boardPosition, color)) return boardPosition;
            }
            throw new System.Exception("No legal moves available.");
        }
    }

    class HeuristicAI : AI
    {
        Heuristic heuristic;

        public HeuristicAI(Heuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            Point bestMove = new Point(0, 0);
            double bestScore = double.MinValue;

            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.isLegalMove(boardPosition, color))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.makeMove(boardPosition);
                    double score = heuristic.GetScore(boardWithMove, color);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                }
            }
            return bestMove;
        }
    }

    class MinMax : AI
    {
        Heuristic heuristic;
        int searchdepth;

        public MinMax(Heuristic heuristic, int searchdepth)
        {
            this.heuristic = heuristic;
            this.searchdepth = searchdepth;
            if (searchdepth < 1) throw new ArgumentException("Searchdepth can not be smaller than 1.");
        }

        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            Point bestMove = new Point(0, 0);
            double bestScore = double.MinValue;

            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.isLegalMove(boardPosition, color))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.makeMove(boardPosition);
                    double score = getBoardValuation(boardWithMove, color, searchdepth - 1, false);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                }
            }

            return bestMove;
        }

        double getBoardValuation(ReversiBoard board, PlayerColor color, int currentSearchDepth, bool maximising)
        {
            if (currentSearchDepth == 0) return heuristic.GetScore(board, color);

            bool minimising;
            if (maximising) minimising = false; else minimising = true;

            Point bestMove = new Point(0, 0);
            double bestScore = double.MaxValue;
            if(maximising) bestScore = double.MinValue;

            PlayerColor currentPlayerColor = ReversiBoard.otherPlayerColor(color);
            if (maximising) currentPlayerColor = color;

            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.isLegalMove(boardPosition, currentPlayerColor))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.makeMove(boardPosition);
                    double score = getBoardValuation(boardWithMove, color, currentSearchDepth - 1, minimising);
                    if (maximising)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = boardPosition;
                        }
                    }
                    else
                    {
                        if(score < bestScore)
                        {
                            bestScore = score;
                            bestMove = boardPosition;
                        }
                    }
                }
            }
            return bestScore;
        }
    }




    interface Heuristic
    {
        double GetScore(ReversiBoard board, PlayerColor playerColor);
    }

/* Did not want to design a heuristic myself (beyond scope of this project) so used a heuristic from someone else
 * Author: Kartikkukreja https://github.com/kartikkukreja
 * Code: https://github.com/kartikkukreja/blog-codes/blob/master/src/Heuristic%20Function%20for%20Reversi%20(Othello).cpp
 * Code ported to C# and my implementation of Reversi by me
 * Blogpost describing the heuristic: https://kartikkukreja.wordpress.com/2013/03/30/heuristic-function-for-reversiothello/ 
 */
    class DynamicHeuristic : Heuristic
    {
        public double GetScore(ReversiBoard board, PlayerColor playerColor)
        {
            PlayerColor opponentColor = ReversiBoard.otherPlayerColor(playerColor);
            BoardSquareState playerTile = ReversiBoard.getTile(playerColor);
            BoardSquareState opponentTile = ReversiBoard.getTile(opponentColor);

            List<Point> boardLocations = ReversiBoard.GetBoardPositions();

            int my_tiles = 0, opp_tiles = 0, my_front_tiles = 0, opp_front_tiles = 0;
            double p = 0, c = 0, l = 0, m = 0, f = 0, d = 0;

            int[,] V = { {20, -3, 11, 8, 8, 11, -3, 20},
                           {-3, -7, -4, 1, 1, -4, -7, -3},
                           {11, -4, 2, 2, 2, 2, -4, 11},
                           {8, 1, 2, -3, -3, 2, 1, 8},
                           {8, 1, 2, -3, -3, 2, 1, 8},
                           {11, -4, 2, 2, 2, 2, -4, 11},
                           {-3, -7, -4, 1, 1, -4, -7, -3},
                           {20, -3, 11, 8, 8, 11, -3, 20}};
            // Piece difference, frontier disks and disk squares
            foreach (Point location in boardLocations)
            {
                if (board.GetBoardState(location) == playerTile)
                {
                    d += V[location.X, location.Y];
                    my_tiles++;
                }
                else if (board.GetBoardState(location) == opponentTile)
                {
                    d -= V[location.X, location.Y];
                    opp_tiles++;
                }
                if (board.GetBoardState(location) != BoardSquareState.Empty)
                {
                    foreach (Point direction in board.directions)
                    {
                        Point adj_space = location; adj_space.Offset(direction);
                        if (!ReversiBoard.OutOfBounds(adj_space) && board.GetBoardState(adj_space) == BoardSquareState.Empty)
                        {
                            if (board.GetBoardState(location) == playerTile) my_front_tiles++;
                            else opp_front_tiles++;
                            break;
                        }
                    }
                }
            }
            if (my_tiles > opp_tiles)
                p = (100.0 * my_tiles) / (my_tiles + opp_tiles);
            else if (my_tiles < opp_tiles)
                p = -(100.0 * opp_tiles) / (my_tiles + opp_tiles);
            else p = 0;

            if (my_front_tiles > opp_front_tiles)
                f = -(100.0 * my_front_tiles) / (my_front_tiles + opp_front_tiles);
            else if (my_front_tiles < opp_front_tiles)
                f = (100.0 * opp_front_tiles) / (my_front_tiles + opp_front_tiles);
            else f = 0;

            // Corner occupancy
            int playerCorner = 0, opponentCorner = 0;
            if (board.GetBoardState(new Point(0, 0)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(0, 0)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(7, 0)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(7, 0)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(0, 7)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(0, 7)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(7, 7)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(7, 7)) == opponentTile) ++opponentCorner;
            c = 25 * (playerCorner - opponentCorner);

            // Corner closeness
            int playerCornerClose = 0, opponentCornerClose = 0;
            if (board.GetBoardState(new Point(0, 0)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(0, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(0, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 0)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 0)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(0, 7)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(0, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(0, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 7)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 7)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(7, 0)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(7, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(7, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 0)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 0)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(7, 7)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(6, 7)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 7)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(7, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(7, 6)) == opponentTile) opponentCornerClose++;
            }
            l = -12.5 * (playerCornerClose - opponentCornerClose);

            // Mobility
            int playerValidMoves = board.NumLegalMoves(playerColor);
            int opponentValidMoves = board.NumLegalMoves(opponentColor);
            if (playerValidMoves > opponentValidMoves)
                m = (100.0 * playerValidMoves) / (playerValidMoves + opponentValidMoves);
            else if (playerValidMoves < opponentValidMoves)
                m = -(100.0 * opponentValidMoves) / (playerValidMoves + opponentValidMoves);
            else m = 0;

            // final weighted score
            double score = (10 * p) + (801.724 * c) + (382.026 * l) + (78.922 * m) + (74.396 * f) + (10 * d);
            return score;
        }
    }



}
