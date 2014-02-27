﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Represents the Invoice Repository
    /// </summary>
    internal class InvoiceRepository : MerchelloPetaPocoRepositoryBase<IInvoice>, IInvoiceRepository
    {
        private readonly ILineItemRepository _lineItemRepository; 
        
        public InvoiceRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILineItemRepository lineItemRepository) 
            : base(work, cache)
        {
            Mandate.ParameterNotNull(lineItemRepository, "lineItemRepository");

            _lineItemRepository = lineItemRepository;
        }

        protected override IInvoice PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            
            var lineItems = GetLineItemCollection(key);
           
            var factory = new InvoiceFactory(lineItems);
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IInvoice> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {
                ;
                var dtos = Database.Fetch<InvoiceDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        protected override IEnumerable<IInvoice> PerformGetByQuery(IQuery<IInvoice> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoice>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<InvoiceDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoice.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO deleting invoices is going to be a pretty involved process
            // that will require much more than this repository can handle alone. 
            // TODO come back to this
            var list = new List<string>
            {
                "DELETE FROM merchAppliedPayment WHERE invoiceKey = @Key",
                "DELETE FROM merchInvoiceItem WHERE invoiceKey = @Key",
                "DELETE FROM merchInvoice WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IInvoice entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new InvoiceFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            foreach (var item in entity.Items)
            {
                _lineItemRepository.AddOrUpdate(item);
            }

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoice entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            var existing = _lineItemRepository.GetByContainerKey(entity.Key);
            var removes = existing.Where(x => !entity.Items.Contains(x));

            foreach(var remove in removes) _lineItemRepository.Delete(remove);

            foreach (var item in entity.Items)
            {
                _lineItemRepository.AddOrUpdate(item);
            }

            entity.ResetDirtyProperties();
        }

        private LineItemCollection GetLineItemCollection(Guid invoiceKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceItemDto>()
                .Where<InvoiceItemDto>(x => x.ContainerKey == invoiceKey);

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }
    }
}