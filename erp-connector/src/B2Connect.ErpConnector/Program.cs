using System;
using Topshelf;
using NLog;

namespace B2Connect.ErpConnector
{
    /// <summary>
    /// Entry point for the ERP Connector service.
    /// Can run as console application or Windows Service via TopShelf.
    /// </summary>
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            // Configure NLog
            ConfigureLogging();

            Logger.Info("B2Connect ERP Connector starting...");

            try
            {
                var exitCode = HostFactory.Run(x =>
                {
                    x.Service<ErpConnectorService>(s =>
                    {
                        s.ConstructUsing(name => new ErpConnectorService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });

                    x.RunAsLocalSystem();
                    x.SetServiceName("B2ConnectErpConnector");
                    x.SetDisplayName("B2Connect ERP Connector");
                    x.SetDescription("Connects B2Connect to enventa Trade ERP system via HTTP API");

                    x.StartAutomatically();
                    x.EnableServiceRecovery(r =>
                    {
                        r.RestartService(1); // Restart after 1 minute on first failure
                        r.RestartService(5); // Restart after 5 minutes on second failure
                        r.RestartService(10); // Restart after 10 minutes on subsequent failures
                        r.SetResetPeriod(1); // Reset failure count after 1 day
                    });

                    x.UseNLog();
                });

                return (int)exitCode;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Fatal error during startup");
                return -1;
            }
        }

        private static void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Console target
            var consoleTarget = new NLog.Targets.ConsoleTarget("console")
            {
                Layout = "${longdate} ${level:uppercase=true:padding=-5} ${logger:shortName=true} - ${message} ${exception:format=tostring}"
            };

            // File target
            var fileTarget = new NLog.Targets.FileTarget("file")
            {
                FileName = "${basedir}/logs/erp-connector-${shortdate}.log",
                Layout = "${longdate} ${level:uppercase=true:padding=-5} ${logger} - ${message} ${exception:format=tostring}",
                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
                MaxArchiveFiles = 30
            };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);

            LogManager.Configuration = config;
        }
    }
}
