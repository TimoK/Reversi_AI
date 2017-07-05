using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReversiAI
{
    public partial class ReversiVisualiser : Form
    {
        ReversiBoard board;
        ReversiGame game;

        public ReversiVisualiser(ReversiGame game)
        {
            this.game = game;
            this.board = game.board;
            InitializeComponent();
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(OnClick);
        }

        Point offset = new Point(20, 50);
        int tileLength = 50;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int boardLength = tileLength * ReversiBoard.boardSize;

            Font drawFont = new System.Drawing.Font("Arial", 16);
            g.DrawString("Red score: " + board.WhiteScore, drawFont, Brushes.Black, new Point(0, 0));
            g.DrawString("Black score: " + board.BlackScore, drawFont, Brushes.Black, new Point(0, 20));

            for (int x = 0; x <= ReversiBoard.boardSize; ++x)
            {
                g.DrawLine(Pens.Black, new Point(offset.X, offset.Y + tileLength * x), new Point(offset.X + boardLength, offset.Y + tileLength * x));
                g.DrawLine(Pens.Black, new Point(offset.X + tileLength * x, offset.Y), new Point(offset.X + tileLength * x, offset.Y + boardLength));
            }
            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                BoardSquareState boardSquareState = board.GetBoardState(boardPosition);
                Rectangle positionRectangle = new Rectangle(new Point(offset.X + tileLength * boardPosition.X, offset.Y + tileLength * boardPosition.Y), new Size(tileLength, tileLength));

                if (boardSquareState == BoardSquareState.Empty)
                {
                    if (board.isLegalMove(boardPosition, board.CurrentPlayerColor))
                    {
                        g.DrawEllipse(Pens.Yellow, positionRectangle);
                    }
                }
                else
                {
                    Brush playerColor = null;
                    if (boardSquareState == BoardSquareState.White) playerColor = Brushes.Red;
                    if (boardSquareState == BoardSquareState.Black) playerColor = Brushes.Black;
                    g.FillEllipse(playerColor, positionRectangle);
                }

            }


            if (board.GameEnded)
            {
                string gameEndedString = "";
                if (board.WhiteScore > board.BlackScore) gameEndedString = "Red has won.";
                else if (board.BlackScore > board.WhiteScore) gameEndedString = "Black has won.";
                else gameEndedString = "It's a tie.";
                g.DrawString(gameEndedString, drawFont, Brushes.Black, new Point(0, offset.Y + boardLength + 10));
            }

            base.OnPaint(e);
        }

        private void OnClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            Console.WriteLine(x);

            if (!(x < offset.X || y < offset.Y || x >= tileLength * ReversiBoard.boardSize + offset.X || y >= tileLength * ReversiBoard.boardSize + offset.Y))
            {
                game.MakeMove(new Point((x - offset.X) / tileLength, (y - offset.Y) / tileLength));
            }
            Invalidate();
            base.OnClick(e);
        }
    }


}
