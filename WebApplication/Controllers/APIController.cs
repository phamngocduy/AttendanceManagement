using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class APIController : ApiController
    {
        cap21t4Entities db = new cap21t4Entities();
        public HttpResponseMessage getMember(string groupname)
        {
            try
            {
                var memberlist = (from us in db.Users
                                  from gr in us.Groups.Where(x => x.GroupName == groupname)
                                  select us);
                List<UserProfile> list = new List<UserProfile>();
                foreach(var item in memberlist)
                {
                    UserProfile us = new UserProfile
                    {
                        StID = item.StID,
                        FullName = item.FullName,
                        PhoneNumber = item.PhoneNumber,
                        Email = item.Email,
                        DoB = item.DoB
                    };
                    list.Add(us);
                }
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage getUser(string email)
        {
            try
            {
                var user = db.Users.FirstOrDefault(x => x.Email == email);
                List<UserProfile> list = new List<UserProfile>();
                    UserProfile us = new UserProfile
                    {
                        StID = user.StID,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        DoB = user.DoB
                    };
                    list.Add(us);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
