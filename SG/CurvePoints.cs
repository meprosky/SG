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
        public static int CompareCurvePoint(CurvePoint c1, CurvePoint c2)
        {

            if (c1.x == c2.x)
                return 0;

            if (c1.x > c2.x || (c1.x == c2.x && c1.y > c2.y))
                return 1;


            return -1;


        }

        [Serializable()]
        public class CurvePoint
        {
            public double xfrom, yfrom, xto, yto;

            public CurvePoint prev;
            public CurvePoint next;


            public bool isHidden;
            private int _x, _y;

            public int x { set { _x = value; rect.X = (float)_x - r; rect.Width = 2 * r;} get { return _x; } }
            public int y { set { _y = value; rect.Y = (float)_y - r; rect.Height = 2 * r;} get { return _y; } }  
            
            
            SGJob job;
            public RectangleF rect = new RectangleF();
            public float r = 2f;
         
            public CurvePoint(int x, int y, SGJob job)
            {

                this.job = job;
                this.isHidden = false;

                

                if (job.curvePoints.Count == 0)
                {

                    PointF pf = GetProjection(new PointF(x, y), new PointF(job.from.X, job.from.Y), new PointF(job.to.X, job.to.Y));

                    this.x = Convert.ToInt32(pf.X); // x;
                    this.y = Convert.ToInt32(pf.Y); //y;

                    List<CurvePoint> c = job.curvePoints;
                    c.Add(new CurvePoint(job.xfrom, job.yfrom, job)); //второй конструктор
                    c.Add(new CurvePoint(job.xto, job.yto, job));
                    c.Add(this);
                    c[0].prev = null;
                    c[0].next = this;
                    c[0].isHidden = true;
                    c[1].prev = this;
                    c[1].next = null;
                    c[1].isHidden = true;
                    c[2].prev = c[0];
                    c[2].next = c[1];
                    c[2].isHidden = false;
                }
                else
                {
                    if (FindCurvePointLine(job, x, y, ref this.prev, ref this.next))
                    {
                        PointF pf = GetProjection(new PointF(x, y), new PointF(this.prev.x, this.prev.y), new PointF(this.next.x, this.next.y));

                        this.x = Convert.ToInt32(pf.X); // x;
                        this.y = Convert.ToInt32(pf.Y); //y;
                        
                        this.prev.next = this;
                        this.next.prev = this;

                    }
                    else
                    {
                        MessageBox.Show("ErrorCurvePoint");
                    }
                }
            }

            public CurvePoint(double x, double y, SGJob job)
            {
                this.job = job;
                this.isHidden = true;
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
            }

            public void Move(int x, int y)
            {

                if (this.isHidden)
                    return;
                
                this.x = x;
                this.y = y;

                if (prev.isHidden)
                {
                    double xfrom = 0, yfrom = 0, xto = 0, yto = 0;
                    job.Popravke(job.from.r, 1, job.from.X, job.from.Y, x, y, ref xfrom, ref yfrom, ref xto, ref yto);
                    this.prev.x = Convert.ToInt32(xfrom);
                    this.prev.y = Convert.ToInt32(yfrom);
                }

                if (next.isHidden)
                {
                    double xfrom = 0, yfrom = 0, xto = 0, yto = 0;
                    job.Popravke(1, job.to.r, x, y, job.to.X, job.to.Y, ref xfrom, ref yfrom, ref xto, ref yto);
                    this.next.x = Convert.ToInt32(xto);
                    this.next.y = Convert.ToInt32(yto);
                }
                

            }


            

            
         


           
        }
    
    
        public static PointF GetProjection(PointF p, PointF p1, PointF p2)
        {
            float fDenominator = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
            if (fDenominator == 0) // p1 and p2 are the same
                return p1;

            float t = (p.X * (p2.X - p1.X) - (p2.X - p1.X) * p1.X + p.Y * (p2.Y - p1.Y) - (p2.Y - p1.Y) * p1.Y) / fDenominator;

            return new PointF(p1.X + (p2.X - p1.X) * t, p1.Y + (p2.Y - p1.Y) * t);
        }
    
    }
}
