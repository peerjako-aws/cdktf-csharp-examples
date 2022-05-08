---
typora-copy-images-to: ../vpc-ec2-example
---

# VPC with EC2 instance Terraform for CDK example

The Cloud Development Kit for Terraform (CDKTF) allows you to define your infrastructure in a familiar programming language such as TypeScript, Python, Go, C#, or Java.

In this tutorial, you will provision an EC2 instance on AWS using the C# programming language.

## Initialize a new CDK for Terraform application

Start by creating a directory named vpc-ec2-example for your project

```bash
mkdir vpc-ec2-example
```

The navigate into it.

```bash
cd vpc-ec2-example
```

Inside the directory, run the following command to initialize a cdktf project using the C# template and storing Terraform state locally

```
cdktf init --local --template csharp
```

If all went well you should see a "Your cdktf csharp project is ready!" message. Accept the defaults for "Project Name" and "Project Description".

## Install AWS provider

CDKTF provides packages with prebuilt classes for several common Terraform providers that you can use in your C# projects. For other Terraform providers and modules, you can add them to `cdktf.json` and use `cdktf get` to [generate the appropriate C# classes](https://www.terraform.io/cdktf/concepts/providers-and-resources#providers).

Install the AWS provider with the following dotnet command

```bash
dotnet add package HashiCorp.Cdktf.Providers.Aws
```

## Define your CDK for Terraform Application

In the Cloud9 explorer, navigate to the vpc-ec2-example/Main.cs file to view your application code. The template creates a scaffold with no functionality.

In this example we will be using vpc and ec2 resources so start by adding the following using statements to Main.cs

```c#
using System.Collections.Generic;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Aws.Ec2;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
```

![image-20220506145937284](images/image-20220506145937284.png)

Let us configure the AWS provider to use the eu-north-1 region (or whatever favorite region you have) by adding the following line to the MyApp stack part of Main.cs

```c#
new AwsProvider(this, "AWS", new AwsProviderConfig { Region = "eu-north-1" });
```

Lets start creating some infrastructure. First we will add a VPC with a CIDR block of "10.0.0.0/16". Add the following code just after the AwsProvider code-line

```c#
Vpc vpc = new Vpc(this, "vpc", new VpcConfig{
    CidrBlock = "10.0.0.0/16"
});
```

We also want a subnet in our VPC where we can deploy our EC2. Add the following code which creates a subnet with CIDR block of "10.0.10.0/24" in the eu-north-1a availability zone (chose another AZ if your region is not eu-north-1)

```c#
Subnet subnet = new Subnet(this, "subnet", new SubnetConfig{
  	VpcId = vpc.Id,
  	CidrBlock = "10.0.10.0/24",
  	AvailabilityZone = "eu-north-1a",
  	Tags = new Dictionary<string, string>{{"Name","vpc-ec2-trading-hub"}}
});

```

Lets see what kind of Terraform we get with the VPC and Subnet. Run the following terminal command

```bash
cdktf synth
```

The cdktf tool synthezises the stack into the file found at cdktf.out/vpc-ec2/example/cdk.tf.json . Navigate into that folder as shown on the image below. Open the file and inspect the content. You can see we have the AWS provider, the VPC and the Subnet.

![image-20220508180916080](images/image-20220508180916080.png)

Now we create the VPC and subnet in you AWS account using the cdktf deploy command. When asked to "Approve" hit enter and wait for the command to finnish

```bash
cdktf deploy
```

If you see "Apply complete! Resources: 2 added, 0 changed, 0 destroyed." then the resources were succesfully create. Go and check your new VPC called "vpc-ec2-trading-hub" in the [AWS console](https://eu-north-1.console.aws.amazon.com/vpc/home#vpcs:).

![image-20220508182007660](images/image-20220508182007660.png)
