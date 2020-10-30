using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using WebApplication8.Controllers;

namespace WebApplication8
{
    /// <summary>
    /// Routing convention to force directing all requests to the generic controller.
    /// </summary>
    public class GenericControllerRoutingConvention : IODataRoutingConvention
    {
        public IEnumerable<ControllerActionDescriptor> SelectAction(RouteContext routeContext)
        {
            string odataPath = routeContext.HttpContext.Request.ODataFeature().Path?.ToString();

            if (string.IsNullOrEmpty(odataPath) || odataPath == "$metadata")
            {
                return new MetadataRoutingConvention().SelectAction(routeContext);
            }

            IActionDescriptorCollectionProvider actionCollectionProvider = routeContext.HttpContext.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

            return actionCollectionProvider.ActionDescriptors
                                           .Items
                                           .OfType<ControllerActionDescriptor>()
                                           .Where(c => c.ControllerName == OData4Controller.Name)
                                           ;
        }
    }
}
