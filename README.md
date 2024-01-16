# Sportsbet Coding Challenge
## Design Decisions
### Why .net 8.0.?
.net is a cross platform tool that will work on Mac, Windows and Linux, so there should be no worry on whether this will work on the testers device.
It is good for data processing and the way this challenge is setup it seemed like an api would fit well. 

### CQRS And Entity Framework
Cqrs helps with the [single-responsibility principle](https://en.wikipedia.org/wiki/Single-responsibility_principle). Requests will usually have a focused task so there won't be a muddle of code for different purposes.
Downsides? It is easy for commands and queries to do the same thing when the project is scaled up (different developers may not know the request is already created).
Entity framework is great way to implement the repository pattern, allowing for a sergerate interface that is easy to extend but hard to modify. It also has the nice ability to interface with other db's easily. For the test I use an in memory database as an example.

### Why not update the database to sql server or mysql
Safety and time.
Ideally I would move away from using sqllite but mostly due to time constraints, I couldn't change the datasource.
Also as this a public repo, the datasource would need to be made public which is never smart.


## Running the project
- Download or pull this repo
### If you have Visual Studio installed
1. Open the soloution
2. Hit the start button and a browser should open, navigating to the swagger url. If it doesn't it will be located here: https://localhost:7146/index.html.
3. Goto excute heading below.
### If you DON'T have Visual studio installed
1. Open up a command console or terminal window and naviate to the root of the project.
2. Run the "dotnet restore" command.
3. Navigate to "depthchart.api" project folder in terminal/cmd.
4. Run the "dotnet run" command which will launch the api.
5. In a browser navigate to the swagger page: http://localhost:5024/index.html.
6. Goto excute heading below.
### Execute
1. In the "Depth Chart API" swagger page, click an endpoint to "try it out".
2. Request bodies should be auto filled in but you can modify them as needed.
3. Press the "Execute" button once you have input details.
4. The output will be displayed in the "Responses" section.
5. You are done!
#### Execution Notes
I have added an extra two endpoints, one for adding players and one for seeing all players added.
This means that all players are added via a player id. I felt that allowing someone to add a player whilst they were adding to the depth chart was too much for one endpoint.

## Running the tests
### If you have Visual Studio installed
1. In the test explorer, hit the run all tests button.
2. The 34 tests should pass.
### If you DON'T have Visual studio installed
1. Open up a command console or terminal window and naviate to the root of the project.
2. Run the "dotnet restore" command.
3. Run the "dotnet test" command.
4. The 34 tests should pass -> Displayed in green text.

## Future Improvements
### Missed Intergration Tests
I started the process of creating intergration tests, but due to time, I left most of them out. 
With the unit tests I have in place, I would still feel relatively comfortable with releasing this code.
### Adding/Sport Team
I started the process of adding players to sports, so you could create depth charts base upon different sports.
The same could be done for teams as I mention that would important for the future too.