using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SG
{
    partial class Form1
    {
        public void CreateDatagridView(List<SGEvent> sglist)
        {

            //SGJob job = new SGJob();
            Type t = typeof(JobData);
            FieldInfo f;


            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.ColumnHeadersDefaultCellStyle.Font.Size = 4;
            dataGridView1.RowHeadersWidth = 20;

            int i;
            i = dataGridView1.Columns.Add("nPP", "π œ/œ");
            dataGridView1.Columns[i].Width = 30;
            dataGridView1.Columns[i].ReadOnly = true;

            i = dataGridView1.Columns.Add("jobName", "Õ¿»Ã≈ÕŒ¬¿Õ»≈ –¿¡Œ“€");
            dataGridView1.Columns[i].Width = 330;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("_jobName");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("kod", " Œƒ");
            dataGridView1.Columns[i].Width = 50;
            dataGridView1.Columns[i].ReadOnly = true;
            //f = t.GetField("_jobName");
            //dataGridView1.Columns[i].Tag = f;


            i = dataGridView1.Columns.Add("prevJob", "œ–≈ƒ. –¿¡.");
            dataGridView1.Columns[i].Width = 70;
            dataGridView1.Columns[i].ReadOnly = true;


            i = dataGridView1.Columns.Add("nextJob", "—À≈ƒ. –¿¡.");
            dataGridView1.Columns[i].Width = 70;
            dataGridView1.Columns[i].ReadOnly = true;
            dataGridView1.Columns[i].Tag = null;

            i = dataGridView1.Columns.Add("tNormativ", "ÕŒ–Ã. ¬–≈Ãﬂ");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("N");
            dataGridView1.Columns[i].Tag = f;


            i = dataGridView1.Columns.Add("Min", "Ã»Õ. œ–-“‹");
            dataGridView1.Columns[i].Width = 60;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("Min");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("Max", "Ã¿ —. œ–-“‹");
            dataGridView1.Columns[i].Width = 60;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("Max");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("Ozidaem", "Œ∆»ƒ. œ–-“‹");
            dataGridView1.Columns[i].Width = 60;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("Ozidaem");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("Disperse", "ƒ»—œ≈–- —»ﬂ");
            dataGridView1.Columns[i].Width = 60;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("Disperse");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("Verojat", "¬≈–Œﬂ“- ÕŒ—“‹");
            dataGridView1.Columns[i].Width = 60;
            dataGridView1.Columns[i].ReadOnly = false;
            f = t.GetField("Ver");
            dataGridView1.Columns[i].Tag = f;
            
            i = dataGridView1.Columns.Add("tKrit", " –»“. ¬–≈Ãﬂ");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("Krit");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("tStartJobEarly", "–¿ÕÕ.—–Œ  Õ¿◊¿À¿ –¿¡Œ“");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("RN");
            dataGridView1.Columns[i].Tag = f;


            i = dataGridView1.Columns.Add("tStopJobEarly", "–¿ÕÕ.—–Œ  «¿¬≈–ÿ≈Õ»ﬂ –¿¡Œ“");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("RO");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("tStartJobLate", "œŒ«ƒ.—–Œ  Õ¿◊¿À¿ –¿¡Œ“");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("PN");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("tStopJobLate", "œŒ«ƒ.—–Œ  «¿¬≈–ÿ≈Õ»ﬂ –¿¡Œ“");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("PO");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("tReservFull", "–≈«≈–¬ ¬–≈Ã≈Õ» œŒÀÕ€…");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("ResP");
            dataGridView1.Columns[i].Tag = f;

            i = dataGridView1.Columns.Add("tReservFree", "–≈«≈–¬ ¬–≈Ã≈Õ» —¬Œ¡ŒƒÕ€…");
            dataGridView1.Columns[i].Width = 80;
            dataGridView1.Columns[i].ReadOnly = true;
            f = t.GetField("ResS");
            dataGridView1.Columns[i].Tag = f;



            //TimeSpan ts;
            //StringToTimeSpan("", out ts);

        }

        public void AddJobToDataGridView(object sender, EventArgs e)
        {
            SGJob job = (SGJob)sender;
            JobData jd = job.JD;

            string prev = "";
            string next = "";

            int i = 0;
            foreach (SGJob j in job.from.parents)
            {
                
                string s = j.from._eventID.ToString() + "-" +
                    j.to._eventID.ToString();
                prev += (s + ((i < job.from.parents.Count - 1)? "," : ""));
                i++;

            }

            i = 0;
            foreach (SGJob j in job.to.childs)
            {
                string s = j.from._eventID.ToString() + "-" +
                    j.to._eventID.ToString();

                next += (s + ((i < job.to.childs.Count - 1) ? "," : ""));
                i++;

            }



           

            int indexRow = dataGridView1.Rows.Add(
                count_row.ToString(),
                jd._jobName,
                " " + job._fto,
                " " + prev,
                " " + next,
                FormatTimeSpan(jd.N),
                FormatTimeSpan(jd.Min),
                FormatTimeSpan(jd.Max),
                FormatTimeSpan(jd.Ozidaem),
                jd.Disperse.ToString("0.###"),
                jd.Ver.ToString("0.##"),
                FormatTimeSpan(jd.Krit),
                // "1‰10:50", "Ú", "Ú",
                FormatTimeSpan(jd.RN), FormatTimeSpan(jd.RO),
                FormatTimeSpan(jd.PN), FormatTimeSpan(jd.PO),
                FormatTimeSpan(jd.ResP), FormatTimeSpan(jd.ResS));

            dataGridView1.Rows[indexRow].Tag = job;


            if (job.selected)
            {
                dataGridView1.Rows[indexRow].DefaultCellStyle.BackColor = Color.Blue;
                dataGridView1.Rows[indexRow].DefaultCellStyle.ForeColor = Color.White;
            }
            else if (job.showForwardPath)
                dataGridView1.Rows[indexRow].DefaultCellStyle.BackColor = Color.LightBlue;
            else if (job.showBackPath)
                dataGridView1.Rows[indexRow].DefaultCellStyle.BackColor = Color.Gold;
            else if (
                job.from.selected)  dataGridView1.Rows[indexRow].DefaultCellStyle.BackColor = Color.LightBlue;
            else if
                (job.to.selected)  dataGridView1.Rows[indexRow].DefaultCellStyle.BackColor = Color.Gold;

           

        }

        public string FormatTimeSpan(TimeSpan ts)
        {
            if(ts.Days != 0)
                return string.Format("{0:D}‰.{1:D2}:{2:D2}", ts.Days, ts.Hours, ts.Minutes);
            else
                return string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes);
        }

        public bool StringToTimeSpan(string s, out TimeSpan ts)
        {
            string[] dhm =  s.Split( new Char[] {'‰', '.', ':'}, StringSplitOptions.RemoveEmptyEntries);
            int d = 0, h = 0, m = 0;

            ts = new TimeSpan();
            try
            {
                if (dhm.Length == 3)
            {
                d = Convert.ToInt32(dhm[0]);
                h = Convert.ToInt32(dhm[1]);
                m = Convert.ToInt32(dhm[2]);
                ts = new TimeSpan(d, h, m, 0);
                return true;
            }
            else if (dhm.Length == 2)
            {
                h = Convert.ToInt32(dhm[0]);
                m = Convert.ToInt32(dhm[1]);
                ts = new TimeSpan(0, h, m, 0);
                return true;
            }
            }
            catch
            {
                ts = new TimeSpan();
                return false;
            }

            return false;
        
        }

        int count_row = 1;

        public void UpdateDatagridView()
        {
            dataGridView1.Rows.Clear();
            count_row = 1;

            foreach (SGEvent s in sglist)
            {
                foreach (SGJob j in s.childs)
                {
                    AddJobToDataGridView(j, null);
                    count_row++;
                }
            }

        }

        public void UpdateDatagridView2(DataGridView dgv, bool isClickFromScheme)
        {
            //dgv.CurrentCell.Selected = false;
            

            foreach (DataGridViewRow r in dgv.Rows)
            {
                SGJob job = (SGJob)r.Tag;
                DataGridViewCellStyle s = r.DefaultCellStyle;
                s.ForeColor = Color.Black;
                if (job.selected)
                {
                    if (isClickFromScheme)
                    {
                        dgv.ClearSelection();
                        r.Cells[0].Selected = true;
                        dgv.CurrentCell = r.Cells[0];
                    }

                    s.BackColor = Color.Blue;
                    s.ForeColor = Color.White;
                }
                else if (job.showForwardPath) s.BackColor = Color.LightBlue;
                else if (job.showBackPath)    s.BackColor = Color.Gold;
                else if (job.from.selected)   s.BackColor = Color.LightBlue;
                else if (job.to.selected)     s.BackColor = Color.Gold;
                else s.BackColor = Color.White;
               
            }

        }

        public void ClearColor()
        {

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                r.DefaultCellStyle.BackColor = Color.White;
                r.DefaultCellStyle.ForeColor = Color.Black;
            }

        }

        /*
        public void UpdDetails(SGJob job)
        {
            JobData jd = job.JD;
            
            textBox_userJobID.Text = jd.userJobID.ToString();
            textBox_fto.Text = job._fto;
            textBox_jobName.Text = jd.jobName;
            textBox_tNormativ.Text      = FormatTimeSpan(jd.N);
            textBox_tStartJobEarly.Text = FormatTimeSpan(jd.RN);
            textBox_tStopJobEarly.Text = FormatTimeSpan(jd.RO);
            textBox_tStartJobLate.Text = FormatTimeSpan(jd.PN);
            textBox_tStopJobLate.Text = FormatTimeSpan(jd.PO);
            textBox_tReservFull.Text = FormatTimeSpan(jd.ResP);
            textBox_tReservFree.Text = FormatTimeSpan(jd.ResS);
            textBox_tKrit.Text = FormatTimeSpan(jd.Krit);
        }
        */

        public void CopyDataGridViewToClipboard(DataGridView dgv)
        {
            if (dgv.GetClipboardContent() != null)
            {
                Clipboard.SetDataObject(dgv.GetClipboardContent());
                Clipboard.GetData(DataFormats.Text);
                IDataObject dt = Clipboard.GetDataObject();
                if (dt.GetDataPresent(typeof(string)))
                {
                    string tb = (string)(dt.GetData(typeof(string)));
                    //ASCIIEncoding ee = new ASCIIEncoding();
                    Encoding myEncoding = Encoding.GetEncoding(1251);
                    byte[] abyDataStr = new byte[tb.Length];
                    abyDataStr = myEncoding.GetBytes(tb);
                    Clipboard.SetDataObject(myEncoding.GetString(abyDataStr));
                }

            }
        }

    
    
    
    
    
    }






}
