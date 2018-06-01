using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace myRetail.Models
{
    /// <summary>
    /// this is a object representation of the product
    /// </summary>
    public class Product
    {
        [BsonId]
        public BsonObjectId _id { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public ProductPrice productPrice { get; set; }
    }
}
