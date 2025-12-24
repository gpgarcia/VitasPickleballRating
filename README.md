# VitasPickleballRating
Pickleball Rating system for use by Vitas players.
1. It keeps track of players, and games played.
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

Next remove Redundant Navigational properties.
1. `DbSet<GamePrediction>`
1. GameDetail and `DbSet<GameDetail>` and mapping.
1. TypeGame.Games
1. TypeFacility.Facilities
1. Player.TeamXPlayerY (4 of them)
1. PlayerRating.Game
1. Game.PlayerRating
1. Facility.TypeFacility
1. Facility.Games

### Setting up the Database
install Microsoft/sqlpackage tool
```sh
dotnet tool install --global microsoft.sqlpackage
```

Start the mssql server

  ``` sh
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong?Password" -e "MSSQL_PID=Express" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

  ```
Deploy the dacpac

Go to the database project folder  that also contains the .publish.xml file, find the DACPAC in a sub folder of bin.

Run the following command to deploy the dacpac to the local mssql server:
if you have a publish profile set up, you can use:
``` cmd
sqlpackage.exe /Action:Publish /SourceFile:vpr\bin\Debug\net4.7.2\vpr.dacpac /Profile:vpr\vpr.publish.xml

```
or you can specify the parameters directly:
``` cmd
sqlpackage.exe /Action:Publish /SourceFile:"vpr\bin\Debug\net4.7.2\vpr.dacpac" /TargetServerName:localhost /TargetDatabaseName:vpr /TargetTrustServerCertificate:True /TargetUser:sa /TargetPassword:"YourStrong?Password"

```

*__NOTE:__*
To convert a visual Studio SSDT database project to a SDK project suitable for dotnet core follow instruction 
[Here](https://learn.microsoft.com/en-us/sql/tools/sql-database-projects/howto/convert-original-sql-project?view=sql-server-ver17&pivots=sq1-visual-studio).
You will not be able to publish using VS2022 and under after converting. The above commandline methods will work locally and in a pipeline.

### To build the Docker Image
From the solution folder run:
``` sh
docker build -f PickleBallAPI/Dockerfile -t api:latest .
```
### To run the application
``` sh
docker run -d -e ASPNET_ENVIRONMENT=Development -p 8080:8080 api

```
### Warning
To get the 'Generate Code Map to work, deploy vpr to (localdb)\MSSQLlocalDB.

### Database Schema
The Schema is shown [Here](./VPR_Schema.png)

## Building
