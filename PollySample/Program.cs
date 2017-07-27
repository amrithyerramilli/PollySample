using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollySample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(
                exceptionsAllowedBeforeBreaking: 1,
                durationOfBreak: TimeSpan.FromSeconds(2), 
                onBreak: (exception, span) =>
                {
                    Console.WriteLine(string.Format("onBreak -> Error while opening connection. Exception: {0}, Span: {1}",
                        exception.Message, span));
                }, 
                onHalfOpen: () =>
                {
                    Console.WriteLine("onHalfOpen");
                },
                onReset: () =>
                {
                    Console.WriteLine("onHalfOpen");
                });

            Parallel.For(1, Int32.MaxValue, (i, state) =>
            {
                Console.WriteLine(string.Format("Openning Connection {0}", i));
                circuitBreakerPolicy.Execute(() => OpenSqlConnection());
            });
        }

        public static void OpenSqlConnection()
        {
            const string connectionString = "";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Thread.Sleep(10000);
                connection.Close();
            }
        }
    }
}