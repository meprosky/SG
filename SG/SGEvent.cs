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
        //private static int _cR = 20;
        //public int curRadius { set { _cR = Convert.ToInt32(SGparam.scale * value); SGparam.curRadius = value; foreach (SGEvent s in sglist) s.r = _cR; } get { return _cR; } }
        private static Pen penDefaultEvent = new Pen(Brushes.Green, 1);
        private static Pen penFakeEvent = new Pen(Brushes.Brown, 1);

        [Serializable()]
        public class SGEvent
        {
            public List<SGJob> childs = new List<SGJob>();
            public List<SGJob> parents = new List<SGJob>();
            public List<List<SGJob>> paths;
            public List<int> cyclic = new List<int>();
            public List<List<SGJob>> cyclicPaths;

            private SG SGparam;
            private bool _isFakeEvent;
            public int _eventID;
            private int _x, _y;
            public bool drag = false;
            public int X { set { _x = value; rect.X = _x - r; rect.Width = 2 * r; p0.X = value; } get { return _x; } }
            public int Y { set { _y = value; rect.Y = _y - r; rect.Height = 2 * r; p0.Y = value; } get { return _y; } }
            public Point p0;

            public int _r;
            public int r { set { _r = value; X = _x; Y = _y; Move(_x, _y, null); } get { return _r; } }
            public Rectangle rect;
            public string text;

            public Font font;
            public Font lineFont;

            public bool isFakeEvent
            {
                set
                {
                    _isFakeEvent = value;
                    //penEvent = (value) ? new Pen(Brushes.Brown, 1) : new Pen(Brushes.Green, 1);
                }
                get
                {
                    return _isFakeEvent;
                }
            }

            public bool selected = false;

            public SGEvent()
            {
                selected = false;
            }

            public SGEvent(SGEvent parent, int eventID, string text, int x, int y, bool isFake, SG SGparam)
            {
                this._eventID = eventID;
                this.SGparam = SGparam;
                this.isFakeEvent = isFake;
                this.r = SGparam.GetScaledRadius(); 
                this.X = x;
                this.Y = y;
                this.p0 = new Point(x, y);
                
                this.text = text + eventID.ToString();

                this.font = new Font("Arial", 12);
                this.font = new Font("Arial", 8);


                if (parent != null)
                {
                    SGJob inParents = new SGJob("", parent, this, isFake); //, false);
                    this.parents.Add(inParents);
                    parent.childs.Add(inParents);
                }




            }

            public void Draw(BufferedGraphics buf)
            {
                if (selected)
                    buf.Graphics.FillEllipse(Brushes.Yellow, rect);

                
                if(isFakeEvent)
                    buf.Graphics.DrawEllipse(penFakeEvent, rect);
                else
                    buf.Graphics.DrawEllipse(penDefaultEvent, rect);

                SizeF size = buf.Graphics.MeasureString(_eventID.ToString(), font);

                PointF ptext = new PointF(p0.X - size.Width / 2, p0.Y - size.Height / 2);

                buf.Graphics.DrawString(_eventID.ToString(), font, Brushes.Black, ptext);

                foreach (SGJob links in childs)
                {
                    links.Draw(buf); //, this);
                }
            }

            public void Move(int toX, int toY, BufferedGraphics buf)
            {
                X = toX;
                Y = toY;

                double xfrom = 0, yfrom = 0, xto = 0, yto = 0;

                foreach (SGJob j in this.childs)
                {
                    if (j.curvePoints.Count == 0) continue;
                    j.Popravke(r, 1, X, Y, j.curvePoints[0].next.x, j.curvePoints[0].next.y, ref xfrom, ref yfrom, ref xto, ref yto);
                    j.curvePoints[0].x = Convert.ToInt32(xfrom);
                    j.curvePoints[0].y = Convert.ToInt32(yfrom);
                }

                foreach (SGJob j in this.parents)
                {
                    foreach (SGJob j2 in j.from.childs)
                    {
                        if (j2.to == this)
                        {
                            //j2.to.X = X;
                            //j2.to.Y = Y;

                            if (j2.curvePoints.Count == 0) continue;
                            j2.Popravke(1, r, j2.curvePoints[1].prev.x, j2.curvePoints[1].prev.y, X, Y, ref xfrom, ref yfrom, ref xto, ref yto);
                            j2.curvePoints[1].x = Convert.ToInt32(xto);
                            j2.curvePoints[1].y = Convert.ToInt32(yto);
                        }
                    }
                }

                if (buf == null) return;

                if (isFakeEvent)
                    buf.Graphics.DrawEllipse(penFakeEvent, rect);
                else
                    buf.Graphics.DrawEllipse(penDefaultEvent, rect);
                
                
                SizeF size = buf.Graphics.MeasureString(_eventID.ToString(), font);
                PointF ptext = new PointF(p0.X - size.Width / 2, p0.Y - size.Height / 2);
                buf.Graphics.DrawString(_eventID.ToString(), font, Brushes.Black, ptext);
            }

            public void ConnectNearNode(SGEvent removingEvent)
            {

                removingEvent.RemoveLink(this);

                foreach (SGJob s in removingEvent.childs)
                {
                    SGJob job = this.AddNewLink(this, s.to, s.isFake);
                    if (job != null)
                    {
                        this.childs.Add(job);
                        s.to.parents.Add(job);
                    }
                    s.to.RemoveLink(removingEvent);
                }

                foreach (SGJob s in removingEvent.parents)
                {
                    
                    SGJob job = s.from.AddNewLink(s.from, this, s.isFake);
                    if (job != null)
                    {
                        this.parents.Add(job);
                        s.from.childs.Add(job);
                    }
                    s.from.RemoveLink(removingEvent);
                }
                this.RemoveLink(removingEvent);
            }

            public SGJob AddNewLink(SGEvent from, SGEvent to, bool isFake)
            {
                if (!CheckJob(from, to))
                    return null;
                return new SGJob("", from, to, isFake); //, false);
            }

            public void RemoveLink(SGEvent deletedEvent)
            {
                for (int i = this.parents.Count - 1; i >= 0; --i)
                    if(this.parents[i].from == deletedEvent)
                            this.parents.RemoveAt(i);

                for (int i = this.childs.Count - 1; i >= 0; --i)
                    if(this.childs[i].to == deletedEvent)
                            this.childs.RemoveAt(i);

            }

            public bool CheckJob(SGEvent from, SGEvent to)
            {

                if (from == to)
                    return false;

                foreach (SGJob s in this.childs)
                    if (s.from == from && s.to == to || s.from == to && s.to == from)
                    {
                        return false;
                    }

                foreach (SGJob s in this.parents)
                    if (s.from == from && s.to == to || s.from == to && s.to == from)
                    {
                        return false;
                    }
                return true;
            }

            public SGJob FindJobInChilds(SGEvent from, SGEvent to)
            {
                foreach (SGJob j in childs)
                    if (j.from == from && j.to == to)
                        return j;

                return null;
            }

            public SGJob FindJobInParents(SGEvent from, SGEvent to)
            {
                foreach (SGJob j in parents)
                    if (j.from == from && j.to == to)
                        return j;

                return null;
            }


        }
    }
}
