namespace Merchello.Web.Models.Note
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Merchello.Core.Models;

    /// <summary>
    /// The audit log display.
    /// </summary>
    public class NoteDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the record date.
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }

    /// <summary>
    /// The note display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class NoteDisplayExtensions
    {
        /// <summary>
        /// The to audit log display.
        /// </summary>
        /// <param name="note">
        /// The audit log.
        /// </param>
        /// <returns>
        /// The <see cref="NoteDisplay"/>.
        /// </returns>
        public static NoteDisplay ToNoteDisplay(this INote note)
        {
            return AutoMapper.Mapper.Map<NoteDisplay>(note);
        }
    }
}
