using log4net;
using System.Web.Mvc;
using TenantMNG.Models;

namespace TenantMNG.Controllers
{
    public class ZoneController : Controller
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(LoginController));

        // GET: Zone
        public ActionResult ZoneList()
        {
            return View();
        }
    }
}