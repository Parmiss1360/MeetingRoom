/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicrosoftGraphAspNetCoreConnectSample.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using MettingRoom.Model;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System;

namespace MicrosoftGraphAspNetCoreConnectSample.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly IGraphSdkHelper _graphSdkHelper;

        public GraphServiceClient graphClient { get; set; }
        public HomeController(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IGraphSdkHelper graphSdkHelper)
        {
            _configuration = configuration;
            _env = hostingEnvironment;
            _graphSdkHelper = graphSdkHelper;



        }
        [AllowAnonymous]
        // Load user's profile.
        public async Task<IActionResult> Index( int page = 1, int row = 10)
        {
            if (User.Identity.IsAuthenticated)
            {

                // Initialize the GraphServiceClient.
                 graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);


            
                        var events = await graphClient.Me.Events
              .Request().Top(100)
              .Header("Prefer", "outlook.timezone=\"W. Europe Standard Time\"")
              .Select(e => new
              {
                  e.Subject,
                  e.Body,
                  e.BodyPreview,
                  e.Organizer,
                  e.Attendees,
                  e.Start,
                  e.End,
                  e.Location,
                  e.Id

              })
              .GetAsync();


                string Datenow = System.DateTime.Now.Date.ToShortDateString();

                var _events = events.Where(e => e.Location.DisplayName != "" && e.Start.DateTime.Split('T')[0].ToString() == Datenow
               
                ).Select(e => new Events { Attendees = e.Attendees, Start = e.Start, End = e.End, IsFull = false ,Id=e.Id }).ToList();

                var li = _events.Where(e => e.Start.DateTime.Split('T')[0].ToString() == Datenow && (Convert.ToDateTime(e.Start.DateTime.Split('T')[1].ToString()).TimeOfDay >= DateTime.Now.TimeOfDay

             || Convert.ToDateTime(e.End.DateTime.Split('T')[1].ToString()).TimeOfDay >= DateTime.Now.TimeOfDay)).ToList();
                var model = PagingList.Create(li, row, page);

                var m = li.Min(P => P.Start.DateTime);

                Events test = li.Select(a=>new Events { Start=a.Start, End=a.End})
                    
                    .Where(c => c.Start.DateTime == m).FirstOrDefault();

                if (test != null && Convert.ToDateTime(test.Start.DateTime.Split('T')[1].ToString()).TimeOfDay <= DateTime.Now.TimeOfDay &&
                    Convert.ToDateTime(test.End.DateTime.Split('T')[1].ToString()).TimeOfDay >= DateTime.Now.TimeOfDay)
                {

                    test.IsFull = true;
                    ViewBag.IsFull = 1;
                }
                else
                { ViewBag.IsFull = 0; }
                  
                var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                    return PartialView("_Meeting", model);
                else
                    return View("Index", model);
            }

            return View();

        }

        
       

        public IActionResult Notification()
        {

            return PartialView("_Notification", TempData["notifictaion"]);
        }
        [HttpGet]
        public IActionResult Create()
        {
            List<Microsoft.Graph.Attendee> li = new List<Attendee>()
            {
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Parmis.Etezadpour@xlent.se"}  },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Anna.Winnel@xlent.se"}  },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Stina.Jonsson@xlent.se"}  },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Per.Jonsson@xlent.se"}  },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Thomas.Calemark@xlent.se"}  },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Annika.Jenderborn@xlent.se"} },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Robin.Henriksson@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Elin.Tordal@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Thomas.Karlsson@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Mikael.Nordlander@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Ove.Svensson@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Anders.Lothigius@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Tomas.Hultberg@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="Daniel.Jardeby@xlent.se" } },
                new Attendee() {EmailAddress=new EmailAddress(){  Name="XrsrcLinkopingMeetingroomXLENTLinkping@xlent.onmicrosoft.com" } },


            }.OrderBy(s=>s.EmailAddress.Address).ToList();
            

            ViewBag.AuthorID = new SelectList(li, "EmailAddress.Name", "EmailAddress.Name");
            return PartialView("_Create");
        }
        [Authorize]
        [HttpPost]
        // Send an email message from the current user.
        public async Task<IActionResult> Create( Events @event)
        {
            
            try
            {

              
                var errors=new List<string>();
                // Initialize the GraphServiceClient.
                var graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);

                // Send the email.

                 GetErrors(@event);
                if (ModelState.IsValid)
                {
                    await GraphService.CreateEvents(graphClient, @event);
                    if (Convert.ToDateTime(@event.Start.DateTime.ToString()).TimeOfDay <= DateTime.Now.TimeOfDay &&
                   Convert.ToDateTime(@event.End.DateTime.ToString()).TimeOfDay >= DateTime.Now.TimeOfDay)
                    {
                        ViewBag.IsFull = 1;
                    }
                    ViewBag.errors = "0";
                    // Reset the current user's email address and the status to display when the page reloads.
                    TempData["notifictaion"] = "Success!Create Appoitments.";
                    
                }

                else
                {
                    ViewBag.errors = "1";
                    ViewBag.Error = "";
                    return PartialView("_Create");
                }
                    return PartialView("_Create",@event);

            }
            catch (ServiceException se)
            {
                if (se.Error.Code == "Caller needs to authenticate.") return new EmptyResult();
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }
        }
     

        private void GetErrors(Events @event)
        {

            var graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);

            var events = GraphService.GetEvents(graphClient).Result.Select(a => new Events { Start = a.Start, End = a.End 

            });
           
            var checklist = events.Where(e => Convert.ToDateTime(e.Start.DateTime) <= Convert.ToDateTime(@event.Start.DateTime) &&

             Convert.ToDateTime(e.End.DateTime) >= Convert.ToDateTime(@event.Start.DateTime) ||

            
             ( Convert.ToDateTime(e.Start.DateTime) <= Convert.ToDateTime(@event.End.DateTime) &&

             Convert.ToDateTime(e.End.DateTime) >= Convert.ToDateTime(@event.End.DateTime))

            ).ToList().Count();


            if(Convert.ToDateTime(@event.Start.DateTime) >= Convert.ToDateTime(@event.End.DateTime))
            {


                ModelState.AddModelError(string.Empty, "det kan inte vara EndTid mindre en start tid");
               
            }
            if(checklist>0)
            {
                ModelState.AddModelError(string.Empty, "Nu finns redan en möte I konferensrummet , försök på en annan tid");

            }

        }


        public async Task <IActionResult> Delete( string Id)
        {

            var graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);

            await GraphService.DeleteEvent(graphClient,Id);
            return RedirectToAction("Index");
        }
    }
}
