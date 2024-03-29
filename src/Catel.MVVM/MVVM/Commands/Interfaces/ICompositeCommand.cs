﻿namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Composite command which allows several commands inside a single command being exposed to a view.
    /// </summary>
    public interface ICompositeCommand : ICatelTaskCommand<ITaskProgressReport>
    {
        /// <summary>
        /// Gets or sets a value indicating whether partial execution of commands is allowed. If this value is <c>true</c>, this composite
        /// command will always be executable and only invoke the internal commands that are executable.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if partial execution is allowed; otherwise, <c>false</c>.</value>
        bool AllowPartialExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether at least one command must be executable. This will prevent the command to be 
        /// executed without any commands.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if at least one command must be executed; otherwise, <c>false</c>.</value>
        bool AtLeastOneMustBeExecutable { get; set; }

        /// <summary>
        /// Gets the commands currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        IEnumerable<ICommand> GetCommands();

        /// <summary>
        /// Gets the actions currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        IEnumerable<Action> GetActions();

        /// <summary>
        /// Gets the actions with parameters currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        IEnumerable<Action<object?>> GetActionsWithParameter();

        /// <summary>
        /// Gets the actions currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        IEnumerable<Func<Task>> GetAsyncActions();

        /// <summary>
        /// Gets the actions with parameters currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        IEnumerable<Func<object?, Task>> GetAsyncActionsWithParameter();

        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model. If specified, the command will automatically be unregistered when the view model is closed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Note that if the view model is not specified, the command must be unregistered manually in order to prevent memory leaks.
        /// </remarks>
        void RegisterCommand(ICommand command, IViewModel? viewModel = null);

        /// <summary>
        /// Unregisters the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        void UnregisterCommand(ICommand command);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Action action);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Action action);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Action<object?> action);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Action<object?> action);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Func<Task> action);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Func<Task> action);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Func<object?, Task> action);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Func<object?, Task> action);
    }
}
