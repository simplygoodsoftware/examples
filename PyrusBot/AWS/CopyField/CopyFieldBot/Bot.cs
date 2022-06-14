using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using PyrusApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PyrusTask = PyrusApiClient.Task;
using Task = System.Threading.Tasks.Task;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Demo bot to copy text in task from one form field to another.
	/// </summary>
	public class Bot
	{
		private TaskWithComments _task;
		private int _botId;
		private BotSettings _settings;
		private PyrusClient _apiClient;

		/// <summary>
		/// Method that is called by AWS Lambda as a starting point.
		/// AWS finds this by using assembly, namespace and method names defined in aws-lambda-tools-defaults.json.
		/// </summary>
		/// <param name="request">API request to AWS.</param>
		/// <param name="context">AWS execution context.</param>
		/// <returns></returns>
		public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
		{
			try
			{
				await ParseAndRun(request);

				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonSerializer.Serialize(request),
				};
			}
			catch (Exception ex)
			{
				try
				{
					LambdaLogger.Log(ex.ToString());
				}
				catch { }

				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = "Unknown exception:\n" + ex.Message
				};
			}
		}

		/// <summary>
		/// Deserialize AWS API request and execute bot logic.
		/// </summary>
		/// <param name="request">API request to AWS.</param>
		/// <returns>Task object for async execution.</returns>
		public async Task ParseAndRun(APIGatewayProxyRequest request)
		{
			var botRequest = JsonSerializer.Deserialize<BotRequest>(request.Body);

			if (botRequest == null)
				throw new Exception("Incorrect request to invoke the bot.");

			await ParseParameters(botRequest);

			var result = await Execute();

			if (result != null)
				ApiHelper.EnsureSuccess(await _apiClient.CommentTask(botRequest.TaskId, result));
		}

		/// <summary>
		/// Parse parameters from the Pyrus request with which the bot was invoked.
		/// </summary>
		/// <param name="botRequest">Request from Pyrus.</param>
		/// <returns>Task object for async execution.</returns>
		private async Task ParseParameters(BotRequest botRequest)
		{
			// create Pyrus API client for subsequent http requests
			_apiClient = new PyrusClient
			{
				Token = botRequest.AccessToken
			};

			_task = ApiHelper.EnsureSuccess(await _apiClient.GetTask(botRequest.TaskId)).Task;
			if (_task == null)
				throw new Exception($"Failed to get task '{botRequest.TaskId}'.");

			_botId = botRequest.UserId;

			if (!string.IsNullOrWhiteSpace(botRequest.BotSettings))
				_settings = JsonSerializer.Deserialize<BotSettings>(botRequest.BotSettings);
		}

		/// <summary>
		/// Execute bot logic.
		/// </summary>
		/// <returns>Request with task comment to send to Pyrus.</returns>
		private async Task<TaskCommentRequest> Execute()
		{
			// get the form of task from which the bot was invoked
			var form = ApiHelper.EnsureSuccess(await _apiClient.GetForm(_task.FormId.Value));

			// fields from the form
			var formSourceField = GetFormFieldByCode(form, _settings.SourceFieldCode);
			var formTargetField = GetFormFieldByCode(form, _settings.TargetFieldCode);

			// get the field from the task (with value)
			var taskSourceField = GetTaskField(_task, formSourceField) as FormFieldText;

			// create a field with data to be modified
			var fieldChanges = FormField.Create<FormFieldText>(formTargetField.Id.Value).WithValue(taskSourceField.Value);

			var changes = new List<FormField>();
			changes.Add(fieldChanges);

			return new TaskCommentRequest
			{
				ApprovalChoice = ApprovalChoice.Approved,
				Text = $"Field value was copied by bot {_botId}.",
				FieldUpdates = changes,
			};
		}

		/// <summary>
		/// Get field from the Pyrus form.
		/// </summary>
		/// <param name="form">Pyrus form.</param>
		/// <param name="code">Code of the field.</param>
		/// <returns></returns>
		private static FormField GetFormFieldByCode(FormResponse form, string code)
		{
			if (string.IsNullOrWhiteSpace(code))
				throw new ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code));

			var field = form.FlatFields?.FirstOrDefault(f => f?.Info?.Code == code);

			if (field?.Id == null)
				throw new Exception($"Field with code '{code}' is not found in form {form.Id}.");

			return field;
		}

		/// <summary>
		/// Get field from the Pyrus task with value.
		/// </summary>
		/// <param name="task">Pyrus task.</param>
		/// <param name="field">Field from the Pyrus form.</param>
		/// <returns></returns>
		private static FormField GetTaskField(PyrusTask task, FormField field)
		{
			var result = task.FlatFields?.FirstOrDefault(f => f?.Id == field.Id.Value);

			if (result == null)
				throw new Exception($"Field with ID '{field.Id.Value}' is not found in task {task.Id}.");

			return result;
		}
	}
}
