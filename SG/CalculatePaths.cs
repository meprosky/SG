using System.Collections.Generic;
using System;

namespace SG
{
    public partial class Form1
    {

        [Serializable]
        public class SG
        {

            private double _scale = 1;

            public double scale { set { _scale = value; _cR = Convert.ToInt32(_scale * _curRadius); } get { return _scale; } }

            private int _curRadius = 20;
            
            private int _cR = 20;

            public TimeSpan KritTime = new TimeSpan();
            
            public double KritTimeInHours;

            public double MinSqrtSumDisperses;

            public int IndexListWithMinDisperse;
            
            public List<double> ListOfSqrtSumDisperses = new List<double>();
            
            public List<List<SGJob>> KritPaths = null;

            public SG()
            {
            }

            public void SetRadius(int radius, List<SGEvent> sglist)
            {
                _curRadius = radius;
                _cR = Convert.ToInt32(scale * _curRadius);
                foreach (SGEvent s in sglist)
                    s.r = _cR;

            }

            public int GetRadius()
            {
                return _curRadius;
            }

            public int GetScaledRadius()
            {
                return _cR;
            }

            public void InitD()
            {
                _curRadius = 20;
                _cR = 20;
                _scale = 1.0;

            }


        }


       


        public void InOutPaths(SGEvent s)
        {
            foreach (SGJob j in s.parents) j.showBackPath = true;
            foreach (SGJob j in s.childs)  j.showForwardPath = true;
        }

        public void InOutPaths(SGJob job)
        {
            foreach (SGJob j in job.from.parents) j.showBackPath = true;
            foreach (SGJob j in job.to.childs)    j.showForwardPath = true;
        }

        public void CalcJobsParam()
        {

            Reset_SG_butNMinMax();

            if (sglist.Count == 0)
                return;

            foreach (SGEvent s in sglist)
            {
                foreach (SGJob j in s.childs)
                {
                    j.CalcOzidaemDisperce();

                    List<List<SGJob>> ll = CreatePathsForJob(j);
                    InitializeFirstJobs(ll);

                    foreach (List<SGJob> jobList in ll)
                    {
                        int jobIndex = jobList.IndexOf(j);
                        for (int i = jobIndex + 1; i < jobList.Count; i++)
                        {
                            jobList[i].CalcRNRO(jobList[i - 1]);
                        }
                    }
                }
            }

            foreach (SGEvent s in sglist)
            {
                foreach (SGJob j in s.childs)
                {
                    InitializeLastJobsAndCalcKrit(j.JobFullPaths);
                }
            }

            foreach (SGEvent s in sglist)
            {
                foreach (SGJob j in s.childs)
                {
                    foreach (List<SGJob> jobList in j.JobFullPaths)
                    {

                        for (int i = jobList.Count - 1; i > 0; i--)
                        {
                            jobList[i - 1].CalcPOPNResPResS(jobList[i]);
                        }
                    }
                }
            }
 
            
        }

        public void CalcKritPathsAndSumDisperses()
        {

            if (sglist.Count == 0)
                return;

            
            
            TimeSpan t0 = new TimeSpan();
            TimeSpan kritPathTime = new TimeSpan();
            List<SGJob> kritPath = null;
            List<List<SGJob>> kritPaths = new List<List<SGJob>>();


            foreach (SGEvent s in sglist)
                foreach (SGJob job in s.childs)
                {
                    if (job.JD.ResP == t0 && job.JD.ResS == t0)
                        job.showKritJob = true;

                    foreach (List<SGJob> lj in job.JobFullPaths)
                    {
                        TimeSpan sumRO = new TimeSpan();
                        foreach (SGJob j in lj)
                        {
                            sumRO = sumRO + j.JD.N;
                        }

                        if (sumRO > kritPathTime) //ищем крит.путь
                        {
                            kritPaths = new List<List<SGJob>>();
                            kritPathTime = sumRO;
                            kritPath = lj;
                            kritPaths.Add(lj);
                        }
                        else if (sumRO == kritPathTime) //нашли еще крит путь с таким же временем
                        {
                            bool isOn = false;
                            foreach (List<SGJob> lsg in kritPaths) //проверяем может он уже есть в списке
                            {
                                isOn = false;
                                for (int i = 0; i < lsg.Count - 1; i++)
                                {
                                    if (lsg[i] != lj[i])
                                    {
                                        isOn = true;
                                        break;
                                    }
                                }
                                if (!isOn)
                                    break;
                            }
                            if(isOn)
                                kritPaths.Add(lj);
                        }
                    }

                }


            //заполняем структуру параметров
            SGparam.KritTime = kritPathTime;
            SGparam.KritTimeInHours = kritPathTime.TotalHours;
            SGparam.KritPaths = kritPaths;
            SGparam.ListOfSqrtSumDisperses = new List<double>();

            double SqrtSumDisperses = 0.0;
            foreach (List<SGJob> lj in SGparam.KritPaths)
            {
                SqrtSumDisperses = 0.0;
                foreach (SGJob j in lj)
                {
                    j.showKritPath = true;
                    j.isOnKritPath = true;
                    SqrtSumDisperses += j.JD.Disperse;
                }

                SqrtSumDisperses = Math.Sqrt(SqrtSumDisperses);

                //корень суммы квадратов дисперсий для крит. путей
                SGparam.ListOfSqrtSumDisperses.Add(SqrtSumDisperses);

                lj[0].JD.SqrtFromSumDispersKrit = SqrtSumDisperses;
                lj[lj.Count - 1].JD.SqrtFromSumDispersKrit = SqrtSumDisperses;
            }

            double sd = SGparam.ListOfSqrtSumDisperses[0];
            for(int i = 0; i < SGparam.ListOfSqrtSumDisperses.Count; i++) //ищем мин. дисперсию
            {
                if (SGparam.ListOfSqrtSumDisperses[i] < sd)
                {
                    SGparam.IndexListWithMinDisperse = i;
                    SGparam.MinSqrtSumDisperses = sd;
                }
            }

      
        }

        public void Reset_SG_butNMinMax()
        {
            foreach (SGEvent s in sglist)
                foreach (SGJob job in s.childs)
                {
                    job.ClearJobData();
                    job.showKritPath = false;
                    job.showKritJob = false;
                    job.isOnKritPath = false;
                    job.JobFullPaths = null;
                    SGparam = new SG();
                }

        }

        public List<List<SGJob>> CreatePathsForJob(SGJob job)
        {
            List<List<SGJob>> b_paths = RecursiveBack.getreversed(job.from);
            List<List<SGJob>> f_paths = RecursiveForward.get(job.to);

            List<List<SGJob>> ll = new List<List<SGJob>>();

            if (b_paths.Count == 0 && f_paths.Count == 0)
            {
                List<SGJob> l = new List<SGJob>();
                l.Add(job);
                ll.Add(l);
            }
            else if (f_paths.Count == 0)
            {
                List<SGJob> l = new List<SGJob>();
                foreach (List<SGJob> bl in b_paths)
                {
                    List<SGJob> c = new List<SGJob>(bl);
                    c.Add(job);
                    ll.Add(c);
                }
            }
            else if (b_paths.Count == 0)
            {
                List<SGJob> l = new List<SGJob>();
                l.Add(job);
                b_paths.Add(l);
                foreach (List<SGJob> bl in b_paths)
                {
                    foreach (List<SGJob> fl in f_paths)
                    {
                        List<SGJob> c = new List<SGJob>(bl);
                        c.AddRange(fl);
                        ll.Add(c);
                    }
                }
            }
            else
            {
                foreach (List<SGJob> bl in b_paths)
                {
                    foreach (List<SGJob> fl in f_paths)
                    {
                        List<SGJob> c = new List<SGJob>(bl);
                        c.Add(job);
                        c.AddRange(fl);
                        ll.Add(c);
                    }
                }
            }

            job.JobFullPaths = ll;

            return ll;
        }

        public void InitializeFirstJobs(List<List<SGJob>> ll)
        {
            foreach (List<SGJob> lj in ll)
            {
                JobData jd = lj[0].JD;
                jd.RN = new TimeSpan();
                jd.RO = jd.RN + jd.N;
                jd.Krit = jd.RO;
            }
        }

        public void InitializeLastJobsAndCalcKrit(List<List<SGJob>> ll)
        {
            foreach (List<SGJob> jobList in ll)
            {
                int lastIndex = jobList.Count - 1;

                TimeSpan t = new TimeSpan();
                foreach (SGJob kj in jobList[lastIndex].to.parents)
                    if (kj.JD.RO > t)
                        t = kj.JD.RO;

                foreach (SGJob kj in jobList[lastIndex].to.parents)
                {
                    JobData jd = kj.JD;
                    jd.Krit = t;
                    jd.PO = t;
                    jd.PN = t - kj.JD.N;
                    jd.ResP = kj.JD.PO - kj.JD.RO;
                    jd.ResS = t - kj.JD.RO;
                }
            }
        }

    }

}