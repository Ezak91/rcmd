using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace rcmd
{
    public class rcmd
    {
        /// <summary>
        /// The connection options
        /// </summary>
        private ConnectionOptions connectionOptions;
        /// <summary>
        /// The management scope
        /// </summary>
        private ManagementScope managementScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="rcmd"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="enablePrivileges">if set to <c>true</c> [enable privileges].</param>
        public rcmd(String username, String password, String domain, String hostName, bool enablePrivileges)
        {
            setConnectionOptions(username, password, domain, enablePrivileges);
            setManagementScope(hostName);
            connectScope();
        }

        /// <summary>
        /// Sets the connection options.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="enablePrivileges">if set to <c>true</c> [enable privileges].</param>
        public void setConnectionOptions(String username, String password, String domain, bool enablePrivileges)
        {
            this.connectionOptions = new ConnectionOptions();
            this.connectionOptions.Username = username;
            this.connectionOptions.Password = password;
            this.connectionOptions.EnablePrivileges = enablePrivileges;
            this.connectionOptions.Authority = "ntlmdomain:" + domain;

        }

        /// <summary>
        /// Sets the management scope.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        public void setManagementScope(String hostName)
        {
            string host = @"\\" + hostName + @"\root\CIMV2";
            this.managementScope = new ManagementScope(host, connectionOptions);
        }

        /// <summary>
        /// Connects the scope.
        /// </summary>
        public void connectScope()
        {
            managementScope.Connect();
        }


        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns></returns>
        public int executeCommand(String command, String currentDirectory)
        {
            ObjectGetOptions objectOption = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
            ManagementPath spoolerPath = new ManagementPath("Win32_Process");
            ManagementClass servicesManager = new ManagementClass(managementScope, spoolerPath, objectOption);
            ManagementBaseObject inParams = servicesManager.GetMethodParameters("Create");

            inParams["CommandLine"] = command;
            if (currentDirectory != null)
            {
                inParams["CurrentDirectory"] = currentDirectory;
            }

            ManagementBaseObject outParams = servicesManager.InvokeMethod("Create", inParams, null);

            int processID = Convert.ToInt16(outParams["processId"]);

            return processID;
        }

        /// <summary>
        /// Determines whether [is process running] [the specified process identifier].
        /// </summary>
        /// <param name="processID">The process identifier.</param>
        /// <returns></returns>
        public Boolean isProcessRunning(int processID)
        {
            var query = new ObjectQuery(string.Format("Select * From Win32_Process Where ProcessID = '{0}'", processID));
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(managementScope, query);
            var processList = searcher.Get();

            if (processList.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
