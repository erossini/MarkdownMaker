using System;

namespace MarkdownMaker.Enums
{
    /// <summary>
    /// Enum TableOptions
    /// </summary>
    [Flags]
    public enum TableOptions
    {
        /// <summary>
        /// The default
        /// </summary>
        Default = 0,

        /// <summary>
        /// The exclude collection properties
        /// </summary>
        ExcludeCollectionProperties = 1
    }
}