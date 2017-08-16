using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;
using Microsoft.Samples.Kinect.BodyBasics.Game;

namespace Microsoft.Samples.Kinect.BodyBasics.Menus
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private CoordinateMapper coordinateMapper = null;
        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;

        private List<Tuple<JointType, JointType>> bones;

        private int displayWidth;
        private int displayHeight;
        private const float InferredZPositionClamp = 0.1f;

        private double lastX, lastY, lastZ;
        private double vpX, vpY;
        private bool firstCap;
        private bool canPlay;

        public StartWindow()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

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

            // open the sensor
            this.kinectSensor.Open();
            firstCap = true;
            vpX = 475;
            vpY = 375;

            canPlay = true;

            InitializeComponent();

            cursor.Opacity = 0;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }
        }

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
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }
            this.cursor.Opacity = 0;

            if (dataReceived)
            {
                foreach (Body body in bodies)
                {

                    if (body.IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                        foreach (JointType jointType in joints.Keys)
                        {

                            CameraSpacePoint position = joints[jointType].Position;
                            if (position.Z < 0)
                            {
                                position.Z = InferredZPositionClamp;
                            }

                            DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                            jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        }

                        if (body.HandRightState != HandState.NotTracked)
                        {
                            this.cursor.Opacity = 1.0;
                            double x = jointPoints[JointType.HandRight].X;
                            double y = jointPoints[JointType.HandRight].Y;

                            if (!firstCap)
                            {
                                double deltaX = (lastX - x);
                                double deltaY = (lastY - y);

                                move(deltaX, deltaY);

                                if (body.HandRightState == HandState.Closed){
                                    Console.WriteLine("PUNCH DETECTED "+vpX+" , "+vpY);
                                    punch();
                                }

                                this.cursor.Margin = getMargins(100, vpX, vpY);
                            }
                            lastX = x;
                            lastY = y;
                            lastZ = joints[JointType.HandRight].Position.Z;
                            //Console.WriteLine("Hand Pos: " + (int)vpX + " , " + (int)vpY);
                        }
                    }
                }
            }
            firstCap = false;
        }

        public void move(double dX, double dY) {
            if (dX >= 0.8) {
                vpX += 10 * (dX + 0.2);
                if (vpX > 900) { vpX = 900; }
            }
            else if (dX <= -0.8){
                vpX += 10 * (dX + 0.2);
                if (vpX < 0) { vpX = 0;}
            }

            if (dY >= 0.8)
            {
                vpY += 10 * (dY + 0.2);
                if (vpY > 700) { vpY = 700; }
            }
            else if (dY <= -0.8)
            {
                vpY+=10*(dY+0.2);
                if (vpY < 0) { vpY = 0; }
            }   
        }

        public Thickness getMargins(double scale, double x, double y) {
            double right = x - (scale / 2);
            double left = 950 - (right+scale);
            double top = y - (scale / 2);
            double bottom = 750 - (top+scale);

            return new Thickness(left, bottom, right, top);
        }

        public void punch() { 
           //PLAY BUTTON 
            if(overlaps((int)vpX,(int)vpY,100,100,800,420,100,40)){
                //Move to Game Window
                Configs.loadSettings();
                MainWindow m = new MainWindow();
                this.Close(); 
                m.Show();
                   
            }
            else if (overlaps((int)vpX, (int)vpY, 100, 100, 600, 285, 100, 40)) {
                //Move to HighScoresScreen
                HSWindow hs = new HSWindow();
                this.Close();
                hs.Show();
            }
            else if (overlaps((int)vpX, (int)vpY, 100, 100, 340, 270, 100, 40))
            {
               //Move to Settings Screen
                SettingsWindow sw = new SettingsWindow();
                this.Close();
                sw.Show();
            }
            else if (overlaps((int)vpX, (int)vpY, 100, 100, 145, 435, 100, 40)){
                this.Close();
            }
        
        }

        public bool overlaps(int r1X, int r1Y, int r1W, int r1H, int r2X, int r2Y, int r2W, int r2H){
            System.Drawing.Rectangle a = new System.Drawing.Rectangle(r1X - (r1W / 2), r1Y - (r1H / 2), r1W, r1H);
            System.Drawing.Rectangle b = new System.Drawing.Rectangle(r2X - (r2W / 2), r2Y - (r2H / 2), r2W, r2H);

            System.Drawing.Rectangle c = System.Drawing.Rectangle.Intersect(a, b);

            return !c.IsEmpty;
        }

    }
}
