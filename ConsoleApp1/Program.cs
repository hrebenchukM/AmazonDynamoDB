using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                // ❗ ВСТАВЬ СВОИ КЛЮЧИ (как в видео)
                string access_key = "AKIAVGDN36AR762WPJ22";
                string secret_key = "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";

                var credentials = new BasicAWSCredentials(access_key, secret_key);

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.USEast1
                };

                using var client = new AmazonDynamoDBClient(credentials, config);

                var tableName = "Students";

                var request = new ScanRequest
                {
                    TableName = tableName
                };

                var response = await client.ScanAsync(request);

                var sb = new StringBuilder();
                int index = 1;

                sb.AppendLine("STUDENTS LIST");
                sb.AppendLine("======================");
                sb.AppendLine();

                while (true)
                {
                    foreach (var item in response.Items)
                    {
                        sb.AppendLine($"Student #{index}");

                        foreach (var attribute in item)
                        {
                            sb.AppendLine($"{attribute.Key}: {GetValue(attribute.Value)}");
                        }

                        sb.AppendLine("----------------------");
                        index++;
                    }

                    if (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count > 0)
                    {
                        request.ExclusiveStartKey = response.LastEvaluatedKey;
                        response = await client.ScanAsync(request);
                    }
                    else
                    {
                        break;
                    }
                }

                File.WriteAllText("students.txt", sb.ToString());

                Console.WriteLine("Data successfully written to students.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string GetValue(AttributeValue value)
        {
            if (!string.IsNullOrEmpty(value.S))
                return value.S;

            if (!string.IsNullOrEmpty(value.N))
                return value.N;

            if (value.BOOL.HasValue)
                return value.BOOL.Value.ToString();

            return "null";
        }
    }
}
