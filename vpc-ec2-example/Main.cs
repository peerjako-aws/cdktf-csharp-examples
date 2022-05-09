using System;
using Constructs;
using HashiCorp.Cdktf;

using System.Collections.Generic;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Vpc;

namespace MyCompany.MyApp
{
    class MyApp : TerraformStack
    {
        public MyApp(Construct scope, string id) : base(scope, id)
        {
            // define resources here
            new AwsProvider(this, "AWS", new AwsProviderConfig { Region = "eu-west-1" });
            
            Vpc vpc = new Vpc(this, "vpc", new VpcConfig{
                CidrBlock = "10.0.0.0/16",
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
            });
            
            Subnet subnet = new Subnet(this, "subnet", new SubnetConfig{
                VpcId = vpc.Id,
                CidrBlock = "10.0.10.0/24",
                AvailabilityZone = "eu-west-1a",
                Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
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