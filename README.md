# powerplant-coding-challenge

Calculate how much power each of a multitude of different powerplants need to produce (a.k.a. the production-plan) when the load is given and taking into account the cost of the underlying energy sources (gas, kerosine) and the Pmin and Pmax of each powerplant.

## The Power Calculator solution

## Getting Started

### Requirement

- .NET 7
- Visual Studio 2022 
- Docker

## How to build and launch the API

### Visual Studio 2022

Open Visual Studio and following instructions:

- Open `PowerPlantCodingChallenge.sln` in root folder.
- Right click into project `PowerPlantCodingChallenge.API` and select `Set as Startup Project`.
- Click on `IIS Express` run or type <kbd>F5</kbd> to **build and run**.

### Docker

Open a command prompt and navigate to the root folder, where contains the `Dockerfile` and type the following command:

- To build the image:

```
docker build -t api-powerplantcodingchallenge-image -f Dockerfile .
```

- To run the container:

```
docker container run -d --name powerplantcodingchallenge-container -p 8888:80 api-powerplantcodingchallenge-image
```

- To stop the container:

```
docker stop powerplantcodingchallenge-container
```

- To start the stopped container:

```
docker start powerplantcodingchallenge-container
```

## How to call the API on Port 8888

You can access the API by sending a POST request to the following URL **http://localhost:8888/PowerPlan** with the JSON payload in the body.

> The `PowerPlantCodingChallenge.Tests/payload` folder contains payload examples for tests along with unit test.