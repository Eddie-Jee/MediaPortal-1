﻿#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Castle.Windsor;
using MediaPortal.Common.Utils;

namespace Mediaportal.TV.Common.Logging
{
  public class Logger : IntegrationProviderInterfaces.ILogger
  {
    private static readonly IDictionary<Type, ILogger> _logCache = new Dictionary<Type, ILogger>();
    private static readonly object _logCacheLock = new object();

    private static ILogger GetLogger()
    {
      return GetLogger(typeof(Logger));
    }

    private static ILogger GetLogger(Type type)
    {
      Castle.Core.Logging.ILogger logger;
      lock (_logCacheLock)
      {
        bool hasLogger = _logCache.TryGetValue(type, out logger);
        if (!hasLogger)
        {
          var container = GlobalServiceProvider.Instance.Get<IWindsorContainer>();
          var loggerFactory = container.Resolve<ILoggerFactory>();
          logger = loggerFactory.Create(type);      
          _logCache[type] = logger;
        }
      }
      return logger;
    }
   
    
    /// <summary>
    /// Writes a debug message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Debug(Type caller, string format, params object[] args)
    {
      GetLogger(caller).DebugFormat(format, args);
    }

    public void Debug(string format, params object[] args)
    {
      GetLogger().DebugFormat(format, args);
    }

    /// <summary>
    /// Writes a debug message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="ex">The <see cref="Exception"/> that caused the message.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Debug(Type caller, string format, Exception ex, params object[] args)
    {
      GetLogger(caller).Debug(string.Format(format, args), ex);
    }

    public void Debug(string format, Exception ex, params object[] args)
    {
      GetLogger().Debug(string.Format(format, args), ex);
    }

    /// <summary>
    /// Writes an informational message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Info(Type caller, string format, params object[] args)
    {
      GetLogger(caller).InfoFormat(format, args);
    }

    public void Info(string format, params object[] args)
    {
      GetLogger().InfoFormat(format, args);
    }

    /// <summary>
    /// Writes an informational message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="ex">The <see cref="Exception"/> that caused the message.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Info(Type caller, string format, Exception ex, params object[] args)
    {
      GetLogger(caller).Info(string.Format(format, args), ex);
    }

    public void Info(string format, Exception ex, params object[] args)
    {
      GetLogger().Info(string.Format(format, args), ex);
    }

    /// <summary>
    /// Writes a warning to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Warn(Type caller, string format, params object[] args)
    {
      GetLogger(caller).WarnFormat(format, args);
    }

    public void Warn(string format, params object[] args)
    {
      GetLogger().WarnFormat(format, args);
    }

    /// <summary>
    /// Writes a warning to the log, passing the original <see cref="Exception"/>.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="ex">The <see cref="Exception"/> that caused the message.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Warn(Type caller, string format, Exception ex, params object[] args)
    {
      GetLogger(caller).Warn(string.Format(format, args), ex);
    }

    public void Warn(string format, Exception ex, params object[] args)
    {
      GetLogger().Warn(string.Format(format, args), ex);
    }

    /// <summary>
    /// Writes an error message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Error(Type caller, string format, params object[] args)
    {
      GetLogger(caller).ErrorFormat(format, args);
    }

    public void Error(string format, params object[] args)
    {
      GetLogger().ErrorFormat(format, args);
    }

    /// <summary>
    /// Writes an error message to the log, passing the original <see cref="Exception"/>.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="ex">The <see cref="Exception"/> that caused the message.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Error(Type caller, string format, Exception ex, params object[] args)
    {
      GetLogger(caller).Error(string.Format(format, args), ex);
    }

    public void Error(string format, Exception ex, params object[] args)
    {
      GetLogger().Error(string.Format(format, args), ex);
    }

    /// <summary>
    /// Writes an Error <see cref="Exception"/> to the log.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to write.</param>
    public void Error(Type caller, Exception ex)
    {
      GetLogger(caller).Error("", ex);
    }

    public void Error(Exception ex)
    {
      GetLogger().Error("", ex);
    }

    /// <summary>
    /// Writes a critical error system message to the log.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Critical(Type caller, string format, params object[] args)
    {
      GetLogger(caller).FatalFormat(format, args);
    }

    public void Critical(string format, params object[] args)
    {
      GetLogger().FatalFormat(format, args);
    }

    /// <summary>
    /// Writes a critical error system message to the log, passing the original <see cref="Exception"/>.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="ex">The <see cref="Exception"/> that caused the message.</param>
    /// <param name="args">An array of objects to write using format.</param>
    public void Critical(Type caller, string format, Exception ex, params object[] args)
    {
      GetLogger(caller).Fatal(string.Format(format, args), ex);
    }

    public void Critical(string format, Exception ex, params object[] args)
    {
      GetLogger().Fatal(string.Format(format, args), ex);
    }

    public void Critical(Exception ex)
    {
      GetLogger().Fatal("", ex);
    }

    /// <summary>
    /// Writes an Critical error <see cref="Exception"/> to the log.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to write.</param>
    public void Critical(Type caller, Exception ex)
    {
      GetLogger(caller).Fatal("", ex);
    }
  }
}
