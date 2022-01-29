using System;
using System.Collections.Generic;

namespace SG
{
    public partial class Form1
    {
        public static Random random = new Random();

        [Serializable()]
        public class JobData
        {
            //public int jobID;

            public SGJob job;

            private int _userJobID;
            public int userJobID { set { _userJobID = value; } get { return _userJobID; } }

            public string _fto;
            public string fto { set { _fto = value; } get { return _fto; } }

            public string _jobName;
            public string jobName { set { _jobName = value; } get { return _jobName; } }

            public DateTime _tStart;
            public DateTime tStart { set { _tStart = value; } get { return _tStart; } }

            public TimeSpan N;

            public TimeSpan RN;
            public TimeSpan tStartJobEarly { set { RN = value; } get { return RN; } }

            public TimeSpan RO;
            public TimeSpan tStopJobEarly { set { RO = value; } get { return RO; } }

            public TimeSpan PN;
            public TimeSpan tStartJobLate { set { PN = value; } get { return PN; } }

            public TimeSpan PO;
            public TimeSpan tStopJobLate { set { PO = value; } get { return PO; } }

            public TimeSpan ResP;
            public TimeSpan tReservFull { set { ResP = value; } get { return ResP; } }

            public TimeSpan ResS;
            public TimeSpan tReservFree { set { ResS = value; } get { return ResS; } }

            public TimeSpan Krit = new TimeSpan();
            public TimeSpan tKrit { set { Krit = value; } get { return Krit; } }

            public TimeSpan Min;

            public TimeSpan Max;
            
            public TimeSpan Verojat;

            public double Ver;

            public TimeSpan Ozidaem;

            public double Disperse;

            public double SqrtFromSumDispersKrit;
            
            public TimeSpan Disperse2;

            public List<int> pathKrit = new List<int>();

            public JobData(SGJob job, string jobName)
            {
                this.job = job;
                this.jobName = jobName; // "Работа № " + jobID.ToString();
                //Random tr = new Random();
                int r = random.Next(1, 20);
                this.N = new TimeSpan(0, r, 0);
            }

            public JobData(SGJob job)
            {
                this.job = job;
                this.jobName = job.JD.jobName;
                this.N = job.JD.N;
                this.Min = job.JD.Min;
                this.Max = job.JD.Max;

            }
           



        }



    }
}