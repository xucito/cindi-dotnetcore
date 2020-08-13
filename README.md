# Cindi.DotNetCore
Library to implement a .NET Core based Worker Bot.

## Installation

`PM> Install-Package Cindi.DotNetCore.BotExtensions`

## Usage

It is advised to create bots based on the API Template provided by Visual Studio. The usage below is a API Project based bot. 

### 1. Define Bot Handler

Create a new class that implements the abstract class `WorkerBotHandler<WorkerBotHandlerOptions>` where your step handlers will be defined.

There is a abstract method that needs to be overwritten called `HandleStep(Step step)`.

```csharp
public class MyBotHandler : WorkerBotHandler<WorkerBotHandlerOptions>
{
	public MyBotHandler(IOptionsMonitor<WorkerBotHandlerOptions> options,, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
	{

	}

	public override Task<UpdateStepRequest> HandleStep(Step step)
	{
		throw new NotImplementedException();
	}
}
```

### 2. Update ConfigureServices (startup.cs)

In the startup.cs update the ConfigureServices function to add the bot handler to the service collection.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddWorkerBot<MyBotHandler>(o =>
	{
		o.NodeURL = Configuration.GetValue<string>("cindiurl"); //the url of cindi
		o.SleepTime = 100; //sleep time in between querying for tasks in ms
		o.StepTemplateLibrary = new List<StepTemplate>() {
			//Include here your step Templates
		};
		o.AutoStart = true; //Set to true to auto start searching the cindi queue
	});

	services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
}
```

### 3. Enable on program startup 
If the workerbot should start querying Cindi on startup, add the bot to your Configure function in startup.cs

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, MyBotHandler handler)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}

	app.UseMvc();
}
```

### 4. Add Options

### 4. (Optional) Add a controller to be able to invoke the Steps ad-hoc for testing

```csharp
[Route("api/[controller]")]
public class BotController : BotController<CindiVCDBotHandler>
{
	public BotController(CindiVCDBotHandler bot, ILoggerFactory logger) : base(bot, logger)
	{

	}
}
```