# HotelBooking

Thanks for your time to review this repo, please explore it all.

Next are some of my considerations when I coded this sample:

## General:
* As this app is insecure (does not have authentication and authorization) we cannot ensure a user will not alter the info of others.
* I'll assume there is no dead time between reservations to maintain room etc.
* Nullable types have been selected, it is important to not fuse nulls on the code, this prevents null exceptions along the code.
* Use interfaces, SOLID, and interface segregation.
* There are many tools in the market to perform data layers like EntityFramework, but I've chosen not to use them as I want to you see my work.
* Async DB commands are not only good, but they are also critical in achieving scale, throughput, and latency. So I've implemented them async.
* Logging has been implemented to catch server errors and not reveal details to the consumer.
* Cache is implemented for the get rooms endpoint which does not require a short refresh. Also, a very short cache has been implemented for bookings endpoints. This will improve a little availability.
* Exception filter for controllers has been implemented to not repeat exceptions everywhere. The stack trace is filtered for 500 server errors, for security reasons, like a hacker seeing the stack trace.
* Try to do not to use reflection as this may make the execution heavy.
 
## Layers: 
Following KISS (Keep It Simple…),
* Domain, containing the objects to handle data along the business layer.
* Business, handles the data objects make some validations, and throw exceptions when code fails, this layer will validate the data anytime no matter if that was validated already in the UI.
* Persistence, stores and retrieves data from the data source, interfaces are servers to be able to interchange between persistence implementations, and SQL implementation will be made. NO entity framework is made because it has some disadvantages for the use of reflection in a production scenario.
 
## Resource manager: 
No resource manager has been implemented since the langs and error messages are the responsibility of a user interface which is not included in this API.
 
## Unit tests: 
To demonstrate the implementation I've tested some of the static validations.
 
## Database: 
SQL Server for easy implementation has been chosen, please use the attached scripts to generate the model. See below some considerations to create the DB model efficiently.
 
### Rooms:
* The room is only one but is good to have a list of rooms with one in them, NFR: Adaptability, configurability etc
* Choosing easy tinyint as there is only one room per hotel. (this might vary from BD/Implementation to others)
* Choosing nvarchar because the information might be stored in multiple langs in the future. (no lang )
* Choosing no null because a room must have a name.
* Choosing non-incremental, a table with countable rows should not contain extra logic to manage indexes, also this is not a large transactional table.
* Room name is unique.
 
### Guests:
* just for sake of the test the guest will be only identified for the email.
* A bigint key to allow many guests to register, autoincremented.
* The email must be unique
* will be stored in lowercase
* varchar as an email should not contain special characters
 
### Bookings:
* A bigint key to allow many bookings, autoincremented
* No unique between booking and guest, no sense due to the booking dynamics
* From and To dates will be stored in date format assuming these already have been converted to UTC, assuming many people around the world will make requests.
* Booking time instead will store offset part, this date will not serve any purpose after calculating the booking date rules but generally is good to keep the creation date and modification date for administration concerns. NFR: traceability.
