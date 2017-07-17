using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ReversiAI
{
    public class ReversiGame
    {
        public ReversiBoard board = new ReversiBoard();

        bool player1_human, player2_human;
        AI ai1, ai2;

        public ReversiGame()
        {
            // Hardcoded for now, can later add it to initialize options or input it in console
            player1_human = false;
            player2_human = false;
            ai1 = new MinMax(new DynamicHeuristic(), 2, true);
            ai2 = new MinMax(new DynamicHeuristic(), 3, true);
            //ai2 = new HeuristicAI(new DynamicHeuristic());
           
            if (!player1_human) player1Move();
        }

        private void player1Move()
        {
            if (player1_human) return;
            MakeMove(ai1.GetMove(board, PlayerColor.White));
        }

        private void player2Move()
        {
            if (player2_human) return;
            MakeMove(ai2.GetMove(board, PlayerColor.Black));
        }

        private void currentPlayerMove()
        {
            if (board.CurrentPlayerColor == PlayerColor.White) player1Move();
            else player2Move();
        }

        public void MakeHumanMove(Point move)
        {
            if (board.CurrentPlayerColor == PlayerColor.White && !player1_human) return;
            if (board.CurrentPlayerColor == PlayerColor.Black && !player2_human) return;
            if (!board.isLegalMove(move, board.CurrentPlayerColor)) return;

            MakeMove(move);
        }

        public void MakeMove(Point move)
        {
            if (board.GameEnded) return;

            if (!board.isLegalMove(move, board.CurrentPlayerColor))
            {
                throw new Exception("Illegal move was made.");
            }
            board.makeMove(move);

            if (!board.GameEnded)
            {
                currentPlayerMove();
            }
        }
    }

    public class ReversiBoard
    {
        public static int boardSize = 8;

        private BoardSquareState[,] board = new BoardSquareState[8, 8];
        private PlayerColor currentPlayerColor = PlayerColor.White;
        public List<Point> directions;

        int whiteScore = 2;
        int blackScore = 2;
        bool gameEnded = false;

        public int WhiteScore
        {
            get
            {
                return whiteScore;
            }
        }

        public int BlackScore
        {
            get
            {
                return blackScore;
            }
        }

        public int GetScore(PlayerColor color)
        {
            if (color == PlayerColor.White) return WhiteScore;
            return BlackScore;
        }

        public bool GameEnded
        {
            get
            {
                return gameEnded;
            }
        }

        public PlayerColor CurrentPlayerColor
        {
            get
            {
                return currentPlayerColor;
            }
        }

        public ReversiBoard()
        {
            startUpBoard();
            directions = makeDirections();
        }

        private void startUpBoard()
        {
            // First empty the board, then put the starting pieces on the board
            emptyBoard();
            board[3, 3] = BoardSquareState.White;
            board[4, 3] = BoardSquareState.Black;
            board[3, 4] = BoardSquareState.Black;
            board[4, 4] = BoardSquareState.White;
            // Make white the first player to move
            currentPlayerColor = PlayerColor.White;
        }

        private void emptyBoard()
        {
            // Make all the tiles empty
            for (int x = 0; x < boardSize; ++x)
                for (int y = 0; y < boardSize; ++y)
                    board[x, y] = BoardSquareState.Empty;
        }

        public void makeMove(Point move)
        {
            if(OutOfBounds(move)) throw new System.ArgumentException("Move position out of board bounds.");
            if (board[move.X, move.Y] != BoardSquareState.Empty) throw new System.ArgumentException("Boardmove not possible, position not empty.");
            if (!isLegalMove(move, currentPlayerColor)) return;

            PlayerColor opponentPlayerColor = otherPlayerColor(currentPlayerColor);
            // Assign the tile that the player picked to the player
            board[move.X, move.Y] = getTile(currentPlayerColor);
            increasePlayerScore(currentPlayerColor, 1);
            // Go over into each direction, if a own tile is discovered change all opponent tiles inbetween to own
            foreach (Point direction in directions)
            {
                Point position = move;
                List<Point> opponentTilesToChange = new List<Point>();
                while (true)
                {
                    position.Offset(direction);
                    if (OutOfBounds(position)) break;
                    if (board[position.X, position.Y] == BoardSquareState.Empty) break;
                    if (board[position.X, position.Y] == getTile(opponentPlayerColor)) opponentTilesToChange.Add(position);
                    if (board[position.X, position.Y] == getTile(currentPlayerColor))
                    {
                        foreach (Point opponentTileToChange in opponentTilesToChange) board[opponentTileToChange.X, opponentTileToChange.Y] = getTile(currentPlayerColor);
                        increasePlayerScore(currentPlayerColor, opponentTilesToChange.Count);
                        increasePlayerScore(opponentPlayerColor, -opponentTilesToChange.Count);
                        break;
                    }
                }
            }

            //Pass the turn
            PlayerColor previousPlayerColor = currentPlayerColor;
            currentPlayerColor = opponentPlayerColor;
            if (currentPlayerNoLegalMoves())
            {
                currentPlayerColor = previousPlayerColor;
                if (currentPlayerNoLegalMoves()) gameEnded = true;
            }
        }

        private void increasePlayerScore(PlayerColor playerColor, int amount)
        {
            if (playerColor == PlayerColor.White) whiteScore += amount;
            else blackScore += amount;
        }

        private bool currentPlayerNoLegalMoves()
        {
            List<Point> boardPositions = GetBoardPositions();
            foreach (Point boardPosition in boardPositions) if (isLegalMove(boardPosition, currentPlayerColor)) return false;
            return true;
        }

        public ReversiBoard Copy()
        {
            ReversiBoard copy = new ReversiBoard();
            copy.CopyBoard(this);
            return copy;
        }

        public void CopyBoard(ReversiBoard toCopy)
        {
            this.blackScore = toCopy.BlackScore;
            this.whiteScore = toCopy.WhiteScore;
            this.gameEnded = toCopy.GameEnded;
            this.currentPlayerColor = toCopy.CurrentPlayerColor;
            foreach (Point position in ReversiBoard.GetBoardPositions())
            {
                this.board[position.X, position.Y] = toCopy.GetBoardState(position);
            }
        }

        public int NumLegalMoves(PlayerColor playerColor)
        {
            int numLegalMoves = 0;
            foreach (Point position in GetBoardPositions())
            {
                if (isLegalMove(position, playerColor)) ++numLegalMoves;
            }
            return numLegalMoves;
        }

        public bool isLegalMove(Point move, PlayerColor playerColor)
        {
            if (OutOfBounds(move)) return false;
            if (board[move.X, move.Y] != BoardSquareState.Empty) return false;

            BoardSquareState movingPlayerTile = getTile(playerColor);
            BoardSquareState opponentTile = getTile(otherPlayerColor(playerColor));

            foreach (Point direction in directions)
            {
                Point position = move;
                bool encounteredOpponent = false;
                while (true)
                {
                    position.Offset(direction);
                    if (OutOfBounds(position)) break;
                    if (board[position.X, position.Y] == BoardSquareState.Empty) break;
                    if (board[position.X, position.Y] == opponentTile) encounteredOpponent = true;
                    if (board[position.X, position.Y] == movingPlayerTile)
                    {
                        if (encounteredOpponent) return true;
                        break;
                    }
                }
            }

            return false;
        }

        public static bool OutOfBounds(Point position)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= boardSize || position.Y >= boardSize) return true;
            return false;
        }

        public BoardSquareState GetBoardState(Point position)
        {
            return board[position.X, position.Y];
        }

        public static BoardSquareState getTile(PlayerColor playerColor)
        {
            if (playerColor == PlayerColor.Black) return BoardSquareState.Black;
            return BoardSquareState.White;
        }

        public static PlayerColor otherPlayerColor(PlayerColor playerColor)
        {
            if (playerColor == PlayerColor.Black) return PlayerColor.White;
            return PlayerColor.Black;
        }

        private static List<Point> makeDirections()
        {
            List<Point> directions = new List<Point>();
            for(int x = -1; x <= 1; ++x)
                for (int y = -1; y <= 1; ++y)
                {
                    if (x == 0 & y == 0) continue;
                    Point direction = new Point();
                    direction.X = x;
                    direction.Y = y;
                    directions.Add(direction);
                }
            return directions;
        }

        public static List<Point> GetBoardPositions()
        {
            List<Point> boardPositions = new List<Point>();
            for(int x = 0; x < boardSize; ++x)
                for (int y = 0; y < boardSize; ++y)
                {
                    boardPositions.Add(new Point(x, y));
                }
            return boardPositions;
        }

    }

    public enum PlayerColor { Black, White }
    public enum BoardSquareState { Black, White, Empty }

}
