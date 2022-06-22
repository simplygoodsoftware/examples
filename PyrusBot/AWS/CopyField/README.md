# Copy Field AWS Bot

This demo bot will copy the content of one field to another in the Pyrus task.
Bot is intended to be deployed to AWS Labmda as a function, and to interact with Pyrus.


## The process of interaction

1. When a task is routed to the bot, Pyrus invokes a call to the AWS.
2. The bot gets parameters from the HTTP request from Pyrus.
3. The bot retrives codes of the form fields it will operate with.
4. It sends requests to Pyrus to get the form and the task from which it was invoked.
5. It generates a request to Pyrus to fill the field, approve the task and write a comment.
6. AWS sends this request back to Pyrus after finishing the bot program code.

## How to use

Solution consists of two projects:

- CopyFileBot - a bot that will be deployed to AWS Labmda.
- DemoRunner - a console application that can run a bot code locally without AWS.

| Mode                            | Description                                                                                                   |
|---------------------------------|---------------------------------------------------------------------------------------------------------------|
| DemoRunner + Pyrus              | You can run and test the bot locally using DemoRunner without AWS Labmda having only the valid Pyrus account. |
| DemoRunner + Pyrus + AWS Labmda | You can deploy the bot as a AWS Labmda function and interact from DemoRunner.                                 |
| Pyrus + AWS Labmda              | You can deploy the bot as a AWS Labmda function, create a bot in Pyrus and place it on the step of approvals. |


## Environment setup

### DemoRunner setup

Modify the Program.cs file in DemoRunner project:

1. Set bot authorization.

Open the bot page in Pyrus and copy the following information to the corresponding places in Program.cs.

- id of the bot - a number from the end of URL.
- login.
- security key.

2. Set task id.

Open the task that was created on setup phase and copy id from URL - a number from the end of URL.

### AWS Lambda function setup

1. Enter the AWS Management Console.
2. Go to Lambda service.
3. Click "Create function" button.
4. Do the following on the next page:
  * Ensure that "Author from scratch" pane is selected;
  * Enter the function name;
  * Select ".NET Core 3.1 (C#/PowerShell)" in the Runtime list;
  * Ensure that "x86_64" option is chosen in Architecture;
  * Click "Create function" button.
5. On the page of the function you just created click "Add trigger" button.
6. Do the following on the next page:
  * Select "API Gateway" option in the "Select a trigger" list;
  * Select "Create an API" option in the API list if it's not selected yet;
  * Click the "HTTP API" button in the API type block;
  * Select the "Open" option in the "Security" list;
  * Click "Add" button.
7. You will return back to the function page, then click the "API Gateway" button just above the "Add trigger" button.
8. Remember the API Endpoint URL from the trigger that was just created - it will needed to setup Pyrus bot.
9. Open the Visual Studio Developer Command Prompt.
10. Install the .NET Core Global Tools for AWS (if it's not installed yet) by this command: `dotnet tool install -g Amazon.Lambda.Tools`
11. Go to the directory where the CopyFieldBot project for AWS is located.
12. Run the command `dotnet lambda deploy-function CopyField`

### Pyrus setup

1. Create a form.

Add two text fields and give them the following codes: u_Source and u_Target.

![Screenshot](images/form.png)

2. Create a bot.

![Screenshot](images/bot.png)

3. Set the bot URL

Go to the bot profile and enter the URL of the AWS Lambda function that you just created.

4. Set bot settings.

Go to the bot profile and enter bot settings in json format. Names of the json elements are from *BotSetting.cs* file. Values are the codes of the fields that were created on step 1.

`{
    "SourceFieldCode": "u_Source",
    "TargetFieldCode": "u_Target"
}
`

![Screenshot](images/bot_profile.png)

5. Setup form workflow.

Return to the form that was created on step 2, go to Configure tab, then to Workflow.
Place the bot on the first step of approval list, and the author on the second like on the screenshot below.

![Screenshot](images/approvers.png)

6. Create a task.

Fill the form - enter any text in the source field.

![Screenshot](images/create_task.png)


## Running demo

### Expected results

If the setup procedures were done correctly, you'll see the following changes in the task:

- the target field is filled with content of the source field;
- comment was added;
- the task was approved by the bot;
- the task is to the second step.

### Using DemoRunner

Run the program from Visual Studio or CLI. 

### Using Pyrus workflow

1. Enter the task that you created during Pyrus setup.
2. Approve the task so it will move to stage 2.
3. Pyrus then will call the bot.
