using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;



namespace SG
{
    public partial class Form1 : Form
    {
        
        
        public int eventID = 0;

        int timerInterval = 100;

        public string fileName = null;
        public string tempfileName = @"d:\temp.sgs";

        bool isToolTip = false;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public SG SGparam = new SG();

        public SolidBrush brushGrid = new SolidBrush(Color.Gray);
        public static bool snapToGrid = true;
        public static int snapGridStep = 10;
        public static int snapGridStep2 = snapGridStep / 2;
        
        public BufferedGraphicsContext context;
        public BufferedGraphics buf;
        public Color backPanelColor = Color.Ivory;

        public Color button_SG_backColor;

        public int mouse_click_X = 0;
        public int mouse_click_Y = 0;

        public int mouse_cur_X = 0;
        public int mouse_cur_Y = 0;

        public int mouse_prev_X = 0;
        public int mouse_prev_Y = 0;

        public int mouse_down_X = 0;
        public int mouse_down_Y = 0;

        public int dddx = 0;
        public int dddy = 0;

        public int dx = 0;
        public int dy = 0;

        
        public bool drawon = false;
        public bool isCurved = false;
        public bool dragCurvePoint = false;
        public bool isMouseLeaveGraphicPanel = true;
        public bool isMoveAll = false;
        public bool isShowTooltip = true;
        
        public int dgvRowClicked = -1;
        public int dgvColClicked = -1;

        public CurvePoint curvePoint = null;

        public SGEvent selectedEvent = new SGEvent();
        public SGJob selectedJob = new SGJob();

        SGEvent ev;

        
        List<SGEvent> sglist = new List<SGEvent>();
        
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateDatagridView(sglist);

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(panel1.Width + 1, panel1.Height + 1);
            buf = context.Allocate(panel1.CreateGraphics(),
                new Rectangle(0, 0, panel1.Width, panel1.Height));

            buf.Graphics.SmoothingMode = SmoothingMode.Default;
            
            sglist.Add(new SGEvent(null, eventID, "Работа", 100, 100, false, SGparam));

            SelectEvent(sglist[0]);

            InitializeJobsGraphic();

            this.Text = "Сетевой график - Новый";

            timer.Interval = timerInterval;
            timer.Tick += new EventHandler(timer_Tick);
            // включаем таймер
            timer.Enabled = true;


            //timer.Start();


            MyEvent += AddJobToDataGridView;

        }

        int cc = 0;

        public void timer_Tick(object sender, EventArgs e)
        {
            if (!isShowTooltip)
                return;
            
            if (mouse_cur_X != mouse_prev_X && mouse_cur_Y != mouse_prev_Y)
            {
                mouse_prev_X = mouse_cur_X;
                mouse_prev_Y = mouse_cur_Y;
                isToolTip = false;
                return;
            }

            if (!isToolTip)
            {
                string SGKritTime = FormatTimeSpan(SGparam.KritTime); 
                string toolTipText = "";
                
                for (int i = sglist.Count - 1; i >= 0; --i)
                {
                    toolTip1.ToolTipIcon = ToolTipIcon.Info;
                    
                    SGEvent s = sglist[i];
                    SGJob job = null;

                    //toolTip1.ToolTipTitle = "Сетевой график";

                    //toolTipText = "Критическое время: " + SGKritTime;

                    

                    if (FindEventObject(s, mouse_cur_X, mouse_cur_Y))
                    {
                        string Krit = (s.parents.Count > 0) ? FormatTimeSpan(s.parents[0].JD.Krit) : "00:00";
                        string PO = (s.parents.Count > 0) ? FormatTimeSpan(s.parents[0].JD.PO) : "00:00";

                        toolTip1.ToolTipTitle = "Событие № " + s._eventID.ToString();

                        toolTipText = 
                            "Ранн. начало след. работ: " + Krit + "\n" +
                            "Позд. оконч. пред. работ: " + PO + "\n" +
                            "Крит. время cетевого графика: " + SGKritTime;

                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);

                        //toolTip1.SetToolTip(panel1, toolTipText);

                        break;
                    }
                    else if (FindLineObject(s, mouse_cur_X, mouse_cur_Y, ref job))
                    {
                        toolTip1.ToolTipTitle = "Работа " + job._fto + ". " +  job.JD._jobName;

                        if (job.isOnKritPath)
                            toolTip1.ToolTipIcon = ToolTipIcon.Warning;

                        toolTipText =
                            "Нормативн. пр-ть: " + FormatTimeSpan(job.JD.N) + "\n" +
                            "Мин.-Макс. пр-ть: " + FormatTimeSpan(job.JD.Min) + " - " + FormatTimeSpan(job.JD.Max) + "\n" +
                            "Ожидаемое время: " + FormatTimeSpan(job.JD.Ozidaem) + "\n" +
                            "Ранние сроки\n" +
                            "Начала работ: " + FormatTimeSpan(job.JD.RN) + ". Окончания работ: " + FormatTimeSpan(job.JD.RO) + "\n" +
                            "Поздние сроки\n" +
                            "Начала работ: " + FormatTimeSpan(job.JD.PN) + ". Окончания работ: " + FormatTimeSpan(job.JD.PO) + "\n" +
                            "Резерв времени полный : " + FormatTimeSpan(job.JD.ResS) + "\n" +
                            "Резерв времени свобод.: " + FormatTimeSpan(job.JD.ResP) + "\n" +
                            "Крит. время сетевого графика: " + SGKritTime;
                        
                        //toolTip1.SetToolTip(panel1, toolTipText);

                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
                        
                        break;
                    }
                }


                toolTip1.SetToolTip(panel1, toolTipText);
                
                isToolTip = true;

            }

            mouse_prev_X = mouse_cur_X;
            mouse_prev_Y = mouse_cur_Y;

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_down_X = e.X;
            mouse_down_Y = e.Y;

            isShowTooltip = false;

            if (isMoveAll)
                return;


            drawon = true;

            UnselectAll();
            //ClearColor();

            for(int i = sglist.Count - 1; i >= 0; i--)
            {

                SGEvent s = sglist[i];

                if (FindEventObject(s, e.X, e.Y))
                {

                    dx = s.X - e.X; //смещение мыши относительно центра события
                    dy = s.Y - e.Y;


                    if (e.Button == MouseButtons.Left)
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            eventID++;
                            SGEvent snew = new SGEvent(s, eventID, "Работа", e.X + dx, e.Y + dy, false, SGparam);
                            sglist.Add(snew);
                            ev = snew;
                            ev.drag = true;
                            snew.Draw(buf);

                            SelectEvent(snew);
                            SelectJob(s.FindJobInChilds(s, snew));

                            UpdateDatagridView();

                            return;
                        }
                        else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                        {
                            eventID++;
                            SGEvent snew = new SGEvent(s, eventID, "Работа", e.X + dx, e.Y + dy, true, SGparam);
                            sglist.Add(snew);
                            ev = snew;
                            ev.drag = true;
                            snew.Draw(buf);
                            UpdateDatagridView();
                            return;
                        }
                        else
                        {
                            
                            s.drag = true;
                            ev = s;

                            SelectEvent(ev);
                            UnselectJob(selectedJob);

                            UpdateDatagridView();

                            return;
                        }

                }
                else if(FindCurvePoint(s, ref curvePoint, e.X, e.Y))
                {
                    dragCurvePoint = true;
                }

            }

            

            





        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            mouse_cur_X = e.X;
            mouse_cur_Y = e.Y;
            
            if (e.Button == MouseButtons.Left)
            {
                drawon = true;

                if (ev != null)
                {
                    if (ev.drag)
                    {
                        ev.Move(e.X + dx, e.Y + dy, buf);
                    }
                }
                
                if (dragCurvePoint && curvePoint != null)
                {
                    isCurved = true;
                    curvePoint.Move(e.X, e.Y);
                }
                
                if (isMoveAll)
                {
                    foreach (SGEvent s in sglist)
                    {
                        s.Move(s.X + e.X - mouse_down_X, s.Y + e.Y - mouse_down_Y, buf);

                        foreach (SGJob j in s.childs)
                            foreach (CurvePoint cp in j.curvePoints)
                                cp.Move(cp.x + e.X - mouse_down_X, cp.y + e.Y - mouse_down_Y);
                    }

                    mouse_down_X = e.X;
                    mouse_down_Y = e.Y;
                }
                
                RedrawAll();
            }




        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            
            drawon = false;
            //isMoveAll = false;
            //this.panel1.Cursor = Cursors.Default;

            if (isMoveAll)
            {
                if (sglist.Count == 0)
                    return;
                
                Point ofset = PopravkeToSnapGrid(new Point(sglist[0].X, sglist[0].Y), snapGridStep);

                foreach (SGEvent s in sglist)
                {
                    s.Move(s.X + ofset.X, s.Y + ofset.Y, buf);
                    foreach (SGJob j in s.childs)
                        foreach (CurvePoint cp in j.curvePoints)
                            cp.Move(cp.x + ofset.X, cp.y + ofset.Y);
                }
                

                RedrawAll();
                return;
            }

            isShowTooltip = true;

            if (dragCurvePoint)
            {
                Point ofset = PopravkeToSnapGrid(new Point(e.X, e.Y), snapGridStep);
                curvePoint.Move(e.X + ofset.X, e.Y + ofset.Y);
                dragCurvePoint = false;
                RedrawAll();
                return;
            }

            if (ev == null)
                return;

            if (!ev.drag)
            {
                return;
            }
            else
            {
                Point ofset = PopravkeToSnapGrid(new Point(e.X + dx, e.Y + dy), snapGridStep);
                ev.Move(e.X + dx + ofset.X, e.Y + dy + ofset.Y, buf);
            }

            
            ev.drag = false;

            for (int i = sglist.Count - 1; i >= 0; --i)
            {
                SGEvent s = sglist[i];
                if (s != ev && FindEventObject(s, e.X, e.Y))
                {
                    s.ConnectNearNode(ev);

                    if (selectedJob.to == ev || selectedJob.from == ev)
                    {
                        SGJob j1 = s.FindJobInChilds(s, selectedJob.to);
                        SGJob j2 = s.FindJobInChilds(s, selectedJob.from);
                        SGJob j3 = s.FindJobInParents(selectedJob.to, s);
                        SGJob j4 = s.FindJobInParents(selectedJob.from, s);

                        UnselectJob(selectedJob);

                        if (j1 != null) SelectJob(j1);
                        else if (j2 != null) SelectJob(j2);
                        else if (j3 != null) SelectJob(j3.from.FindJobInChilds(j3.from, s));
                        else if (j4 != null) SelectJob(j4.from.FindJobInChilds(j4.from, s));
                    }
                    sglist.Remove(ev);
                    
                    UnselectAll();
                    SelectEvent(s);
                    //CalcCirclePath();

                    UpdateDatagridView();
                    RedrawAll();
                    return;
                }
            }
            RedrawAll();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            mouse_click_X = e.X;
            mouse_click_Y = e.Y;
            
            if (isMoveAll)
            {
                if (e.Button == MouseButtons.Right)
                    this.contextMenuStrip1.Show((Control)sender, e.X, e.Y);

                return;
            }
            
           
            bool isFinded = false;

            UnselectAll();
            //ClearColor();

            if (e.Button == MouseButtons.Right)
            {
                if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                {

                }
                else
                {
                    for (int i = sglist.Count - 1; i >= 0; --i)
                    {
                        SGEvent s = sglist[i];
                        SGJob job = null;

                        if (FindEventObject(s, e.X, e.Y))
                        {
                            contextMenuStrip1.Show((Control)sender, e.X, e.Y);
                            List<SGEvent> l = new List<SGEvent>();
                            l.Add(s);
                            contextMenuStrip1.Items[0].Tag = l;
                            isFinded = true;
                            break;

                        }
                        else if (FindLineObject(s, e.X, e.Y, ref job))
                        {
                            contextMenuStrip1.Show((Control)sender, e.X, e.Y);
                            contextMenuStrip1.Items[0].Tag = job;
                            isFinded = true;
                            break;
                        }

                    }

                    if (!isFinded)
                    {
                        contextMenuStrip1.Show((Control)sender, e.X, e.Y);
                        contextMenuStrip1.Items[0].Tag = "new";
                    }

                }



                if (e.Button == MouseButtons.Right && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                }

            }
            else if (e.Button == MouseButtons.Left)
            {
                if (isCurved)
                {
                    isCurved = false;
                    return;
                }


                for (int i = sglist.Count - 1; i >= 0; --i)
                {
                    SGEvent s = sglist[i];
                    SGJob job = null;
                    if (FindEventObject(s, e.X, e.Y))
                    {
                        SelectEvent(s);
                        InOutPaths(s);

                        //CalcBackPath(s);
                        //CalcForwardPath(s);

                        RedrawAll();
                        UpdateDatagridView2(dataGridView1, true);
                        return;
                    }
                    else if (FindLineObject(s, e.X, e.Y, ref job))
                    {
                        UnselectAll();
                        SelectJob(job);
                        InOutPaths(job);
                        RedrawAll();
                        UpdateDatagridView2(dataGridView1, true);
                        return;
                    }
                }
                UnselectJob(selectedJob);
                RedrawAll();
                UpdateDatagridView();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            RedrawAll();
        }

        private void RedrawAll()
        {
            context = BufferedGraphicsManager.Current;
            
            
            context.MaximumBuffer = new Size(panel1.Width + 1, panel1.Height + 1);
            buf = context.Allocate(panel1.CreateGraphics(),
                new Rectangle(0, 0, panel1.Width, panel1.Height));
            
            buf.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            buf.Graphics.Clear(backPanelColor);



            for (int i = 0; i < panel1.Width; i += snapGridStep)
                for (int j = 0; j < panel1.Height; j += snapGridStep)
                    buf.Graphics.FillRectangle(brushGrid, i, j, 1, 1);


            foreach (SGEvent s in sglist)
                s.Draw(buf);

            buf.Render();



        }

        private void RadiusBig_Item_Click(object sender, EventArgs e)
        {
            среднийToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            SGparam.SetRadius(30, sglist);
            RedrawAll();
            
           
        }

        private void RadiusMedium_Item_Click(object sender, EventArgs e)
        {
            menuItemBig.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            SGparam.SetRadius(20, sglist);
            RedrawAll();
        }

        private void RadiusSmall_Item_Click(object sender, EventArgs e)
        {
            menuItemBig.Checked = false;
            среднийToolStripMenuItem.Checked = false;
            SGparam.SetRadius(10, sglist);
            RedrawAll();
        }
               
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            
            SGJob job = (SGJob)dataGridView1.Rows[row].Tag;
            
            if (job == null)
                return;

            JobData jd = job.JD;

            

            FieldInfo f = (FieldInfo)dataGridView1.Columns[col].Tag;

            string s = dataGridView1.Rows[row].Cells[col].Value.ToString();

            TimeSpan ts;
            if (f != null)
            {
                if (f.FieldType == typeof(TimeSpan))
                {
                    if (StringToTimeSpan(s, out ts))
                    {
                        f.SetValue(jd, ts);
                        dataGridView1.Rows[row].Cells[col].Value = FormatTimeSpan(ts);

                    }
                    else
                    {
                        MessageBox.Show("Ошибка ввода временного интервала\nДопустимые значения (пример 12:30, 2д.10:30)");
                        f.SetValue(jd, new TimeSpan());
                        dataGridView1.Rows[row].Cells[col].Value = FormatTimeSpan(ts);
                    }
                }
                else if(f.FieldType == typeof(string))
                    f.SetValue(jd, s);
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvRowClicked == e.RowIndex)
                return;
            else
                dgvRowClicked = e.RowIndex;

            if (e.RowIndex == -1)
                return;

            SGJob job = (SGJob)dataGridView1.Rows[e.RowIndex].Tag;

            if (job == null)
                return;

            int row = e.RowIndex;
            int col = e.ColumnIndex;

            UnselectAll();
            ClearColor();
            InOutPaths(job);
            SelectJob(job);

            UpdateDatagridView2(dataGridView1, false);

            dataGridView1.CurrentCell.Selected = false;

            dataGridView1.Rows[row].Cells[col].Selected = true;

            RedrawAll();


        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = ((DataGridView)sender);
            DataGridViewCell curentCell = dgv.CurrentCell;
            int row = curentCell.RowIndex;
            SGJob job = (SGJob)dgv.Rows[row].Tag;

            if (job == null)
                return;

            UnselectAll();
            ClearColor();
            InOutPaths(job);
            SelectJob(job);
            UpdateDatagridView2(dgv, false);

            //dataGridView1.CurrentCell.Selected = false;

            //dataGridView1.Rows[row].Cells[col].Selected = true;

            //UpdateJobDGW(job);
            
            
            RedrawAll();


           

        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            isMouseLeaveGraphicPanel = true;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            isMouseLeaveGraphicPanel = false;
        }
        
        private void ViewLinks_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;
            if (o == null)
                return;
            System.Type type = o.GetType();

            if (type == typeof(List<SGEvent>))
            {
                SGEvent s = ((List<SGEvent>)(contextMenuStrip1.Items[0].Tag))[0];
                if (s != null)
                {
                    UnselectAll();
                    SelectEvent(s);
                    RecursiveBack.calc(s);
                    //CalcBackPath(s);
                    RecursiveForward.calc(s);
                    //CalcForwardPath(s);
                    UpdateDatagridView();
                    RedrawAll();
                }
            }
            else if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                if (job != null)
                {
                    UnselectAll();
                    SelectJob(job);
                    RecursiveBack.calc(job.from);
                    RecursiveForward.calc(job.to);
                    UpdateDatagridView();
                    RedrawAll();
                }
            }
        }

        private void ViewCyclicPaths_ContextMenuItem_Click(object sender, EventArgs e)
        {
            UnselectAll();
            RecursiveCyclic.mark(sglist);
            RedrawAll();
        }

        private void FakeOnOff_ContextMenuItem_Click(object sender, EventArgs e)
        {

            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            System.Type type = o.GetType();

            if (type == typeof(List<SGEvent>))
            {
                SGEvent sge = ((List<SGEvent>)(contextMenuStrip1.Items[0].Tag))[0];
                if (sge != null)
                    sge.isFakeEvent = (sge.isFakeEvent) ? false : true;
            }
            else if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)(contextMenuStrip1.Items[0].Tag);

                if (job != null)
                {
                    job.isFake = (job.isFake) ? false : true;
                }
            }
            RedrawAll();
        }
        
        private void AddEvent_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            if (o is String)
            {
                if ((string)o == "new")
                {
                    eventID++;
                    SGEvent snew = new SGEvent(null, eventID, "", mouse_click_X, mouse_click_Y, false, SGparam);
                    sglist.Add(snew);

                    SelectEvent(snew);
                    UnselectJob(selectedJob);

                    UpdateDatagridView();
                    RedrawAll();
                    return;
                }
            }


            System.Type type = o.GetType();

            if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                if (job != null)
                {
                    eventID++;

                    job.from.childs.Remove(job);
                    job.to.parents.Remove(job);

                    SGEvent snew = new SGEvent(job.from, eventID, "", mouse_click_X, mouse_click_Y, job.isFake, SGparam);
                    sglist.Add(snew);

                    SGJob j = new SGJob("", snew, job.to, job.isFake); //, true);
                    snew.childs.Add(j);
                    j.to.parents.Add(j);

                    SelectEvent(snew);
                    InOutPaths(snew);
                    UnselectJob(selectedJob);
                    UpdateDatagridView();
                    RedrawAll();
                    return;
                }

            }

        }

        private void Delete_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            System.Type type = o.GetType();

            if (type == typeof(List<SGEvent>))
            {
                SGEvent s = ((List<SGEvent>)(contextMenuStrip1.Items[0].Tag))[0];
                if (s != null)
                {
                    sglist.Remove(s);
                    foreach (SGEvent sev in sglist)
                    {
                        sev.RemoveLink(s);
                    }
                    UnselectAll();
                    //CalcCirclePath();
                    UpdateDatagridView();
                    RedrawAll();
                    return;
                }
            }
            else if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                if (job != null)
                {
                    job.from.childs.Remove(job);
                    job.to.parents.Remove(job);

                    UnselectAll();
                    RecursiveCyclic.mark(sglist);
                    UpdateDatagridView();
                    RedrawAll();
                    return;
                }
            }





        }
       
        private void AddCurvePoint_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            System.Type type = o.GetType();

            if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                if (job.curvePoints.Count == 0)
                    new CurvePoint(mouse_click_X, mouse_click_Y, job);
                else
                    job.curvePoints.Add(new CurvePoint(mouse_click_X, mouse_click_Y, job));

                RedrawAll();
            }
        }
        
        private void DeleteCurvePoint_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            System.Type type = o.GetType();

            if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                CurvePoint cp = null;

                if (FindCurvePointInJob(job, mouse_click_X, mouse_click_Y, ref cp))
                {
                    job.RemoveCurvePoint(cp);
                    RedrawAll();
                }
            }

        }

        private void DeleteAllCurvePoints_ContextMenuItem_Click(object sender, EventArgs e)
        {
            object o = contextMenuStrip1.Items[0].Tag;

            if (o == null)
                return;

            System.Type type = o.GetType();

            if (type == typeof(SGJob))
            {
                SGJob job = (SGJob)contextMenuStrip1.Items[0].Tag;

                if (job.curvePoints.Count != 0)
                    job.curvePoints = new List<CurvePoint>();

                RedrawAll();
            }

        }

        private void FileNew_Click(object sender, EventArgs e)
        {
            sglist.Clear();
            eventID = -1;
            this.Text = "Сетевой график - Новый";
            SGparam = new SG();
            textBox_DirTime.Text = "00д.00:00";
            textBox_Ver.Text = "";
            fileName = null;
            UpdateDatagridView();
            RedrawAll();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            fileName = OpenSGSfromFile();

            if (fileName != null)
            {
                this.Text = "Сетевой график - " + fileName;
                label_KritTime.Text = FormatTimeSpan(SGparam.KritTime);
                textBox_DirTime.Text = "00д.00:00";
                textBox_Ver.Text = "";
                UnselectAll();
                RedrawAll();
                UpdateDatagridView();
            }
            else
            {
                //this.Text = "Сетевой график - Новая схема";
            }

           

            
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            UnselectAll();
            if (fileName == null)
            {
                fileName = SaveAsSGStoFile();
                this.Text = (fileName != null) ? "Сетевой график - " + fileName : "Сетевой график - Новая схема";

            }
            else
            {
                if (WriteSGStoFile(fileName))
                {
                    MessageBox.Show("Схема сохранене в файле " + fileName);
                    this.Text = "Сетевой график - " + fileName;
                }
            }

           
        }

        private void FileSaveAs_Click(object sender, EventArgs e)
        {

            UnselectAll();
            string fileSaveAsName = SaveAsSGStoFile();
            if (fileSaveAsName != null)
            {
                fileName = fileSaveAsName;
                this.Text = "Сетевой график - " + fileName;
            }


        }
        
        private void FileExportToExcel_Click(object sender, EventArgs e)
        {
            WriteToExcelFile("FFF");
        }

        private void EditCopyDGViewToClipBoard_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
            CopyDataGridViewToClipboard(dataGridView1);
        }

        private void button_CalcSG_Click(object sender, EventArgs e)
        {
            CalcJobsParam();

            CalcKritPathsAndSumDisperses();

            label_KritTime.Text = FormatTimeSpan(SGparam.KritTime);

            UnselectAll();
            UpdateDatagridView();
            RedrawAll();

        }

        private void button_ResetSG_Click(object sender, EventArgs e)
        {
            Reset_SG_butNMinMax();
            UpdateDatagridView();
            //UpdateDatagridView2(dataGridView1, false);
            RedrawAll();
        }

        private void buttonVer_Click(object sender, EventArgs e)
        {



            TimeSpan t;
            if (!StringToTimeSpan(textBox_DirTime.Text, out t))
                return;

            if (SGparam.KritPaths == null)
                return;

            double dirTime = t.TotalHours;
            double d = NormRasp(dirTime, SGparam.KritTimeInHours, SGparam.MinSqrtSumDisperses);
            double d2 = NormRasp2(dirTime, SGparam.KritTimeInHours, SGparam.MinSqrtSumDisperses);

            textBox_Ver.Text = (d * 100).ToString("0") + " %"; // + ", " + d2.ToString("0.000");


        }
        
        private void button_toFile_Click(object sender, EventArgs e)
        {
            UnselectAll();
            RecursiveCyclic.mark(sglist);
            WriteSGStoFile(tempfileName);
            RedrawAll();
        }

        private void button_fromFile_Click(object sender, EventArgs e)
        {

            if (ReadSGSfromFile(tempfileName))
            {
                this.Text = "Сетевой график - " + fileName;
                label_KritTime.Text = FormatTimeSpan(SGparam.KritTime);
                textBox_DirTime.Text = "00д.00:00";
                textBox_Ver.Text = "";
                
                if (sglist.Count == 0)
                    eventID = -1;
                else
                    eventID = sglist[sglist.Count - 1]._eventID;

                //SGparam.InitD();


                RedrawAll();
                UpdateDatagridView();
            }
        }

        private void MoveAll_ContextMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MoveAll_ContextMenuItem.Checked)
            {
                this.MoveAll_ContextMenuItem.Checked = false;
                isMoveAll = false;
                panel1.Cursor = Cursors.Default;
                isShowTooltip = true;
                
                foreach (ToolStripItem it in contextMenuStrip1.Items)
                    it.Enabled = true;

            }
            else
            {
                this.MoveAll_ContextMenuItem.Checked = true;
                isMoveAll = true;
                this.panel1.Cursor = Cursors.Hand;
                isShowTooltip = false;
                
                foreach (ToolStripItem it in contextMenuStrip1.Items)
                    it.Enabled = false;

                MoveAll_ContextMenuItem.Enabled = true;
                
            }
        }

        public Point PopravkeToSnapGrid(Point p, int GridStep)
        {
            int GridStep2 = GridStep / 2;

            Point d = new Point(p.X % GridStep, p.Y % GridStep);
            return new Point((d.X > GridStep2) ? GridStep - d.X : -d.X,
                (d.Y > GridStep2) ? GridStep - d.Y : -d.Y);
        }
       
        private void Scale(double scale)
        {

            if (SGparam.scale < 0.00001)
                SGparam.scale = 1.0;

            double m = scale / SGparam.scale;

            SGparam.scale = scale;
            
            int x0 = sglist[0].X, x1 = 0, y0 = sglist[0].Y, y1 = 0;
            
            foreach (SGEvent s in sglist)
            {
                if (s.X < x0) x0 = s.X;
                if (s.X > x1) x1 = s.X;
                if (s.Y < y0) y0 = s.Y;
                if (s.Y > y1) y1 = s.Y;
            }

            int centX = x0 + (x1 - x0) / 2;
            int centY = y0 + (y1 - y0) / 2;

            int offsX = centX - Convert.ToInt32(centX * m);
            int offsY = centY - Convert.ToInt32(centY * m);

            foreach (SGEvent s in sglist)
            {
                s.X = offsX + Convert.ToInt32(s.X * m);//  +CenterDX;
                s.Y = offsY + Convert.ToInt32(s.Y * m);// + CenterDY;
                s.r = Convert.ToInt32(s.r * m);

                foreach (SGJob j in s.childs)
                {
                    foreach (CurvePoint cp in j.curvePoints)
                    {
                        cp.x = offsX + Convert.ToInt32(cp.x * m);// + CenterDX;
                        cp.y = offsY + Convert.ToInt32(cp.y * m);// + CenterDY;
                    }

                }

                s.Move(s.X, s.Y, buf);

            }

            RedrawAll();
        }

        private void M08_MenuItem_Click(object sender, EventArgs e)
        {
            Scale(0.8);

        }

        private void M07_MenuItem_Click(object sender, EventArgs e)
        {
            Scale(0.7);
        }

        private void M1_MenuItem_Click(object sender, EventArgs e)
        {
            Scale(1.0);
        }

        private void M05_MenuItem_Click(object sender, EventArgs e)
        {
            Scale(0.5);
        }

        private void M03_MenuItem_Click(object sender, EventArgs e)
        {
            Scale(0.3);
        }

        private void SnapAllToGrid_Click(object sender, EventArgs e)
        {
            Point ofset;
            foreach (SGEvent s in sglist)
            {

                ofset = PopravkeToSnapGrid(new Point(s.X, s.Y), snapGridStep);
                
                s.Move(s.X + ofset.X, s.Y + ofset.Y, buf);
                
                foreach (SGJob j in s.childs)
                    foreach (CurvePoint cp in j.curvePoints)
                    {
                        ofset = PopravkeToSnapGrid(new Point(cp.x, cp.y), snapGridStep);
                        cp.Move(cp.x + ofset.X, cp.y + ofset.Y);
                    }
            }

        }

      




       
      












    }
}