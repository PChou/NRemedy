//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NRemedy
//{
//    public class ARContextPool : IDisposable
//    {
//        public static int MinCount = 10;
//        public static int MaxCount = 50;
//        public static int Increase = 5;

//        public static string ARServer { get; protected set; }
//        public static string ARUser { get; protected set; }
//        public static string ARPwd { get; protected set; }
//        public static string ARAuth { get; protected set; }
//        public static IARServerFactory Factory { get; protected set; }


//        //don't allow new,only way to init pool is CreatePool()
//        protected ARContextPool()
//        {

//        }

//        private static ARContextPool Pool = null;
//        ////sync
//        //private static PoolStatus poolStatus = new PoolStatus { status = PoolState.Initialize };

//        //sync
//        private static List<Connection> PoolConnections = new List<Connection>(MaxCount);


//        public static void CreatePool(string server, string user, string password, string authentication)
//        {
//            CreatePool(server, user, password, authentication, new ARServerDefaultFactory());
//        }


//        public static void CreatePool(string server, string user, string password, string authentication, IARServerFactory factory)
//        {
//            lock (Pool){
//                if (Pool == null)
//                {
//                    Pool = new ARContextPool();

//                    ARServer = server;
//                    ARUser = user;
//                    ARPwd = password;
//                    ARAuth = authentication;
//                    Factory = factory;

//                    for (int i = 0; i < MinCount; i++)
//                    {
//                        Connection conn = new Connection();
//                        try{
//                            conn.context = new ARLoginContext(ARServer, ARUser, ARPwd, Factory);
//                            conn.status = ConnectionState.Ready;
//                        }
//                        catch {
//                            conn.status = ConnectionState.UnInitialize;
//                        }
//                        PoolConnections.Add(conn);
//                    }



//                }

//            }
//        }

//        public static Connection GetConnection()
//        {
//            Connection con = PoolConnections.FirstOrDefault(c => c.status == ConnectionState.Ready);
//            if (con == null)
//                Increase();
//            con = PoolConnections.FirstOrDefault(c => c.status == ConnectionState.Ready);
//            if (con == null)
//                throw new ARContextPoolOutOfRange();
//            con.status = ConnectionState.Busy;
//            return con;
//        }







//        private void Increase()
//        {
 
//        }

//        private static void CheckInitialize()
//        {
//            lock (poolStatus)
//            {
//                if (poolStatus.status == PoolState.Initialize)
//                {

 
//                }
//            }
//        }

//        //distory all connection when pool dispose
//        public void Dispose()
//        {
            
//        }
//    }

//    public enum PoolState
//    {
//        Initialize,// 初始化中，该状态下服务正在按照参数初始化连接池。
//        Run,//运行中
//        Stop//停止状态
//    }

//    public enum ConnectionState
//    {
//        Ready,//空闲
//        Busy,//正在被调用
//        UnInitialize,//dispose后

//    }

//    public class PoolStatus
//    {
//        public PoolState status { get; set; }
//    }

//    public class Connection : IDisposable
//    {
//        public ARLoginContext context { get; set; }

//        public ConnectionState status { get; set; }

//        //the pool should garuntee the distory
//        public void Distory()
//        {
//            context.Dispose();
//            status = ConnectionState.UnInitialize;
//        }

//        //user code should garuntee the dispose
//        public void Dispose()
//        {
//            status = ConnectionState.Ready;
//        }
//    }


//    public class ARContextPoolOutOfRange : Exception
//    {
 
//    }
//}
