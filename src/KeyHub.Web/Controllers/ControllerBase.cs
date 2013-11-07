using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KeyHub.Data;
using KeyHub.Web.Models;
using KeyHub.Web.ViewModels;

namespace KeyHub.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public ControllerBase(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }
    }
}