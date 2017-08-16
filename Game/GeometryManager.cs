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
    class GeometryManager
    {
        public Model3DGroup[] layerGeometry;
        public Model3DGroup pieceGeometry;
        public Model3DGroup previewGeometry;

        public GeometryManager() {
            layerGeometry = new Model3DGroup[Configs.DEPTH+1];
            for (int i = 0; i < layerGeometry.Length; i++) { 
                layerGeometry[i] = new Model3DGroup();
            }

            pieceGeometry = new Model3DGroup();
            previewGeometry = new Model3DGroup();
        }

        private GeometryModel3D BuildSolid(float scaleX, float scaleY,float scaleZ, float x, float y, float z, Brush color)
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();

            float sX = scaleX / 2;
            float sY = scaleY / 2;
            float sZ = scaleZ / 2;

            mesh.Positions.Add(new Point3D(x - sX, y - sY, z + sZ));
            mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(x + sX, y - sY, z + sZ));
            mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(x + sX, y + sY, z + sZ));
            mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(x - sX, y + sY, z + sZ));
            mesh.Normals.Add(new Vector3D(0, 0, 1));

            mesh.Positions.Add(new Point3D(x - sX, y - sY, z - sZ));
            mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(x + sX, y - sY, z - sZ));
            mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(x + sX, y + sY, z - sZ));
            mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(x - sX, y + sY, z - sZ));
            mesh.Normals.Add(new Vector3D(0, 0, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            return new GeometryModel3D(mesh, new DiffuseMaterial(color));
        }

        public void rotatePiece(Vector3D axis, int angle){
            Transform3DGroup group = pieceGeometry.Transform as Transform3DGroup;
            QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, angle * 180 / Math.PI));
            group.Children.Add(new RotateTransform3D(r));
        }

        public void movePiece(int x, int y, int z){
            Transform3DGroup group = pieceGeometry.Transform as Transform3DGroup;
            TranslateTransform3D trans = new TranslateTransform3D(x, y, z);
            group.Children.Add(trans);
        }

        public void popLayer(int idx){
            for (int i = idx - 1; i > 0; i--) {
                layerGeometry[i + 1] = layerGeometry[i].CloneCurrentValue();
                Transform3DGroup group = layerGeometry[i + 1].Transform as Transform3DGroup;
                TranslateTransform3D trans = new TranslateTransform3D(0, 0, -Configs.getVPZ());
                group.Children.Add(trans);
            }
        }

        public void addToBlocks(float scale, int x, int y, int z, Brush color){
            GeometryModel3D model = BuildSolid(scale,scale,scale, Configs.blockCenterX(x), Configs.blockCenterY(y),
                Configs.blockCenterZ(z), color);
            layerGeometry[(int)z].Children.Add(model);   
        }

        #region Set Piece and Piece Projections
        public void addToPiece(float scale, int x, int y, int z, Brush color){
            GeometryModel3D model = BuildSolid(scale,scale,scale, Configs.blockCenterX(x), Configs.blockCenterY(y),
                Configs.blockCenterZ(z), color);
            pieceGeometry.Children.Add(model);
        }

        public void addPieceProjectionBot(float scaleX, float scaleY, float scaleZ, int x, int z, Brush color){
            GeometryModel3D model = BuildSolid(scaleX, scaleY, scaleZ, Configs.blockCenterX(x), Configs.blockCenterY(0)-0.82f,
                Configs.blockCenterZ(z), color);
            pieceGeometry.Children.Add(model);
        }

        public void addPieceProjectionTop(float scaleX, float scaleY, float scaleZ, int x, int z, Brush color){
            GeometryModel3D model = BuildSolid(scaleX, scaleY, scaleZ, Configs.blockCenterX(x), Configs.blockCenterY(Configs.HEIGHT-1) + 0.82f,
                Configs.blockCenterZ(z), color);
            pieceGeometry.Children.Add(model);
        }

        public void addPieceProjectionLeft(float scaleX, float scaleY, float scaleZ, int y, int z, Brush color)
        {
            GeometryModel3D model = BuildSolid(scaleX, scaleY, scaleZ, Configs.blockCenterX(0)-0.82f, Configs.blockCenterY(y),
                Configs.blockCenterZ(z), color);
            pieceGeometry.Children.Add(model);
        }

        public void addPieceProjectionRight(float scaleX, float scaleY, float scaleZ, int y, int z, Brush color)
        {
            GeometryModel3D model = BuildSolid(scaleX, scaleY, scaleZ, Configs.blockCenterX(Configs.WIDTH-1) + 0.82f, Configs.blockCenterY(y),
                Configs.blockCenterZ(z), color);
            pieceGeometry.Children.Add(model);
        }

        #endregion


        public void setPiecePreview(Vector3D[] blocks) {
            previewGeometry.Children.Clear();
            float scale = 1.5f;
            foreach(Vector3D v in blocks){
                GeometryModel3D model = BuildSolid(scale, scale, scale, 1.5f+(float)v.X * scale, 1.5f+(float)v.Y * scale,
                    (float)v.Z * scale, Brushes.White);
                previewGeometry.Children.Add(model);
            }
        }

        public void drawHorizontalLines() {

            float pad = 2.5f*Configs.scale;
           
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) - 0.9f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) + 0.9f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) + 2.5f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) + 4.1f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                    Configs.blockCenterY(0) + 5.7f, Configs.blockCenterZ(9), Brushes.White));

            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH-1) + pad,
                    Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(0) - 0.9f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(0) + 0.9f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(0) + 2.5f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(0) + 4.1f, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(0) + 5.7f, Configs.blockCenterZ(9), Brushes.White));

            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - pad,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - 0.9f,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 0.9f,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 2.5f,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 4.1f,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 5.7f,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));


            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) - 0.9f,
                Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 0.9f,
                Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 2.5f,
                Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 4.1f,
                Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));
            layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale / 20, 20.0f, Configs.blockCenterX(0) + 5.7f,
                Configs.blockCenterY(0) - pad, Configs.blockCenterZ(9), Brushes.White));

            pad = 0.5f * Configs.scale;
            for (int i = 9; i > -2; i--)
            {
                layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale * Configs.HEIGHT, Configs.scale / 20, Configs.blockCenterX(0) - pad,
                  Configs.blockCenterY(Configs.HEIGHT / 2) - pad, Configs.blockCenterZ(i), Brushes.White));
                layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale * Configs.WIDTH, Configs.scale / 20, Configs.scale / 20, Configs.blockCenterX(Configs.WIDTH / 2) - pad,
                    Configs.blockCenterY(Configs.HEIGHT - 1) + pad, Configs.blockCenterZ(i), Brushes.White));
                layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale / 20, Configs.scale * Configs.HEIGHT, Configs.scale / 20, Configs.blockCenterX(Configs.WIDTH - 1) + pad,
                    Configs.blockCenterY(Configs.HEIGHT / 2) - pad, Configs.blockCenterZ(i), Brushes.White));
                layerGeometry[Configs.DEPTH].Children.Add(BuildSolid(Configs.scale * Configs.WIDTH, Configs.scale / 20, Configs.scale / 20, Configs.blockCenterX(Configs.WIDTH / 2) - pad,
                    Configs.blockCenterY(0) - pad, Configs.blockCenterZ(i), Brushes.White));
            }
        }

        public void clearPiece() {
            pieceGeometry.Children.Clear();
        }

        public void clearLayers()
        {
            foreach (Model3DGroup m in layerGeometry){
                m.Children.Clear();
            }
        }

        public Model3DGroup getMasterGroup(){
            Model3DGroup master = new Model3DGroup();

            foreach (Model3DGroup m in layerGeometry){
                foreach (GeometryModel3D g in m.Children) {
                    master.Children.Add(g);               
                }          
            }

            foreach (GeometryModel3D p in pieceGeometry.Children){
                master.Children.Add(p);
            }
            return master;
        }
    }
}
