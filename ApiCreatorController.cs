using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using RJP.MultiUrlPicker.Models;
using TaranakiGolfAssociation.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace MyNameSpace.Controllers
{
    public class ApiCreatorController : UmbracoApiController
    {
        [HttpGet]
        public IEnumerable<EventCalendar> GetAllEvents()
        {
            var result = new List<EventCalendar>();

            var rootNode = Umbraco.TypedContentAtRoot().FirstOrDefault(n => n.DocumentTypeAlias == "homepage");
            if(rootNode == null)
            {
                throw new NullReferenceException("Root node not found");
            }

            // Grab all clubs
            var allClubs = rootNode.Descendants().Where(n => n.DocumentTypeAlias == "clubPage");

            // foreach club, get the events
            foreach (var club in allClubs)
            {
                EventCalendar eventcalendar = new EventCalendar();
                eventcalendar.ClubId = club.Id;
                eventcalendar.EventHost = club.GetPropertyValue("clubPageTitle").ToString();
                var listEventItems = club.Descendants()
                    .Where(n => n.DocumentTypeAlias == "event")
                    .Select(ev =>
                    {
                        Link redirection = ev.GetPropertyValue<Link>("redirectionPage"); //In case it's empty, otherwise it will gives warning message
                        return new EventItem()
                        {
                            EventTitle = ev.GetPropertyValue<string>("eventTitle"),
                            EventDate = ev.GetPropertyValue<DateTime>("eventDate"),
                            EventColour = ev.GetPropertyValue<string>("eventColour"),
                            EventDescription = ev.GetPropertyValue<string>("eventDescription"),
                            EventLink = redirection?.Url
                        };
                    })
                    .OrderBy(ev => ev.EventDate)
                    .ToList();

                eventcalendar.Events = listEventItems;
                result.Add(eventcalendar);
            }
            return result; //returns in JSON format
        }
    }
}