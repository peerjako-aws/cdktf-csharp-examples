using System;
using Constructs;
using HashiCorp.Cdktf;

using System.Collections.Generic;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.S3;
using HashiCorp.Cdktf.Providers.Aws.Iam;
using HashiCorp.Cdktf.Providers.Aws.Lambdafunction;
using HashiCorp.Cdktf.Providers.Aws.Apigatewayv2;

namespace MyCompany.MyApp
{

    class LambdaFunctionProps
    {
        public string Path { get; set; }
        public string Handler { get; set; }
        public string RunTime { get; set; }
        public string StageName { get; set; }
        public string Version { get; set; }
    }

    class LambdaStack : TerraformStack
    {
        public LambdaStack(Construct scope, string id, LambdaFunctionProps config) : base(scope, id)
        {
            // define resources here
            new AwsProvider(this, "aws", new AwsProviderConfig{
                Region = "eu-west-1"
            });

            new random.RandomProvider(this, "random");

            random.Pet pet = new random.Pet(this, "random-name", new random.PetConfig {
                Length = 2
            });

            // Create a Terraform asset as zip archive from the path to the dist folder
            TerraformAsset asset = new TerraformAsset(this, "lambda-asset", new TerraformAssetConfig{
                Path = config.Path,
                Type = AssetType.ARCHIVE
            });

            // Create an S3 bucket to store our Lambda source code
            S3Bucket bucket = new S3Bucket(this, "bucket", new S3BucketConfig{
                BucketPrefix = "lambda-example-"
            });

            // Upload Lambda zip file to newly created S3 bucket
            S3Object lambdaArchive = new S3Object(this, "lambda-archive", new S3ObjectConfig{
                Bucket = bucket.Bucket,
                Key = config.Version + "/"+asset.FileName,
                Source = asset.Path
            });

            // Create Lambda role
            var lambdaRolePolicy = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                    ""Action"": ""sts:AssumeRole"",
                    ""Principal"": {
                        ""Service"": ""lambda.amazonaws.com""
                    },
                    ""Effect"": ""Allow"",
                    ""Sid"": """"
                    }
                ]
            }";
        
            IamRole role = new IamRole(this, "lambda-exec", new IamRoleConfig{
                Name = "lambda-example-" + pet.Id,
                AssumeRolePolicy = lambdaRolePolicy
            });

            // Add an execution role for lambda to write to CloudWatch logs
            new IamRolePolicyAttachment(this, "lambda-managed-policy", new IamRolePolicyAttachmentConfig {
                PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole",
                Role = role.Name
            } );
    
            // Create a lambda function environment variable
            var lambdaFuncEnv = new LambdaFunctionEnvironment{
                Variables = new Dictionary<string, string>
                {
                    {"table", "dyndb123"}
                }
            };

            // Create Lambda function
            LambdaFunction lambdaFunc = new LambdaFunction(this, "lambda-example-lambda", new LambdaFunctionConfig{
                FunctionName = "lambda-example-" + pet.Id,
                S3Bucket = bucket.Bucket,
                S3Key = lambdaArchive.Key,
                Handler = config.Handler,
                Runtime = config.RunTime,
                Role = role.Arn,
                Environment = lambdaFuncEnv
            });

            // Create and configure API gateway
            Apigatewayv2Api api = new Apigatewayv2Api(this, "api-gw", new Apigatewayv2ApiConfig{
                Name = pet.Id,
                ProtocolType = "HTTP",
                Target = lambdaFunc.Arn
            });

            new LambdaPermission(this, "apigw-lambda", new LambdaPermissionConfig{
                FunctionName = lambdaFunc.FunctionName,
                Action = "lambda:InvokeFunction",
                Principal = "apigateway.amazonaws.com",
                SourceArn = api.ExecutionArn + "/*/*"
            });

            // Output the url for the API endpoint
            new TerraformOutput(this, "url-" + config.StageName, new TerraformOutputConfig{
                Value = api.ApiEndpoint
            });
        }

        public static void Main(string[] args)
        {
            App app = new App();
            new LambdaStack(app, "lambda-hello-world", new LambdaFunctionProps
                {
                    Path = "../lambda-hello-world/dist",
                    Handler = "index.handler",
                    RunTime = "nodejs14.x",
                    StageName = "hello-world",
                    Version = "v0.0.3"
                }
            ); 
            new LambdaStack(app, "lambda-hello-name", new LambdaFunctionProps
                {
                    Path = "../lambda-hello-name/dist",
                    Handler = "index.handler",
                    RunTime = "nodejs14.x",
                    StageName = "hello-name",
                    Version = "v0.0.1"
                }
            );

            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}

