using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics.Game
{
    class Piece
    {
        public int type;
        public int rotationX, rotationY;
        public Vector3D center;

        public Piece(int type) {
            this.type = type;
            rotationX = 0;
            rotationY = 0;
            center = new Vector3D(Configs.WIDTH/2,Configs.HEIGHT/2,0);
        }

        #region Obtains Piece Blocks
        public Vector3D[] getPieceBlocks() {  
            return rotateBlocks(getBlocks());
        }

        private Vector3D[] getBlocks(){
            Vector3D[] blocks = new Vector3D[4];

            if (type == 0)
            {
                blocks[0] = new Vector3D(0, 0, 0);
                blocks[1] = new Vector3D(-1, 0, 0);
                blocks[2] = new Vector3D(0, -1, 0);
                blocks[3] = new Vector3D(-1, -1, 0);
            }
            else if (type == 1) {
                blocks[0] = new Vector3D(-1, 0, 0);
                blocks[1] = new Vector3D(0, 0, 0);
                blocks[2] = new Vector3D(1, 0, 0);
                blocks[3] = new Vector3D(2, 0, 0);    
            }
            else if (type == 2)
            {
                blocks[0] = new Vector3D(-1, -1, 0);
                blocks[1] = new Vector3D(0, -1, 0);
                blocks[2] = new Vector3D(0, 0, 0);
                blocks[3] = new Vector3D(1, 0, 0);
            }
            else if (type == 3)
            {
                blocks[0] = new Vector3D(-1, 0, 0);
                blocks[1] = new Vector3D(-1, -1, 0);
                blocks[2] = new Vector3D(0, -1, 0);
                blocks[3] = new Vector3D(1, -1, 0);
            }
            else if (type == 4)
            {
                blocks[0] = new Vector3D(-1, 0, 0);
                blocks[1] = new Vector3D(0, 0, 0);
                blocks[2] = new Vector3D(0, -1, 0);
                blocks[3] = new Vector3D(1, 0, 0);
            }

            return blocks;
        }

        public Vector3D[] rotateBlocks(Vector3D[] blocks) {

            for (int i = 0; i < blocks.Length; i++) {
                double temp = 0;

                switch (rotationX) {
                    case 1: temp = blocks[i].Z;
                            blocks[i].Z = blocks[i].X;
                            blocks[i].X = -temp;
                            break;

                    case 2: blocks[i].X *= -1;
                            blocks[i].Z *= -1;
                            break;

                    case 3: temp = blocks[i].Z;
                            blocks[i].Z = -blocks[i].X;
                            blocks[i].X = temp;
                            break;
                }

                switch (rotationY){
                    case 1: temp = blocks[i].Z;
                            blocks[i].Z = blocks[i].Y;
                            blocks[i].Y = -temp;
                            break;

                    case 2: blocks[i].Y *= -1;
                            blocks[i].Z *= -1;
                            break;

                    case 3: temp = blocks[i].Z;
                            blocks[i].Z = -blocks[i].Y;
                            blocks[i].Y = temp;
                            break;
                }
            }

            int lowZ=(int)center.Z;
            foreach (Vector3D v in blocks) {
                if (v.Z < lowZ) {
                    lowZ = (int)v.Z;
                }
            }

            lowZ*=-1;

            for (int i = 0; i < blocks.Length; i++){
                blocks[i].Z += lowZ;
            }

            return blocks;
        }
        #endregion

        #region Rotate Piece
        public void rotateX(int dx){
            rotationX = (rotationX + dx) % 4;
            if (rotationX == -1) { rotationX = 3; }
        }
        
        public void rotateY(int dy){
            rotationY = (rotationY + dy) % 4;
            if (rotationY == -1) { rotationY = 3; }
        }
        #endregion

        #region Piece Boundaries
        public Vector3D[] getPieceH() {
            Vector3D[] blocks = getPieceBlocks();
            List<Vector3D> vectors = new List<Vector3D>();

            foreach (Vector3D b in blocks) {
                bool add = true;
                foreach (Vector3D v in vectors) {
                    if (b.X == v.X && v.Z == b.Z) {
                        add = false;
                    }
                }
                if (add) {
                    vectors.Add(b);
                }
            }
            return vectors.ToArray<Vector3D>();
        }

        public Vector3D[] getPieceV(){
            Vector3D[] blocks = getPieceBlocks();
            List<Vector3D> vectors = new List<Vector3D>();

            foreach (Vector3D b in blocks)
            {
                bool add = true;
                foreach (Vector3D v in vectors)
                {
                    if (b.Y == v.Y && v.Z == b.Z)
                    {
                        add = false;
                    }
                }
                if (add)
                {
                    vectors.Add(b);
                }
            }
            return vectors.ToArray<Vector3D>();
        }
        #endregion
    }
}
