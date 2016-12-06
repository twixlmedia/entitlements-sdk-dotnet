using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace EntitlementServerAllAccess.Controllers {

    public class HomeController : Controller {

        // The entitlement token used to identify a user which has full access
        //
        // For security reasons, change this to a custom value
        private String defaultToken = "d6b3959ecdce4cc29830bcfc0473c938";

        // The default page which is shown when no action is specified
        public IActionResult Index() {
            ViewData["title"] = "All Access Twixl Entitlements Server";
            return View();
        }

        // Shows the signin form used to ask for the username and password
        public IActionResult signin_form() {
            ViewData["title"] = "Login";
            return View();
        }

        // Check the username and password from the signin form and return an
        // entitlement token uniquely identifying this user on the entitlement
        // server.
        //
        // As we are only interested in finding out if the user has access or
        // not, we return the same entitlement token for all users.
        //
        // For added security, you can return a unique token for each user
        // which can then be verified in the actionEntitlements method.
        //
        // If the login fails, we return an error message.
        //
        // The different parameters are sent as a HTTP POST request.
        public IActionResult signin(string username, string password) {
            try {
                string token = checkLogin(username, password);
                return okResult(new { token = token });
            } catch (Exception e) {
                return errorResult(e.Message);
            }
        }

        // The signin succeeded, so we are just closing the entitlements
        // popup by calling a specific url
        public IActionResult signin_succeeded() {
            return Redirect("tp-close://self");
        }

        // The signin didn't work, we retrieve the error from the URL and
        // render the error screen.
        public IActionResult signin_error(String error) {
            ViewData["title"] = "An Error Occurred";
            ViewData["error"] = error;
            return View();
        }

        // The entitlements call checks the token, and decides based on the
        // token if the user has access to content or not.
        //
        // If the token is empty, we return an empty list of allowed product
        // identifiers and use the entitlement mode "hide_unentitled". This
        // causes the app to not show any content.
        //
        // If the token is correct, we return "*" as the entitled_products
        // which tells the app to show all content.
        //
        // If the token is invalid, we return an error message.
        //
        // The different parameters are sent as a HTTP POST request.
        public IActionResult entitlements(string token) {
            try {

                ArrayList entitledProducts = new ArrayList();

                if (token == null || token == "") {
                } else if (token == defaultToken) {
                    entitledProducts.Add("*");
                } else {
                    throw new Exception("Invalid credentials");
                }

                return okResult(new {
                    entitled_products = entitledProducts,
                    entitlement_mode = "hide_unentitled",
                    token = token
                });

            } catch (Exception e) {
                return errorResult(e.Message);
            }
        }

        // This is a helper method to check if the login is correct or not.
        //
        // If the login is correct, we return an entitlement token.
        //
        // If the loign is incorrect, we throw an Exception.
        //
        // This is the place where you can customize the way a username and
        // password are verified. You can for example perform a database call
        // to verify the credentials.
        private string checkLogin(string username, string password) {
            if (username != "test" && password != "test") {
                throw new Exception("Invalid username or password");
            }
            return defaultToken;
        }

        // Helper method to return a HTTP 200 response
        private IActionResult okResult(object data) {
            return Json(data);
        }

        // Helper method to return a HTTP 500 response
        private IActionResult errorResult(string error) {
            return Json(new { error = error });
        }

    }

}
