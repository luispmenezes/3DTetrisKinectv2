using Microsoft.Samples.Kinect.BodyBasics.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class GameLogic
    {
        public int score;
        public int level;
        public int nextPiece;
        public Layer[] layers;
        public Piece piece;
        public Piece nextPieceP;
        public bool running;

        private int lastTime;
        private int timer;

        public GameLogic() {
            layers = new Layer[Configs.DEPTH];
            piece = new Piece(getRandomInt(0,4));
            nextPieceP = new Piece(0);
            resetGame();        
        }

        public void update() {
            if (running) {
                //CHECK LAYER MATCH
                foreach (Layer l in layers) {
                    if (l.isFull()) {
                        l.reset();
                        score += 10;
                        level = 1 + (score / 50);
                    }
                }
                //CHECK TIMER
                timer += (System.Environment.TickCount - lastTime);
                lastTime = System.Environment.TickCount;

                int lvlDif = (level - 1) * 200;

                if (timer >= (Configs.timerLimit-lvlDif)){
                    timer = 0;
                    movePieceDown();
                }     
            }
        }

        public void resetGame() {
            score = 0;
            level = 1;
            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new Layer(Configs.colors[i]);   
            }
            piece = new Piece(getRandomInt(0, 4));
            nextPiece = getRandomInt(0, 4);

            nextPieceP.type = nextPiece;

            lastTime = System.Environment.TickCount;

            running = true;
        }

        public bool newPiece() {
            piece.center = new Vector3D(Configs.WIDTH/2,Configs.HEIGHT/2,0);
            piece.rotationX = 0;
            piece.rotationY = 0;
            piece.type = nextPiece;
            if (layers[0].doesPieceFit((int)piece.center.X, (int)piece.center.Y, piece.getPieceBlocks())){
                nextPiece = getRandomInt(0, 4);
                nextPieceP.type = nextPiece;
                return true;
            }
            else { return false; }
        }

        #region Kinect Events
        public void movePiece(int dX, int dY) {
            
            piece.center.X += dX;
            piece.center.Y += dY;
            if (!isValid(piece.getPieceBlocks()) && checkPieceCollision()) {
                piece.center.X -= dX;
                piece.center.Y -= dY;
            }
        }

        public void rotatePiece(int dX, int dY){
            piece.rotateX(dX);
            piece.rotateY(dY);
            if (!isValid(piece.getPieceBlocks()))
            {
                piece.rotateX(-dX);
                piece.rotateY(-dY);
            }
        }

        public bool isValid(Vector3D[] p) { 
            foreach (Vector3D v in p) {
                    if (piece.center.X + v.X < 0 || piece.center.X + v.X > Configs.WIDTH-1 ||
                        piece.center.Y + v.Y < 0 || piece.center.Y + v.Y > Configs.HEIGHT-1){
                        return false;
                    }
                    if (piece.center.Z + v.Z < 0 || piece.center.Z + v.Z >= Configs.DEPTH)
                    {
                        return false;
                    }
            }
            return true;
        }

        public void movePieceDown() {
            piece.center.Z++;
            if (checkPieceCollision())
            {
                dropPiece();
            }       
        }

        public bool checkPieceCollision() {
            Console.WriteLine("PIECE CENTER: " + piece.center);
            foreach (Vector3D v in piece.getPieceBlocks()) {
                Console.WriteLine("COL TEST -> " + v);
                int x = (int)(v.X + piece.center.X);
                int y = (int)(v.Y + piece.center.Y);
                int z = (int)(v.Z + piece.center.Z);

                if ( z == 9 || x < 0 || x >= Configs.WIDTH || y < 0 || y >= Configs.HEIGHT ||
                    layers[z].blocks[x,y]) {
                    return true;
                }     
            } 
            return false; 
        }

        public void dropPiece() {
            
            foreach (Vector3D v in piece.getPieceBlocks()){
                for (int i = 1; i < layers.Length; i++){
                    if ((i == layers.Length-1 && !layers[i].blocks[(int)(v.X + piece.center.X), (int)(v.Y + piece.center.Y)]) ||
                        (layers[i].blocks[(int)(v.X + piece.center.X), (int)(v.Y + piece.center.Y)] &&
                        !layers[i - 1].blocks[(int)(v.X + piece.center.X), (int)(v.Y + piece.center.Y)])){

                            layers[i - 1].newBlock((int)(v.X + piece.center.X), (int)(v.Y + piece.center.Y));
                            i = layers.Length;  
                    
                    }
                }
            }
            if(!newPiece()){
                running = false;
            }
        }
        
        #endregion

        public int getRandomInt(int min, int max) {
            Random r = new Random();
            return min + (int)(r.NextDouble() * ((max - min) + 1));
        }
    }
}
