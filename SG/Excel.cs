using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing.Drawing2D;

namespace SG
{
    public partial class Form1 : Form
    {
        public string SaveToExcel()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xls files (*.xls)|*.xls";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;

                if (WriteToExcelFile(fileName))
                    return fileName;
                else
                    return null;
            }
            return null;
        }


        public bool WriteToExcelFile(string fileName)
        {
            Excel.Application xlApp  = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook    xlWbk  = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet   xlWsht = (Excel.Worksheet)xlWbk.Worksheets.get_Item(1);
            Excel.Range       xlRange;

            xlWsht.Cells.Font.Size = 8;

            int x0 =  sglist[0].X, x1 = 0, y0 = sglist[0].Y, y1 = 0;
            int offs = 0; // 4 * sglist[0].r;
            foreach (SGEvent s in sglist)
            {
                if (s.X < x0) x0 = s.X;
                if (s.X > x1) x1 = s.X;
                if (s.Y < y0) y0 = s.Y;
                if (s.Y > y1) y1 = s.Y;
            }

            int W = x0 + x1 - x0 + x0;
            int H = y0 + y1 - y0 + y0;

            
            Bitmap bmp = new Bitmap(W, H); // , buf.Graphics) ;
            Graphics gr = Graphics.FromImage(bmp);
            
            BufferedGraphicsContext context1;
            BufferedGraphics buf1;
            context1 = BufferedGraphicsManager.Current;
            context1.MaximumBuffer = new Size(W, H);
            buf1 = context1.Allocate(gr, new Rectangle(0, 0, W, H));
            buf1.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            buf1.Graphics.Clear(backPanelColor);

            for (int i = 0; i < W; i += snapGridStep)
                for (int j = 0; j < H; j += snapGridStep)
                    buf1.Graphics.FillRectangle(brushGrid, i, j, 1, 1);

            foreach (SGEvent s in sglist)
                s.Draw(buf1);
            
            buf1.Render(gr);

            
            //Image img = bmp;

            Clipboard.SetImage(bmp);


            int r = 25;

            xlWsht.Cells[1, 4] = "ÑÅÒÅÂÎÉ ÃÐÀÔÈÊ ÏÐÎÈÇÂÎÄÑÒÂÀ ÐÀÁÎÒ";
            
            ((Excel.Range)xlWsht.Cells[1, 4]).Font.Size = 12;
            ((Excel.Range)xlWsht.Cells[1, 4]).Font.Bold = true;
            //((Excel.Range)xlWsht.Cells[1, 4]). = Color.Red;

            
            for (int j = 0; j < dataGridView1.ColumnCount; j++)
            {

                xlRange = ((Excel.Range)xlWsht.Cells[1 + r, j + 1]);
                xlRange.NumberFormat = "@";
                xlRange.Font.Bold = true;
                xlWsht.Cells[1 + r, j + 1] = dataGridView1.Columns[j].HeaderText;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlTop;
                xlRange.WrapText = true;
                xlRange.ColumnWidth = 7;

                //xlRange.EntireColumn.AutoFit(); 

                /*
                Type ft;
                if (dataGridView1.Columns[j].Tag != null)
                    ft = ((FieldInfo)dataGridView1.Columns[j].Tag).FieldType;
                else
                    ft = typeof(string);
                */

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {

                    /*
                    if (ft == typeof(string))
                        xlRange.NumberFormat = "@";
                    else if (ft == typeof(double))
                        //xlRange.NumberFormat = "0.00";
                        xlRange.NumberFormatLocal = "0.00";
                    else if (ft == typeof(TimeSpan))
                        xlRange.NumberFormat = @"÷:ìì";
                    */

                    xlRange = ((Excel.Range)xlWsht.Cells[i + 2 + r, j + 1]);
                    xlRange.NumberFormat = "@";
                    xlWsht.Cells[i + 2 + r, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                    xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;
                    xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlTop;
                }
            }

            xlRange = ((Excel.Range)xlWsht.Cells[1 + r, 2]);
            xlRange.ColumnWidth = 20;

            xlRange = xlWsht.get_Range("B3", "C5");
            
            xlWsht.Paste(xlRange, false);
            Excel.Shape sh = xlWsht.Shapes.Item("Picture 1");
            //ScaleWidth 0.68, msoFalse, msoScaleFromTopLeft
            //Selection.ShapeRange.ScaleHeight 0.68, msoFalse, msoScaleFromBottomRight
            sh.ScaleHeight(0.5f, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
            sh.ScaleWidth(0.5f, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
            
            

            xlApp.Visible = true;
            xlApp.UserControl = true;  


            return true;
        
        }


        
    
    
    }
}