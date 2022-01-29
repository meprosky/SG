using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SG
{
    public partial class Form1
    {
        public bool WriteSGStoFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, SGparam);
                    bin.Serialize(stream, sglist);
                    stream.Close();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка записи файла " + fileName);
                return false;
            }
            
            return true;
        }

        public bool ReadSGSfromFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    SGparam = (SG)bin.Deserialize(stream);
                    sglist = (List<SGEvent>)bin.Deserialize(stream);
                    stream.Close();
                }
            }
            catch // (IOException)
            {
                MessageBox.Show("Ошибка чтения файла " + fileName);
                return false;
            }
            
            return true;
        }

        public string OpenSGSfromFile()
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "sgs files (*.sgs)|*.sgs";         //|All files (*.*)|*.*";
            ofd.RestoreDirectory = true;

            string fileName = null;
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;

                if (ReadSGSfromFile(fileName))
                    return fileName;
                else                
                    return null;
            }
            
            return null;
        }

        public string SaveAsSGStoFile()
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "sgs files (*.sgs)|*.sgs";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;

                if (WriteSGStoFile(fileName))
                    return fileName;
                else
                    return null;
            }

            return null;
        }




       
    }
}