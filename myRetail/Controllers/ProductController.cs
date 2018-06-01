using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using myRetail.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace myRetail.Controllers
{
    /// <summary>
    /// this controller class supports GET and PUT actions
    /// Ex: GET at address"http://localhost:55971/product/" presents the user with a message to provide the product ID
    ///     GET at address"http://localhost:55971/product/13860428" makes a call to an external API and retrieves, stores and return product information in JSON format
    ///     PUT at address"http://localhost:55971/product/13860428" takes a product ID and a JSON object and updates the price of the product
    /// </summary>
    [Route("/product")]
    public class ProductController : Controller
    {        
        static HttpClient client = new HttpClient();
        IproductRepository _repository;
        IConfiguration _iconfiguration;

        /// <summary>
        /// this constructor takes a configuration and a product repository and injects them into the controller to be used by the application
        /// </summary>
        /// <param name="iconfiguration">stores the application configuration information 
        /// sucha as the database connection information and the external url used to retrieve product information</param>
        /// <param name="repository"> respository used to retrieve price information</param>
        public ProductController(IConfiguration iconfiguration, IproductRepository repository) {
            _iconfiguration = iconfiguration;
            _repository = repository;
        }
       
        /// <summary>
        /// This method takes inteder product id and builds the URL used to get product information
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>external url used to retrieve product information</returns>
        private string BuildProductURL(int id)
        {
            return "http://redsky.target.com/v2/pdp/tcin/" +
                    id.ToString().Replace("\n", String.Empty);// +
                   // "?excludes=taxonomy,price,promotion,bulk_ship,rating_and_review_reviews,rating_and_review_statistics,question_answer_statistics";
        }

        /// <summary>
        /// this method takes the built external url and fetches product information
        /// </summary>
        /// <param name="path">external url used to fetch product information</param>
        /// <returns>resulting information from the call to get product information</returns>
        private  async Task<string> GetProductAsync(string path)
        {
            string product = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);                
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
            
            return product;
        }

        /// <summary>
        /// this controller action is here sipmly to return a message if the user does not enter a product ID
        /// </summary>
        /// <returns>message to the user</returns>
        [HttpGet()]
        public string Product()
        {
            return "please provide the product id.";
        }
        
        /// <summary>
        /// this controller action takes a product id and looks in the local database if we already have the product and returns it's information
        /// if the product is not in the local database it goes out to the external API and fetches the product information,
        /// stores the information in a local database and returns the stored product
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>product information</returns>
        [HttpGet("{id}")]
        public string  Product(int id) {            
            int inId = (int)id;
            Product product;

            product = _repository.GetProduct(inId);

            if (product != null)
            {
                product._id = null;
                return JsonConvert.SerializeObject(product);
            }
            else
            {
                try
                {
                    string url = BuildProductURL(inId);
                    string jsonProduct = null;
                    jsonProduct = GetProductAsync(url).Result;
                    if (jsonProduct != null)
                    {
                        int productID;
                        string productPrice = JObject.Parse(jsonProduct)["product"]["price"]["listPrice"]["price"].ToString();
                        string productName = JObject.Parse(jsonProduct)["product"]["item"]["product_description"]["title"].Value<string>().ToString();
                        Int32.TryParse(JObject.Parse(jsonProduct)["product"]["available_to_promise_network"]["product_id"].Value<string>(), out productID).ToString();

                        product = new Product();
                        ProductPrice price = new ProductPrice();
                        product.productID = productID;
                        product.productName = productName;

                        price.price = productPrice;
                        price.currency = "USD";
                        product.productPrice = price;
                        product._id = ObjectId.GenerateNewId();
                        product = _repository.updateProduct(product);
                        product._id = null;
                        return JsonConvert.SerializeObject(product);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                }
                
                return "no product found for product id:" + id.ToString();
            }
            
        }

        /// <summary>
        /// this controller takes a product id and a product json object and updates the stored product information
        /// </summary>
        /// <param name="id">product ID</param>
        /// <param name="value">product JSON object</param>
        /// <returns>product information</returns>
        [HttpPut("{id}")]
        public Product Product(int id, [FromBody]Object value)
        {

            Product product = new Product();
            ProductPrice price = new ProductPrice();
            try
            {
                JObject updated = (JObject)value;
                int productID;
                string productPrice = updated["product"]["price"]["listPrice"]["price"].ToString();
                string productName = updated["product"]["item"]["product_description"]["title"].ToString();
                Int32.TryParse(updated["product"]["available_to_promise_network"]["product_id"].ToString(), out productID);

                product.productID = productID;
                product.productName = productName;

                price.price = productPrice;
                price.currency = "USD";
                product.productPrice = price;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
            return _repository.updateProduct(product);
        }

    }
}
