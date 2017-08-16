
namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Windows.Media.Media3D;
    using Microsoft.Samples.Kinect.BodyBasics.Game;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        // Hand Coordinates
        private Vector3D rightHand;
        private Vector3D leftHand;

        enum HandMotion { STOP, UP, DOWN, LEFT, RIGHT };

        private HandMotion rightMotion;
        private HandMotion leftMotion;

        private GeometryManager gm;
        private GameLogic gl;

        private int loop;
        private int punchD, moveD, rotateD;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            // open the sensor
            this.kinectSensor.Open();
       
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            this.leftHand = new Vector3D();
            this.rightHand = new Vector3D();

            rightMotion = HandMotion.STOP;
            leftMotion = HandMotion.STOP;

            gm = new GeometryManager();
            gl = new GameLogic();
            Configs.fixBrushTransparency();
            
            drawBounds();

            setPieceBlocks((int)gl.piece.center.X, (int)gl.piece.center.Y, (int)gl.piece.center.Z,
                gl.piece.getPieceBlocks());
            gm.setPiecePreview(gl.nextPieceP.getPieceBlocks());

            ModelVisual3D temp = new ModelVisual3D();
            viewport.Children.Add(temp);
            ModelVisual3D temp2 = new ModelVisual3D();
            preview.Children.Add(temp2);

            loop = 0;
            punchD = 0;
            moveD = 0;
            rotateD = 0;

            renderGeometry();
        }

        #region Kinect Stuff
    
        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {               
                return this.imageSource;
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {  
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                    int penIndex = 0;
                    int i = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // sometimes the depth(Z) of an inferred joint may show as negative
                                // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);

                            if (i == 0)
                            {
                                float left_deltaX = (float)(leftHand.X - jointPoints[JointType.HandLeft].X);
                                float left_deltaY = (float)(leftHand.Y - jointPoints[JointType.HandLeft].Y);
                                float right_deltaX = (float)(rightHand.X - jointPoints[JointType.HandRight].X);
                                float right_deltaY = (float)(rightHand.Y - jointPoints[JointType.HandRight].Y);

                                leftMotion = getMotionDirection(left_deltaX, left_deltaY,1.0f);
                                rightMotion = getMotionDirection(right_deltaX, right_deltaY,1.0f);

                                bool leftPunch = (joints[JointType.HandLeft].Position.Z - leftHand.Z) > 0.05;
                                bool rightPunch = (joints[JointType.HandRight].Position.Z - rightHand.Z) > 0.03;

                                this.rightHand = new Vector3D(jointPoints[JointType.HandRight].X, jointPoints[JointType.HandRight].Y,
                                     joints[JointType.HandRight].Position.Z);
                                this.leftHand = new Vector3D(jointPoints[JointType.HandLeft].X, jointPoints[JointType.HandLeft].Y,
                                      joints[JointType.HandLeft].Position.Z);
                
                                if (gl.running){
                                    if (loop == 2) { loop = 0; }
                                    if (Configs.hand == 1){
                                        switch (body.HandRightState)
                                        {
                                            case HandState.Open: if (rightPunch)
                                                {
                                                    if (punchD >= Configs.SENSITIVITY_PUNCH)
                                                    {
                                                        gl.dropPiece();
                                                        punchD = 0;
                                                    }
                                                    else { punchD++; }
                                                }
                                                break;
                                            case HandState.Closed: if (moveD >= Configs.SENSITIVITY_MOVE)
                                                {
                                                    moveP(rightMotion);
                                                    moveD = 0;
                                                }
                                                else { moveD++; }
                                                break;
                                            case HandState.Lasso: if (rotateD >= Configs.SENSITIVITY_ROTATE)
                                                {
                                                    rotateD = 0;
                                                    rotateP(rightMotion);
                                                }
                                                else { rotateD++; }
                                                break;
                                        }
                                    }
                                    else if (Configs.hand == 0){
                                        switch (body.HandLeftState)
                                        {
                                            case HandState.Open: if (leftPunch)
                                                {
                                                    if (punchD >= Configs.SENSITIVITY_PUNCH)
                                                    {
                                                        gl.dropPiece();
                                                        punchD = 0;
                                                    }
                                                    else { punchD++; }
                                                }
                                                break;
                                            case HandState.Closed: if (moveD >= Configs.SENSITIVITY_MOVE)
                                                {
                                                    moveP(leftMotion);
                                                    moveD = 0;
                                                }
                                                else { moveD++; }
                                                break;
                                            case HandState.Lasso: if (rotateD >= Configs.SENSITIVITY_ROTATE)
                                                {
                                                    rotateD = 0;
                                                    rotateP(leftMotion);
                                                }
                                                else { rotateD++; }
                                                break;
                                        }
                                    }

                                    gl.update();

                                    setPieceBlocks((int)gl.piece.center.X, (int)gl.piece.center.Y, (int)gl.piece.center.Z,
                                        gl.piece.getPieceBlocks());
                                    setLayerBlocks();
                                    gm.setPiecePreview(gl.nextPieceP.getPieceBlocks());

                                    renderGeometry();
                                    UI_level.Text = "Level: " + gl.level;
                                    UI_score.Text = "" + gl.score;
                                }
                            }
                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
                
            }
           
        }

        private HandMotion getMotionDirection(float deltaX, float deltaY, float range) {
            if (deltaX <= range && deltaX >= -range && deltaY <= range && deltaY >= -range){
                return HandMotion.STOP;
            }
            else if (deltaY > range && Math.Abs(deltaY) > Math.Abs(deltaX)){
                return HandMotion.UP;
            }
            else if (deltaY < -range && Math.Abs(deltaY) > Math.Abs(deltaX)){
                return HandMotion.DOWN;
            }
            else if (deltaX < -range && Math.Abs(deltaX) > Math.Abs(deltaY)){
                return HandMotion.RIGHT;
            }
            else {
                return HandMotion.LEFT;
            }           
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        #endregion

        #region Game Stuff
        public void setPieceBlocks(int x, int y, int z, Vector3D[] blocks) {
            gm.clearPiece();
            foreach (Vector3D v in blocks){
                gm.addToPiece(Configs.scale, x + (int)(v.X * Configs.scale), y + (int)(v.Y * Configs.scale),
                    z + (int)(v.Z * Configs.scale), Configs.pieceColor);
            }
            SolidColorBrush pieceColor = new SolidColorBrush(Colors.Red);
            pieceColor.Opacity = 0.5;
            foreach (Vector3D v in gl.piece.getPieceH()) {
                gm.addPieceProjectionBot(Configs.scale, 0.1f, Configs.scale, x + (int)(v.X * Configs.scale), z + (int)(v.Z * Configs.scale), pieceColor);
                gm.addPieceProjectionTop(Configs.scale, 0.1f, Configs.scale, x + (int)(v.X * Configs.scale), z + (int)(v.Z * Configs.scale), pieceColor);
            }
            foreach (Vector3D v in gl.piece.getPieceV()){
                gm.addPieceProjectionLeft(0.05f, Configs.scale, Configs.scale, y + (int)(v.Y * Configs.scale), z + (int)(v.Z * Configs.scale), pieceColor);
                gm.addPieceProjectionRight(0.05f, Configs.scale, Configs.scale, y + (int)(v.Y * Configs.scale), z + (int)(v.Z * Configs.scale), pieceColor);
            }
        }

        public void setLayerBlocks(){
            gm.clearLayers();
            int count = 0;
            //Percorre as layers
            for (int i = 0; i < gl.layers.Length; i++) {
                //Filtra Layers Vazias
                if (gl.layers[i].count != 0) {
                    //Percorre os blocos da layer
                    for (int x = 0; x < Configs.WIDTH; x++) {
                        for (int y = 0; y < Configs.HEIGHT; y++) {
                            //Adiciona o bloco à geometria
                            if (gl.layers[i].blocks[x, y]) { 
                                gm.addToBlocks(Configs.scale, x, y, i, Configs.colors[i]);
                                count++;
                            }
                        }
                    }
                }
            }
            drawBounds();
        }

        private void drawBounds() {
            gm.addToBlocks(Configs.scale, 0, 0, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 2, 0, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 4, 0, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 1, 1, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 3, 1, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 5, 1, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 0, 2, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 2, 2, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 4, 2, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 1, 3, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 3, 3, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 5, 3, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 0, 4, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 2, 4, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 4, 4, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 1, 5, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 3, 5, Configs.DEPTH, Brushes.White);
            gm.addToBlocks(Configs.scale, 5, 5, Configs.DEPTH, Brushes.White);
            
            gm.drawHorizontalLines();
        }

        private void moveP(HandMotion hand) {
            
            switch (hand) {
                case HandMotion.STOP: loop = 0; break;
                case HandMotion.LEFT: if (loop >= 0) { 
                                            gl.movePiece(-1, 0);
                                            loop++;
                                      }
                                      break;
                case HandMotion.RIGHT: if (loop >= 0){
                                          gl.movePiece(1, 0);
                                          loop++;
                                      }
                                      break;
                case HandMotion.UP: if (loop >= 0){
                                          gl.movePiece(0,1);
                                          loop++;
                                      }
                                      break;
                case HandMotion.DOWN: if (loop >= 0){
                                          gl.movePiece(0, -1);
                                          loop++;
                                      }
                                      break;
            }
        }

        private void rotateP(HandMotion hand) {
            switch (hand)
            {
                case HandMotion.STOP: loop = 0; break;
                case HandMotion.LEFT: if (loop == 0){
                                        gl.rotatePiece(-1, 0);
                                        loop++;   
                                    }break;
                case HandMotion.RIGHT: if (loop == 0){
                                        gl.rotatePiece(1, 0);
                                        loop++;
                                    } break;
                case HandMotion.UP: if (loop == 0) {
                                        gl.rotatePiece(0, 1);
                                        loop++;
                                    } break;
                case HandMotion.DOWN: if (loop == 0){
                                        gl.rotatePiece(0, -1);
                                        loop++;
                                    } break;
            }
        }

        public void renderGeometry() {
            viewport.Children.RemoveAt(1);
            ModelVisual3D temp = new ModelVisual3D();
            temp.Content = gm.getMasterGroup();
            viewport.Children.Add(temp);
            
            preview.Children.RemoveAt(1);
            ModelVisual3D temp2 = new ModelVisual3D();
            temp2.Content = gm.previewGeometry;
            preview.Children.Add(temp2);
        }

        #endregion
    }
}
