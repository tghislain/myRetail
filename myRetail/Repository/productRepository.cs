using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using myRetail.Models;
using System;
using System.Threading.Tasks;

namespace myRetail.Repository
{
    /// <summary>
    /// this is a product repository to access product data from a local database
    /// </summary>
    public class productRepository : IproductRepository
    {
        IConfiguration _iconfiguration;
        MongoClient _client;
        IMongoDatabase _db;

        string _connectionString;
        string _databaseName;
        string _collectionName;

        /// <summary>
        /// this contructor initializes the configuration, the database client and the database
        /// </summary>
        /// <param name="iconfiguration">stores configuration information</param>
        public productRepository(IConfiguration iconfiguration)
        {
            _connectionString  = iconfiguration.GetSection("mongoUrl").Value;
            _databaseName  = iconfiguration.GetSection("dbName").Value;
            _collectionName  = iconfiguration.GetSection("collectionName").Value;
            _iconfiguration = iconfiguration;
            _client = new MongoClient(_connectionString);
            _db = _client.GetDatabase(_databaseName);
        }
        /// <summary>
        /// this method takes in a product ID and returns a product object
        /// </summary>
        /// <param name="id">product ID</param>
        /// <returns>product object</returns>
        public Product GetProduct(int id)
        {
            Task<Product> results = null;
            try
            {
                results = _db.GetCollection<Product>(_collectionName).Find(product => product.productID == id).FirstOrDefaultAsync();
                results.GetAwaiter().GetResult();
                
            } catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
            if (results.Result != null)
                return results.Result;
            else
                return null;
        }

        /// <summary>
        /// this method takes in a product object and if it exists, the product is updated with given information
        /// if the product does not exist this method inserts the product in the database
        /// </summary>
        /// <param name="updated">an updated product to be inserted into the database</param>
        /// <returns>the updated product object</returns>
        public Product updateProduct(Product updated)
        {
            Product returnProduct = null;
            try
            {
                Product returnedProduct = GetProduct(updated.productID);
                if (returnedProduct != null)
                {
                    updated._id = returnedProduct._id;
                }
                var result = _db.GetCollection<Product>(_collectionName)
                    .ReplaceOneAsync(product => product.productID == updated.productID, updated, new UpdateOptions { IsUpsert = true });
                result.GetAwaiter().GetResult();
                returnProduct = GetProduct(updated.productID);
                if (returnedProduct != null)
                    returnProduct._id = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
           
            return returnProduct;
        }
    }
}
