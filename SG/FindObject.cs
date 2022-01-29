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

        public delegate void MyEventHandler(object sender, EventArgs e);
        public static event MyEventHandler MyEvent;
        
        public bool FindLineObject(SGEvent sEvent, int x, int y, ref SGJob job)
        {
            foreach (SGJob j in sEvent.childs)
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(j.points);
                //gp.AddLine((float)j.xfrom, (float)j.yfrom, (float)j.xto, (float)j.yto);
                if (gp.IsOutlineVisible(x, y, new Pen(Brushes.Blue, 8f)))
                {
                    job = j;
                    return true;
                }


            }
            

            return false;

        }
        

        public bool FindEventObject(SGEvent sgEvent, int x, int y)
        {

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(sgEvent.rect);
            if (gp.IsVisible(x, y))
            {
                return true;
            }
            return false;
        }

        public bool FindCurvePoint(SGEvent sEvent, ref CurvePoint curvePoint, int x, int y)
        {
            foreach (SGJob j in sEvent.childs)
            {
                foreach (CurvePoint c in j.curvePoints)
                {
                    GraphicsPath gp = new GraphicsPath();
                    RectangleF t = new RectangleF(c.rect.X - 2, c.rect.Y - 2, c.rect.Width + 4, c.rect.Height + 4);
                    gp.AddEllipse(t);
                    if (gp.IsVisible(x, y))
                    {
                        curvePoint = c;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool FindCurvePointLine(SGJob job, int x, int y, ref CurvePoint prev, ref CurvePoint next)
        {

            CurvePoint c = job.curvePoints[0];


            do
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(c.x, c.y, c.next.x, c.next.y);
                if (gp.IsOutlineVisible(x, y, new Pen(Brushes.Blue, 8f)))
                {
                    prev = c;
                    next = c.next;
                    return true;
                }
                c = c.next;

            } while (c.next != null);
            return false;
        }


        public static bool FindCurvePointInJob(SGJob job, int x, int y, ref CurvePoint finded)
        {
            if (job.curvePoints.Count == 0)
                return false;
            
            CurvePoint c = job.curvePoints[0];
            do
            {
                GraphicsPath gp = new GraphicsPath();

                RectangleF t = new RectangleF(c.rect.X - 2, c.rect.Y - 2, c.rect.Width + 4, c.rect.Height + 4);
                gp.AddEllipse(t);
                if (gp.IsVisible(x, y))
                {
                    finded = c;
                    return true;
                }
                                
                c = c.next;
            } while (c.next != null);
            
            return false;
        }


    }
}
