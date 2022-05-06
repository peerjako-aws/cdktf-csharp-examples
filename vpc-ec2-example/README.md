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

