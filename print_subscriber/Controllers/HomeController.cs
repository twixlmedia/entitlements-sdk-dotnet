using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace print_subscriber.Controllers {

    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        // The default page which is shown when no action is specified
        public IActionResult Index() {
            ViewData["title"] = "Print Subscriber Twixl Entitlements Server";
            return View();
        }

        // Shows the signin form used to ask for the username and password
        public IActionResult signin_form() {
            ViewData["title"] = "Login As Print Subscriber";
            return View();
        }

        // Check the username and password from the signin form and return an
        // entitlement token uniquely identifying this user on the entitlement
        // server.
        //
        // In this case, we use the username as the entitlement token so that
        // the actionEntitlements method can use this same token to check what
        // products the user should get free access to.
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

        // The signin succeeded, so we render a page that welcomes the user
        // and gives them some more information.
        public IActionResult signin_succeeded(string token) {
            ViewData["title"] = "Welcome";
            ViewData["token"] = token;
            return View();
        }

        // The signin didn't work, we retrieve the error from the URL and
        // render the error screen.
        public IActionResult signin_error(String error) {
            ViewData["title"] = "An Error Occurred";
            ViewData["error"] = error;
            return View();
        }

        // The entitlements call checks the token, and decides based on the
        // token if the user has free access to certain content or not.
        //
        // If the token is empty, we return an empty list of allowed product
        // identifiers and use the entitlement mode "purchase_unentitled". This
        // causes the app to not change any content from purchase to free.
        //
        // If the token is correct, we return a list of two product identifiers
        // as the entitled_products which tells the app to change the issues or
        // collections with these identifiers from purchase to free.
        //
        // If the token is invalid, we return an error message.
        //
        // The different parameters are sent as a HTTP POST request.
        public IActionResult entitlements(string token) {
            try {

                ArrayList entitledProducts = new ArrayList();

                if (token == null || token == "") {
                } else if (token == "test") {
                    entitledProducts.Add("com.twixlmedia.demo.product1");
                    entitledProducts.Add("com.twixlmedia.demo.product2");
                } else {
                    throw new Exception("Invalid credentials");
                }

                return okResult(new {
                    entitled_products = entitledProducts,
                    entitlement_mode = "purchase_unentitled",
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
            return username;
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
