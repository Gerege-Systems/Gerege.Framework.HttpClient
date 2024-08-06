using System;
using System.Diagnostics;
using System.Collections.Generic;
using Gerege.Framework.Logger;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SharedExample;

/// <summary>
/// Хөгжүүлэлтийн орчинд Debug горимтой үед Output консол дээр лог харуулах обьект.
/// </summary>
public class ConsoleLogger : DatabaseLogger
{
    /// <inheritdoc />
    public override bool Connect(string? connection = null)
    {
        return true;
    }

    /// <inheritdoc />
    protected override bool Assert()
    {
        return true;
    }

    /// <inheritdoc />
    protected override void Log(string table, string level, string message, object context)
    {
        Debug.WriteLine($"[{GetType().Name.ToUpper()}] Me writing log in {level} to {table} : message => \"{message}\" and context => {Convert.ToString(context)}");
    }

    /// <inheritdoc />
    public override List<Log> SelectFrom(string table, string level, string condition)
    {
        throw new NotImplementedException();
    }
}
