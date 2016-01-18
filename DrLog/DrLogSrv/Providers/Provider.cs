using System;
using DrOpen.DrCommon.DrData;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    public abstract class Provider : IProvider
    {

        #region Provider
        /// <summary>
        /// create provider with specified configuration
        /// </summary>
        /// <param name="config">provider configuration</param>
        public Provider(DDNode config)
            : this(config, false)
        { }
        /// <summary>
        /// create provider with specified configuration and merge with default configuration for this provider
        /// </summary>
        /// <param name="config">provider configuration</param>
        /// <param name="mergeWithDefault">specify true for merge with default configuration for this provider</param>
        public Provider(DDNode config, bool mergeWithDefault)
        {
            if (config == null) throw new ApplicationException("The provider configuration cannot be null.");
            if (config.Type != Type) throw new ApplicationException(string.Format("The type '{0}' of provider configuration is not supported. The expected type is '{1}'.", config.Type, Type));
            if ((mergeWithDefault) || (DefaultConfig != null))
            {
                Config = DefaultConfig;
                Config.Merge(config, DDNode.DDNODE_MERGE_OPTION.ALL, ResolveConflict.OVERWRITE);
            }
            else
            {
                Config = config;
            }
            RebuildConfiguration();
        }
        #endregion Provider

        public abstract void Write(DDNode msg);
        public abstract void Dispose();

        /// <summary>
        /// Provider name. By default is equls to provider Type
        /// </summary>
        virtual public string Name
        {
            get { return Type.ToString(); }
        }
        /// <summary>
        /// curent configuration
        /// </summary>
        virtual public DDNode Config
        {
            get;
            set;
        }
        /// <summary>
        /// default configuration for this provider
        /// </summary>
        public abstract DDNode DefaultConfig { get; }

        virtual public DDType Type
        {
            get { return this.GetType().Name; }
        }

        virtual public DrLogSrv.LogLevel Level
        { get; set; }
        virtual public DrLogSrv.LogExceptionLevel ExceptionLevel
        { get; set; }
        virtual public void RebuildConfiguration()
        {
            this.Level = (DrLogSrv.LogLevel)Enum.Parse(typeof(DrLogSrv.LogLevel), Config.Attributes.GetValue(DrLogProviderConst.AttLevel, DefaultLevel), true);
            this.ExceptionLevel = (DrLogSrv.LogExceptionLevel)Enum.Parse(typeof(DrLogSrv.LogExceptionLevel), Config.Attributes.GetValue(DrLogProviderConst.AttExceptionLevel, DrLogSrv.LogExceptionLevel.ALL), true);
        }
        /// <summary>
        /// default log level
        /// </summary>
        public const DrLogSrv.LogLevel DefaultLevel = LogLevel.ALL;
        /// <summary>
        /// default log exception level
        /// </summary>
        public const DrLogSrv.LogExceptionLevel DefaultExceptionLevel = LogExceptionLevel.ALL;

        /// <summary>
        /// returns configuration provider node with common attributes: 'AttLevel=All', 'AttExceptionLevel=All'
        /// </summary>
        /// <returns></returns>
        public static DDNode GetCommonConfig(DDType type)
        {
            var n = new DDNode(type);
            n.Attributes.Add(DrLogProviderConst.AttLevel, DefaultLevel.ToString());
            n.Attributes.Add(DrLogProviderConst.AttExceptionLevel, DefaultExceptionLevel.ToString());
            return n;
        }
    }
}
