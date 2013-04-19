//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using ARNative;

namespace NRemedy
{
    /// <summary>
    /// AR登录会话对象，可以使用using关键字实例化，ARProxy和ARSet两大入口都需要该对象作为会话来访问AR
    /// since Transaction API impelement in this class, so it becomes thread unsafed.
    /// </summary>
    public class ARLoginContext : IDisposable
    {
        /// <summary>
        /// AR Server IP
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 操作API的用户名
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 操作API的用户名对应的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 登录时所用的认证字串
        /// TODO:未实现
        /// </summary>
        public string Authentication { get; set; }

        /// <summary>
        /// 可使用该接口来访问原生封装的API
        /// </summary>
        public IARServer ServerInstance { get; set; }

        /// <summary>
        /// 登录状态
        /// </summary>
        public ARLoginStatus LoginStatus { get; set; }

        /// <summary>
        /// Constructor 1
        /// </summary>
        public ARLoginContext()
        { 
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        public ARLoginContext(string server, string user, string password)
        { 
            Init(server,user,password,string.Empty,new ARServerDefaultFactory());
        }

        /// <summary>
        /// Constructor 3
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <param name="factory">IARServer的工厂接口</param>
        public ARLoginContext(string server, string user, string password,IARServerFactory factory)
        {
            Init(server, user, password, string.Empty,factory);
        }


        /// <summary>
        /// Init the server
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <param name="authentication">Authentication</param>
        /// <param name="factory">IARServer的工厂接口</param>
        public void Init(string server, string user, string password, string authentication, IARServerFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            Server = server;
            User = user;
            Password = password;
            Authentication = authentication;
            ServerInstance = factory.CreateARServer();
            Login();
        }

        /// <summary>
        /// LogIn IARServer
        /// </summary>
        /// <param name="server">ARServer，使用NRemedy内建的ARServer传入</param>
        public void Login()
        {
            if (LoginStatus != ARLoginStatus.Success)
            {
                try
                {
                    ServerInstance.Login(Server, User, Password, Authentication);
                    LoginStatus = ARLoginStatus.Success;
                }
                catch (ARException are)
                {
                    LoginStatus = ARLoginStatus.Failed;
                    throw are;
                }
                catch (Exception ex)
                {
                    LoginStatus = ARLoginStatus.Failed;
                    throw ex;
                }
            }
        }

        public void TransactionBegin()
        {
            ServerInstance.BeginBulkEntryTransaction();
        }

        public ARTransactionResult TransactionCommit()
        {
            return ServerInstance.CommitBulkEntryTransaction();
        }

        public void TransactionCancel()
        {
            ServerInstance.CancelBulkEntryTransaction();
        }



        ~ARLoginContext()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (ServerInstance != null)
                ServerInstance.Logout();
        }
    }

}
