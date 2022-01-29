using System.Collections.Generic;
using System;

namespace SG
{
    public partial class Form1
    {
        public static class RecursiveBack
        {
            private static List<List<SGJob>> paths;
            private static List<List<SGJob>> cyclic;
            private static bool isEnd = false;
            private static bool isCyclic = false;
            private static int position = 0;
            private static SGEvent sge;
            private static bool isCreatePaths = true;

            public static List<List<SGJob>> get(SGEvent sgevent)
            {
                
                sge = sgevent;
                isCreatePaths = true;
                CalcBack();
                return paths;
            }

            public static void calc(SGEvent sgevent)
            {
                sge = sgevent;
                isCreatePaths = false;
                CalcBack();
            }

            public static List<List<SGJob>> getreversed(SGEvent sgevent)
            {
                sge = sgevent;
                isCreatePaths = true;
                CalcBack();

                foreach (List<SGJob> sgl in paths)
                {
                    sgl.Reverse();
                }

                

                return paths;
            }

            private static void CalcBack()
            {
                paths = new List<List<SGJob>>(); 
                cyclic = new List<List<SGJob>>();

                foreach (SGJob j in sge.parents)
                {
                    isEnd = true;
                    isCyclic = false;

                    j.inCycle = false;


                    List<SGJob> p = new List<SGJob>();
                    p.Add(j);

                    //j.showBackPath = true;
                    j.showBackPath = true;
                    RecursionBack(j, p);
                }

            }

            private static List<SGJob> RecursionBack(SGJob j, List<SGJob> p)
            {
                foreach (SGJob k in j.from.parents)
                {
                    isEnd = true;
                    isCyclic = false;

                    position = p.IndexOf(j);

                    for (int i = p.Count - 1; i > position; i--)
                        p[i] = null;

                    p.RemoveAll(RemoveNullValue);

                    if (p.Contains(k))       // //циклический путь
                    {
                        isEnd = true;
                        isCyclic = true;

                        int startCycl = p.IndexOf(k);
                        int endCycl = p.IndexOf(j);

                        List<SGJob> cycllist = p.GetRange(startCycl, endCycl - startCycl + 1);

                        foreach (SGJob jc in cycllist)
                        {
                            jc.inCycle = true;

                            //jc.showInCycle = true;

                            jc.showBackPath = true;

                        }

                        //cyclicPathsBak.Add(cycllist);
                        //pathsBack.Add(cycllist);

                        break;
                    }

                    k.inCycle = false;

                    p.Add(k);

                    //k.showBackPath = true;
                    k.showBackPath = true;
                    p = RecursionBack(k, p);
                    
                }
                
                
                if (isEnd && !isCyclic)
                    {
                        if (isCreatePaths)
                        {
                            paths.Add(new List<SGJob>(p));
                        }
                        isEnd = false;
                    }
                

                return p;
               

            }

            private static bool RemoveNullValue(SGJob job)
            {
                if (job == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static class RecursiveForward
        {
            private static List<List<SGJob>> paths;
            private static List<List<SGJob>> cyclic;
            private static bool isEnd = false;
            private static bool isCyclic = false;
            private static int position = 0;
            private static SGEvent sge;
            private static bool isCreatePaths = true;

            public static List<List<SGJob>> get(SGEvent sgevent)
            {

                sge = sgevent;
                isCreatePaths = true;
                CalcForward();
                return paths;
            }

            public static void calc(SGEvent sgevent)
            {
                sge = sgevent;
                isCreatePaths = false;
                CalcForward();
            }

            public static List<List<SGJob>> getreversed(SGEvent sgevent)
            {
                sge = sgevent;
                isCreatePaths = true;
                CalcForward();

                foreach (List<SGJob> sgl in paths)
                {
                    sgl.Reverse();
                }

                return paths;
            }

            private static void CalcForward()
            {
                paths = new List<List<SGJob>>();
                cyclic = new List<List<SGJob>>();

                foreach (SGJob j in sge.childs)
                {
                    isEnd = true;
                    isCyclic = false;

                    j.inCycle = false;


                    List<SGJob> p = new List<SGJob>();
                    p.Add(j);

                    j.showForwardPath = true;
                    //j.backRef.j.showForwardPath = true;
                    RecursionForward(j, p);
                }

            }

            private static List<SGJob> RecursionForward(SGJob j, List<SGJob> p)
            {
                foreach (SGJob k in j.to.childs)
                {
                    isEnd = true;
                    isCyclic = false;

                    position = p.IndexOf(j);

                    for (int i = p.Count - 1; i > position; i--)
                        p[i] = null;

                    p.RemoveAll(RemoveNullValue);

                    if (p.Contains(k))       // //циклический путь
                    {
                        isEnd = true;
                        isCyclic = true;

                        int startCycl = p.IndexOf(k);
                        int endCycl = p.IndexOf(j);

                        List<SGJob> cycllist = p.GetRange(startCycl, endCycl - startCycl + 1);

                        foreach (SGJob jc in cycllist)
                        {
                            jc.inCycle = true;
                            //jc.showInCycle = true;
                            jc.showForwardPath = true;
                        }

                        //cyclic.Add(cycllist);
                        //paths.Add(cycllist);

                        break;
                    }

                    k.inCycle = false;
                    p.Add(k);

                    k.showForwardPath = true;
                    //k.backRef.showBackPath = true;
                    p = RecursionForward(k, p);

                }


                if (isEnd && !isCyclic)
                {
                    if (isCreatePaths)
                    {
                        paths.Add(new List<SGJob>(p));
                    }
                    isEnd = false;
                }


                return p;


            }

            private static bool RemoveNullValue(SGJob job)
            {
                if (job == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static class RecursiveCyclic
        {

            public static bool isEnd = false;
            public static bool isCyclic = false;
            public static int position = 0;
            public static List<List<SGJob>> cyclicPaths = new List<List<SGJob>>();

            public static void mark(List<SGEvent> sglist)
            {
                foreach (SGEvent s in sglist)
                {
                    foreach (SGJob j in s.childs)
                    {
                        j.inCycle = false;
                    }
                }

                foreach (SGEvent s in sglist)
                {
                    foreach (SGJob j in s.childs)
                    {
                        List<SGJob> p = new List<SGJob>();
                        p.Add(j);
                        RecurciveCirclePath(j, p);
                    }
                }
            }


            public static List<List<SGJob>> get(List<SGEvent> sglist)
            {
                mark(sglist);
                return cyclicPaths;
            }

            public static List<SGJob> RecurciveCirclePath(SGJob j, List<SGJob> p)
            {
                foreach (SGJob k in j.to.childs)
                {
                    position = p.IndexOf(j);
                    for (int i = p.Count - 1; i > position; i--)
                        p[i] = null;
                    p.RemoveAll(Removenull);

                    if (p.Contains(k))       //циклический путь
                    {
                        int startCycl = p.IndexOf(k);
                        int endCycl = p.IndexOf(j);
                        List<SGJob> cycllist = p.GetRange(startCycl, endCycl - startCycl + 1);

                        foreach (SGJob jc in cycllist)
                        {
                            jc.inCycle = true;
                            //jc.showInCycle = true;
                        }

                        cyclicPaths.Add(cycllist);

                        break;
                    }
                    p.Add(k);
                    p = RecurciveCirclePath(k, p);
                }
                return p;
            }


        }

        private static bool Removenull(SGJob job)
        {
            if (job == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}