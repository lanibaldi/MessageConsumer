using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Transactions;
using log4net;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Reflection;

namespace MessageConsumer
{
    /// <summary>
    /// The main entry for the application
    /// </summary>
    class Program
    {
        const string ServiceDisplayName = "Message Consumer";
        const string ServiceDescription = "Message consumer service.";
    	  const string ServiceName = "ConsumerService";

        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger(typeof(Program).Assembly.GetName().Name);

     		if (args.Length > 0)
     		{
     			if (args[0] == "--install")
     			{
                    logger.Info("Trying to install the service");
     				try
     				{
     					Install();
                        logger.Info("Service succesfully installed");
     				}
     				catch (Exception e)
     				{
                        logger.Error("Installation failed:" + e.Message);
     				}
     				return;
     
     			}
     			if (args[0] == "--uninstall")
     			{
                    logger.Info("Trying to remove the service");
     				try
     				{
     					Uninstall();
                        logger.Info("Service succesfully removed");
     				}
     				catch (Exception e)
     				{
                        logger.Error("Installation failed:" + e.Message);
     				}
     				return;
     			}
     		}

     		if (!Environment.UserInteractive)
     		{
     			logger.Info("Starting as a service...");
     			logger.Info(ServiceDisplayName);
     			logger.Info(ServiceDescription);
     			ServiceBase[] ServicesToRun;
     			ServicesToRun = new ServiceBase[] 
     			{ 
     				new ConsumerService() 
     			};
     			ServiceBase.Run(ServicesToRun);
     		}
     		else
     		{
     			logger.Info("Starting as a console...");
     			logger.Info(ServiceDisplayName);
     			logger.Info(ServiceDescription);
     			var s = new ConsumerService();
     			s.GetType().GetMethod("OnStart"
     									, BindingFlags.Instance | BindingFlags.NonPublic
     									).Invoke(s, new object[] { args });
     			Console.WriteLine("Press enter to exit the console mode");
     			Console.ReadLine();
     			s.GetType().GetMethod("OnStop"
     										, BindingFlags.Instance | BindingFlags.NonPublic
     										).Invoke(s, null);
     		}
        }

        #region Utilities
        private static void Install()
        {
     	    ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();
     	    ProcesServiceInstaller.Account = ServiceAccount.LocalSystem;
    	    //ProcesServiceInstaller.Username = "<<username>>";
    	    //ProcesServiceInstaller.Password = "<<password>>";
    
    	    ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
    	    InstallContext Context = new System.Configuration.Install.InstallContext();
    	    String path = String.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
    	    String[] cmdline = { path };
    
    	    Context = new System.Configuration.Install.InstallContext("", cmdline);
    	    ServiceInstallerObj.Context = Context;
    	    ServiceInstallerObj.DisplayName = ServiceDisplayName;
    	    ServiceInstallerObj.Description = ServiceDescription;
    	    ServiceInstallerObj.ServiceName = ServiceName;
    	    ServiceInstallerObj.StartType = ServiceStartMode.Automatic;
    	    ServiceInstallerObj.Parent = ProcesServiceInstaller;
    
    	    System.Collections.Specialized.ListDictionary state = new System.Collections.Specialized.ListDictionary();
    	    //ServiceInstallerObj.Uninstall(null);
    	    ServiceInstallerObj.Install(state);
        }
        private static void Uninstall()
        {
    	    ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();
    	    ProcesServiceInstaller.Account = ServiceAccount.LocalSystem;
    
    	    ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
    	    InstallContext Context = new System.Configuration.Install.InstallContext();
    	    String path = String.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
    	    String[] cmdline = { path };
    
    	    Context = new System.Configuration.Install.InstallContext("", cmdline);
    	    ServiceInstallerObj.Context = Context;
    	    ServiceInstallerObj.DisplayName = ServiceDisplayName;
    	    ServiceInstallerObj.Description = ServiceDescription;
    	    ServiceInstallerObj.ServiceName = ServiceName;
    	    ServiceInstallerObj.StartType = ServiceStartMode.Automatic;
    	    ServiceInstallerObj.Parent = ProcesServiceInstaller;
    	    ServiceInstallerObj.Uninstall(null);
        }
        #endregion
    }
}
