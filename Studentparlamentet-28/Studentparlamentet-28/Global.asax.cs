﻿using Studentparlamentet_28.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Studentparlamentet_28
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
           if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                                       
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        var db = new BrukerBLL();
                        // if brukernavn finnes i admin, return true
                        // if brukernavn finnes i bruker, return false

                        var roles = db.hentRolleAdmin(username);
                        if (roles != null) // admin
                        {
                            string rolle = Convert.ToString(roles.administrator);
                            //let us extract the roles from our own custom cookie
                            //Let us set the Pricipal with our user specific details
                            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(username, "Forms"), rolle.Split(';'));

                        }
                       else // bruker
                        {
                            var roles2 = db.hentRolleBruker(username);
                            if(roles2 != null)
                            {
                                string rolle2 = Convert.ToString(roles2.administrator);
                                //let us extract the roles from our own custom cookie
                                //Let us set the Pricipal with our user specific details
                                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                                new System.Security.Principal.GenericIdentity(username, "Forms"), rolle2.Split(';'));
                            }
                            
                        }
                        
                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
            
            }
 

        }



    }

}

