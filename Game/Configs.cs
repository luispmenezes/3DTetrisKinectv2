using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.BodyBasics.Game
{
    class Configs
    {
        public static int WIDTH=6;
        public static int HEIGHT=6;
        public static int DEPTH=10;
        public static Brush[] colors = { Brushes.SkyBlue, Brushes.Coral, Brushes.Yellow, Brushes.Violet,
                                           Brushes.Red, Brushes.Orange, Brushes.Coral, Brushes.Green, Brushes.Fuchsia, Brushes.Red};
        public static SolidColorBrush pieceColor = new SolidColorBrush(Colors.White);
        public static float viewPortW = 6, viewPortH = 6, viewPortD=10;
        public static float scale = viewPortW / WIDTH;
        public static int timerLimit = 2000;

        public static int SENSITIVITY_PUNCH = 2;
        public static int SENSITIVITY_MOVE = 3;
        public static int SENSITIVITY_ROTATE = 2;

        public static int hand = 1;

        public static int[] highscores = new int[6];
        public static DateTime[] dates = new DateTime[6];

        #region Coordinate Transforms
        public static float blockCenterX(int idx) {
            float sw = viewPortW / WIDTH;
            return (sw * idx) + (sw / 2) - (viewPortW / 2); ;
        }

        public static float blockCenterY(int idx){
            float sh = viewPortH / HEIGHT;
            return (sh * idx) + (sh / 2) - (viewPortH/2);
        }

        public static float blockCenterZ(int idx)
        {
            float sd = viewPortD / DEPTH;
            return -(sd * idx) + (sd / 2);
        }

        public static float getVPZ(){
            return viewPortD / DEPTH;
        }
        #endregion

        public static void fixBrushTransparency(){
            pieceColor.Opacity = 0.80;
        }

        public static void loadScores(){
            highscores = Properties.Settings.Default.Highscores.Split(',').Select(s => Int32.Parse(s)).ToArray();
            dates[0] = Properties.Settings.Default.date0;
            dates[1] = Properties.Settings.Default.date1;
            dates[2] = Properties.Settings.Default.date2;
            dates[3] = Properties.Settings.Default.date3;
            dates[4] = Properties.Settings.Default.date4;
            dates[5] = Properties.Settings.Default.date5;
        }

        public static void addScore(int score) {
            for (int i = 0; i < highscores.Length; i++) {
                if (score > highscores[i]) {  
                    for (int j = highscores.Length-1; j > i ; j--) {
                        highscores[j] = highscores[j + 1];
                        dates[j] = dates[j + 1];
                    }
                    highscores[i] = score;
                    dates[i] = DateTime.Now;
                    break;
                }
            }     
        }

        public static void saveScores() { 
            Properties.Settings.Default.Highscores = String.Join(",", highscores.Select(i => i.ToString()).ToArray());
            Properties.Settings.Default.date0 = dates[0];
            Properties.Settings.Default.date1 = dates[1];
            Properties.Settings.Default.date2 = dates[2];
            Properties.Settings.Default.date3 = dates[3];
            Properties.Settings.Default.date4 = dates[4];
            Properties.Settings.Default.date5 = dates[5];
            Properties.Settings.Default.Save();
        }

        public static void loadSettings() {
            switch (Properties.Settings.Default.Setting0) {
                case 0: timerLimit = 2000;
                    break;
                case 1: timerLimit = 1000;
                    break;
                case 2: timerLimit = 500;
                    break;                
            }

            switch (Properties.Settings.Default.Setting1) { 
                case 0: colors[0]=Brushes.SkyBlue;
                        colors[1]=Brushes.Coral;
                        colors[2]=Brushes.Yellow;
                        colors[3]=Brushes.Violet;
                        colors[4]=Brushes.Red;
                        colors[5]=Brushes.Orange;
                        colors[6]=Brushes.Coral;
                        colors[7]=Brushes.Green;
                        colors[8]=Brushes.Fuchsia;
                        colors[9]=Brushes.Red;
                        break;
                case 1: colors[0]=Brushes.Orange;
                        colors[1]=Brushes.Green;
                        colors[2]=Brushes.Red;
                        colors[3]=Brushes.Violet;
                        colors[4]=Brushes.Red;
                        colors[5]=Brushes.Orange;
                        colors[6]=Brushes.Green;
                        colors[7]=Brushes.Yellow;
                        colors[8]=Brushes.Blue;
                        colors[9]=Brushes.Red;
                        break;
                case 2: colors[0] = Brushes.Fuchsia;
                        colors[1] = Brushes.Coral;
                        colors[2] = Brushes.Yellow;
                        colors[3] = Brushes.Violet;
                        colors[4] = Brushes.Orange;
                        colors[5] = Brushes.Green;
                        colors[6] = Brushes.Red;
                        colors[7] = Brushes.Violet;
                        colors[8] = Brushes.Yellow;
                        colors[9] = Brushes.Red;
                        break;
            }

            hand = Properties.Settings.Default.Setting2;

            switch (Properties.Settings.Default.Setting3){
                case 0: SENSITIVITY_PUNCH = 1;
                        SENSITIVITY_MOVE = 2;
                        SENSITIVITY_ROTATE = 1;
                       break;
                case 1:SENSITIVITY_PUNCH = 2;
                       SENSITIVITY_MOVE = 3;
                       SENSITIVITY_ROTATE = 2;
                       break;
                case 2:SENSITIVITY_PUNCH = 3;
                       SENSITIVITY_MOVE = 4;
                       SENSITIVITY_ROTATE = 3;
                       break;
            }
        }
    }
}
