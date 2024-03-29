﻿namespace Catel.Data
{
    using System.ComponentModel;

    /// <summary>
    /// Class that allows to invoke <see cref="IEditableObject"/> methods on any object.
    /// </summary>
    public static class EditableObjectHelper
    {
        /// <summary>
        /// Begins an edit on an object.
        /// <para />
        /// This method will do nothing when the <paramref name="obj"/> is <c>null</c> or not a <see cref="IEditableObject"/>.
        /// </summary>
        /// <param name="obj">The object to call the <see cref="IEditableObject.BeginEdit"/> method on.</param>
        public static void BeginEditObject(object obj)
        {
            var objAsIEditableObject = obj as IEditableObject;
            if (objAsIEditableObject is not null)
            {
                objAsIEditableObject.BeginEdit();
            }
        }

        /// <summary>
        /// Pushes changes since the last <see cref="IEditableObject.EndEdit()"/> call.
        /// <para />
        /// This method will do nothing when the <paramref name="obj"/> is <c>null</c> or not a <see cref="IEditableObject"/>.
        /// </summary>
        /// <param name="obj">The object to call the <see cref="IEditableObject.EndEdit"/> method on.</param>
        public static void EndEditObject(object obj)
        {
            var objAsIEditableObject = obj as IEditableObject;
            if (objAsIEditableObject is not null)
            {
                objAsIEditableObject.EndEdit();
            }
        }

        /// <summary>
        /// Discards changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// <para />
        /// This method will do nothing when the <paramref name="obj"/> is <c>null</c> or not a <see cref="IEditableObject"/>.
        /// </summary>
        /// <param name="obj">The object to call the <see cref="IEditableObject.CancelEdit"/> method on.</param>
        /// <remarks>
        /// This method wi
        /// </remarks>
        public static void CancelEditObject(object obj)
        {
            var objAsIEditableObject = obj as IEditableObject;
            if (objAsIEditableObject is not null)
            {
                objAsIEditableObject.CancelEdit();
            }
        }
    }
}
