# HTTPBIN

Test you HTTP calls with this API. It captures request and replays the details back to you.

## Bulding and running

You need to have a PostgreSQL database running. Modify the connection string inside appsettings.json to match your database.

```bash
# Install dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Run the application
dotnet run
```

## Design

The point of this project is to offer a simple way for testing HTTP calls. It captures the request and replays it back to you, the frontend is going to display the request details and the response in a more readable way.

Each user will be able to create a bucket, wich will hold the requests. The bucket will have a unique id, and only the user will have access to it. To prevent unauthorized access to the bucket, when a bucket gets created we also inlcude a JWT token that will be used to get access to the bucket. In additon, each bucket get a TTL (time to live) value, after the TTL expires the bucket will be deleted.

The frontend will be a simple SPA, that will allow the user to create a bucket, and make HTTP calls to the bucket. The frontend will also display the request details and the response. The fronted can be found [here](...).

## Available endpoints

The base URL is `http://localhost:7134/api/` and the available endpoints are:

### Buckets

- `POST /buckets` - Create a new bucket, and implicit a new JWT token.
- `GET /buckets` - Get the bucket details from the JWT token.
- `PATCH /buckets` - Update the bucket TTL.
- `DELETE /buckets` - Delete the bucket from the JWT token.

### Requests

- `GET /requests/{id}` - Get the request details.
- `DELETE /requests/{id}` - Delete the request.
- `GET /requests/{id}/bucket/{bucketId}` - Get all the requests from a bucket.

### Request Reciever

- `GET/HEAD/POST/PUT/OPTIONS/PATCH/DELETE /{bucketId}/{*}` - Recieve the request and store it in the database. Optionaly, it can take route parameters aswell, since it matches any path that has the base URL and the bucketId specified.
