using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("http://localhost:6000/hub")
                .Build();
            var cts = new CancellationTokenSource();
            //using var message = 
                connection.On<string>("message", Console.WriteLine);
            //using var clients =
                connection.On<string[]>(
                "clients",
                s =>
                {
                    Console.WriteLine("Clients:");
                    foreach (var item in s)
                    {
                        Console.WriteLine($"\t{item}");
                    }
                });
            //using var join = 
                connection.On<string>("join", g => Console.WriteLine($"Joined group {g}."));
            //using var chat = 
                connection.On<string, string, string>("chat", (g, c, m) => Console.WriteLine($"In group {g}, {c} sayed: {m}"));
            connection.Closed += e => { cts.Cancel(); return Task.CompletedTask; };
            connection.Reconnecting += e => { Console.WriteLine("Reconnecting ..."); return Task.CompletedTask; };
            connection.Reconnected += s => { Console.WriteLine("Reconnected."); return Task.CompletedTask; };
            Console.CancelKeyPress += (s, e) => cts.Cancel();
            var t = Start(connection, cts.Token);
            t.Wait();
        }

        private static async Task Start(HubConnection connection, CancellationToken token)
        {
            await connection.StartAsync();
            ShowHelp();
            await connection.SendAsync("hello");

            while (!token.IsCancellationRequested)
            {
                var task = Task.Run(Console.ReadLine);
                if (await Task.WhenAny(task, Task.Delay(-1, token)) != task)
                {
                    break;
                }
                var line = await task;
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                line = line.Trim();
                if (line == "help")
                {
                    ShowHelp();
                    continue;
                }
                if (line == "discover")
                {
                    await connection.SendAsync("discover");
                    continue;
                }
                if (line.StartsWith("chatwith "))
                {
                    var peers = line.Substring("chatwith ".Length).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (peers.Length == 0)
                    {
                        ShowHelp();
                        continue;
                    }
                    await connection.SendAsync("chatwith", peers);
                    continue;
                }
                if (line.StartsWith("chat "))
                {
                    var args = line.Substring("chat ".Length).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length < 2)
                    {
                        ShowHelp();
                        continue;
                    }
                    await connection.SendAsync("chat", args[0], args[1]);
                    continue;
                }
                Console.WriteLine("Bad command.");
                ShowHelp();
            }

            await connection.StopAsync();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  help           show this help.");
            Console.WriteLine("  discover       show peers.");
            Console.WriteLine("  chatwith       chat with peers, e.g.: `chatwith a b c`");
            Console.WriteLine("  chat           chat with peers, e.g.: `chat {groupname} message`");
        }
    }
}
