using SevenWonders.DAL.Context;
using SevenWonders.Models;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace SevenWonders.WebAPI.Controllers
{
    public class SearchController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

    }
}
