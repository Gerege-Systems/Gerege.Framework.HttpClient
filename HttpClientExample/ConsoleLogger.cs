using System;
using System.Diagnostics;
using System.Collections.Generic;

using Gerege.Framework.Logger;

namespace HttpClientExample
{
    /// <author>
    /// codesaur - 2022.01.29
    /// </author>
    /// <package>
    /// Gerege Application Development Framework V5
    /// </package>

    /// <summary>
    /// Хөгжүүлэлтийн орчинд Debug горимтой үед Output консол дээр лог харуулах обьект.
    /// </summary>
    public class ConsoleLogger : DatabaseLogger
    {
        /// <inheritdoc />
        public override bool Connect(string connection = null)
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool Assert()
        {
            return true;
        }

        /// <inheritdoc />
        protected override void Log(string table, string level, string message, dynamic context)
        {
            string log =
                "["
                + GetType().Name.ToUpper()
                + "] Me writing log in "
                + level
                + " to "
                + table
                + " : message => \""
                + message
                + "\" and context => "
                + Convert.ToString(context);

            Debug.WriteLine(log);
        }

        /// <inheritdoc />
        public override List<Log> SelectFrom(string table, string level, string condition)
        {
            throw new NotImplementedException();
        }
    }
}
