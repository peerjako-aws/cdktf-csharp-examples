using System;
using Constructs;
using HashiCorp.Cdktf;

using System.Collections.Generic;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Aws.Iam;

namespace MyCompany.MyApp
{
    class MyApp : TerraformStack
    {
        public MyApp(Construct scope, string id) : base(scope, id)
        {
            // define resources here
            new AwsProvider(this, "AWS", new AwsProviderConfig { Region = "eu-north-1" });
            
            Vpc vpc = new Vpc(this, "vpc", new VpcConfig{
                CidrBlock = "10.0.0.0/16",
                EnableDnsHostnames = true,
                EnableDnsSupport = true,
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
            });
            
            Subnet subnet = new Subnet(this, "subnet", new SubnetConfig{
                VpcId = vpc.Id,
                CidrBlock = "10.0.10.0/24",
                AvailabilityZone = "eu-north-1a",
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
            });

            sessionmanager.Sessionmanager ssm = new sessionmanager.Sessionmanager(this, "ssm", new sessionmanager.SessionmanagerOptions{
                BucketName = "my-session-logs-peerjako",
                AccessLogBucketName = "my-session-access-logs-peerjako",
                VpcId = vpc.Id,
                EnableLogToS3 = true,
                EnableLogToCloudwatch = true,
                VpcEndpointsEnabled = true,
                VpcEndpointPrivateDnsEnabled = true,
                SubnetIds = new string[]{subnet.Id},
            });


            NetworkInterface networkInterface = new NetworkInterface(this, "ec2-network-interface", new NetworkInterfaceConfig{
                SubnetId = subnet.Id,
                PrivateIp = "10.0.10.100",
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
            });
            
            DataAwsAmi latestAmazonLinux2Ami = new DataAwsAmi(this, "latest-ami", new DataAwsAmiConfig{
                MostRecent = true,
                Owners = new string[]{"amazon"},
                Filter = new DataAwsAmiFilter[]{
                    new DataAwsAmiFilter{
                        Name = "name",
                        Values = new string[]{"amzn2-ami-hvm-*-x86_64-gp2"}
                    }
                }
            });

            DataAwsIamPolicyDocumentStatement statement = new DataAwsIamPolicyDocumentStatement{
                Actions = new string[]{"ssm:UpdateInstanceInformation",
                "ssmmessages:CreateControlChannel",
                "ssmmessages:CreateDataChannel",
                "ssmmessages:OpenControlChannel",
                "ssmmessages:OpenDataChannel",
                "s3:GetEncryptionConfiguration"},
                Effect = "Allow",
                Resources = new string[]{"*"}
            };

            DataAwsIamPolicyDocument policyDoc = new DataAwsIamPolicyDocument(this, "iam-policy-doc", new DataAwsIamPolicyDocumentConfig
            {
                Statement = new DataAwsIamPolicyDocumentStatement[]{statement}
            });
            IamPolicy iamPolicy = new IamPolicy(this, "iam-policy", new IamPolicyConfig{
                Policy = policyDoc.Json,
                NamePrefix = "vpc-ec2-trading-hub-"
            });

            DataAwsIamPolicyDocumentStatementPrincipals principals = new DataAwsIamPolicyDocumentStatementPrincipals{
                Type = "Service",
                Identifiers = new string[]{"ec2.amazonaws.com"}
            };
            DataAwsIamPolicyDocumentStatement assumeStatement = new DataAwsIamPolicyDocumentStatement{
                Actions = new string[]{"sts:AssumeRole"},
                Principals = new DataAwsIamPolicyDocumentStatementPrincipals[]{ principals },          
                Effect = "Allow"
            };

            DataAwsIamPolicyDocument assumePolicyDoc = new DataAwsIamPolicyDocument(this, "iam-assume-policy-doc", new DataAwsIamPolicyDocumentConfig
            {
                Statement = new DataAwsIamPolicyDocumentStatement[]{assumeStatement}
            });

            IamRole iamRole = new IamRole(this, "iam-role", new IamRoleConfig{
                AssumeRolePolicy = assumePolicyDoc.Json
            });
            IamRolePolicyAttachment rolePolicyAttachment = new IamRolePolicyAttachment(this, "rpa", new IamRolePolicyAttachmentConfig{
                Role = iamRole.Name,
                PolicyArn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
            });
            IamInstanceProfile instanceProfile = new IamInstanceProfile(this, "instance-profile", new IamInstanceProfileConfig{
                Role = iamRole.Name
            });

            //ssh-keygen -t ed25519 -m PEM -C "youremail@company.com" -f $HOME/.ssh/ec2_id_ed25519; chmod 400 ~/.ssh/ec2_id_ed25519* 
            string pubKey = System.IO.File.ReadAllText(System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("HOME"), ".ssh", "ec2_id_ed25519.pub")).Replace(Environment.NewLine, "");
            
            KeyPair keyPair = new KeyPair(this, "ec2-keypair", new KeyPairConfig{
                KeyNamePrefix = "trading-hub-kp-",
                PublicKey = pubKey
            });
            Instance instance = new Instance(this, "compute", new InstanceConfig
            { 
                Ami = latestAmazonLinux2Ami.Id, //"ami-01ded35841bc93d7f", //latestAmazonLinux2Ami.ImageId,
                InstanceType = "t3.micro",
                NetworkInterface = new InstanceNetworkInterface[]{
                    new InstanceNetworkInterface{
                        DeviceIndex = 0,
                        NetworkInterfaceId = networkInterface.Id
                    }
                },
                KeyName = keyPair.KeyName,
                IamInstanceProfile = instanceProfile.Name,
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
            });

            new TerraformOutput(this, "private_ip", new TerraformOutputConfig
            {
                Value = instance.PrivateIp
            });
            
        }

        public static void Main(string[] args)
        {
            App app = new App();
            new MyApp(app, "vpc-ec2-example");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}