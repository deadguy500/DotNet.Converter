using Converter.Web.Helpers;
using Converter.Web.Models;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var assemblyControllerList = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(x => x.Name.Equals("Index"))
                    .Where(x => !x.DeclaringType.Name.Equals("HomeController"))
                    .Where(x => x.GetCustomAttributes(typeof(HttpGetAttribute), false).Any())
                    .Select(x => new
                    {
                        Controller = x.DeclaringType.Name.Replace("Controller", string.Empty),
                        Action = x.Name,
                    })
                    .OrderBy(x => x.Controller)
                    .ThenBy(x => x.Action)
                    .ToList();

            var model = new HomeModel();

            foreach (var ac in assemblyControllerList)
            {
                var item = new ControllerItemModel()
                {
                    ActionName = ac.Action,
                    ControllerName = ac.Controller,
                    LinkName = CommonHelper.AddSpacesToSentence(ac.Controller)
                };

                model.List.Add(item);
            }

            return View(model);
        }
    }
}