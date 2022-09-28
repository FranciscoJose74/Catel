﻿namespace Catel.Services
{
    using System.Threading.Tasks;
    using Microsoft.Win32;

    /// <summary>
    /// Service to save files.
    /// </summary>
    public partial class SaveFileService
    {
        /// <inheritdoc/>
        public virtual async Task<DetermineSaveFileResult> DetermineFileAsync(DetermineSaveFileContext context)
        {
            Argument.IsNotNull("context", context);

            var fileDialog = new SaveFileDialog();

            await ConfigureFileDialogAsync(fileDialog, context);

            var result = new DetermineSaveFileResult
            {
                Result = fileDialog.ShowDialog() ?? false,
                FileName = fileDialog.FileName
            };

            return result;
        }
    }
}
