/*
  Provider.cs -- abstract provider for DrLog 1.1.0, January 24, 2016
 
  Copyright (c) 2013-2016 Kudryashov Andrey aka Dr
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation would be
      appreciated but is not required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>
 */

using System;
using DrOpen.DrData.DrDataObject;
using DrOpen.DrCommon.DrLog.DrLogSrv;
using DrOpen.DrCommon.DrLog.DrLogSrv.Res;

namespace DrOpen.DrCommon.DrLog.DrLogSrv.Providers
{
    /// <summary>
    /// abstract provider for DrLog
    /// </summary>
    public abstract class Provider : IProvider//, IDDTypeSupport
    {

        #region Provider
        /// <summary>
        /// create provider with specified configuration and build it
        /// </summary>
        /// <param name="config">provider configuration</param>
        public Provider(DDNode config)
            : this(config, false)
        { }
        /// <summary>
        /// create provider with specified configuration and merge with default configuration for this provider then build configuration
        /// </summary>
        /// <param name="config">provider configuration</param>
        /// <param name="mergeWithDefault">specify true for merge with default configuration for this provider</param>
        public Provider(DDNode config, bool mergeWithDefault)
        {
            if (config == null) throw new ApplicationException(Msg.PROVIDER_CONF_CANNOT_NULL);
            if (config.Type != Type) throw new ApplicationException(string.Format(Msg.PROVIDER_UNSUPPORTED_TYPE, config.Type, Type));
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
            get { return this.GetDDType().Name; }
        }
        /// <summary>
        /// returns the log level filter for current provider
        /// </summary>
        virtual public DrLogSrv.LogLevel Level
        { get; set; }
        /// <summary>
        /// returns the log exception level filter for current provider
        /// </summary>
        virtual public DrLogSrv.LogExceptionLevel ExceptionLevel
        { get; set; }
        /// <summary>
        /// Update settings to current config
        /// </summary>
        virtual public void RebuildConfiguration()
        {
            this.Level = (DrLogSrv.LogLevel)Enum.Parse(typeof(DrLogSrv.LogLevel), Config.Attributes.GetValue(SchemaProvider.AttLevel, DefaultLevel), true);
            this.ExceptionLevel = (DrLogSrv.LogExceptionLevel)Enum.Parse(typeof(DrLogSrv.LogExceptionLevel), Config.Attributes.GetValue(SchemaProvider.AttExceptionLevel, DrLogSrv.LogExceptionLevel.ALL), true);
        }
        /// <summary>
        /// Update settings from config
        /// </summary>
        virtual public void RebuildConfiguration(DDNode config)
        {
            this.Config = config;
            this.RebuildConfiguration();
        }
        /// <summary>
        /// by default provider is enabled
        /// </summary>
        public const bool DefaultEnabled = true;
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
            n.Attributes.Add(SchemaProvider.AttLevel, DefaultLevel.ToString());
            n.Attributes.Add(SchemaProvider.AttExceptionLevel, DefaultExceptionLevel.ToString());
            n.Attributes.Add(SchemaProvider.AttEnabled, DefaultEnabled);
            return n;
        }

        public virtual DDType GetDDType()
        {
            return new DDType(this.GetType().AssemblyQualifiedName);
        }

    }
}
