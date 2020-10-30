using Microsoft.OData.Edm;
using System;

namespace WebApplication8
{
    public class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            EdmModel edmModel = new EdmModel();

            string entityName = "Product";

            EdmEntityType edmEntityType = new EdmEntityType
                                            ( "WebApplication8"
                                            , entityName
                                            );

            EdmStructuralProperty idProperty = edmEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Guid);

            edmEntityType.AddKeys(idProperty);

            edmEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            edmEntityType.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Decimal);
            edmEntityType.AddStructuralProperty("Category", EdmPrimitiveTypeKind.String);

            edmModel.AddElement(edmEntityType);

            EdmEntityContainer container = new EdmEntityContainer("WebApplication8", "Container");

            container.AddEntitySet("Products", edmEntityType);

            edmModel.AddElement(container);

            return edmModel;
        }
    }
}
