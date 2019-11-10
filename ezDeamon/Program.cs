using ezDeamon.biz;
using ezDeamon.cls;
using System;

namespace ezDeamon
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.NonStaticMethod(args);
        }

        public void NonStaticMethod(string[] args)
        {
            NonStaticMain obj = new NonStaticMain();
            if (obj != null)
            {
                obj.Initialize(args);
            }
            else
                return;
        }
    }

    class NonStaticMain
    {
        LogControl Log;

        public bool IsTest = false;

        public void Initialize(string[] args)
        {
            Log = new LogControl();

            Start(args);
        }

        private void Start(string[] args)
        {
            Log.WriteLog("PROGRAM START");

            if (args.Length == 0)
            {
                DmsStatus st = new DmsStatus();
                st.StatusManage();

                DmsPartFile fi = new DmsPartFile();
                fi.FileRemove();
                fi.EoDisable();
            }
            else
            {
                switch (args[0])
                {
                    case "s":
                        DmsStatus st = new DmsStatus();
                        st.StatusManage();
                        break;

                    case "f":
                        DmsPartFile fi = new DmsPartFile();
                        fi.FileRemove();
                        break;

                    case "e":
                        DmsPartFile e = new DmsPartFile();
                        e.EoDisable();
                        break;

                }

            }
            


            Log.WriteLog("PROGRAM END");


            GC.Collect();
        }
    }
}
