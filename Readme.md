MyRetail Case study

Prerequisites
visual studio 2017
ASP.NET Core 2.0
Mongo DB

Preparation. 
Start the mongo service
Create a database with the name "products"
create a collection with the name "products"

if you wish to change the name on the database and collection you can do so but make sure you update the it in appsetting.Development.json

default mongo url is "mongodb://localhost:27017"
if you have a different url make sure to update it in appsetting.Development.json
Running the tests
The product API listens at : http://localhost:55971/product responds to GET and PUT verbs
url can be changed in launchsettings.json

import the solution in visual studio and run the application.

EX: GET: http://localhost:55971/product
	-->returns: "please provide the product id."

    GET: http://localhost:55971/product/16696652
	-->returns: {"_id":null,"productID":16696652,"productName":"Beats Solo 2 Wireless - Black","productPrice":{"price":"235.99","currency":"USD"}}

    PUT: http://localhost:55971/product/16696652
	 with the same product id and a full body of the product	 
	 modify the price at : "product>price>listPrice>price" in the JSON object
    	-->returns: {
    "_id": null,
    "productID": 16696652,
    "productName": "Beats Solo 2 Wireless - Black",
    "productPrice": {
        "price": "789.99",
        "currency": "USD"
    }
}

Note: if a put request is received and the product is not in the local database the product will be created with the given information

Built with
ASP.Net Core 2.0
mongocsharp driver 2.6
Mongo DB 3.65

Author Ghislain Twagirayezu
email: twagghi@gmail.com