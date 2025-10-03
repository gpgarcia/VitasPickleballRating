# VitasPickleballRating
Pickleball Rating system for use by Vitas players.
1. It keeps track of players, teams and games played.
1. It estimates the score of a game between two teams.
1. It computes a new rating for each player after a game.
1. It displays win-loss standings of player and their rating..
1. It displays a players rating over time.

## Getting Started
When change the Database schema, you will need to regenerate the EF model.
To regenerate the EF model from the database, run the following command in the Package Manager Console:
```
Scaffold-DbContext -Connection "Server=(localdb)\ProjectModels;Database=vpr;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -NoOnConfiguring -Force -OutputDir Models

```
### Warning
To get the 'Generate Code Map to work, deploy vpr to (localdb)\MSSQLlocalDB.

### Database Schema
The Schema is shown [Here](./VPR_Schema.png)

## Building
