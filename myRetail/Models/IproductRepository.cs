namespace myRetail.Models
{
    public interface IproductRepository
    {
        /// <summary>
        /// this method takes in a product ID and returns a product object
        /// </summary>
        /// <param name="id">product ID</param>
        /// <returns>product object</returns>
        Product GetProduct(int id);

        /// <summary>
        /// this method takes in a product object and if it exists, the product is updated with given information
        /// if the product does not exist this method inserts the product in the database
        /// </summary>
        /// <param name="updated">an updated product to be inserted into the database</param>
        /// <returns>the updated product object</returns>
        Product updateProduct(Product updated);
    }
}
