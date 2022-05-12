using System;
using Constructs;
using System.Collections.Generic;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Elb;

namespace MyCompany.MyApp
{
    class MyApp : TerraformStack
    {
        public MyApp(Construct scope, string id) : base(scope, id)
        {
            // define resources here
            new AwsProvider(this, "aws", new AwsProviderConfig{
                Region = "eu-west-1",
            });

            Vpc vpc = new Vpc(this, "vpc", new VpcConfig{
                CidrBlock = "10.0.0.0/16",
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPC"}
                },
                EnableDnsHostnames = true,
                EnableDnsSupport = true
            });

            InternetGateway igw = new InternetGateway(this, "igw", new InternetGatewayConfig{
                VpcId = vpc.Id,
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCIGW"}
                },
            });

            Eip eip = new Eip(this, "eip", new EipConfig{
                Vpc = true,
                DependsOn = new ITerraformDependable[]{
                    igw
                }
            });

            Subnet publicSubnet = new Subnet(this, "public-subnet", new SubnetConfig{
                VpcId = vpc.Id,
                CidrBlock = "10.0.10.0/24",
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCPublic"}
                },
                MapPublicIpOnLaunch = true,
                AvailabilityZone = "eu-west-1a"
            });


            Subnet publicSubnet2 = new Subnet(this, "public-subnet2", new SubnetConfig{
                VpcId = vpc.Id,
                CidrBlock = "10.0.20.0/24",
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCPublic2"}
                },
                MapPublicIpOnLaunch = true,
                AvailabilityZone = "eu-west-1b"
            });
            NatGateway nat = new NatGateway(this, "nat", new NatGatewayConfig{
                AllocationId = eip.Id,
                SubnetId = publicSubnet.Id,
                DependsOn = new ITerraformDependable[]{
                    igw
                },
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCNAT"}
                }
            });

            VpcEndpoint vpcEndpointS3 = new VpcEndpoint(this, "vpc-endpoint-s3", new VpcEndpointConfig{
                VpcId = vpc.Id,
                ServiceName = "com.amazonaws.eu-west-1.s3"
            });

            Subnet privateSubnet = new Subnet(this, "private-subnet", new SubnetConfig{
                VpcId = vpc.Id,
                CidrBlock = "10.0.11.0/24",
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCPrivate"}
                },
                MapPublicIpOnLaunch = false
            });

            RouteTable rtPrivate = new RouteTable(this, "rtPrivate", new RouteTableConfig{
                VpcId = vpc.Id,
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCPrivateRT"}
                }
            });

            RouteTable rtPublic = new RouteTable(this, "rtPublic", new RouteTableConfig{
                VpcId = vpc.Id,
                Tags = new Dictionary<string, string>(){
                    {"Name", "DemoDemoVPCPrivateRT"}
                }
            });

            SecurityGroup albSG = new SecurityGroup(this, "sg-alb", new SecurityGroupConfig{
                Name = "ALBSG",
                VpcId = vpc.Id,
                Description = "DemoVPCALBSG",
                Ingress = new SecurityGroupIngress[]{
                    new SecurityGroupIngress{
                        FromPort = 443,
                        ToPort = 443,
                        Protocol = "tcp",
                        CidrBlocks = new string[]{
                            "0.0.0.0/0"
                        }
                    }
                },
                Egress = new SecurityGroupEgress[]{
                    new SecurityGroupEgress{
                        FromPort = 0,
                        ToPort = 0,
                        Protocol = "-1",
                        CidrBlocks = new string[]{
                            "0.0.0.0/0"
                        }
                    }
                }

            });

            Lb alb = new Lb(this, "lb", new LbConfig{
                Name = "DemoAlb",
                Internal = false,
                LoadBalancerType = "application",
                SecurityGroups = new string[]{
                    albSG.Id
                },
                Subnets = new string[]{
                    publicSubnet.Id,
                    publicSubnet2.Id
                }
                
            });

        }

        public static void Main(string[] args)
        {
            App app = new App();
            new MyApp(app, "demodemo");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}