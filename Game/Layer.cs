using Microsoft.Samples.Kinect.BodyBasics.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class Layer
    {
        public int count;
        public Brush color;
        public bool[,] blocks;
        public int width, height;

        public Layer(Brush color){
            this.width = Configs.WIDTH;
            this.height = Configs.HEIGHT;
            this.color = color;
            blocks = new bool[width,height];
            reset();
        }

        #region New Piece
        public bool doesPieceFit(int centerX, int centerY, Vector3D[] pBlocks) {         
            for (int i = 0; i < pBlocks.Length; i++) {
                if (blocks[centerX + (int)pBlocks[i].X, centerY + (int)pBlocks[i].Y]) {
                    return false;
                }
            }
            return true;
        }

        public void newPiece(int centerX, int centerY, Vector3D[] pBlocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[centerX + (int)pBlocks[i].X, centerY + (int)pBlocks[i].Y] = true;
                count++;
            }
        }

        public void newBlock(int centerX, int centerY){
            blocks[centerX , centerY] = true;
            count++;
        }

        #endregion

        #region FullLayer Logic
        public bool isFull() {
            return count == (width * height);
        }

        public void reset() {
            count = 0;
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++){
                    blocks[i,j] = false;
                }
            }
        }
        #endregion

    }
}
