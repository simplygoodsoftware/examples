# Copy Field AWS Bot

This demo bot will copy the content of one field to another in the Pyrus task.
Bot is intended to be deployed to AWS Labmda as a function, and to interact with Pyrus.

The process of execution is as following:

1. When a task is routed to the bot, Pyrus invokes a call to the AWS.
2. The bot gets parameters from the HTTP request from Pyrus.
3. The bot retrives codes of the form fields it will operate with.
4. It sends requests to Pyrus to get the form and the task from which it was invoked.
5. It generates a request to Pyrus to fill the field, approve the task and write a comment.
6. AWS sends this request back to Pyrus after finishing the bot program code.

You can run and test the bot locally without AWS Lambda having only the valid Pyrus account.

Solution consists of two projects:

- CopyFileBot - a bot that will be deployed to AWS Lambda.
- DemoRunner - a console application that can run a bot code locally without AWS.

## Preparation for local run

### Setup Pyrus

1. Create a form.

Add two text fields and give them the following codes: u_Source and u_Target.

![Screenshot](images/form.png)

2. Create a bot.

![Screenshot](images/bot.png)

3. Set bot settings.

Go to the bot profile and enter bot settings in json format. Names of the json elements are from *BotSetting.cs* file. Values are the codes of the fields that were created on step 1.

`{
    "SourceFieldCode": "u_Source",
    "TargetFieldCode": "u_Target"
}
`

![Screenshot](images/bot_profile.png)

4. Setup form workflow.

Return to the form that was created on step 2, go to Configure tab, then to Workflow.
Place the bot on the first step of approval list, and the author on the second like on the screenshot below.

![Screenshot](images/approvers.png)

5. Create a task.

Fill the form - enter any text in the source field.

![Screenshot](images/create_task.png)

### Setup DemoRunner

Modify the Program.cs file in DemoRunner project:

1. Set bot authorization.

Open the bot page in Pyrus and copy the following information to the corresponding places in Program.cs.

- id of the bot - a number from the end of URL.
- login.
- security key.

2. Set task id.

Open the task that was created on setup phase and copy id from URL - a number from the end of URL.

### Run DemoRunner

Run the program from Visual Studio or CLI. If the setup procedured were done correctly, you'll see the following changes in the task:

- the target field is filled with content of the source field;
- comment was added;
- the task was approved by the bot;
- the task is to the second step.
