using PyrusApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PyrusTask = PyrusApiClient.Task;
using Task = System.Threading.Tasks.Task;

namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Demo bot to copy text in task from one form field to another.
	/// </summary>
	public class Bot
	{
		private readonly TimeSpan YandexDefaultTimeSpan = TimeSpan.FromSeconds(59);

		private TaskWithComments _task;
		private int _botId;
		private BotSettings _settings;
		private PyrusClient _apiClient;

		/// <summary>
		/// Handler that is invoked by Yandex
		/// </summary>
		/// <param name="input">Request</param>
		/// <returns></returns>
		public string FunctionHandler(string input)
		{
			try
			{
				var request = JsonSerializer.Deserialize<Request>(input)?.Body;

				var body = request.Contains("{")
					? request
					: System.Text.Encoding.Default.GetString(Convert.FromBase64String(request));

				// making some changes because Yandex modifies the request
				// that prevents to correct deserialization
				body = body.Trim('\"').Replace(@"\u0022", @"""").Replace(@"\\", @"\");

				var botRequest = JsonSerializer.Deserialize<BotRequest>(body);

				var mainTask = Task.Run(() => ParseAndRun(botRequest));
				if (mainTask.Wait(YandexDefaultTimeSpan))
				{
					var response = new BotResponse
					{
						StatusCode = 200,
						Body = "Success"
					};

					return SerializeResponse(response);
				}

				var timeoutResponse = new BotResponse
				{
					StatusCode = 500,
					Body = "Failed to handle response in the specified time limit. Please, try again later or contact support."
				};

				return SerializeResponse(timeoutResponse);
			}
			catch (Exception ex)
			{
				var message = ex.Message;
				if (ex is AggregateException agr)
				{
					message = string.Join("\r\n", agr.InnerExceptions.Select(x => x.Message));
				}

				Console.WriteLine(message);

				var errorResponse = new BotResponse
				{
					StatusCode = 500,
					Body = message
				};

				return SerializeResponse(errorResponse);
			}
		}

		/// <summary>
		/// Makes a JSON with CamelCase.
		/// </summary>
		/// <param name="src">Bot response to be serialized.</param>
		/// <returns></returns>
		private string SerializeResponse(BotResponse src)
		{
			var serializeOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};

			return JsonSerializer.Serialize(src, serializeOptions);
		}

		/// <summary>
		/// Deserialize request and execute bot logic.
		/// </summary>
		/// <param name="request">API request to Yandex.</param>
		/// <returns>Task object for async execution.</returns>
		private async Task ParseAndRun(BotRequest botRequest)
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

			var result = await Execute();

			if (result != null)
				ApiHelper.EnsureSuccess(await _apiClient.CommentTask(botRequest.TaskId, result));
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
