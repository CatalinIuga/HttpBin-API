# HTTPBIN

or BucketHead! or HTTP Head

## Flow for the app:

- user connection -> session gets initialized or extended
- create a session with the bucket
- return the bucket and session back to user
- use server sent events to replay any new requests

## TODO

- [x] update bucket time
- [x] service that expires buckets and deletes them from database
- [] add unit tests
- [] frontend -> button -> set session with a bucket id as claim
