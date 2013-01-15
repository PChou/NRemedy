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
    /// 
    /// </summary>
    public class ARLoginContext : IDisposable
    {
        public string Server { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Authentication { get; set; }

        /// <summary>
        /// 可使用该接口来访问BMC.ARSystem的API
        /// </summary>
        public IARServer ServerInstance { get; set; }

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
            Init(server,user,password,string.Empty);
        }

        /// <summary>
        /// Init the server
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        public void Init(string server, string user, string password)
        {
            Init(server,user,password,string.Empty);
        }

        /// <summary>
        /// Init the server
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <param name="authentication">Authentication</param>
        public void Init(string server, string user, string password, string authentication)
        {
            Server = server;
            User = user;
            Password = password;
            Authentication = authentication;
        }

        /// <summary>
        /// LogIn IARServer
        /// </summary>
        /// <param name="server">ARServer，使用NRemedy内建的ARServer传入</param>
        public void Login(IARServer server)
        {
            if (LoginStatus != ARLoginStatus.Success || ServerInstance == null)
            {
                try
                {
                    server.Login(Server, User, Password, Authentication);
                    LoginStatus = ARLoginStatus.Success;
                    ServerInstance = server;
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
