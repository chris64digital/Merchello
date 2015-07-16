namespace Merchello.Core.Chains.InvoiceCreation
{
    using System.IO;
    using Models;
    using Sales;
    using Umbraco.Core;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;

    /// <summary>
    /// Represents a task responsible for adding billing information collected a checkout process to the
    /// invoice.
    /// </summary>
    internal class AddNotesToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBillingInfoToInvoiceTask"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public AddNotesToInvoiceTask(SalePreparationBase salePreparation)
            : base(salePreparation)
        {            
        }

        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var note = SalePreparation.Customer.ExtendedData.GetNote();
            if (note != null)
            {
                note.EntityKey = value.Key;
                note.EntityTfKey = EnumTypeFieldConverter.EntityType.GetTypeField(EntityType.Invoice).TypeKey;
                var noteService = new NoteService();
                noteService.Save(note);
            }

            return Attempt<IInvoice>.Succeed(value);            
        }
    }
}