using Acr.UserDialogs;
using MonkeyCache.FileStore;
using Plugin.Connectivity.Abstractions;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace XFWeatherApp.Services
{
    public class BaseService
    {
        protected string Host { get; set; }
        protected readonly IUserDialogs _userDialogs;
        protected readonly IConnectivity _connectivity;
        protected bool IsConnected { get; set; }
        protected bool IsReachable { get; set; }
        protected readonly Dictionary<int, CancellationTokenSource> RunningTasks = new Dictionary<int, CancellationTokenSource>();
        protected readonly Dictionary<string, Task<HttpResponseMessage>> TaskContainer = new Dictionary<string, Task<HttpResponseMessage>>();
        public BaseService(IUserDialogs userDialogs, IConnectivity connectivity, string baseUrl)
        {
            _userDialogs = userDialogs;
            _connectivity = connectivity;
            IsConnected = _connectivity.IsConnected;
            _connectivity.ConnectivityTypeChanged += OnConnectivityChanged;
            Host = ApiHostName(baseUrl);
            Barrel.ApplicationId = "XFWeatherApp";
        }
        protected async Task<TData> RemoteRequestAsync<TData>(Task<TData> task)
            where TData : HttpResponseMessage, new()
        {
            TData data = new TData();

            IsReachable = await _connectivity.IsRemoteReachable(Host);

            if (!IsConnected || !IsReachable)
            {
                var errorMessage = "Oops! Something went wrong, {0}";
                var stringResponse = !IsConnected ? string.Format(errorMessage, "check your internet connection")
                    : string.Format(errorMessage, "couldn't stablish connection with the server.");
                data.StatusCode = HttpStatusCode.BadRequest;
                data.Content = new StringContent(stringResponse);

                _userDialogs.Toast(stringResponse, TimeSpan.FromSeconds(1));
                return data;
            }

            data = await Policy
                .Handle<WebException>()
                .Or<ApiException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync
                (
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
                .ExecuteAsync(async () =>
                {
                    var result = await task;
                    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await _userDialogs.AlertAsync(new AlertConfig
                        {
                            Title = "Error",
                            Message = "Invalid API Key.",
                            OkText = "Ok"

                        });
                    }
                    return result;
                });

            return data;
        }
        protected async Task<HttpResponseMessage> ExecRequest(Task<HttpResponseMessage> request)
        {
            var cancellationToken = new CancellationTokenSource();
            var task = RemoteRequestAsync(request);
            RunningTasks.Add(task.Id, cancellationToken);
            return await task;
        }
        string ApiHostName(string host)
        {
            return Regex.Replace(host, @"^(?:http(?:s)?://)?(?:www(?:[0-9]+)?\.)?", string.Empty, RegexOptions.IgnoreCase)
                    .Replace("/", string.Empty);
        }
        void OnConnectivityChanged(object sender, ConnectivityTypeChangedEventArgs e)
        {
            IsConnected = e.IsConnected;

            if (!IsConnected)
            {
                var items = RunningTasks.ToList();
                foreach (var item in items)
                {
                    item.Value.Cancel();
                    RunningTasks.Remove(item.Key);
                }
            }
        }


    }
}
