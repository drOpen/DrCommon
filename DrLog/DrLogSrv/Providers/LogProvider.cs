using System;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public abstract class LogProvider: ILogProvider
    {

        public LogProvider(DDNode config)
        {
            Config = config;
        }

        public abstract void Write(DDNode msg);
        public abstract void Dispose();

        public abstract string Name { get; }

        virtual public DDNode Config {get; set;}

        virtual public DDType ConfigNodeType
        {
            get { return Config.Type ; }
        }

        #region basic attributes for messages
        /// <summary>
        /// creation time of the message 
        /// </summary>
        public const string AttDateTime = "DateTime";
        /// <summary>
        /// body of message
        /// </summary>
        public const string AttBody = "Body";
        /// <summary>
        /// Log level of message
        /// </summary>
        public const string AttLevel = "Level";
        /// <summary>
        /// Who created the message 
        /// </summary>
        public const string AttSource = "Source";
        /// <summary>
        /// The list of providers who will be read this message. by default all providers
        /// </summary>
        public const string AttProviders = "Providers";
        /// <summary>
        /// The list of recipients who will be receive this message. by default all recipients
        /// </summary>
        public const string AttRecipients = "Recipients";
        #endregion basic attributes for messages


    }
}
