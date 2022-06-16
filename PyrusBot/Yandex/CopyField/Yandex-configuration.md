# Copy Field Yandex Bot

This demo bot will copy the content of one field to another in the Pyrus task.
Bot is intended to be deployed to Yandex as a cloud function, and to interact with Pyrus.

The process of installation and configuration in Yandex is following:

1. Go to Yandex cloud console https://console.cloud.yandex.ru
2. In the "Services" pane press "Colud functions".
3. In the list of functions press "Create function" button on the right upper conner.
4. Give a name to the cloud function, and press "Create".
5. On the Editor page choose runtime type - select .NET Core, and press "Continue".
6. Press "ZIP archive" button.
7. Press "Choose a file" button and select zip-archive with project executables [1].
8. Enter Bots.CopyFieldBot.Bot in the "Entrypoint" fiels. It must contain namespace and class name of the bot (a tooltip is misconfusing).
9. Enter 59 in the "Timeout" field.
10. Go to "Overview" by selecting an item in the left menu.
11. Switch on the "Public function" button to be able to make requests without authorization.

[1] You can make project executables with .NET CLI with the following command: `dotnet publish -o path_to_deployment_folder`
