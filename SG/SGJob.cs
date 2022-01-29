using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SG
{
    
    
    
    
    public partial class Form1
    {
        public static Font font;
        public static CustomLineCap customEndCap1;
        public static CustomLineCap customEndCap2;
        public static Pen current;

        public static Pen penDefaultLine = new Pen(Color.Green, 1.2f);
        public static Pen penRedLine = new Pen(Color.Red, 7f);
        public static Pen penKritPath = new Pen(Color.Red, 9f);
        public static Pen penKritJob = new Pen(Color.Orange, 19f);
        public static Pen penGreenLine = new Pen(Color.Green, 1.2f);
        public static Pen penCyclicLine = new Pen(Color.Black, 1.3f);
        public static Pen penSelected = new Pen(Color.Blue, 4f);
        public static Pen penFakeLine = new Pen(Color.Brown, 1.2f);
        public static Pen penBackLine = new Pen(Color.Gold, 3f);
        public static Pen penForwardLine = new Pen(Color.DeepSkyBlue, 3f);
        public static Pen penBackAndForward = new Pen(Brushes.Yellow, 3f);
        public static Pen penCurvePoints = new Pen(Color.DarkBlue, 1);
        
        public static int radiusCurvePoints = 3;

        public void InitializeJobsGraphic()
        {
            font = new Font("Arial", 8);
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(0, 0), new Point(-2, -7));
            gp.AddLine(new Point(-2, -7), new Point(2, -7));
            gp.AddLine(new Point(0, 0), new Point(2, -7));

            customEndCap1 = new CustomLineCap(null, gp);
            customEndCap1.SetStrokeCaps(LineCap.Round, LineCap.Round);

            gp = new GraphicsPath();
            gp.AddLine(new PointF(0, -0.7f), new PointF(-0.7f, -4));
            gp.AddLine(new PointF(-0.7f, -4), new PointF(0.7f, -4));
            gp.AddLine(new PointF(0.7f, -4), new PointF(0, -0.7f));

            customEndCap2 = new CustomLineCap(null, gp);
            customEndCap2.SetStrokeCaps(LineCap.Round, LineCap.Round);

            penDefaultLine.CustomEndCap = customEndCap1;
            penFakeLine.CustomEndCap = customEndCap1;
            penBackLine.CustomEndCap = customEndCap2;
            penFakeLine.DashStyle = DashStyle.Dash;

            penForwardLine.CustomEndCap = customEndCap2;
            penSelected.CustomEndCap = customEndCap2;
            penCyclicLine.CustomEndCap = customEndCap1;
            penCyclicLine.DashStyle = DashStyle.Dot;

            penBackAndForward.DashStyle = DashStyle.Dot;
            penBackAndForward.CustomEndCap = customEndCap2;

        }


        [Serializable()]
        public class SGJob
        {
            public static int globalJobCount = 0;
            public List<List<SGJob>> JobFullPaths;
            //public List<List<SGJob>> KritPaths;
            public string _fto;
            public SGEvent from;
            public SGEvent to;
            public bool inCycle;
            public List<CurvePoint> curvePoints;
            public JobData JD;
            public double xfrom, yfrom, xto, yto;
            public double xfrom1, yfrom1, xto1, yto1;
            public PointF[] points;
            
            private bool _isFake = false;
            //private bool _showInCycle = false;
            private bool _showBackPath = false;
            private bool _showForwardPath = false;
            private bool _showKritPath = false;
            public bool  showKritJob = false;
            public bool  isOnKritPath = false;

            
            //public bool showInCycle
            //{
            //    set { _showInCycle = value; }
            //    get { return _showInCycle; }
            //}
            
            public bool isFake
            {
                set
                {
                    _isFake = value;
                    if (_isFake)
                    {
                        JD.N = new TimeSpan();
                    }
                }
                get { return _isFake; }
            }
            public bool showBackPath
            {
                set { _showBackPath = value; } //current = (_showBackPath) ? penBackLine : penDefaultLine; }
                get { return _showBackPath; }
            }
            public bool showForwardPath
            {
                set { _showForwardPath = value; } // current = (_showForwardPath) ? penForwardLine : penDefaultLine; }
                get { return _showForwardPath; }
            }
            public bool showKritPath
            {
                set { _showKritPath = value; }
                get { return _showKritPath; }
            }
            public bool selected
            {
                set { _selected = value; } // current = (_selected) ? penSelected : penDefaultLine; }
                get { return _selected; }
            }
            private bool _selected = false;

            public SGJob()
            {
                selected = false;
            }

            public SGJob(string jobName, SGEvent from, SGEvent to, bool isFake)
            {
                
                string drawingText = "Работа №" + globalJobCount.ToString() + "(" + from._eventID.ToString() + "-" + to._eventID.ToString() + ")";
                JD = new JobData(this, drawingText);
                this.isFake = isFake;
                this.inCycle = false;
                this.from = from;
                this.to = to;
                this._fto = from._eventID.ToString() + "-" + to._eventID.ToString();
                this.curvePoints = new List<CurvePoint>();

                if (MyEvent != null)
                {
                    MyEvent(this, new EventArgs());
                }
            }

            public void Draw(BufferedGraphics buf)
            {
                double x0 = from.X;
                double y0 = from.Y;
                double x1 = to.X;
                double y1 = to.Y;
                double r0 = from.r;
                double r1 = to.r;

                if (curvePoints.Count == 0)
                {
                    points = new PointF[2];

                    Popravke(r0, r1, x0, y0, x1, y1, ref xfrom, ref yfrom, ref xto, ref yto);


                    points[0].X = (float)xfrom;
                    points[0].Y = (float)yfrom;
                    points[1].X = (float)xto;
                    points[1].Y = (float)yto;

                    if (inCycle)
                        buf.Graphics.DrawLine(penRedLine, points[0], points[1]);

                    if (showKritJob)
                        buf.Graphics.DrawLine(penKritJob, points[0], points[1]);

                    if (showKritPath)
                        buf.Graphics.DrawLine(penKritPath, points[0], points[1]);

                    if (selected)
                        buf.Graphics.DrawLine(penSelected, points[0], points[1]);
                    else
                    {
                        if (showBackPath && showForwardPath)
                        {
                            buf.Graphics.DrawLines(penForwardLine, points);
                            buf.Graphics.DrawLines(penBackAndForward, points);
                        }
                        else if (showBackPath) buf.Graphics.DrawLine(penBackLine, points[0], points[1]);
                        else if (showForwardPath) buf.Graphics.DrawLine(penForwardLine, points[0], points[1]);
                        else if (_isFake) buf.Graphics.DrawLine(penFakeLine, points[0], points[1]);
                        else buf.Graphics.DrawLine(penDefaultLine, points[0], points[1]);
                    }

                    PointF ptext2 = new PointF((float)(x0 + (x1 - x0) / 2), (float)(y0 + (y1 - y0) / 2));
                    buf.Graphics.DrawString(from._eventID.ToString() + "-" + to._eventID.ToString(), font, Brushes.Black, ptext2);

                }
                else
                {
                    //Popravke(r0, r1, x0, y0, x1, y1, ref xfrom, ref yfrom, ref xto, ref yto);

                    points = new PointF[curvePoints.Count];
                    CurvePoint cc = curvePoints[0];
                    for (int k = 0; k < curvePoints.Count && cc != null; ++k)
                    {
                        points[k] = new PointF(cc.x, cc.y);
                        PointF ptext2 = new PointF(cc.x + 6, cc.y + 6);
                        if(k != 0 && k != curvePoints.Count - 1)
                            buf.Graphics.DrawString(from._eventID.ToString() + "-" + to._eventID.ToString(), font, Brushes.Black, ptext2);
                        cc = cc.next;
                    }

                    if (inCycle)
                        buf.Graphics.DrawLines(penRedLine, points);

                    if (showKritJob)
                        buf.Graphics.DrawLines(penKritJob, points);

                    if (showKritPath)
                        buf.Graphics.DrawLines(penKritPath, points);

                    if (selected)
                        buf.Graphics.DrawLines(penSelected, points);
                    else
                    {
                        if (showBackPath && showForwardPath)
                        {
                            buf.Graphics.DrawLines(penForwardLine, points);
                            buf.Graphics.DrawLines(penBackAndForward, points);
                        }
                        else if (showBackPath) buf.Graphics.DrawLines(penBackLine, points);
                        else if (showForwardPath) buf.Graphics.DrawLines(penForwardLine, points);
                        else if (_isFake) buf.Graphics.DrawLines(penFakeLine, points);
                        else buf.Graphics.DrawLines(penDefaultLine, points);
                    }


                    cc = curvePoints[0];
                    for (int k = 0; k < curvePoints.Count && cc != null; ++k)
                    {
                        if (!cc.isHidden)
                        {
                            buf.Graphics.DrawEllipse(penCurvePoints,
                            (float)cc.x - radiusCurvePoints, (float)cc.y - radiusCurvePoints,
                            2 * radiusCurvePoints, 2 * radiusCurvePoints);
                        }

                        cc = cc.next;
                    }




                }




               
            }

            public void Popravke(double rfrom, double rto, double x0, double y0, double x1, double y1,
                ref double xfrom, ref double yfrom, ref double xto, ref double yto)
            {


                double dx = x1 - x0;
                double dy = y1 - y0;

                if (dx == 0 && dy == 0)
                {
                    xfrom = x0;
                    yfrom = y0;
                    xto = x1;
                    yto = y1;

                    return;

                }

                double tgalfa = dy / dx;
                double alfa = Math.Abs(Math.Atan(tgalfa));

                //double xfrom, yfrom, xto, yto;

                if (dx < 0)
                {
                    xfrom = x0 - rfrom * Math.Cos(alfa);
                    xto = x1 + rto * Math.Cos(alfa);
                }
                else
                {
                    xfrom = x0 + rfrom * Math.Cos(alfa);
                    xto = x1 - rto * Math.Cos(alfa);
                }

                if (dy < 0)
                {
                    yfrom = y0 - rfrom * Math.Sin(alfa);
                    yto = y1 + rto * Math.Sin(alfa);
                }
                else
                {
                    yfrom = y0 + from.r * Math.Sin(alfa);
                    yto = y1 - rto * Math.Sin(alfa);
                }
            }

            public void RemoveCurvePoint(CurvePoint cp)
            {
                int index = curvePoints.IndexOf(cp);

                if (index > -1)
                {
                    if (curvePoints.Count == 3) //начало конец и наш
                    {
                        curvePoints = new List<CurvePoint>();
                    }
                    else
                    {
                        CurvePoint prev = curvePoints[index].prev;
                        CurvePoint next = curvePoints[index].next;

                        prev.next = next;
                        next.prev = prev;

                        if (prev.isHidden) this.from.Move(this.from.X, this.from.Y, null);
                        if (next.isHidden) this.to.Move(this.to.X, this.to.Y, null);

                        curvePoints.Remove(cp);
                    }
                }
            }

            public void CalcRNRO(SGJob prevJob)
            {
                if (prevJob.JD.RO > this.JD.RN)
                {
                    this.JD.RN = prevJob.JD.RO;
                    this.JD.RO = this.JD.RN + this.JD.N;
                }
            }

            public void CalcPOPNResPResS(SGJob nextJob)
            {
                if (this.JD.PO > nextJob.JD.PN || this.JD.PO == new TimeSpan())
                {
                    this.JD.PO = nextJob.JD.PN;
                    this.JD.PN = this.JD.PO - this.JD.N;
                }
                
                this.JD.Krit = nextJob.JD.RN;
                
                this.JD.ResP = this.JD.PN - this.JD.RN;
                this.JD.ResS = nextJob.JD.RN - this.JD.RO;
            }


            public void ClearJobData()
            {
                JD = new JobData(JD.job);
            }

            public void CalcOzidaemDisperce()
            {
                int secMin = Convert.ToInt32(this.JD.Min.TotalSeconds);
                int secMax = Convert.ToInt32(this.JD.Max.TotalSeconds);
                double d = ((double)(secMax - secMin))/3600;
                d = d * d / 25;
                
                
                this.JD.Ozidaem =  new TimeSpan(0, 0, (3 * secMin + 2 * secMax) / 5);
                this.JD.Disperse = d;

            }




        }

       
    }
}
