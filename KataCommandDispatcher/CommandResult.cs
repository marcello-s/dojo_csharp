#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public class CommandResult(bool success = true) : ICommandResult
{
    public bool Success { get; protected set; } = success;
}
