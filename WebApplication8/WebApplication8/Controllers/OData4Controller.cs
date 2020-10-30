using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using System;
using System.Linq;

namespace WebApplication8.Controllers
{
    public class OData4Controller : ODataController
    {
        internal const string Name = "OData4";

        public OData4Controller()
        { }

        private static IQueryable<IEdmEntityObject> GetProductsAsEntityObject(IEdmModel model) => Enumerable.Range(0, 20).Select(i =>
        {
            IEdmEntityType productType = (IEdmEntityType)model.FindType("WebApplication8.Product");

            EdmEntityObject product = new EdmEntityObject(productType);
            product.TrySetPropertyValue("ID", Guid.NewGuid());
            product.TrySetPropertyValue("Name", "Product " + i);
            product.TrySetPropertyValue("Price", i + 0.01m);
            product.TrySetPropertyValue("Category", "Category - " + i);

            return product;
        }).AsQueryable();

        private static IQueryable<Product> GetProductsAsClrType() => Enumerable.Range(0, 20).Select(i =>
        {
            return new Product()
                    { ID = Guid.NewGuid()
                    , Name = "Product " + i
                    , Price = i + 0.01m
                    , Category = "Category - " + i
                    };
        }).AsQueryable();

        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns>An entity collection.</returns>
        public IActionResult Get()
        {
            ODataPath path = Request.ODataFeature().Path;

            IEdmModel model = EdmModelBuilder.GetEdmModel();

            //
            // Working, but uses CLR types.
            //
            {
                ODataQueryContext queryContext = new ODataQueryContext(model, typeof(Product), path);
                ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);

                IQueryable<Product> products = GetProductsAsClrType();

                IQueryable query = queryOptions.ApplyTo(products, new ODataQuerySettings());

                return Ok(query);
            }

            //
            // Not working, uses EDM entity objects.
            //
            {
                IEdmType edmType = path.EdmType;
                IEdmCollectionType collectionType = edmType as IEdmCollectionType;
                IEdmEntityType entityType = collectionType.ElementType.Definition as IEdmEntityType;

                ODataQueryContext queryContext = new ODataQueryContext(model, entityType, path);
                ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);

                IQueryable<IEdmEntityObject> products = GetProductsAsEntityObject(model);

                IQueryable query = queryOptions.ApplyTo(products, new ODataQuerySettings());
                // NotSupportedException: The query option is not bound to any CLR type. 'ApplyTo' is only supported with a query option bound to a CLR type.

                return Ok(new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType), query.Cast<IEdmEntityObject>().ToList()));
            }
        }
    }
}