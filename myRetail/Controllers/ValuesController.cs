
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace myRetail.Controllers
{
    [Route("/[controller]")]
    public class ProductsController : Controller
    {
        static HttpClient client = new HttpClient();
        
        private string buildProductURL(int id) {
            return "http://redsky.target.com/v2/pdp/tcin/" +
                    id.ToString() +
                    "?excludes=taxonomy,price,promotion,bulk_ship,rating_and_review_reviews,rating_and_review_statistics,question_answer_statistics";
        }
        
        // GET api/values/5
        [HttpGet("{id}")]
        public string product(int id)
        {
            return client.GetAsync(buildProductURL(id)).ToString();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void product(int id, [FromBody]string value)
        {
        }

    }
}
