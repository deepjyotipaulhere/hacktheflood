# Gamification
Azure Endpoint https://gamify-hacktheflood2.azurewebsites.net/

## Users API
```
POST /users/            // Add user {"name": String}
GET /users              // Get all users list
GET /users/<id>         // Get user details by id:int
```

## Objects API
```
POST /objects/          // Create an object {"item":String, "point":int}
GET /objects            // Get a list of created objects
GET /objects/<id>       // Get object details by id:int
```

## Point API
```
POST /points/           // Save a user's score {"user":int(user's ID), "points":int}
GET /points             // Get score leaderboard order by descending points
```